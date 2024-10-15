dotnet publish -c Release -r win-x64 /p:PublishReadyToRun=false /p:TieredCompilation=false --self-contained ..\TimeTracker.Worker\

$exePath = "C:\Personal\TimeTracker\TimeTracker.Api\bin\Release\net8.0\win-x86\publish\TimeTracker.Api.exe" 

sc create TimeTrackingService binPath=$exePath
sc config TimeTrackingService binPath=$exePath

sc start TimeTrackingService
