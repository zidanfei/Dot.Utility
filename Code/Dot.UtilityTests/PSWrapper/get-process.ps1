
param(
    [string]$processName=$(throw "process name is required.")
    )
 get-process $processName* –fileversioninfo