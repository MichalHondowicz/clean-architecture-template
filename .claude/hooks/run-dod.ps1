# Script for /ship's pre-flight and the verifier, not a hook: runs one part of docs/agents/dod.json.
# Usage, inside the repository: pwsh -File run-dod.ps1 -Check environment|commands [-TimeoutSeconds 300]
#   environment: every environment check; a failing check with a `start` command is started, then re-checked
#                until it passes or the timeout runs out.
#   commands:    the DoD commands in order, stopping at the first failure.
# Prints {"ok":bool,"failed":[{"name","exitCode","output"}]}; exits 1 when something failed, 2 when the DoD
# file is missing or unreadable.

param(
    [Parameter(Mandatory)] [ValidateSet('environment', 'commands')] [string]$Check,
    [int]$TimeoutSeconds = 300
)

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'dod-common.ps1')

$root = Find-DodRoot (Get-Location).Path
if (-not $root) {
    [Console]::Error.WriteLine("No $DodRelativePath at or above $(Get-Location).")
    exit 2
}
try {
    $dod = Read-Dod $root
}
catch {
    [Console]::Error.WriteLine("$DodRelativePath is not valid JSON: $_")
    exit 2
}

$failed = [Collections.Generic.List[object]]::new()

Push-Location $root
try {
    if ($Check -eq 'environment') {
        # PowerShell variable names ignore case: a loop variable named $check would overwrite -Check.
        foreach ($environmentCheck in $dod.environmentChecks) {
            $result = Invoke-DodStep $environmentCheck
            if ($result.ExitCode -ne 0 -and $environmentCheck.start) {
                Invoke-DodStep ([pscustomobject]@{ run = $environmentCheck.start }) | Out-Null
                $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
                do {
                    $result = Invoke-DodStep $environmentCheck
                    if ($result.ExitCode -eq 0 -or (Get-Date) -ge $deadline) {
                        break
                    }
                    Start-Sleep -Seconds 5
                } while ($true)
            }
            if ($result.ExitCode -ne 0) {
                $failed.Add([ordered]@{ name = $environmentCheck.name; exitCode = $result.ExitCode; output = $result.Output })
            }
        }
    }
    else {
        foreach ($command in $dod.commands) {
            $result = Invoke-DodStep $command
            if ($result.ExitCode -ne 0) {
                $failed.Add([ordered]@{ name = $command.name; exitCode = $result.ExitCode; output = $result.Output })
                break
            }
        }
    }
}
finally {
    Pop-Location
}

[ordered]@{ ok = $failed.Count -eq 0; failed = @($failed) } | ConvertTo-Json -Depth 5 -Compress
exit ($failed.Count -eq 0 ? 0 : 1)
