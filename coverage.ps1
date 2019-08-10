﻿if($env:APPVEYOR_PULL_REQUEST_NUMBER) {
	exit 0
} 

choco install opencover.portable --no-progress
choco install codecov --version 1.6.1 --no-progress

$testPath = Join-Path $PSScriptRoot "test\stashbox.tests.csproj"
$coverageReportDir = Join-Path $PSScriptRoot "coverageresults"

$arguments = "-returntargetcode", "-register:user", "`"-filter:+[*]Stashbox.* -[Stashbox.Tests]* -[Stashbox]*.Utils* -[Stashbox]*.Expressions.Compile*`"", "-target:dotnet.exe", "`"-targetargs:test $testPath -f net45 -c Release`"", "-output:coverage.xml", "-skipautoprops", "-hideskipped:All"
. OpenCover.Console.exe $arguments
. codecov -f coverage.xml