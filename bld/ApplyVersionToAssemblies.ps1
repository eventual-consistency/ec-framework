## Apply Go.CD Pipeline label to assemblies being built in the src folder adjacent to the folder containing this script.
##
## Originally based on:
##-----------------------------------------------------------------------
## <copyright file="ApplyVersionToAssemblies.ps1">(c) Microsoft Corporation. This source is subject to the Microsoft Permissive License. See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx. All other rights reserved.</copyright>
##-----------------------------------------------------------------------
# Look for a 0.0.0.0 pattern in the build number. 
# If found use it to version the assemblies. Assumes following structure
#
#  /bld/this-script.ps1
#  /src/your-projects-here
#
# For example, if the 'Build number format' build process parameter 
# $(BuildDefinitionName)_$(Year:yyyy).$(Month).$(DayOfMonth)$(Rev:.r)
# then your build numbers come out like this:
# "Build HelloWorld_2013.07.19.1"
# This script would then apply version 2013.07.19.1 to your assemblies.

# Enable -Verbose option
[CmdletBinding()]
$VersionRegex = "\d+\.\d+\.\d+\.\d+"
if(-not ($Env:GO_PIPELINE_LABEL))
{
    Write-Error "You must set the following environment variables"
    Write-Error "to test this script interactively."
    Write-Host '$Env:GO_PIPELINE_LABEL - For example, enter something like:'
    Write-Host '$Env:GO_PIPELINE_LABEL = "HelloWorld_0000.00.00.0"'
    exit 1
}

Write-Verbose "GO_PIPELINE_LABEL: $Env:GO_PIPELINE_LABEL"

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
$NewVersion = $VersionData[0]
Write-Verbose "Version: $NewVersion"

$scriptPath 	= $MyInvocation.MyCommand.Path
$scriptDir 		= Split-Path -Parent $scriptPath
$scanDirectory 		= $scriptDir + "\..\src\";

# Apply the version to the assembly property files
$files = gci $scanDirectory -recurse -include "*Properties*","My Project" | 
    ?{ $_.PSIsContainer } | 
    foreach { gci -Path $_.FullName -Recurse -include AssemblyInfo.* }
if($files)
{
    Write-Verbose "Will apply $NewVersion to $($files.count) files."

    foreach ($file in $files) {
        $filecontent = Get-Content($file)
        attrib $file -r
        $filecontent -replace $VersionRegex, $NewVersion | Out-File $file
        Write-Verbose "$file.FullName - version applied"
    }
}
else
{
    Write-Warning "Found no files."
}
