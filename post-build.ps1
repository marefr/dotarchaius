# upload results to AppVeyor

$testResults = ".\TestResult.xml"

if ((Test-Path $testResults)) {
    $wc = New-Object 'System.Net.WebClient'
    $wc.UploadFile("https://ci.appveyor.com/api/testresults/nunit3/$($env:APPVEYOR_JOB_ID)", (Resolve-Path $testResults))
}