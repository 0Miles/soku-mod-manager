# ModManager
用于管理和操作 SWRSToys 的游戏模组，提供了搜索、加载、保存模组设置等相关操作。

## 成员变量
`ModInfoList`: 存储 ModInfoModel 对象的列表，表示已安装模组的信息。  
`DefaultModsDir`: 默认模组目录的路径。  
`SWRSToysD3d9Exist`: 检查游戏目录中是否存在 "d3d9.dll" 文件。  
`ToBeDeletedDirList`: 存储待删除目录的列表，用于清理操作。  

## 公共方法
- [初始化](#初始化)
  - 构造函数
  - Refresh
  - LoadSWRSToysSetting
- [取得模組資訊](#取得模組資訊)
  - GetModInfoByModName
  - GetModInfoByModFileName
- [模組設置操作](#模組設置操作)
  - ChangeModEnabled
  - ChangeModIniSetting
  - ApplyModSettingGroup
  - SaveSWRSToysIni
- [刪除模組](#刪除模組)
  - AddModToBeDeleted
  - ExecuteDelete

### 初始化
---
#### 构造函数:
```csharp
public ModManager(string? sokuDirFullPath = null)
```
初始化 ModManager 类的实例。  
可选参数 sokuDirFullPath 用于指定游戏目录的完整路径，如果未提供则使用当前可执行文件所在目录。

#### `Refresh` 方法:
```csharp
public void Refresh()
```
初始化现有的模组信息和待删除目录列表。  
搜索默认模组目录并填充 ModInfoList。

#### `LoadSWRSToysSetting` 方法:
```csharp
public void LoadSWRSToysSetting()
```
从 ModLoaderSettings.json 或 SWRSToys.ini 文件加载模组设置。  
根据加载的设置更新 ModInfoList。

### 取得模組資訊
---
#### `GetModInfoByModName` 方法:
```csharp
public ModInfoModel? GetModInfoByModName(string modName)
```
通过模组名称获取 ModInfoModel 对象。

#### `GetModInfoByModFileName` 方法:
```csharp
public ModInfoModel? GetModInfoByModFileName(string modFileName)
```
通过模组文件名获取 ModInfoModel 对象。

### 模組設置操作
---
#### `ChangeModEnabled` 方法:
```csharp
public void ChangeModEnabled(string modName, bool enabled)
```
更改模组的启用状态。

#### `ChangeModIniSetting` 方法:
```csharp
public void ChangeModIniSetting(string modName, IniSettingModel modIniSetting)
```
更改模组的 INI 设置。

#### `ApplyModSettingGroup` 方法:
```csharp
public void ApplyModSettingGroup(ModSettingGroupModel settingGroup)
```
应用模组设置组，包括启用、禁用模组和覆盖 INI 设置。

#### `SaveSWRSToysIni` 方法:
```csharp
public void SaveSWRSToysIni()
```
保存 SWRSToys 的 INI 设置文件。

### 刪除模組
---
#### `AddModToBeDeleted` 方法:
```csharp
public void AddModToBeDeleted(string modName)
public void AddModToBeDeleted(ModInfoModel modInfo)
```
将模组添加到待删除目录列表。

#### `ExecuteDelete` 方法:
```csharp
public void ExecuteDelete()
```
执行删除操作，删除待删除目录列表中的目录。