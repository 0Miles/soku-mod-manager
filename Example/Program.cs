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