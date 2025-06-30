# Set working directory to repo root
$repoRoot = Resolve-Path "$PSScriptRoot/.."
Push-Location $repoRoot

# Helper function
function Fail($message) {
    Write-Error $message
    Pop-Location
    exit 1
}

# 1. Check if npm is installed
if (-not (Get-Command npm -ErrorAction SilentlyContinue)) {
    Fail "npm is not installed or not found in PATH. Please install Node.js (https://nodejs.org/) to continue."
}

# 2. Restore packages if node_modules does not exist
if (-not (Test-Path "node_modules")) {
    Write-Host "node_modules not found. Running 'npm ci' to restore dependencies..."
    npm ci || Fail "Failed to restore npm dependencies."
}

# 3. Run Prettier formatting
Write-Host "`nRunning: npm run format"
npm run format || Fail "Markdown formatting failed."

# 4. Run markdownlint
Write-Host "`nRunning: npm run lint"
npm run lint || Fail "Markdown linting failed."

Pop-Location
Write-Host "Markdown formatting and linting completed successfully."
