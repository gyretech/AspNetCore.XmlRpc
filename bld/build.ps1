#+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
# Script: build.ps1                                                         
# Usage: Scripts to build the library                                            
#+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
[CmdletBinding()]
Param(
	[Parameter(Mandatory=$false)]
    [string] $configuration="Release",
    [string] $versionSuffix="alpha.1"
)


$ScriptDirectory = Split-Path $MyInvocation.MyCommand.Path


# Get absolute path
function To-AbsolutePath([string] $Path)
{
    #$ScriptDirectory = Split-Path $MyInvocation.MyCommand.Path
    return Resolve-Path "$ScriptDirectory\$Path"
}


function Log-Info([string] $Msg)
{
    Write-Host $Msg;
}

function Log-Error([string] $Msg)
{
    Write-Error $Msg;
}

function Restore
{
    param([string] $Arguments) 
    process
    {
        Log-Info -Msg "Restoring projects started: $Arguments" 
        & dotnet restore $Arguments
        Log-Info -Msg "Restoring projects completed: $Arguments"
    }
}


function Build
{
    param([string] $Arguments) 
    process
    {
        Log-Info -Msg "Building projects started: $Arguments" 
        & dotnet build --configuration $configuration $Arguments
        Log-Info -Msg "Building projects completed: $Arguments"
    }
}

function Pack
{
    param([string] $Arguments) 
    process
    {
        Log-Info -Msg "Packing projects started: $Arguments" 
        & dotnet pack --configuration $configuration --version-suffix $versionSuffix $Arguments
        Log-Info -Msg "Packing projects completed: $Arguments"
    }
}


function Build-All
{
    $libPath = (To-AbsolutePath "..\src\XmlRpc\AspNetCore.XmlRpc.csproj")
    Restore -Arguments $libPath
    Build -Arguments $libPath
    Pack -Arguments $libPath
}

Build-All