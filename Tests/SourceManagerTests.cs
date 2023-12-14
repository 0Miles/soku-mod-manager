using SokuModManager.Models.Source;
using SokuModManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class SourceManagerTests
    {
        [TestMethod]
        public async Task FetchSourceAndModules_ShouldSucceed()
        {
            // Arrange
            var sourceConfigs = new List<SourceConfigModel>
            {
                new SourceConfigModel { Name = "ExampleSource", Url = "http://example.soku.latte.today/" }
            };

            var sourceManager = new SourceManager(sourceConfigs);

            // Act
            await sourceManager.FetchSources();
            var sources = sourceManager.Sources;

            // Assert
            Assert.IsNotNull(sources);
            Assert.AreEqual(1, sources.Count);

            var exampleSource = sources.FirstOrDefault(x => x.Name == "ExampleSource");

            Assert.IsNotNull(exampleSource);
            Assert.IsNotNull(exampleSource.ModuleSummaries);
            Assert.IsNotNull(exampleSource.Modules);

            // Assert Module Info for "example" module
            var exampleModule = exampleSource.Modules.FirstOrDefault(x => x.Name.ToLower() == "example");
            Assert.IsNotNull(exampleModule);
            Assert.IsNotNull(exampleModule.VersionNumbers);
            Assert.AreEqual(3, exampleModule.VersionNumbers.Count);

            // Assert RecommendedVersionInfo for "example" module
            Assert.IsNotNull(exampleModule.RecommendedVersionInfo);
            Assert.AreEqual(exampleModule.RecommendedVersion, exampleModule.RecommendedVersionInfo.Version);

            // Assert DownloadModuleImageFiles for "example" module
            string tmpSourceFolder = Path.Combine(sourceManager.SokuModSourceTempDirPath, "ExampleSource");
            string tmpModuleFolder = Path.Combine(tmpSourceFolder, "example");

            Assert.IsTrue(File.Exists(Path.Combine(tmpModuleFolder, "banner.jpg")));
            Assert.IsTrue(File.Exists(Path.Combine(tmpModuleFolder, "icon.png")));

            // Assert Module Info for "example2" module
            var example2Module = exampleSource.ModuleSummaries.FirstOrDefault(x => x.Name == "example2");
            Assert.IsNotNull(example2Module);

            // Assert DownloadModuleImageFiles for "example2" module
            tmpModuleFolder = Path.Combine(tmpSourceFolder, "example2");

            Assert.IsTrue(File.Exists(Path.Combine(tmpModuleFolder, "banner.jpg")));
            Assert.IsTrue(File.Exists(Path.Combine(tmpModuleFolder, "icon.gif")));
        }
    }

}
