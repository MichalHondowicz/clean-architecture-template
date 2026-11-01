# Script for /ship, not a hook: tells whether Windows is waiting to restart, which would stop an unattended run.
# A planned update restart came in the middle of a five-hour wait, inside the active hours, so /ship warns
# before it starts and before it waits instead of relying on them.
# Usage: pwsh -File check-reboot.ps1 [-KeysRoot <path>]
#   -KeysRoot: where the restart keys live; the registry by default, a folder in the tests.
# Prints {"pending":bool,"keys":[key]}; exits 1 when a restart is pending, 0 otherwise (also off Windows).

param(
    [string]$KeysRoot = 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion'
)

$ErrorActionPreference = 'Stop'

$restartKeys = @(
    'WindowsUpdate\Auto Update\RebootRequired'
    'Component Based Servicing\RebootPending'
)

# Not Join-Path: off Windows it throws on the missing HKLM drive, where Test-Path just answers false.
$separator = [IO.Path]::DirectorySeparatorChar
$pendingKeys = @($restartKeys | Where-Object {
        $path = (@($KeysRoot.TrimEnd('\', '/')) + ($_ -split '\\')) -join $separator
        Test-Path -LiteralPath $path
    })

[ordered]@{ pending = $pendingKeys.Count -gt 0; keys = $pendingKeys } | ConvertTo-Json -Compress
exit ($pendingKeys.Count -gt 0 ? 1 : 0)
