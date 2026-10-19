$ErrorActionPreference = 'Stop'
$assemblyPath = Join-Path $PSScriptRoot '..\Theater.Common\bin\Release\Theater.Common.dll'
if (-not (Test-Path -LiteralPath $assemblyPath)) {
    throw 'Build TheaterProject.sln in Visual Studio first.'
}

Add-Type -Path $assemblyPath
$databaseName = ConvertFrom-Json '"\u0422\u0435\u0430\u0442\u0440.accdb"'
$env:THEATER_DATABASE_PATH = Join-Path $PSScriptRoot $databaseName
[Theater.Common.Db]::Initialize()
Write-Output ([Theater.Common.Db]::DatabasePath)
