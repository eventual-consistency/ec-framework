$VersionRegex 		= "\d+\.\d+\.\d+\.\d+"
Write-Verbose "BUILD_BUILDNUMBER: $Env:GO_PIPELINE_LABEL"
# Get and validate the version data
$VersionData = [regex]::matches($Env:GO_PIPELINE_LABEL,$VersionRegex)
switch($VersionData.Count)
{
   0        
      { 
         Write-Error "Could not find version number data in GO_PIPELINE_LABEL."
         exit 1
      }
   1 {}
   default 
      { 
         Write-Warning "Found more than instance of version data in GO_PIPELINE_LABEL." 
         Write-Warning "Will assume first instance is version."
      }
}
$NewVersion 		= $VersionData[0]
Write-Verbose 		"Version: $NewVersion"

$scriptPath 		= $MyInvocation.MyCommand.Path
$scriptDir 		= Split-Path -Parent $scriptPath
$nugetPath 		= $scriptDir + "\nuget.exe"
$nugetUri		= "http://eventualconsistency-nuget.trafficmanager.net/nuget/packages";

$scanDirectory 		= $scriptDir + "\..\src\";
$outputDirectory 	= $scriptDir + "\..\bld\";
Write-Host $scanDirectory 

Write-Host "Packing projects for solution."
$packageSpecs = Get-ChildItem -Recurse -Path $scanDirectory -Filter *.nuspec 
foreach ($spec in $packageSpecs)
{
	
	Write-Host "   Rewriting dependancies:" $spec.FullName
	(get-content $spec.FullName) | foreach-object {$_ -replace "_SOLUTIONVERSION_", $NewVersion} | set-content $spec.FullName
	
	Write-Host "   Packing nuspec file:" $spec.FullName
	& $nugetPath pack $spec.FullName -Version $NewVersion -O $outputDirectory 
}
