# NuGetCoreSymbolLoader

## Introduction
This is a tool used to extract NuGet.Core symbols from the NuGet repository build.

## Instructions

* git clone https://github.com/mishra14/NuGetCoreSymbolLoader.git
* cd NuGetCoreSymbolLoader
* `msbuild`
* `.NuGetCoreSymbolLoader\bin\Debug\NuGetCoreSymbolLoader.exe "Path\to\NuGet\root"`

## Output
The tool places the pdb files at `"Path\to\NuGet\root\artifacts\VS15\symbols"`
The tool places the src files at `"Path\to\NuGet\root\artifacts\VS15\symbols\src\nupkg_name"`
