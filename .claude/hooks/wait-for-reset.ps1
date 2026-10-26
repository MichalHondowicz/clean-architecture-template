# Script for /ship, not a hook: waits out the plan's 5-hour window in the session that hit its threshold.
# /ship runs it in the background, and its exit wakes the session again.
# Usage: pwsh -File wait-for-reset.ps1 -Until <ISO 8601 time, the window's resetsAt> [-MarginSeconds 120]
#                                      [-MaxHours 6]
# Returns after -Until plus the margin, printing {"ok":true,"until","heldAwake"}. Exits 2 when -Until is not a
# time or lies more than -MaxHours ahead: a weekly reset is days away, and /ship ends the session for it.

param(
    [Parameter(Mandatory)] [AllowEmptyString()] [string]$Until,
    [int]$MarginSeconds = 120,
    [int]$MaxHours = 6
)

$ErrorActionPreference = 'Stop'

# Only a full ISO 8601 time with an offset is unambiguous: a local time read as UTC waits hours too long, and a
# bare '03:00' means today.
if ($Until -notmatch '^\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(:\d{2}(\.\d+)?)?(Z|[+-]\d{2}:\d{2})$') {
    [Console]::Error.WriteLine("-Until '$Until' needs a date and an offset, such as 2026-10-27T00:40:00Z.")
    exit 2
}
$reset = [DateTimeOffset]::MinValue
if (-not [DateTimeOffset]::TryParse($Until, [Globalization.CultureInfo]::InvariantCulture, 'None', [ref]$reset)) {
    [Console]::Error.WriteLine("-Until '$Until' is not a time.")
    exit 2
}
$deadline = $reset.AddSeconds($MarginSeconds)
if ($deadline -gt [DateTimeOffset]::UtcNow.AddHours($MaxHours)) {
    [Console]::Error.WriteLine("-Until $Until is more than $MaxHours hours ahead; only the 5-hour window is waited out.")
    exit 2
}

# The app holds the computer awake only while the session works, and Windows idle sleep would stop the clock
# here. ES_CONTINUOUS | ES_SYSTEM_REQUIRED keeps it awake until this process ends; no setting changes.
$heldAwake = $false
if ($IsWindows) {
    Add-Type -Namespace WaitForReset -Name Power -MemberDefinition @'
[DllImport("kernel32.dll")] public static extern uint SetThreadExecutionState(uint esFlags);
'@
    $heldAwake = [WaitForReset.Power]::SetThreadExecutionState(0x80000001u) -ne 0
}

while (($remaining = $deadline - [DateTimeOffset]::UtcNow) -gt [TimeSpan]::Zero) {
    Start-Sleep -Milliseconds ([Math]::Min($remaining.TotalMilliseconds, 30000))
}

[ordered]@{ ok = $true; until = $deadline.ToString('o'); heldAwake = $heldAwake } | ConvertTo-Json -Compress
exit 0
