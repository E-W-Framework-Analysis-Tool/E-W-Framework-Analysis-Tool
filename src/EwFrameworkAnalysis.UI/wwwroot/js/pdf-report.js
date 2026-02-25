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
        y = 6;

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
        doc.setFontSize(11);
        doc.setTextColor(0, 0, 0);
        doc.text('Sectors:', margin, y);
        var pillX = margin + doc.getTextWidth('Sectors:') + 3;
        doc.setFontSize(8);
        for (var si = 0; si < q.sectors.length; si++) {
            var pillText = q.sectors[si];
            var pillW = doc.getTextWidth(pillText) + 4;
            doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
            doc.roundedRect(pillX, y - 3.5, pillW, 6, 3, 3, 'F');
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

        // ── Indicators Grid ────────────────────────────────────────────────────
        var IND_COLS = 3;
        var indColGap = 3;
        var indRowGap = 4;
        var indPad = 3;
        var indCellW = (contentWidth - (IND_COLS - 1) * indColGap) / IND_COLS;
        var indBarH = 3;
        var indNameFS = 8;
        var indNameLineH = 3.8;
        var indPillFS = 6;
        var indPillH = 5;
        var indScoreFS = 9;

        // Pre-compute text layout for each indicator (sets font state, so must run before drawing)
        var indLayouts = q.indicators.map(function (indItem) {
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(indNameFS);
            var nameLines = doc.splitTextToSize(indItem.name, indCellW - indPad * 2);
            if (nameLines.length > 3) { nameLines = nameLines.slice(0, 3); }

            doc.setFont('helvetica', 'bold');
            doc.setFontSize(indPillFS);
            var pillRows = [[]];
            var pillRowW = 0;
            var pillMaxW = indCellW - indPad * 2;
            (indItem.sectors || []).forEach(function (s) {
                var pw = doc.getTextWidth(s) + 4;
                if (pillRowW > 0 && pillRowW + 2 + pw > pillMaxW) {
                    pillRows.push([]);
                    pillRowW = 0;
                }
                pillRows[pillRows.length - 1].push({ text: s, w: pw });
                pillRowW += (pillRowW > 0 ? 2 : 0) + pw;
            });
            if (pillRows[0].length === 0) { pillRows = []; }

            return { indItem: indItem, nameLines: nameLines, pillRows: pillRows };
        });

        // Group layouts into rows of IND_COLS
        var indGridRows = [];
        for (var igr = 0; igr < indLayouts.length; igr += IND_COLS) {
            indGridRows.push(indLayouts.slice(igr, igr + IND_COLS));
        }

        for (var irow = 0; irow < indGridRows.length; irow++) {
            var indRowItems = indGridRows[irow];
            var maxNameLines = Math.max.apply(null, indRowItems.map(function (d) { return d.nameLines.length; }));
            var maxPillRows = Math.max.apply(null, indRowItems.map(function (d) { return d.pillRows.length; }));
            // Cell height: top-pad + name lines (with baseline offset) + gap + pill rows + score/bar/pad anchored at bottom
            var indCellH = 2 * indPad + (maxNameLines + 1) * indNameLineH + 2
                + maxPillRows * (indPillH + 1) + indBarH + 5;

            if (y + indCellH > pageHeight - 10) {
                doc.addPage();
                y = 20;
            }

            for (var ic = 0; ic < indRowItems.length; ic++) {
                var indLayout = indRowItems[ic];
                var indItem = indLayout.indItem;
                var pct = indItem.readinessScore * 100;
                var scoreColor = pct >= 66 ? [34, 197, 94] : pct >= 33 ? [234, 179, 8] : [239, 68, 68];
                var icellX = margin + ic * (indCellW + indColGap);
                var icellY = y;
                var barTop = icellY + indCellH - indPad - indBarH;
                var scoreY = barTop - 1.5;

                // Cell border — light gray, rounded corners
                doc.setDrawColor(210, 210, 210);
                doc.setLineWidth(0.3);
                doc.roundedRect(icellX, icellY, indCellW, indCellH, 2, 2, 'S');

                // Indicator name (bold, from top of cell)
                doc.setFont('helvetica', 'bold');
                doc.setFontSize(indNameFS);
                doc.setTextColor(0, 0, 0);
                var ty = icellY + indPad + indNameLineH;
                for (var nl = 0; nl < indLayout.nameLines.length; nl++) {
                    doc.text(indLayout.nameLines[nl], icellX + indPad, ty + nl * indNameLineH);
                }
                ty += indLayout.nameLines.length * indNameLineH + 2;

                // Sector pills (below name)
                if (indLayout.pillRows.length > 0) {
                    doc.setFont('helvetica', 'bold');
                    doc.setFontSize(indPillFS);
                    for (var ipr = 0; ipr < indLayout.pillRows.length; ipr++) {
                        var pillX = icellX + indPad;
                        for (var ipp = 0; ipp < indLayout.pillRows[ipr].length; ipp++) {
                            var pill = indLayout.pillRows[ipr][ipp];
                            doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
                            doc.roundedRect(pillX, ty - 3, pill.w, indPillH, indPillH / 2, indPillH / 2, 'F');
                            doc.setTextColor(255, 255, 255);
                            doc.text(pill.text, pillX + 2, ty + 0.5);
                            doc.setTextColor(0, 0, 0);
                            pillX += pill.w + 2;
                        }
                        ty += indPillH + 1;
                    }
                }

                // Score percentage — centered, anchored above the progress bar
                doc.setFont('helvetica', 'bold');
                doc.setFontSize(indScoreFS);
                doc.setTextColor(scoreColor[0], scoreColor[1], scoreColor[2]);
                doc.text(pct.toFixed(1) + '%', icellX + indCellW / 2, scoreY, { align: 'center' });

                // Progress bar — gray background track
                var barX = icellX + indPad;
                var barW = indCellW - indPad * 2;
                doc.setFillColor(220, 220, 220);
                doc.roundedRect(barX, barTop, barW, indBarH, 1, 1, 'F');

                // Progress bar — colored fill
                var fillW = barW * pct / 100;
                if (fillW >= 2) {
                    doc.setFillColor(scoreColor[0], scoreColor[1], scoreColor[2]);
                    doc.roundedRect(barX, barTop, fillW, indBarH, 1, 1, 'F');
                } else if (fillW > 0) {
                    doc.setFillColor(scoreColor[0], scoreColor[1], scoreColor[2]);
                    doc.rect(barX, barTop, fillW, indBarH, 'F');
                }
            }

            y += indCellH + indRowGap;
        }

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
                    return [de.name, de.availability]; // availability stored as raw cell value
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
                    // suppress the raw text in the availability column — drawn manually below
                    if (data.column.index === 1 && data.row.section === 'body') {
                        data.cell.text = [''];
                    }
                },
                didDrawCell: function (data) {
                    if (data.column.index !== 1 || data.row.section !== 'body') return;
                    var avail = data.cell.raw;
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
