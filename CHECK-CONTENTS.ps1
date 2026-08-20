$required = @(
  ".github\workflows\build-windows.yml",
  "src\IPASignerPro\IPASignerPro.csproj",
  "src\IPASignerPro\Program.cs",
  "src\IPASignerPro\MainForm.cs",
  "src\IPASignerPro\app.manifest",
  "tools\PUT_SIDELOADER_HERE.txt"
)
$bad = $false
foreach ($f in $required) {
  if (Test-Path $f) { Write-Host "[OK] $f" -ForegroundColor Green }
  else { Write-Host "[MISSING] $f" -ForegroundColor Red; $bad = $true }
}
if ($bad) { exit 1 }
Write-Host "SOURCE PACKAGE COMPLETE" -ForegroundColor Green
