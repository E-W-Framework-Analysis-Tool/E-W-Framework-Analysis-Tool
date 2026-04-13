param(
  [string]$BaseUrl = "http://localhost:5000",
  [string]$Scenario = "GrandBend",
  [string]$ClientId = "RvcohKz9zHI4",
  [string]$ClientSecret = "E1676E88-4D3B-4E4E-B7B7-7C3F8E5D2A9C",
  [int]$EducationOrganizationId = 255901001,
  [int]$MaxStudents = 10
)

$ErrorActionPreference = "Stop"

function Get-OAuthToken {
  Write-Host "Fetching OAuth token..."
  $body = "grant_type=client_credentials&client_id=$ClientId&client_secret=$ClientSecret"
  $response = Invoke-RestMethod `
    -Uri "$BaseUrl/$Scenario/oauth/token" `
    -Method Post `
    -ContentType "application/x-www-form-urlencoded" `
    -Body $body
  return $response.access_token
}

function Get-AuthHeaders($token) {
  return @{
    "Authorization" = "Bearer $token"
    "Accept"        = "application/json"
    "Content-Type"  = "application/json"
  }
}

function Invoke-EdFiGet($uri, $headers) {
  $response = Invoke-WebRequest -Uri $uri -Method Get -Headers $headers
  return @(($response.Content | ConvertFrom-Json))
}

function Build-DescriptorUri($item) {
  $namespace = $item.namespace
  $codeValue = $item.codeValue
  return "${namespace}#${codeValue}"
}

# Authenticate
$token = Get-OAuthToken
$headers = Get-AuthHeaders $token
Write-Host "Authenticated successfully."

# -------------------------------------------------------------------
# Fetch all descriptor types
# -------------------------------------------------------------------

# Race descriptors
Write-Host "`nFetching race descriptors..."
$raceDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/raceDescriptors?limit=100&offset=0" $headers
$raceDescriptors = @($raceDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })

if ($raceDescriptors.Count -eq 0) {
  Write-Error "No race descriptors found. Seed descriptors first."
  exit 1
}
Write-Host "  Found $($raceDescriptors.Count) race descriptor(s)"

# Sex descriptors
Write-Host "Fetching sex descriptors..."
$sexDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/sexDescriptors?limit=100&offset=0" $headers
$sexDescriptors = @($sexDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })
Write-Host "  Found $($sexDescriptors.Count) sex descriptor(s)"

# Disability descriptors
Write-Host "Fetching disability descriptors..."
$disabilityDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/disabilityDescriptors?limit=100&offset=0" $headers
$disabilityDescriptors = @($disabilityDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })
Write-Host "  Found $($disabilityDescriptors.Count) disability descriptor(s)"

# Student characteristic descriptors (used for income, homelessness, military)
Write-Host "Fetching student characteristic descriptors..."
$characteristicDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/studentCharacteristicDescriptors?limit=100&offset=0" $headers
$allCharacteristicDescriptors = @($characteristicDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })

$econDisadvDescriptor = $allCharacteristicDescriptors | Where-Object { $_ -match "Economic Disadvantaged" } | Select-Object -First 1
$homelessDescriptor = $allCharacteristicDescriptors | Where-Object { $_ -match "Homeless" } | Select-Object -First 1
$militaryDescriptor = $allCharacteristicDescriptors | Where-Object { $_ -match "Military" } | Select-Object -First 1

Write-Host "  Found $($allCharacteristicDescriptors.Count) characteristic descriptor(s)"
Write-Host "    Economic Disadvantaged: $(if ($econDisadvDescriptor) { 'found' } else { 'NOT FOUND' })"
Write-Host "    Homeless: $(if ($homelessDescriptor) { 'found' } else { 'NOT FOUND' })"
Write-Host "    Military: $(if ($militaryDescriptor) { 'found' } else { 'NOT FOUND' })"

# Limited English proficiency descriptors
Write-Host "Fetching limited English proficiency descriptors..."
$lepDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/limitedEnglishProficiencyDescriptors?limit=100&offset=0" $headers
$lepDescriptors = @($lepDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })
Write-Host "  Found $($lepDescriptors.Count) LEP descriptor(s)"

# Language descriptors
Write-Host "Fetching language descriptors..."
$languageDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/languageDescriptors?limit=100&offset=0" $headers
$languageDescriptors = @($languageDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })
Write-Host "  Found $($languageDescriptors.Count) language descriptor(s)"

# Language use descriptors (find "Home" use)
Write-Host "Fetching language use descriptors..."
$languageUseDescriptorResponse = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/languageUseDescriptors?limit=100&offset=0" $headers
$allLanguageUseDescriptors = @($languageUseDescriptorResponse | ForEach-Object { Build-DescriptorUri $_ })
$homeLanguageUseDescriptor = $allLanguageUseDescriptors | Where-Object { $_ -match "Home" } | Select-Object -First 1

Write-Host "  Found $($allLanguageUseDescriptors.Count) language use descriptor(s)"
Write-Host "    Home language use: $(if ($homeLanguageUseDescriptor) { 'found' } else { 'NOT FOUND' })"

# -------------------------------------------------------------------
# Fetch students
# -------------------------------------------------------------------
Write-Host "`nFetching students (limit=$MaxStudents)..."
$students = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/students?limit=$MaxStudents&offset=0" $headers

if ($students.Count -eq 0) {
  Write-Error "No students found. Check your BaseUrl and Scenario."
  exit 1
}
Write-Host "Found $($students.Count) student(s)."

# -------------------------------------------------------------------
# Create associations with all disaggregate data
# -------------------------------------------------------------------
$created = 0
$skipped = 0
$failed = 0

for ($i = 0; $i -lt $students.Count; $i++) {
  $student = $students[$i]
  $studentUniqueId = $student.studentUniqueId
  $firstName = $student.firstName
  $lastName = $student.lastSurname

  # --- Race (100% of students) ---
  $primaryDescriptor = $raceDescriptors[$i % $raceDescriptors.Count]
  $races = @(
    @{ raceDescriptor = $primaryDescriptor }
  )
  # Every 3rd student gets a second race
  if ($i % 3 -eq 2 -and $raceDescriptors.Count -gt 1) {
    $secondaryDescriptor = $raceDescriptors[($i + 1) % $raceDescriptors.Count]
    $races += @{ raceDescriptor = $secondaryDescriptor }
  }

  # --- Gender (100% of students) ---
  $sexDescriptor = $null
  if ($sexDescriptors.Count -gt 0) {
    $sexDescriptor = $sexDescriptors[$i % $sexDescriptors.Count]
  }

  # --- Disability (~30% of students: every 3rd starting at index 2) ---
  $disabilities = @()
  if ($disabilityDescriptors.Count -gt 0 -and $i % 3 -eq 2) {
    $disabilities += @{
      disabilityDescriptor = $disabilityDescriptors[$i % $disabilityDescriptors.Count]
    }
  }

  # --- Student Characteristics: income (~40%), homelessness (~10%), military (~15%) ---
  $studentCharacteristics = @()
  # Economic Disadvantaged: every 2nd and 3rd student (indices 0,2,4,6,... roughly 40%)
  if ($econDisadvDescriptor -and $i % 5 -lt 2) {
    $studentCharacteristics += @{
      studentCharacteristicDescriptor = $econDisadvDescriptor
    }
  }
  # Homeless: every 10th student
  if ($homelessDescriptor -and $i % 10 -eq 3) {
    $studentCharacteristics += @{
      studentCharacteristicDescriptor = $homelessDescriptor
    }
  }
  # Military: roughly every 7th student
  if ($militaryDescriptor -and $i % 7 -eq 1) {
    $studentCharacteristics += @{
      studentCharacteristicDescriptor = $militaryDescriptor
    }
  }

  # --- English Learner (~33%: every 3rd student) ---
  $lepDescriptor = $null
  if ($lepDescriptors.Count -gt 0 -and $i % 3 -eq 1) {
    $lepDescriptor = $lepDescriptors[$i % $lepDescriptors.Count]
  }

  # --- Home Language (100% of students) ---
  $languages = @()
  if ($languageDescriptors.Count -gt 0 -and $homeLanguageUseDescriptor) {
    $languages += @{
      languageDescriptor = $languageDescriptors[$i % $languageDescriptors.Count]
      uses = @(
        @{ languageUseDescriptor = $homeLanguageUseDescriptor }
      )
    }
  }

  # --- Migrant (~25%: every 4th student) ---
  $isMigrant = ($i % 4 -eq 0)
  $studentIndicators = @()
  if ($isMigrant) {
    $studentIndicators += @{
      indicatorName = "Migrant"
      indicator     = "true"
    }
  }

  # --- Build the association ---
  $association = @{
    educationOrganizationReference = @{
      educationOrganizationId = $EducationOrganizationId
    }
    studentReference = @{
      studentUniqueId = $studentUniqueId
    }
    races = $races
  }

  if ($sexDescriptor) {
    $association["sexDescriptor"] = $sexDescriptor
  }
  if ($disabilities.Count -gt 0) {
    $association["disabilities"] = $disabilities
  }
  if ($studentCharacteristics.Count -gt 0) {
    $association["studentCharacteristics"] = $studentCharacteristics
  }
  if ($lepDescriptor) {
    $association["limitedEnglishProficiencyDescriptor"] = $lepDescriptor
  }
  if ($languages.Count -gt 0) {
    $association["languages"] = $languages
  }
  if ($studentIndicators.Count -gt 0) {
    $association["studentIndicators"] = $studentIndicators
  }

  $body = $association | ConvertTo-Json -Depth 5

  # Build log label
  $raceLabels = ($races | ForEach-Object { ($_.raceDescriptor -split '#')[1] }) -join ", "
  $tags = @()
  if ($sexDescriptor) { $tags += "Gender:$(($sexDescriptor -split '#')[1])" }
  if ($disabilities.Count -gt 0) { $tags += "Disability" }
  if ($studentCharacteristics.Count -gt 0) {
    $charLabels = ($studentCharacteristics | ForEach-Object { ($_.studentCharacteristicDescriptor -split '#')[1] }) -join "+"
    $tags += "Chars:$charLabels"
  }
  if ($lepDescriptor) { $tags += "EL:$(($lepDescriptor -split '#')[1])" }
  if ($languages.Count -gt 0) { $tags += "HomeLang:$(($languages[0].languageDescriptor -split '#')[1])" }
  if ($isMigrant) { $tags += "Migrant" }

  $tagStr = if ($tags.Count -gt 0) { " [$($tags -join ', ')]" } else { "" }
  Write-Host "  Creating association for $firstName $lastName ($studentUniqueId) - Race: $raceLabels$tagStr"

  try {
    Invoke-RestMethod `
      -Uri "$BaseUrl/$Scenario/data/v3/ed-fi/studentEducationOrganizationAssociations" `
      -Method Post `
      -Headers $headers `
      -Body $body | Out-Null
    $created++
  }
  catch {
    $statusCode = $_.Exception.Response.StatusCode.value__
    if ($statusCode -eq 409) {
      Write-Host "    -> Already exists, skipping."
      $skipped++
    }
    else {
      Write-Warning "    -> Failed ($statusCode): $_"
      $failed++
    }
  }
}

Write-Host "`nDone. Created: $created, Skipped (already existed): $skipped, Failed: $failed"

# -------------------------------------------------------------------
# Verify by fetching associations
# -------------------------------------------------------------------
Write-Host "`nVerifying - fetching studentEducationOrganizationAssociations..."
$associations = Invoke-EdFiGet `
  "$BaseUrl/$Scenario/data/v3/ed-fi/studentEducationOrganizationAssociations?limit=$MaxStudents&offset=0" $headers

$withRace = @($associations | Where-Object { $_.races -and $_.races.Count -gt 0 }).Count
$withGender = @($associations | Where-Object { $_.sexDescriptor }).Count
$withDisability = @($associations | Where-Object { $_.disabilities -and $_.disabilities.Count -gt 0 }).Count
$withCharacteristics = @($associations | Where-Object { $_.studentCharacteristics -and $_.studentCharacteristics.Count -gt 0 }).Count
$withEL = @($associations | Where-Object { $_.limitedEnglishProficiencyDescriptor }).Count
$withLanguage = @($associations | Where-Object { $_.languages -and $_.languages.Count -gt 0 }).Count
$withMigrant = @($associations | Where-Object {
  $_.studentIndicators | Where-Object {
    $_.indicatorName -match "Migrant" -and $_.indicator -eq "true"
  }
}).Count

Write-Host "Associations returned: $($associations.Count)"
Write-Host "  With race data:        $withRace"
Write-Host "  With gender:           $withGender"
Write-Host "  With disability:       $withDisability"
Write-Host "  With characteristics:  $withCharacteristics (income/homeless/military)"
Write-Host "  With English learner:  $withEL"
Write-Host "  With home language:    $withLanguage"
Write-Host "  With migrant status:   $withMigrant"
