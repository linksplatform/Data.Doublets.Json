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

## Manual
### [json2links](https://www.nuget.org/packages/json2links)
   #### SYNOPSIS
   ```shell
   json2links SOURCE DESTINATION [DOCUMENT_NAME]
   ```
   #### PARAMETERS
   * `SOURCE` - a json file path.
   * `DESTINATION` - a links storage path.
   * `DOCUMENT_NAME` - a document name. **Default**: `SOURCE`'s file name.
   #### Note:
   `DOUCMENT_NAME` parameter is required to export json from a links storage. This parameter exists because links storage can contain more than 1 json documents.
   #### Example
1. Install tools.
    ```shell
    dotnet tool install --global json2links
    ```
2. Import json file to doublets links storage.
    ```shell
   json2links /home/json.json /home/links tsconfig
   ```
---
### [links2json](https://www.nuget.org/packages/links2json)
#### SYNOPSIS
   ```shell
   links2json SOURCE DESTINATION [DOCUMENT_NAME]
   ```
#### PARAMETERS
* `SOURCE` - a json file path.
* `DESTINATION` - a links storage path.
* `DOCUMENT_NAME` - a document name. **Default**: `DESTINATION`'s file name.
#### Note:
`DOUCMENT_NAME` parameter is required to export json from a links storage. This parameter exists because links storage can contain more than 1 json documents.
#### Example
1. Install tools.
    ```shell
    dotnet tool install --global links2json
    ```
2. Import json file to doublets links storage.
    ```shell
   links2json /home/links /home/tsconfig.json tsconfig
   ```
