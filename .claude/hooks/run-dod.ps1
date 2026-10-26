# Script for /ship's pre-flight and the verifier, not a hook: runs one part of docs/agents/dod.json.
# Usage, inside the repository: pwsh -File run-dod.ps1 -Check environment|commands [-TimeoutSeconds 300]
#                               pwsh -File run-dod.ps1 -Check quota -FiveHourPercent <n> -WeeklyPercent <n>
#   environment: every environment check; a failing check with a `start` command is started, then re-checked
#                until it passes or the timeout runs out.
#   commands:    the DoD commands in order, stopping at the first failure.
#   quota:       the plan usage /ship read, against the thresholds in `quota`, all percentages from 0 to 100;
#                a window at or above its threshold fails as {"name":"weekly"|"fiveHour","used","threshold"}.
# Prints {"ok":bool,"failed":[{"name","exitCode","output"}]}; exits 1 when something failed, 2 when the DoD
# file is missing or unreadable, or the usage or thresholds a quota check needs are missing.

param(
    [Parameter(Mandatory)] [ValidateSet('environment', 'commands', 'quota')] [string]$Check,
    [int]$TimeoutSeconds = 300,
    [string]$FiveHourPercent,
    [string]$WeeklyPercent
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
$problems = @(Get-DodProblems $dod)
if ($problems) {
    [Console]::Error.WriteLine("$DodRelativePath is incomplete: $($problems -join '; ').")
    exit 2
}

$failed = [Collections.Generic.List[object]]::new()

# A usage argument as a number from 0 to 100, decimals kept; nothing when it is anything else.
function ConvertTo-Percentage([string]$text) {
    $number = 0.0
    $styles = [Globalization.NumberStyles]::Float
    if ([double]::TryParse($text, $styles, [Globalization.CultureInfo]::InvariantCulture, [ref]$number) -and
        $number -ge 0 -and $number -le 100) {
        $number
    }
}

if ($Check -eq 'quota') {
    $weeklyUsed = ConvertTo-Percentage $WeeklyPercent
    $fiveHourUsed = ConvertTo-Percentage $FiveHourPercent
    $quotaProblems = @(
        if ($null -eq $weeklyUsed) { "-WeeklyPercent '$WeeklyPercent' is missing or not a percentage" }
        if ($null -eq $fiveHourUsed) { "-FiveHourPercent '$FiveHourPercent' is missing or not a percentage" }
        foreach ($field in 'weeklyPercent', 'fiveHourPercent') {
            $value = $dod.quota.$field
            $isNumber = $value -is [long] -or $value -is [int] -or $value -is [double] -or $value -is [decimal]
            if (-not $isNumber -or $value -lt 0 -or $value -gt 100) {
                "quota.$field in $DodRelativePath is missing or not a percentage"
            }
        }
    )
    if ($quotaProblems) {
        [Console]::Error.WriteLine("Cannot check the quota: $($quotaProblems -join '; ').")
        exit 2
    }

    # Weekly first: /ship treats a weekly stop as the stronger one when both windows are full.
    $windows = @(
        @{ name = 'weekly'; used = $weeklyUsed; threshold = $dod.quota.weeklyPercent }
        @{ name = 'fiveHour'; used = $fiveHourUsed; threshold = $dod.quota.fiveHourPercent }
    )
    foreach ($window in $windows) {
        if ($window.used -ge $window.threshold) {
            $failed.Add([ordered]@{ name = $window.name; used = $window.used; threshold = $window.threshold })
        }
    }
    [ordered]@{ ok = $failed.Count -eq 0; failed = @($failed) } | ConvertTo-Json -Depth 5 -Compress
    exit ($failed.Count -eq 0 ? 0 : 1)
}

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
        # Waits while the DoD gate (or another run) is building and testing this repository.
        Invoke-DodExclusive $root {
            foreach ($command in $dod.commands) {
                $result = Invoke-DodStep $command
                if ($result.ExitCode -ne 0) {
                    $failed.Add([ordered]@{ name = $command.name; exitCode = $result.ExitCode; output = $result.Output })
                    break
                }
            }
        }
    }
}
finally {
    Pop-Location
}

[ordered]@{ ok = $failed.Count -eq 0; failed = @($failed) } | ConvertTo-Json -Depth 5 -Compress
exit ($failed.Count -eq 0 ? 0 : 1)
