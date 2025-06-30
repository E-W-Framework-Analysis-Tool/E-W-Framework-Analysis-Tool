param(
    [switch]$Check
)

$repoRoot = Resolve-Path "$PSScriptRoot/.."
Push-Location $repoRoot

function Fail($message) {
    Write-Error $message
    Pop-Location
    exit 1
}

# Check for npm
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Fail "npm is not installed or not found in PATH. Please install Node.js (https://nodejs.org/)."
}

# Restore if needed
if (-not (Test-Path "node_modules")) {
    Write-Host "node_modules not found. Running 'npm ci'..."
    npm ci || Fail "Failed to restore npm dependencies."
}

if ($Check) {
    Write-Host "`nRunning: npm run check"
    npm run check || Fail "Markdown formatting or linting failed (check mode)."
} else {
    Write-Host "`nRunning: npm run format"
    npm run format || Fail "Markdown formatting failed."

    Write-Host "`nRunning: npm run lint"
    npm run lint || Fail "Markdown linting failed."
}

Pop-Location
Write-Host "Markdown lint check completed successfully."
