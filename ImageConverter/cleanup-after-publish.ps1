param(
    [Parameter(Mandatory = $true)]
    [string]$RidRoot,
    [Parameter(Mandatory = $true)]
    [string]$KeepFolder
)

if (-not (Test-Path -LiteralPath $RidRoot)) {
    exit 0
}

Get-ChildItem -LiteralPath $RidRoot -Force |
    Where-Object { $_.Name -ne $KeepFolder } |
    Remove-Item -Recurse -Force

Write-Host "Cleaned $RidRoot (kept $KeepFolder\ only)."
