# SokuModManager

SokuModManager is a library for managing Hisoutensoku Mods (SWRSToys), providing classes for handling Mods and Mod source configuration.

## Classes
- [SourceConfig](./docs/zh-Hans/SourceConfig.md)  
Manages configuration information for mod sources, including source names, URLs, and more.

- [SourceManager](./docs/zh-Hans/SourceManager.md)    
Handles the management and operations of mod sources, providing functions for retrieving mod lists, downloading mod data, and images, among other operations.

- [ModManager](./docs/zh-Hans/ModManager.md)   
Manages and operates SWRSToys game mods, offering features such as searching, loading, and saving mod settings.

- [Updater](./docs/zh-Hans/Updater.md)   
Executes update operations for game mods, including download, extraction, and file replacement.

## Example

Here is a simple example of using this library to download and install all available mods from a specified source:

```csharp
using SokuModManager;
using SokuModManager.Models.Source;

namespace Example
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize SourceManager
            var sourceConfigs = new List<SourceConfigModel>
            {
                new() { Name = "TestSource", Url = "https://sokuexample.github.io/testsource/" }
            };
            var sourceManager = new SourceManager(sourceConfigs);

            // Initialize Updater and pass in the configuration in ModManager and SourceManager
            var updater = new Updater(new ModManager());

            try
            {
                // Get module information from source
                await sourceManager.FetchSourceList();

                // Get a list of available updates
                var updateFileInfos = Updater.GetUpdateFileInfosFromSource(sourceManager.SourceList[0]);

                // Refresh the AvailableUpdateList and AvailableInstallList
                updater.RefreshAvailable(updateFileInfos!);

                // Install all installable modules
                var results = await updater.ExecuteUpdates(updater.AvailableInstallList);

                // Output update results
                Console.WriteLine("\nUpdate results:");
                foreach (var result in results)
                {
                    Console.WriteLine($"  {result.Name}: {(result.Success ? "succeeded" : "failed")}");
                    if (!result.Success)
                    {
                        Console.WriteLine($"    message: {result.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }
        }
    }
}
```
