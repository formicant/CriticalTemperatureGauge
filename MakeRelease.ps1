$name = "CriticalTemperatureGauge"
$srcDir = "GameData"
$dstDir = "Releases"

$verFile = "$srcDir\$name\$name.version"
$verJson = Get-Content $verFile -Raw | ConvertFrom-Json
$ver = $verJson.VERSION
$verString = "$($ver.MAJOR).$($ver.MINOR).$($ver.PATCH).$($ver.BUILD)"

$zipFile = "$dstDir\$name-$verString.zip"
Write-Host $zipFile

If (Test-Path $zipFile) {
  Read-Host "Press Enter to overwrite or Ctrl+Break to quit"
}

Compress-Archive $srcDir $zipFile -Force
