$ErrorActionPreference = "Stop"
$htmlPath = Join-Path $PSScriptRoot "terminologies-systems.html"
if (-not (Test-Path -LiteralPath $htmlPath)) {
  $htmlPath = "C:\Users\user\.cursor\projects\e-sjvann-FHIR-Solutions\uploads\terminologies-systems-0.html"
}
$outDir = Join-Path $PSScriptRoot "..\Data"
$outFile = Join-Path $outDir "internal-fhir-code-system-slugs.txt"
$html = Get-Content -Raw -LiteralPath $htmlPath
$m = [regex]::Matches($html, '\| \[([a-z0-9-]+)\]\(codesystem-')
$list = [System.Collections.Generic.List[string]]::new()
foreach ($x in $m) { [void]$list.Add($x.Groups[1].Value) }
$slugs = $list | Sort-Object -Unique
New-Item -ItemType Directory -Force -Path $outDir | Out-Null
$slugs | Set-Content -Encoding utf8 -LiteralPath $outFile
Write-Host "Wrote $($slugs.Count) slugs to $outFile"
$long = $slugs | Where-Object { $_.Length -gt 49 }
if ($long) { Write-Host "Slugs over 49 chars: $($long.Count)"; $long | Select-Object -First 5 }
