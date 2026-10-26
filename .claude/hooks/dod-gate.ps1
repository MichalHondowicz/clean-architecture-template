# SubagentStop hook: the implementer may not finish while a DoD command from docs/agents/dod.json fails.
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

# The reason goes into the implementer's context on every block, so it quotes only the tail of the output.
$tailLines = 100

# Each step runs in a child pwsh found on PATH, not the current process path: when pwsh is installed as a
# dotnet tool, the current process is dotnet.exe.
function Invoke-Step($step) {
    $lines = & pwsh -NoProfile -NonInteractive -Command $step.run 2>&1
    [pscustomobject]@{ ExitCode = $LASTEXITCODE; Output = ($lines | Select-Object -Last $tailLines | Out-String) }
}

Push-Location $root
try {
    foreach ($check in $dod.environmentChecks) {
        $result = Invoke-Step $check
        if ($result.ExitCode -ne 0) {
            $message = "DoD gate skipped: environment check '$($check.name)' failed (exit $($result.ExitCode)): $($check.run)`n$($result.Output)"
            @{ systemMessage = $message } | ConvertTo-Json -Compress
            exit 0
        }
    }

    foreach ($command in $dod.commands) {
        $result = Invoke-Step $command
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
