[![NuGet Version and Downloads count](https://buildstats.info/nuget/Platform.Data.Doublets.Json)](https://www.nuget.org/packages/Platform.Data.Doublets.Json)
[![Actions Status](https://github.com/linksplatform/Data.Doublets.Json/workflows/CD/badge.svg)](https://github.com/linksplatform/Data.Doublets.Json/actions?workflow=CD)
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/b38e839402d9451aa3e58fe05521325f)](https://app.codacy.com/gh/linksplatform/Data.Doublets.Json?utm_source=github.com&utm_medium=referral&utm_content=linksplatform/Data.Doublets.Json&utm_campaign=Badge_Grade_Settings)
[![CodeFactor](https://www.codefactor.io/repository/github/linksplatform/Data.Doublets.Json/badge)](https://www.codefactor.io/repository/github/linksplatform/Data.Doublets.Json)

# [Data.Doublets.Json](https://github.com/linksplatform/Data.Doublets.Json)

LinksPlatform's Platform.Data.Doublets.Json Class Library.

Namespace: [Platform.Data.Doublets.Json](https://linksplatform.github.io/Data.Doublets.Json/csharp/api/Platform.Data.Doublets.Json.html)

NuGet package: [Platform.Data.Doublets.Json](https://www.nuget.org/packages/Platform.Data.Doublets.Json)

## [Documentation](https://linksplatform.github.io/Data.Doublets.Json)
Interface [IJsonStorage<TLink>](https://linksplatform.github.io/Data/csharp/api/IJsonStorage.html).

[PDF file](https://linksplatform.github.io/Data.Doublets.Json/csharp/Platform.Data.Doublets.Json.pdf) with code for e-readers.

## Depends on
*   [Platform.Data.Doublets](https://github.com/linksplatform/Data.Doublets)

## Tools
- [json2links](https://www.nuget.org/packages/json2links)
- [links2json](https://www.nuget.org/packages/links2json)

# Tutorial
1. Create tool-manifest that will contain tools.
    ```shell
    dotnet new tool-manifest
    ```
2. Install tools.
    ```shell
    dotnet tool install --local json2links
    dotnet tool install --local links2json
    ```
3. Import or export your json.
    ```shell
   dotnet json2links path2json path2linksDb documentName
   // or dotnet tool run json2links path2json path2linksDb documentName
   
   dotnet links2json path2linksDb path2json documentName
   // or dotnet tool run links2json path2linksDb path2json documentName
   ```
   Note: `documentName` is optional. **Defaul value**: json file name.
