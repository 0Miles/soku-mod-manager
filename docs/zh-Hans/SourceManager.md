# SourceManager
用于管理和操作模组来源，提供了获取模组列表、下载模组数据和图片等相关操作。

## 成员变量
- `SourceList`: 存储 SourceModel 对象的列表，表示已配置的模组来源。
- `SokuModSourceTempDirPath`: 临时目录路径，用于存储模组来源的临时文件。

## 公共方法
- [初始化](#初始化)
    - 构造函数
- [取得来源资讯](#取得来源资讯)
    - FetchSourceList
    - FetchModuleVersionInfo

### 初始化
---
#### 构造函数:
```csharp
public SourceManager(List<SourceConfigModel> sourceConfigs)
```
初始化 SourceManager 类的实例。  
接收模组来源配置的列表作为参数。

### 取得来源资讯
---
#### `FetchSourceList` 方法:
```csharp
public async Task FetchSourceList()
```
获取模组列表。  
根据配置中的模组来源信息，异步获取模组概要、模组数据和相关图片。

#### `FetchModuleVersionInfo` 方法:
```csharp
public static async Task<SourceModuleVersionModel?> 
FetchModuleVersionInfo(SourceModel source, string moduleName, string versionNumber)
```
异步获取指定模组的版本信息。
根据来源、模组名称和版本号，下载版本信息并返回 SourceModuleVersionModel 对象。