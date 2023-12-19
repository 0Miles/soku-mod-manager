# SourceConfig
用于管理和操作模组来源配置，提供了添加、删除、刷新模组来源等相关操作。

## 成员变量
- `ConfigList`: 存储 SourceConfigModel 对象的列表，表示已配置的模组来源。

## 公共方法
- [初始化](#初始化)
    - 构造函数
    - Refresh
- [获取推荐来源名称](#获取推荐来源名称)
    - GetRecommendedSourceNameByUrl
    - GetSafeSourceName
- [模组来源操作](模组来源操作)
    - AddSource
    - RemoveSource
    - Clear

### 初始化
---
#### 构造函数:
```csharp
public SourceConfig()
```
初始化 SourceConfig 类的实例。
调用 Refresh 方法加载已配置的模组来源。

#### `Refresh` 方法:
```csharp
public void Refresh()
```
刷新模组来源配置。
从文件加载已配置的模组来源信息。

### 获取推荐来源名称
---
#### `GetRecommendedSourceNameByUrl` 方法:
```csharp
public string GetRecommendedSourceNameByUrl(string sourceUrl)
```
根据来源的 URL 推断并返回推荐的来源名称。

#### `GetSafeSourceName` 方法:
```csharp
public string GetSafeSourceName(string sourceName)
```
确保来源名称的唯一性，如果已存在相同名称的来源，则回传在名称末尾添加数字的新名称。

### 模组来源操作
---
#### `AddSource` 方法:
```csharp
public void AddSource(string sourceUrl)
public void AddSource(SourceConfigModel newSource)
```
向配置中添加新的模组来源。
如果来源已存在，则不进行添加。

#### `RemoveSource` 方法:
```csharp
public void RemoveSource(string sourceName)
```
从配置中移除指定名称的模组来源。

#### `Clear` 方法:
```csharp
public void Clear()
```
清空模组来源配置并储存。