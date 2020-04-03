# .NET Chainscript

[![NuGet](https://img.shields.io/nuget/v/Stratumn.Chainscript)](https://www.nuget.org/packages/Stratumn.Chainscript/)

Official .NET implementation of [ChainScript](https://github.com/stratumn/chainscript). This is the recommended way to use ChainScript in your .NET projects.

## Installation

Use the NuGet package manager console to install CanonicalJson. Note that this library is [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) compatible.

```bash
Install-Package Stratumn.CanonicalJson
```

## Development

Download the [.NET Core SDK](https://dotnet.microsoft.com/download) if you don't already have it.

```sh
# Install .NET dependencies
dotnet restore

# Start testing
dotnet test ChainscriptTest/ChainscriptTest.csproj
```

## Publishing to NuGet

From [this source](https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-the-dotnet-cli):
```sh
# Create the .nupkg package
dotnet pack --configuration release
# Publish it
dotnet nuget push Chainscript/bin/Release/Stratumn.Chainscript.<version>.nupkg -k <nuget_api_key> -s https://api.nuget.org/v3/index.json
```
