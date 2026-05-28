param(
    [Parameter(Mandatory)]
    [string]$WwwRootPath
)

$indexPath = Join-Path $WwwRootPath "index.html"
if (-not (Test-Path $indexPath)) {
    Write-Error "index.html not found at: $indexPath"
    exit 1
}

$indexContent = Get-Content $indexPath -Raw

# Match local CSS (href="css/...") and JS (src="js/...") references
$regexes = @(
    'href="(css/[^"]+\.css)"',
    'src="(js/[^"]+\.js)"'
)

foreach ($pattern in $regexes) {
    $found = [regex]::Matches($indexContent, $pattern)
    foreach ($match in $found) {
        $relativePath = $match.Groups[1].Value
        $fullPath = Join-Path $WwwRootPath ($relativePath -replace '/', [IO.Path]::DirectorySeparatorChar)

        if (-not (Test-Path $fullPath)) {
            Write-Warning "File not found, skipping: $fullPath"
            continue
        }

        $hash = (Get-FileHash $fullPath -Algorithm SHA256).Hash.Substring(0, 8).ToLower()

        $ext = [IO.Path]::GetExtension($relativePath)
        $stem = $relativePath.Substring(0, $relativePath.Length - $ext.Length)
        $fingerprintedRelative = "${stem}.${hash}${ext}"
        $fingerprintedFull = Join-Path $WwwRootPath ($fingerprintedRelative -replace '/', [IO.Path]::DirectorySeparatorChar)

        Move-Item $fullPath $fingerprintedFull -Force

        # Rename pre-compressed variants (.br, .gz) to match
        foreach ($compExt in @('.br', '.gz')) {
            $compSource = "${fullPath}${compExt}"
            if (Test-Path $compSource) {
                Move-Item $compSource "${fingerprintedFull}${compExt}" -Force
            }
        }

        $indexContent = $indexContent.Replace($relativePath, $fingerprintedRelative)

        Write-Host "  $relativePath -> $fingerprintedRelative"
    }
}

Set-Content $indexPath $indexContent -NoNewline
Write-Host "Static asset fingerprinting complete."
