const THEME = {
    brandBlue:  [2, 40, 71],
    white:      [255, 255, 255],
    black:      [0, 0, 0],
    green:      [34, 197, 94],
    ltGreen:    [74, 222, 128],
    lime:       [163, 230, 53],
    amber:      [234, 179, 8],
    orange:     [249, 115, 22],
    red:        [239, 68, 68],
    grayLight:  [225, 225, 225],
    gray:       [220, 220, 220],
    grayDark:   [210, 210, 210],
};

function scoreColor(pct) {
    return pct >= 70 ? THEME.green : pct >= 50 ? THEME.amber : THEME.red;
}

// ── Shared context ─────────────────────────────────────────────────────────────
function buildCtx() {
    const { jsPDF } = window.jspdf;
    const doc = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });
    const margin = 14;
    const pageWidth = doc.internal.pageSize.getWidth();
    return {
        doc: doc,
        pageWidth: pageWidth,
        pageHeight: doc.internal.pageSize.getHeight(),
        margin: margin,
        contentWidth: pageWidth - 2 * margin,
        brandBlue: THEME.brandBlue,
        y: 0,
        eqRowBounds: [],     // populated by drawEssentialQuestionsTable; consumed by applyEqTableLinks
        eqBandLinkQueue: [], // populated by drawEqReadinessSummary;    consumed by applyEqTableLinks
        questionPageMap: {}  // populated by drawQuestionDetailPage;     consumed by applyEqTableLinks
    };
}

// ── Header Banner ───────────────────────────────────────────────────────────────
function drawHeaderBanner(ctx, projectTitle) {
  const doc = ctx.doc;
  const bb = ctx.brandBlue;
  const hasProject = typeof projectTitle === 'string' && projectTitle.length > 0;
  const bannerH = hasProject ? 42 : 36;
  const cx = ctx.pageWidth / 2;

  doc.setFillColor(bb[0], bb[1], bb[2]);
  doc.rect(0, 0, ctx.pageWidth, bannerH, 'F');
  doc.setTextColor.apply(doc, THEME.white);

  // Tool name — small, normal, centered, slightly dimmed
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(9);
  doc.setTextColor(180, 200, 220);
  doc.text('E-W Framework Analysis Tool', cx, 9, { align: 'center' });

  // Report title — large, bold, centered, white
  doc.setFont('helvetica', 'bold');
  doc.setFontSize(20);
  doc.setTextColor.apply(doc, THEME.white);
  doc.text('COVERAGE REPORT', cx, 18, { align: 'center' });

  // Thin rule
  const ruleY = hasProject ? 23 : 22;
  doc.setDrawColor(255, 255, 255);
  doc.setLineWidth(0.2);
  doc.setGState(doc.GState({ opacity: 0.3 }));
  doc.line(ctx.margin, ruleY, ctx.pageWidth - ctx.margin, ruleY);
  doc.setGState(doc.GState({ opacity: 1 }));

  // Project title and date — small, normal, centered
  doc.setFont('helvetica', 'normal');
  doc.setFontSize(10);
  doc.setTextColor.apply(doc, THEME.white);

  if (hasProject) {
    doc.text(projectTitle, cx, ruleY + 6, { align: 'center' });
  }

  const dateStr = new Date().toLocaleDateString('en-US', {
    month: 'numeric', day: 'numeric', year: 'numeric'
  });
  doc.text('Generated: ' + dateStr, cx, ruleY + (hasProject ? 13 : 7), { align: 'center' });

  doc.setTextColor.apply(doc, THEME.black);
  ctx.y = bannerH + 6;
}

// ── EQ Readiness Summary ────────────────────────────────────────────────────────
function drawEqReadinessSummary(ctx, summary, questions) {
    const doc = ctx.doc;
    const margin = ctx.margin;
    const contentWidth = ctx.contentWidth;
    const brandBlue = ctx.brandBlue;

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('Essential Question (EQ) Coverage Summary', margin, ctx.y);
    ctx.y += 7;

    const halfWidth = (contentWidth - 6) / 2;
    const chartX = margin + halfWidth + 6;

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.text('Essential Questions by Score', margin, ctx.y);
    doc.text('Score Distribution', chartX, ctx.y);
    ctx.y += 3;

    const distributionStartY = ctx.y;

    // EQ membership per band — thresholds mirror PdfReportService.cs
    const sortByNumber = (a, b) => a.number - b.number;
    const bandEqLists = [
        questions.filter(q => q.coverageScore >= 0.90).sort(sortByNumber),
        questions.filter(q => q.coverageScore >= 0.80 && q.coverageScore < 0.90).sort(sortByNumber),
        questions.filter(q => q.coverageScore >= 0.70 && q.coverageScore < 0.80).sort(sortByNumber),
        questions.filter(q => q.coverageScore >= 0.60 && q.coverageScore < 0.70).sort(sortByNumber),
        questions.filter(q => q.coverageScore >= 0.50 && q.coverageScore < 0.60).sort(sortByNumber),
        questions.filter(q => q.coverageScore < 0.50).sort(sortByNumber),
    ];

    doc.autoTable({
        startY: distributionStartY,
        margin: { left: margin, right: margin },
        head: [['', 'EQs', 'Count']],
        body: summary.eqBands.map((b, i) => {
            const eqText = bandEqLists[i].map(q => 'EQ-' + q.number).join(', ');
            return [b.label, eqText, String(b.count)];
        }),
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: {
            2: { halign: 'center', cellWidth: 24 },
        },
        tableWidth: halfWidth,
        didDrawCell: function (data) {
            if (data.column.index !== 1 || data.row.section !== 'body') return;
            const eqItems = bandEqLists[data.row.index];
            if (eqItems.length === 0) return;

            doc.setFontSize(9);
            doc.setFont('helvetica', 'normal');

            const cellPad = 2;
            const maxW = data.cell.width - cellPad * 2;
            const lineH = data.cell.height / Math.ceil(/* estimated */ 1); // see below
            let textX = data.cell.x + cellPad;
            let textY = data.cell.y;
            let lineW = 0;

            for (let i = 0; i < eqItems.length; i++) {
                const eqLabel = 'EQ-' + eqItems[i].number;
                const eqW = doc.getTextWidth(eqLabel);
                const sepW = i < eqItems.length - 1 ? doc.getTextWidth(', ') : 0;
                const chunkW = eqW + sepW;

                // Wrap to next line if this chunk won't fit
                if (lineW > 0 && lineW + eqW > maxW) {
                    textX = data.cell.x + cellPad;
                    textY += 4; // match autoTable's line height for fontSize 9
                    lineW = 0;
                }

                ctx.eqBandLinkQueue.push({
                    questionNumber: eqItems[i].number,
                    page: doc.internal.getCurrentPageInfo().pageNumber,
                    x: textX,
                    y: textY,
                    w: eqW,
                    h: 4,
                });

                textX += chunkW;
                lineW += chunkW;
            }
        },
    });

    // ── Score Distribution bar chart (right half) ────────────────────────────
    const bins = [
        { label: '90-100%', count: questions.filter(q => q.coverageScore >= 0.90).length,                                             color: THEME.green },
        { label: '80-90%',  count: questions.filter(q => q.coverageScore >= 0.80 && q.coverageScore < 0.90).length,                  color: THEME.ltGreen },
        { label: '70-80%',  count: questions.filter(q => q.coverageScore >= 0.70 && q.coverageScore < 0.80).length,                  color: THEME.lime },
        { label: '60-70%',  count: questions.filter(q => q.coverageScore >= 0.60 && q.coverageScore < 0.70).length,                  color: THEME.amber },
        { label: '50-60%',  count: questions.filter(q => q.coverageScore >= 0.50 && q.coverageScore < 0.60).length,                  color: THEME.orange },
        { label: '<50%',    count: questions.filter(q => q.coverageScore < 0.50).length,                                              color: THEME.red },
    ];

    let maxCount = Math.max.apply(null, bins.map(b => b.count));
    if (maxCount === 0) maxCount = 1;

    const headerH  = 7;
    const barH     = 6;
    const barGap   = 2;
    const labelW   = 16;
    const countW   = 7;
    const barPad   = 2;
    const barAreaW = halfWidth - labelW - countW - barPad * 2;

    // Header bar — matches autoTable headStyles colour
    doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
    doc.rect(chartX, distributionStartY, halfWidth, headerH, 'F');
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(9);
    doc.setTextColor.apply(doc, THEME.white);
    doc.text('EQ Distribution', chartX + halfWidth / 2, distributionStartY + 5, { align: 'center' });
    doc.setTextColor.apply(doc, THEME.black);

    let bY = distributionStartY + headerH + 2;

    for (let bi = 0; bi < bins.length; bi++) {
        const bin = bins[bi];
        const barX = chartX + labelW + barPad;

        // Label — right-aligned in the label column
        doc.setFont('helvetica', 'normal');
        doc.setFontSize(8);
        doc.text(bin.label, chartX + labelW - 1, bY + barH / 2 + 1.5, { align: 'right' });

        // Bar track (light gray)
        doc.setFillColor.apply(doc, THEME.grayLight);
        doc.roundedRect(barX, bY, barAreaW, barH, 1, 1, 'F');

        // Colored fill proportional to count vs max
        if (bin.count > 0) {
            const fillW = barAreaW * (bin.count / maxCount);
            doc.setFillColor(bin.color[0], bin.color[1], bin.color[2]);
            if (fillW >= 2) {
                doc.roundedRect(barX, bY, fillW, barH, 1, 1, 'F');
            } else {
                doc.rect(barX, bY, fillW, barH, 'F');
            }
        }

        // Count label — after the bar track
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(8);
        doc.text(String(bin.count), barX + barAreaW + barPad, bY + barH / 2 + 1.5);

        bY += barH + barGap;
    }

    ctx.y = Math.max(doc.lastAutoTable.finalY, bY + 2) + 8;

    doc.setFont('helvetica', 'normal');
    doc.setFontSize(10);
    doc.text('Coverage by Sector', margin, ctx.y);
    doc.text('Coverage by Data Source', margin + halfWidth + 6, ctx.y);
    ctx.y += 3;

    const sectorTableStartY = ctx.y;

    // Left: Sector table
    doc.autoTable({
        startY: sectorTableStartY,
        margin: { left: margin },
        head: [['Sector', 'Score']],
        body: summary.sectorCoverage.map(s => [s.sector, (s.score * 100).toFixed(1) + '%']),
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: { 1: { halign: 'center', cellWidth: 22 } },
        tableWidth: halfWidth,
    });

    const leftFinalY = doc.lastAutoTable.finalY;

    // Right: Data Source table
    const dsRows = (summary.dataSourceCoverage || []).map(ds => [ds.name, (ds.score * 100).toFixed(1) + '%']);

    doc.autoTable({
        startY: sectorTableStartY,
        margin: { left: margin + halfWidth + 6 },
        head: [['Source', 'Score']],
        body: dsRows,
        styles: { fontSize: 9 },
        headStyles: { fillColor: brandBlue },
        columnStyles: { 1: { halign: 'center', cellWidth: 22 } },
        tableWidth: halfWidth,
    });

    ctx.y = Math.max(leftFinalY, doc.lastAutoTable.finalY) + 20;
}

// ── Overall Readiness ────────────────────────────────────────────────────────────
function drawOverallCoverage(ctx, overallCoverage) {
    const doc = ctx.doc;
    const margin = ctx.margin;
    const contentWidth = ctx.contentWidth;
    const brandBlue = ctx.brandBlue;

    if (ctx.y + 60 > ctx.pageHeight) {
        doc.addPage();
        ctx.y = 20;
    }

    const halfWidth = (contentWidth - 6) / 2;
    const rightX = margin + halfWidth + 6;

    // ── Left column: Overall Readiness scores ────────────────────────────────
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('Coverage By Data Source Type', margin, ctx.y);

    // Right column title at same height
    doc.text('Highest ROI Data Elements', rightX, ctx.y);
    ctx.y += 8;

    const rows = [
        { label: 'Custom Manual', value: overallCoverage.manual },
        { label: 'Automated (Ed-Fi & CEDS)', value: overallCoverage.automated },
        { label: 'Public Data (ECS)', value: overallCoverage.ecs },
        { label: 'Combined (All Data Sources)', value: overallCoverage.combined },
    ];

    const barH = 4;
    const rowH = 14;
    const barWidth = halfWidth - 4;
    const leftStartY = ctx.y;

    for (let i = 0; i < rows.length; i++) {
        const row = rows[i];
        const pct = row.value * 100;
        const rowY = leftStartY + i * rowH;

        // Label (left) and percentage (right)
        doc.setFont('helvetica', 'normal');
        doc.setFontSize(9);
        doc.setTextColor.apply(doc, THEME.black);
        doc.text(row.label, margin, rowY + 4);

        doc.setFont('helvetica', 'bold');
        doc.setFontSize(9);
        const pctColor = scoreColor(pct);
        doc.setTextColor(pctColor[0], pctColor[1], pctColor[2]);
        doc.text(pct.toFixed(1) + '%', margin + halfWidth - 2, rowY + 4, { align: 'right' });

        // Progress bar track
        const barY = rowY + 6;
        doc.setFillColor.apply(doc, THEME.grayLight);
        doc.roundedRect(margin, barY, barWidth, barH, 1, 1, 'F');

        // Colored fill
        if (pct > 0) {
            const fillW = barWidth * pct / 100;
            doc.setFillColor(pctColor[0], pctColor[1], pctColor[2]);
            if (fillW >= 2) {
                doc.roundedRect(margin, barY, fillW, barH, 1, 1, 'F');
            } else {
                doc.rect(margin, barY, fillW, barH, 'F');
            }
        }
    }

    // ── Right column: ROI Data Elements ──────────────────────────────────────
    doc.setFont('helvetica', 'normal');
    doc.setFontSize(8);
    doc.setTextColor.apply(doc, THEME.black);
    doc.text('Data elements whose availability would improve the most indicators', rightX, leftStartY - 1);

    const roiItems = overallCoverage.roiItems || [];

    if (roiItems.length === 0) {
        doc.setFont('helvetica', 'italic');
        doc.setFontSize(9);
        doc.text('No unscored data elements found.', rightX, leftStartY + 8);
    } else {
        const nameX = rightX + 6;
        const maxNameW = halfWidth - 40;
        const nameLineH = 3.5;
        let roiY = leftStartY + 6;

        for (let ri = 0; ri < roiItems.length; ri++) {
            const item = roiItems[ri];

            // Number
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(9);
            doc.setTextColor.apply(doc, THEME.black);
            doc.text((ri + 1) + '.', rightX, roiY);

            // Data element name — wrap to multiple lines
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(9);
            const nameLines = doc.splitTextToSize(item.name, maxNameW);
            doc.text(nameLines, nameX, roiY);

            // Indicator count badge — aligned to first line
            const badgeText = item.indicators.length + ' indicator' + (item.indicators.length !== 1 ? 's' : '');
            doc.setFont('helvetica', 'normal');
            doc.setFontSize(7);
            const badgeW = doc.getTextWidth(badgeText) + 4;
            const badgeX = margin + contentWidth - badgeW;
            const badgeY = roiY - 3.5;
            doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
            doc.roundedRect(badgeX, badgeY, badgeW, 5, 2.5, 2.5, 'F');
            doc.setTextColor.apply(doc, THEME.white);
            doc.text(badgeText, badgeX + 2, roiY - 0.2);
            doc.setTextColor.apply(doc, THEME.black);

            roiY += Math.max(nameLines.length, 1) * nameLineH + 6.5;
        }
    }

    ctx.y = leftStartY + rows.length * rowH + 8;
}

// ── Essential Questions Table ───────────────────────────────────────────────────
function drawEssentialQuestionsTable(ctx, questions) {
    const doc = ctx.doc;
    const margin = ctx.margin;
    const contentWidth = ctx.contentWidth;
    const brandBlue = ctx.brandBlue;

    doc.addPage();
    ctx.y = 10;

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(14);
    doc.text('Essential Questions', margin, ctx.y);
    ctx.y += 4;

    const sortedQuestions = questions.slice().sort((a, b) => b.coverageScore - a.coverageScore);

    // Keyed by question number for safe lookup inside autoTable callbacks —
    // row.index is page-relative and resets on each page break, so it cannot
    // be used as a direct index into sortedQuestions.
    const questionByNumber = {};
    sortedQuestions.forEach(q => { questionByNumber[q.number] = q; });

    // Columns: #, Question, Indicators, Data Elements (colored dots), Score
    doc.autoTable({
        startY: ctx.y,
        margin: { left: margin, right: margin },
        head: [['#', 'Question', 'Indicators', 'Data Elements', 'Score']],
        body: sortedQuestions.map(q => [
            String(q.number),
            q.question,
            String(q.indicatorCount),
            '', // drawn via didDrawCell
            (q.coverageScore * 100).toFixed(1) + '%',
        ]),
        styles: { fontSize: 9, valign: 'middle' },
        headStyles: { fillColor: brandBlue },
        columnStyles: {
            0: { cellWidth: 10, halign: 'center' },
            2: { cellWidth: 28, halign: 'center' },
            3: { cellWidth: 42, halign: 'center' },
            4: { cellWidth: 18, halign: 'right' },
        },
        tableWidth: contentWidth,
        didParseCell: function (data) {
            if (data.row.section === 'head') {
                data.cell.styles.halign = 'center';
            }
            if (data.column.index === 4 && data.row.section === 'body') {
                const q = questionByNumber[parseInt(data.row.raw[0], 10)];
                if (!q) return;
                const pct = q.coverageScore * 100;
                data.cell.styles.textColor = scoreColor(pct);
            }
        },
        didDrawCell: function (data) {
            if (data.row.section === 'body' && data.column.index === 0) {
                const qNum = parseInt(data.row.raw[0], 10);
                if (!isNaN(qNum)) {
                    ctx.eqRowBounds.push({
                        questionNumber: qNum,
                        x: margin,
                        y: data.cell.y,
                        w: contentWidth,
                        h: data.cell.height,
                        page: doc.internal.getCurrentPageInfo().pageNumber,
                    });
                }
            }
            if (data.column.index !== 3 || data.row.section !== 'body') return;

            const de = (questionByNumber[parseInt(data.row.raw[0], 10)] || {}).dataElements;
            if (!de) return;
            let cx = data.cell.x + 3;
            const cy = data.cell.y + data.cell.height / 2;
            const r = 1.5;
            const textGap = r * 2 + 1;
            const groupStep = 13;

            doc.setFontSize(8);

            // Green — available
            doc.setFillColor.apply(doc, THEME.green);
            doc.circle(cx + r, cy, r, 'F');
            doc.setTextColor.apply(doc, THEME.black);
            doc.text(String(de.available), cx + textGap, cy + 0.8);

            // Yellow — partially available
            cx += groupStep;
            doc.setFillColor.apply(doc, THEME.amber);
            doc.circle(cx + r, cy, r, 'F');
            doc.text(String(de.partial), cx + textGap, cy + 0.8);

            // Red — not available / insufficient
            cx += groupStep;
            doc.setFillColor.apply(doc, THEME.red);
            doc.circle(cx + r, cy, r, 'F');
            doc.text(String(de.notAvailable), cx + textGap, cy + 0.8);

            doc.setTextColor.apply(doc, THEME.black);
        },
    });
}

// ── Per-Question Detail Page ────────────────────────────────────────────────────
function drawQuestionDetailPage(ctx, q) {
    const doc = ctx.doc;
    const margin = ctx.margin;
    const contentWidth = ctx.contentWidth;
    const pageWidth = ctx.pageWidth;
    const pageHeight = ctx.pageHeight;
    const brandBlue = ctx.brandBlue;

    doc.addPage();
    ctx.questionPageMap[q.number] = doc.internal.getCurrentPageInfo().pageNumber;
    let y = 0;

    // Section Header
    doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
    doc.rect(0, y, pageWidth, 14, 'F');
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(12);
    doc.setTextColor.apply(doc, THEME.white);
    doc.text('EQ ' + q.number + ': ' + q.summary, margin, y + 9);
    doc.setTextColor.apply(doc, THEME.black);
    y += 20;

    // Full Question Text
    doc.setFont('helvetica', 'italic');
    doc.setFontSize(10);
    const qLines = doc.splitTextToSize(q.question, contentWidth);
    doc.text(qLines, margin, y);
    y += qLines.length * 5 + 6;

    // Sector Pills
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(11);
    doc.setTextColor.apply(doc, THEME.black);
    doc.text('Sectors:', margin, y);
    let pillX = margin + doc.getTextWidth('Sectors:') + 3;
    doc.setFontSize(8);
    for (let si = 0; si < q.sectors.length; si++) {
        const pillText = q.sectors[si];
        const pillW = doc.getTextWidth(pillText) + 4;
        doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
        doc.roundedRect(pillX, y - 3.5, pillW, 6, 3, 3, 'F');
        doc.setTextColor.apply(doc, THEME.white);
        doc.text(pillText, pillX + 2, y + 0.5);
        doc.setTextColor.apply(doc, THEME.black);
        pillX += pillW + 2;
    }
    y += 12;

    // Indicators Subheading
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(11);
    doc.text('Indicators', margin, y);
    y += 4;

    // ── Indicators Grid ─────────────────────────────────────────────────────────
    const IND_COLS = 3;
    const indColGap = 3;
    const indRowGap = 4;
    const indPad = 3;
    const indCellW = (contentWidth - (IND_COLS - 1) * indColGap) / IND_COLS;
    const indBarH = 3;
    const indNameFS = 8;
    const indNameLineH = 3.8;
    const indPillFS = 8;
    const indPillH = 6;
    const indScoreFS = 9;

    // Pre-compute text layout for each indicator (sets font state, so must run before drawing)
    const indLayouts = q.indicators.map(function (indItem) {
        doc.setFont('helvetica', 'bold');
        doc.setFontSize(indNameFS);
        let nameLines = doc.splitTextToSize(indItem.name, indCellW - indPad * 2);
        if (nameLines.length > 3) {
            nameLines = nameLines.slice(0, 3);
            nameLines[2] = nameLines[2].trimEnd() + '…';
        }

        doc.setFont('helvetica', 'bold');
        doc.setFontSize(indPillFS);
        let pillRows = [[]];
        let pillRowW = 0;
        const pillMaxW = indCellW - indPad * 2;
        (indItem.sectors || []).forEach(function (s) {
            const pw = doc.getTextWidth(s) + 4;
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
    const indGridRows = [];
    for (let igr = 0; igr < indLayouts.length; igr += IND_COLS) {
        indGridRows.push(indLayouts.slice(igr, igr + IND_COLS));
    }

    for (let irow = 0; irow < indGridRows.length; irow++) {
        const indRowItems = indGridRows[irow];
        const maxNameLines = Math.max.apply(null, indRowItems.map(d => d.nameLines.length));
        const maxPillRows = Math.max.apply(null, indRowItems.map(d => d.pillRows.length));
        // Cell height: top-pad + name lines (with baseline offset) + gap + pill rows + score/bar/pad anchored at bottom
        const indCellH = 2 * indPad + (maxNameLines + 1) * indNameLineH + 2
            + maxPillRows * (indPillH + 1) + indBarH + 5;

        if (y + indCellH > pageHeight - 10) {
            doc.addPage();
            y = 20;
        }

        for (let ic = 0; ic < indRowItems.length; ic++) {
            const indLayout = indRowItems[ic];
            const indItem = indLayout.indItem;
            const pct = indItem.coverageScore * 100;
            const indScoreColor = scoreColor(pct);
            const icellX = margin + ic * (indCellW + indColGap);
            const icellY = y;
            const barTop = icellY + indCellH - indPad - indBarH;
            const scoreY = barTop - 1.5;

            // Cell border — light gray, rounded corners
            doc.setDrawColor.apply(doc, THEME.grayDark);
            doc.setLineWidth(0.3);
            doc.roundedRect(icellX, icellY, indCellW, indCellH, 2, 2, 'S');

            // Indicator name (bold, from top of cell)
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(indNameFS);
            doc.setTextColor.apply(doc, THEME.black);
            let ty = icellY + indPad + indNameLineH;
            for (let nl = 0; nl < indLayout.nameLines.length; nl++) {
                doc.text(indLayout.nameLines[nl], icellX + indPad, ty + nl * indNameLineH);
            }
            ty += indLayout.nameLines.length * indNameLineH + 2;

            // Sector pills (below name)
            if (indLayout.pillRows.length > 0) {
                doc.setFont('helvetica', 'bold');
                doc.setFontSize(indPillFS);
                for (let ipr = 0; ipr < indLayout.pillRows.length; ipr++) {
                    pillX = icellX + indPad;
                    for (let ipp = 0; ipp < indLayout.pillRows[ipr].length; ipp++) {
                        const pill = indLayout.pillRows[ipr][ipp];
                        doc.setFillColor(brandBlue[0], brandBlue[1], brandBlue[2]);
                        doc.roundedRect(pillX, ty - 3.5, pill.w, indPillH, indPillH / 2, indPillH / 2, 'F');
                        doc.setTextColor.apply(doc, THEME.white);
                        doc.text(pill.text, pillX + 2, ty + 0.5);
                        doc.setTextColor.apply(doc, THEME.black);
                        pillX += pill.w + 2;
                    }
                    ty += indPillH + 1;
                }
            }

            // Score percentage — centered, anchored above the progress bar
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(indScoreFS);
            doc.setTextColor(indScoreColor[0], indScoreColor[1], indScoreColor[2]);
            doc.text(pct.toFixed(1) + '%', icellX + indCellW / 2, scoreY, { align: 'center' });

            // Progress bar — gray background track
            const barX = icellX + indPad;
            const barW = indCellW - indPad * 2;
            doc.setFillColor.apply(doc, THEME.gray);
            doc.roundedRect(barX, barTop, barW, indBarH, 1, 1, 'F');

            // Progress bar — colored fill
            const fillW = barW * pct / 100;
            if (fillW >= 2) {
                doc.setFillColor(indScoreColor[0], indScoreColor[1], indScoreColor[2]);
                doc.roundedRect(barX, barTop, fillW, indBarH, 1, 1, 'F');
            } else if (fillW > 0) {
                doc.setFillColor(indScoreColor[0], indScoreColor[1], indScoreColor[2]);
                doc.rect(barX, barTop, fillW, indBarH, 'F');
            }
        }

        y += indCellH + indRowGap;
    }

    y += 10;

    // Data Elements Subheading (new page if near bottom)
    if (y + 20 > pageHeight) {
        doc.addPage();
        y = 20;
    }

    doc.setFont('helvetica', 'bold');
    doc.setFontSize(11);
    doc.setTextColor.apply(doc, THEME.black);
    doc.text('Data Elements', margin, y);
    y += 4;

    // ── Data Elements Grid (2-column, horizontal-first) ──────────────────────────
    const DE_COLS = 2;
    const deColGap = 8;
    const deColW = (contentWidth - deColGap) / 2;
    const deCircleR = 1.5;
    const deCircleGap = 1.5;
    const deFontSize = 8;
    const deLineH = 3.8;
    const dePad = 1.5;
    const deRowGap = 1.5;
    const deTextW = deColW - deCircleR * 2 - deCircleGap;

    // Pre-compute line wrapping and availability color for each element
    doc.setFont('helvetica', 'bold');
    doc.setFontSize(deFontSize);
    const deLayouts = q.distinctDataElements.map(function (de) {
        let nameLines = doc.splitTextToSize(de.name, deTextW);
        if (nameLines.length > 2) { nameLines = nameLines.slice(0, 2); }
        let deColor;
        if (de.availability === 'Available') {
            deColor = THEME.green;
        } else if (de.availability === 'PartiallyAvailable') {
            deColor = THEME.amber;
        } else {
            deColor = THEME.red;
        }
        return { de: de, nameLines: nameLines, deColor: deColor };
    });

    // Group into pairs (horizontal-first order)
    const deGridRows = [];
    for (let dgr = 0; dgr < deLayouts.length; dgr += DE_COLS) {
        deGridRows.push(deLayouts.slice(dgr, dgr + DE_COLS));
    }

    for (let drow = 0; drow < deGridRows.length; drow++) {
        const deRowItems = deGridRows[drow];
        const maxDeLines = Math.max.apply(null, deRowItems.map(d => d.nameLines.length));
        const deRowH = dePad + maxDeLines * deLineH + dePad;

        if (y + deRowH > pageHeight - 10) {
            doc.addPage();
            y = 20;
        }

        // Circle center and first-line baseline, consistent for all cells in this row
        const deCy = y + dePad + deLineH - 1.0;
        const deTextY = y + dePad + deLineH;

        for (let dc = 0; dc < deRowItems.length; dc++) {
            const deLayout = deRowItems[dc];
            const dx = margin + dc * (deColW + deColGap);

            // Colored status circle
            doc.setFillColor(deLayout.deColor[0], deLayout.deColor[1], deLayout.deColor[2]);
            doc.circle(dx + deCircleR, deCy, deCircleR, 'F');

            // Bold data element name
            doc.setFont('helvetica', 'bold');
            doc.setFontSize(deFontSize);
            doc.setTextColor.apply(doc, THEME.black);
            const deTextX = dx + deCircleR * 2 + deCircleGap;
            for (let dnl = 0; dnl < deLayout.nameLines.length; dnl++) {
                doc.text(deLayout.nameLines[dnl], deTextX, deTextY + dnl * deLineH);
            }
        }

        y += deRowH + deRowGap;
    }
}

// ── Back-fill internal links: EQ table rows → question detail pages ─────────────
function applyEqTableLinks(ctx) {
    const doc = ctx.doc;

    // EQ summary table row links
    ctx.eqRowBounds.forEach(function (row) {
        const targetPage = ctx.questionPageMap[row.questionNumber];
        if (targetPage) {
            doc.setPage(row.page);
            doc.link(row.x, row.y, row.w, row.h, { pageNumber: targetPage });
        }
    });

    // Readiness Score Distribution — individual EQ label links
    ctx.eqBandLinkQueue.forEach(function (entry) {
        const targetPage = ctx.questionPageMap[entry.questionNumber];
        if (targetPage) {
            doc.setPage(entry.page);
            doc.link(entry.x, entry.y, entry.w, entry.h, { pageNumber: targetPage });
        }
    });

    doc.setPage(doc.internal.getNumberOfPages());
}

// ── Orchestrator ────────────────────────────────────────────────────────────────
window.generatePdfReport = function (reportData) {
    const ctx = buildCtx();

    drawHeaderBanner(ctx, reportData.projectTitle);
    drawEqReadinessSummary(ctx, reportData.summary, reportData.questions);
    drawOverallCoverage(ctx, reportData.overallCoverage);
    drawEssentialQuestionsTable(ctx, reportData.questions);

    const orderedQuestions = reportData.questions.slice().sort((a, b) => a.number - b.number);
    for (let qi = 0; qi < orderedQuestions.length; qi++) {
        drawQuestionDetailPage(ctx, orderedQuestions[qi]);
    }

    applyEqTableLinks(ctx);

    const pdfUrl = ctx.doc.output('bloburl');
    window.open(pdfUrl, '_blank');
};
