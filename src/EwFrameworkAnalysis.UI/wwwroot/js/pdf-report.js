window.generatePdfReport = function (reportData) {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });

    const pageWidth = doc.internal.pageSize.getWidth();
    const pageHeight = doc.internal.pageSize.getHeight();
    const margin = 14;
    const contentWidth = pageWidth - 2 * margin;
    const brandBlue = [2, 40, 71];

    // ── Header Banner ──────────────────────────────────────────────────────────
    doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
    doc.rect(0, 0, pageWidth, 28, 'F');

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(18);
    doc.setTextColor(255, 255, 255);
    doc.text('E-W Framework Readiness Report', margin, 13);

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    var dateStr = new Date().toLocaleDateString('en-US', {
        month: 'numeric', day: 'numeric', year: 'numeric'
    });
    doc.text('Generated: ' + dateStr, margin, 22);

    doc.setTextColor(0, 0, 0);
    var y = 36;

    // ── EQ Readiness Summary ───────────────────────────────────────────────────
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('EQ Readiness Summary', margin, y);
    y += 7;

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.text('Readiness Score Distribution', margin, y);
    y += 3;

    doc.autoTable({
        startY: y,
        margin: { left: margin, right: margin },
        head: [['Band', 'Count']],
        body: reportData.summary.eqBands.map(function (b) {
            return [b.label, String(b.count)];
        }),
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: { 1: { halign: 'center', cellWidth: 24 } },
        tableWidth: contentWidth,
    });

    y = doc.lastAutoTable.finalY + 8;

    // ── Sector + Data Source tables (side by side) ─────────────────────────────
    var halfWidth = (contentWidth - 6) / 2;

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.text('Readiness by Sector', margin, y);
    doc.text('Readiness by Data Source', margin + halfWidth + 6, y);
    y += 3;

    var sectorTableStartY = y;

    // Left: Sector table
    doc.autoTable({
        startY: sectorTableStartY,
        margin: { left: margin },
        head: [['Sector', 'Score']],
        body: reportData.summary.sectorReadiness.map(function (s) {
            return [s.sector, (s.score * 100).toFixed(1) + '%'];
        }),
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: { 1: { halign: 'right', cellWidth: 22 } },
        tableWidth: halfWidth,
    });

    var leftFinalY = doc.lastAutoTable.finalY;

    // Right: Data Source table
    var dsRows = [
        ['Custom Manual', (reportData.summary.dataSourceReadiness.custom * 100).toFixed(1) + '%'],
        ['Automated (Ed-Fi & CEDS)', (reportData.summary.dataSourceReadiness.automated * 100).toFixed(1) + '%'],
    ];
    if (reportData.summary.dataSourceReadiness.ecsActive) {
        dsRows.push(['ECS State Data', (reportData.summary.dataSourceReadiness.ecs * 100).toFixed(1) + '%']);
    }

    doc.autoTable({
        startY: sectorTableStartY,
        margin: { left: margin + halfWidth + 6 },
        head: [['Source', 'Score']],
        body: dsRows,
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: { 1: { halign: 'right', cellWidth: 22 } },
        tableWidth: halfWidth,
    });

    var rightFinalY = doc.lastAutoTable.finalY;
    y = Math.max(leftFinalY, rightFinalY) + 10;

    // ── Essential Questions ────────────────────────────────────────────────────
    if (y + 20 > pageHeight) {
        doc.addPage();
        y = 20;
    }

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('Essential Questions', margin, y);
    y += 4;

    var sortedQuestions = reportData.questions.slice().sort(function (a, b) {
        return b.readinessScore - a.readinessScore;
    });

    // Columns: #, Question, Indicators, Data Elements (colored dots), Score
    doc.autoTable({
        startY: y,
        margin: { left: margin, right: margin },
        head: [['#', 'Question', 'Indicators', 'Data Elements', 'Score']],
        body: sortedQuestions.map(function (q) {
            return [
                String(q.number),
                q.question,
                String(q.indicatorCount),
                '', // drawn via didDrawCell
                (q.readinessScore * 100).toFixed(1) + '%',
            ];
        }),
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: {
            0: { cellWidth: 12, halign: 'center' },
            2: { cellWidth: 24, halign: 'center' },
            3: { cellWidth: 42, halign: 'center' },
            4: { cellWidth: 20, halign: 'right' },
        },
        tableWidth: contentWidth,
        didParseCell: function (data) {
            if (data.row.section === 'head') {
                data.cell.styles.halign = 'center';
            }
            if (data.column.index === 4 && data.row.section === 'body') {
                var pct = sortedQuestions[data.row.index].readinessScore * 100;
                if (pct >= 66) {
                    data.cell.styles.textColor = [34, 197, 94];
                } else if (pct >= 33) {
                    data.cell.styles.textColor = [234, 179, 8];
                } else {
                    data.cell.styles.textColor = [239, 68, 68];
                }
            }
        },
        didDrawCell: function (data) {
            if (data.column.index !== 3 || data.row.section !== 'body') return;

            var de = sortedQuestions[data.row.index].dataElements;
            var cx = data.cell.x + 3;
            var cy = data.cell.y + data.cell.height / 2;
            var r = 1.5;
            var textGap = r * 2 + 1;
            var groupStep = 13;

            doc.setFontSize(8);

            // Green — available
            doc.setFillColor(34, 197, 94);
            doc.circle(cx + r, cy, r, 'F');
            doc.setTextColor(0, 0, 0);
            doc.text(String(de.available), cx + textGap, cy + 0.8);

            // Yellow — partially available
            cx += groupStep;
            doc.setFillColor(234, 179, 8);
            doc.circle(cx + r, cy, r, 'F');
            doc.text(String(de.partial), cx + textGap, cy + 0.8);

            // Red — not available / insufficient
            cx += groupStep;
            doc.setFillColor(239, 68, 68);
            doc.circle(cx + r, cy, r, 'F');
            doc.text(String(de.notAvailable), cx + textGap, cy + 0.8);

            doc.setTextColor(0, 0, 0);
        },
    });

    // ── Per-Question Detail Pages ──────────────────────────────────────────────
    var orderedQuestions = reportData.questions.slice().sort(function (a, b) {
        return a.number - b.number;
    });

    for (var qi = 0; qi < orderedQuestions.length; qi++) {
        var q = orderedQuestions[qi];
        doc.addPage();
        y = 14;

        // Section Header
        doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
        doc.rect(0, y, pageWidth, 14, 'F');
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(12);
        doc.setTextColor(255, 255, 255);
        doc.text('EQ ' + q.number + ': ' + q.summary, margin, y + 9);
        doc.setTextColor(0, 0, 0);
        y += 20;

        // Full Question Text
        doc.setFont('helvetica', 'italic');
        doc.setFontSize(10);
        var qLines = doc.splitTextToSize(q.question, contentWidth);
        doc.text(qLines, margin, y);
        y += qLines.length * 5 + 6;

        // Sector Pills
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(9);
        doc.setTextColor(0, 0, 0);
        doc.text('Sectors:', margin, y);
        var pillX = margin + doc.getTextWidth('Sectors:') + 3;
        doc.setFontSize(8);
        for (var si = 0; si < q.sectors.length; si++) {
            var pillText = q.sectors[si];
            var pillW = doc.getTextWidth(pillText) + 4;
            doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
            doc.roundedRect(pillX, y - 3.5, pillW, 6, 1, 1, 'F');
            doc.setTextColor(255, 255, 255);
            doc.text(pillText, pillX + 2, y + 0.5);
            doc.setTextColor(0, 0, 0);
            pillX += pillW + 2;
        }
        y += 12;

        // Indicators Subheading
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(11);
        doc.text('Indicators', margin, y);
        y += 4;

        // Indicators Table
        (function (currentQ) {
            doc.autoTable({
                startY: y,
                margin: { left: margin, right: margin },
                head: [['Indicator', 'Sectors', 'Score']],
                body: currentQ.indicators.map(function (ind) {
                    return [
                        ind.name,
                        ind.sectors.join(', '),
                        (ind.readinessScore * 100).toFixed(1) + '%',
                    ];
                }),
                styles: { fontSize: 9 },
                headStyles: { fillColor: brandBlue },
                columnStyles: {
                    2: { cellWidth: 20, halign: 'right' },
                },
                tableWidth: contentWidth,
                didParseCell: function (data) {
                    if (data.row.section === 'head') {
                        data.cell.styles.halign = 'center';
                    }
                    if (data.column.index === 2 && data.row.section === 'body') {
                        var pct = currentQ.indicators[data.row.index].readinessScore * 100;
                        if (pct >= 66) {
                            data.cell.styles.textColor = [34, 197, 94];
                        } else if (pct >= 33) {
                            data.cell.styles.textColor = [234, 179, 8];
                        } else {
                            data.cell.styles.textColor = [239, 68, 68];
                        }
                    }
                },
            });
        }(q));

        y = doc.lastAutoTable.finalY + 8;

        // Data Elements Subheading (new page if near bottom)
        if (y + 20 > pageHeight) {
            doc.addPage();
            y = 20;
        }

        doc.setFont('helvetica', 'bold');
        doc.setFontSize(11);
        doc.text('Data Elements', margin, y);
        y += 4;

        // Data Elements Table
        (function (currentQ) {
            doc.autoTable({
                startY: y,
                margin: { left: margin, right: margin },
                head: [['Data Element', 'Availability']],
                body: currentQ.distinctDataElements.map(function (de) {
                    return [de.name, ''];
                }),
                styles: { fontSize: 9 },
                headStyles: { fillColor: brandBlue },
                columnStyles: {
                    1: { cellWidth: 44 },
                },
                tableWidth: contentWidth,
                didParseCell: function (data) {
                    if (data.row.section === 'head') {
                        data.cell.styles.halign = 'center';
                    }
                },
                didDrawCell: function (data) {
                    if (data.column.index !== 1 || data.row.section !== 'body') return;
                    var avail = currentQ.distinctDataElements[data.row.index].availability;
                    var color, label;
                    if (avail === 'Available') {
                        color = [34, 197, 94]; label = 'Available';
                    } else if (avail === 'PartiallyAvailable') {
                        color = [234, 179, 8]; label = 'Partially Available';
                    } else if (avail === 'NotAvailable') {
                        color = [239, 68, 68]; label = 'Not Available';
                    } else {
                        color = [239, 68, 68]; label = 'Insufficient Data';
                    }
                    var cx = data.cell.x + 4;
                    var cy = data.cell.y + data.cell.height / 2;
                    doc.setFillColor(color[0], color[1], color[2]);
                    doc.circle(cx, cy, 1.5, 'F');
                    doc.setTextColor(0, 0, 0);
                    doc.setFontSize(8);
                    doc.text(label, cx + 3.5, cy + 0.8);
                },
            });
        }(q));
    }

    var pdfUrl = doc.output('bloburl');
    window.open(pdfUrl, '_blank');
};
