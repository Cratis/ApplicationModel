# Cratis Application Model

## Packages / Deployables

[![Nuget](https://img.shields.io/nuget/v/Cratis.Applications?logo=nuget)](http://nuget.org/packages/cratis.applications)
[![NPM](https://img.shields.io/npm/v/@cratis/applications?label=@cratis/applications&logo=npm)](https://www.npmjs.com/package/@cratis/applications)

## Builds

[![.NET Build](https://github.com/cratis/ApplicationModel/actions/workflows/dotnet-build.yml/badge.svg)](https://github.com/cratis/ApplicationModel/actions/workflows/dotnet-build.yml)
[![JavaScript Build](https://github.com/cratis/ApplicationModel/actions/workflows/javascript-build.yml/badge.svg)](https://github.com/cratis/ApplicationModel/actions/workflows/javascript-build.yml)
[![Documentation site](https://github.com/Cratis/Documentation/actions/workflows/pages.yml/badge.svg)](https://github.com/Cratis/Documentation/actions/workflows/pages.yml)

## Description

The Cratis Application model represents an opinionated approach to building consistent applications based on the concepts behind CQRS.
It offers extensions for different frameworks and is built on top of ASP.NET Core. One of the traits the application model has is the
bridging between the backend and the frontend. The application model provides a tool, called **ProxyGenerator** that generates TypeScript
code for recognized artifacts matching the criteria of what is considered a **commmand** or a **query**.

## Contributing

If you want to jump into building this repository and possibly contributing, please refer to [contributing](./Documentation/contributing/index.md).

### Prerequisites

The following are prerequisites to work with this repository.

* [.NET 8+](https://dotnet.microsoft.com/en-us/).
* [Node 16+](https://nodejs.org/en)
* [Yarn](https://yarnpkg.com)

### Central Package Management

This repository leverages [Central Package Management](https://learn.microsoft.com/en-us/nuget/consume-packages/Central-Package-Management), which
means that all package versions are managed from a file at the root level called [Directory.Packages.props](./Directory.Packages.props).

In addition there are also [Directory.Build.props](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022#directorybuildprops-and-directorybuildtargets) files for
setting up common settings that are applied cross cuttingly.

### Root package.json

The `package.json` found at the root level defines all the workspaces. It is assumed

All developer dependencies are defined in the top level `package.json`. The reason for this is to be able to provide global scripts
for every package to use for easier maintenance.

The `package.json` found at the top level contains scripts that can then be used in a child project for this to work properly.

In a package, all you need to do is to define the scripts to use the global scripts in the `package.json´ of that project:

```json
{
    "scripts": {
        "prepublish": "yarn g:build",
        "clean": "yarn g:clean",
        "build": "yarn g:build",
        "lint": "yarn g:lint",
        "lint:ci": "yarn g:lint:ci",
        "test": "yarn g:test",
        "ci": "yarn g:ci",
        "up": "yarn g:up"
    }
}
```
