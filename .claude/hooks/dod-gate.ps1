# SubagentStop hook: the implementer may not finish while a DoD command from docs/agents/dod.json fails,
# unless it reports itself stuck.
# Protocol: read the hook event JSON from stdin; print {"decision":"block","reason":...} to keep the subagent going.
# A failed environment check (e.g. Docker down) is not the implementer's to fix: report it, don't block.

$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'dod-common.ps1')

$hookInput = [Console]::In.ReadToEnd() | ConvertFrom-Json
$root = Find-DodRoot $hookInput.cwd
if (-not $root) {
    exit 0
}

$dod = Read-Dod $root
if ($hookInput.agent_type -ne $dod.implementerAgent) {
    exit 0
}

# The gate holds back a claim of success. An implementer that reports itself stuck (implementer contract) may
# stop on a red DoD: /ship sets the ticket to needs-info and puts the branch back to green.
if ("$($hookInput.last_assistant_message)".TrimStart().StartsWith('STATUS: stuck')) {
    exit 0
}

Push-Location $root
try {
    foreach ($check in $dod.environmentChecks) {
        $result = Invoke-DodStep $check
        if ($result.ExitCode -ne 0) {
            $message = "DoD gate skipped: environment check '$($check.name)' failed (exit $($result.ExitCode)): $($check.run)`n$($result.Output)"
            @{ systemMessage = $message } | ConvertTo-Json -Compress
            exit 0
        }
    }

    foreach ($command in $dod.commands) {
        $result = Invoke-DodStep $command
        if ($result.ExitCode -ne 0) {
            $reason = "DoD command '$($command.name)' failed (exit $($result.ExitCode)): $($command.run)`n$($result.Output)"
            @{ decision = 'block'; reason = $reason } | ConvertTo-Json -Compress
            exit 0
        }
    }
}
finally {
    Pop-Location
}

exit 0
