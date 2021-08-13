[![NuGet Version and Downloads count](https://buildstats.info/nuget/Platform.Data.Doublets.Json)](https://www.nuget.org/packages/Platform.Data.Doublets.Json)
[![Actions Status](https://github.com/linksplatform/Data.Doublets.Json/workflows/CD/badge.svg)](https://github.com/linksplatform/Data.Doublets.Json/actions?workflow=CD)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/cd23af97753f4dc2be394daeb2175042)](https://www.codacy.com/gh/linksplatform/Data.Doublets.Json/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=linksplatform/Data.Doublets.Json&amp;utm_campaign=Badge_Grade)
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
### [json2links](https://www.nuget.org/packages/json2links)
   #### SYNOPSIS
   ```shell
   json2links SOURCE DESTINATION [DOCUMENT_NAME]
   ```
   #### PARAMETERS
   * `SOURCE` - a json file path.
   * `DESTINATION` - a links storage path.
   * `DOCUMENT_NAME` - a document name. **Default**: `SOURCE`'s file name without extension.
   #### Note:
   `DOCUMENT_NAME` is used to define what name to save a document with. A links storage can contain multiple json documents.
   #### Example
1. Install
    ```shell
    dotnet tool install --global json2links
    ```
2. Import a json file from a doublets links storage
    ```shell
   json2links documents/enwiki.json databases/wikimedia.links "English Wikipedia"
   ```
---
### [links2json](https://www.nuget.org/packages/links2json)
#### SYNOPSIS
   ```shell
   links2json SOURCE DESTINATION [DOCUMENT_NAME]
   ```
#### PARAMETERS
* `SOURCE` - a links storage path.
* `DESTINATION` - a json file path.
* `DOCUMENT_NAME` - a document name. **Default**: `DESTINATION`'s file name without extension.
#### Note:
`DOCUMENT_NAME` is used to choose which json document to export from a links storage. A links storage can contain multiple json documents.
#### Example
1. Install
    ```shell
    dotnet tool install --global links2json
    ```
2. Export json file to doublets links storage
    ```shell
   links2json databases/wikimedia.links documents/enwiki.json "English Wikipedia"
   ```
