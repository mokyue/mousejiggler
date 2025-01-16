param([String]$ProcessId)

if (($null -eq $ProcessId) -or ("" -eq $ProcessId)) {
    $ProcessId = Read-Host "PID"
}

if (($null -eq $ProcessId) -or ("" -eq $ProcessId)) {
    Write-Output "Empty path"
    Exit
}

$PROJECT_DIR = Split-Path -Parent $MyInvocation.MyCommand.Definition
Write-Host -ForegroundColor Green "Project Directory: $PROJECT_DIR"
Set-Location $PROJECT_DIR

& "$PROJECT_DIR\inj64.exe" "$PROJECT_DIR\inj64.dll" 0 "$ProcessId"
