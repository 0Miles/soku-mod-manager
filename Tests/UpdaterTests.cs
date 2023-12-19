using SokuModManager.Models;
using SokuModManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SokuModManager.Models.Source;

namespace Tests
{
    [TestClass]
    public class UpdaterTests
    {
        [TestMethod]
        public async Task RefreshAvailable_Install()
        {
            var workDir = Path.GetFullPath("../../../TestUpdaterDir", Common.ExecutableDir);
            if (Directory.Exists(workDir))
            {
                Directory.Delete(workDir, true);
            }
            Directory.CreateDirectory(workDir);


            ModManager modManager = new(workDir);
            Updater updater = new(modManager);

            var sourceConfigs = new List<SourceConfigModel>
            {
                new() { Name = "TestSource", Url = "https://sokuexample.github.io/testsource/" }
            };

            SourceManager sourceManager = new(sourceConfigs);
            await sourceManager.FetchSources();

            var updateFileInfos = Updater.GetUpdateFileInfosFromSource(sourceManager.Sources[0]);
            
            updater.RefreshAvailable(updateFileInfos!);
            await updater.ExecuteUpdates(updater.AvailableInstallList);

            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(modManager.DefaultModsDir, "Normal"), "normal.dll")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(modManager.DefaultModsDir, "NegativePriorityTest"), "NegativePriorityTest.dll")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(modManager.DefaultModsDir, "HighPriorityTest"), "HighPriorityTest.dll")));
        }
    }
}
