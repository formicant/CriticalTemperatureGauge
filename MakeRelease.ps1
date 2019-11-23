$name = "CriticalTemperatureGauge"
$srcDir = "GameData\$name"
$dstDir = "Releases"

$verFile = Get-Content "$srcDir\$name.version" -Raw | ConvertFrom-Json
$ver = $verFile.VERSION
$verString = "$($ver.MAJOR).$($ver.MINOR).$($ver.PATCH).$($ver.BUILD)"

$zipFile = "$dstDir\$name-$verString.zip"
Write-Host $zipFile

If (Test-Path $zipFile) {
  Read-Host "Press Enter to overwrite or Ctrl+Break to quit"
}

Compress-Archive $srcDir $zipFile -Force -CompressionLevel Optimal
