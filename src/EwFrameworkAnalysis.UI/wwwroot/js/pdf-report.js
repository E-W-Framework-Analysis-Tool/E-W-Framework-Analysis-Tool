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
                q.summary,
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

    var pdfUrl = doc.output('bloburl');
    window.open(pdfUrl, '_blank');
};
