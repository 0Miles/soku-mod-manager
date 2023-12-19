# Updater
用于管理和执行模组的更新操作，包括下载、解压、替换文件等。

## 成员变量
`UpdaterStatusChanged`: 更新操作状态变化事件接口。
`AvailableUpdateList`: 存储可用的更新文件信息列表。
`AvailableInstallList`: 存储可用的安装文件信息列表。

## 事件
`UpdaterStatusChanged`: 更新操作状态变化事件，当更新操作状态发生变化时触发。

## 公共方法

- [初始化](#初始化)
    - 构造函数
- [获取更新文件信息](#获取更新文件信息)
    - GetUpdateFileInfosFromZip
    - GetUpdateFileInfosFromSource
- [更新操作](#更新操作)
    - RefreshAvailable
    - ExecuteUpdates

### 初始化
---
#### 构造函数
```csharp
public Updater(string? sokuDirFullPath = null)
public Updater(ModManager modManager)
```
初始化 Updater 类的实例  
可接收游戏目录完整路径 sokuDirFullPath 或 ModManager 类的实例作为参数。

### 获取更新文件信息
---

#### `GetUpdateFileInfosFromZip` 方法:
```csharp
public static List<UpdateFileInfoModel>? GetUpdateFileInfosFromZip(string path)
```
从压缩文件中获取更新文件信息。
根据传入的压缩文件路径，读取其中的 version.json 文件，解析并返回更新文件信息列表。

#### `GetUpdateFileInfosFromSource` 方法:
```csharp
public static List<UpdateFileInfoModel>? GetUpdateFileInfosFromSource(SourceModel source)
```
从模组来源中获取更新文件信息。
根据传入的 SourceModel 对象，生成更新文件信息列表并返回。

### 更新操作
---
#### `RefreshAvailable` 方法:
```csharp
public void RefreshAvailable(List<UpdateFileInfoModel> updateFileInfos, List<string>? checkOnlyTheseMods = null)
```
刷新可用的更新列表。
根据传入的更新文件信息列表和指定的模组名称列表，更新可用的更新和安装列表。

#### `ExecuteUpdates` 方法:
```csharp
public async Task<List<UpdateResultModel>> ExecuteUpdates(List<UpdateFileInfoModel> selectedUpdates)
```
执行选定的更新操作。
根据传入的更新文件信息列表，异步执行下载、解压和替换文件的操作，并返回更新操作的结果列表。
