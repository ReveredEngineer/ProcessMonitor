$highCPU = 50       # Red if CPU exceeds this percentage
$mediumCPU = 30     # Yellow if CPU is between this and highCPU
$highMemory = 500   # Red if memory exceeds this in MB
$mediumMemory = 300 # Yellow if memory is between this and highMemory

$processName = Read-Host "Enter process name to monitor"

function Monitor-Process {
    while ($true) {
        Clear-Host
        Write-Host "Monitoring process: $processName`n"

        $process = Get-Process -Name $processName -ErrorAction SilentlyContinue

        if ($process) {
            foreach ($p in $process) {
                $cpu = (Get-Counter "\Process($($p.ProcessName))\% Processor Time").CounterSamples.CookedValue / $env:NUMBER_OF_PROCESSORS
                $mem = [math]::Round($p.WorkingSet64 / 1MB, 2)

                # Determine CPU color
                if ($cpu -gt $highCPU) {
                    $cpuColor = "Red"
                } elseif ($cpu -gt $mediumCPU) {
                    $cpuColor = "Yellow"
                } else {
                    $cpuColor = "Green"
                }

                if ($mem -gt $highMemory) {
                    $memColor = "Red"
                } elseif ($mem -gt $mediumMemory) {
                    $memColor = "Yellow"
                } else {
                    $memColor = "Green"
                }

                Write-Host ("Process: " + $p.ProcessName + " (PID: " + $p.Id + ")") -ForegroundColor Cyan
                Write-Host ("CPU: " + [math]::Round($cpu, 2) + "%") -ForegroundColor $cpuColor
                Write-Host ("Memory: " + $mem + " MB") -ForegroundColor $memColor
                Write-Host "------------------------------------"
            }
        } else {
            Write-Host "Process $processName not found!" -ForegroundColor Red
        }

        Start-Sleep -Seconds 5
    }
}

Monitor-Process
