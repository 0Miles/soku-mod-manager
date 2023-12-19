using SokuModManager.Models.Source;
using SokuModManager;

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
                new() { Name = "TestSource", Url = "https://sokuexample.github.io/testsource/" }
            };

            var sourceManager = new SourceManager(sourceConfigs);

            // Act
            await sourceManager.FetchSourceList();
            var sources = sourceManager.SourceList;

            // Assert
            Assert.IsNotNull(sources);
            Assert.AreEqual(1, sources.Count);

            var testSource = sources.FirstOrDefault(x => x.Name == "TestSource");

            Assert.IsNotNull(testSource);
            Assert.IsNotNull(testSource.ModuleSummaries);
            Assert.IsNotNull(testSource.Modules);


            var normalModule = testSource.Modules.FirstOrDefault(x => x.Name == "Normal");
            var noVersionModule = testSource.Modules.FirstOrDefault(x => x.Name == "NoVersionMod");
            var negativePriorityTestModule = testSource.Modules.FirstOrDefault(x => x.Name == "NegativePriorityTest");
            var highPriorityTestModule = testSource.Modules.FirstOrDefault(x => x.Name == "HighPriorityTest");
            var badDownloadLinkTestModule = testSource.Modules.FirstOrDefault(x => x.Name == "BadDownloadLinkTest");
            Assert.IsNotNull(normalModule);
            Assert.IsNotNull(noVersionModule);
            Assert.IsNotNull(negativePriorityTestModule);
            Assert.IsNotNull(highPriorityTestModule);
            Assert.IsNotNull(badDownloadLinkTestModule);


            Assert.IsNotNull(normalModule.VersionNumbers);
            Assert.IsNotNull(negativePriorityTestModule.VersionNumbers);
            Assert.IsNotNull(highPriorityTestModule.VersionNumbers);
            Assert.IsNotNull(badDownloadLinkTestModule.VersionNumbers);
            Assert.AreEqual(1, normalModule.VersionNumbers.Count);
            Assert.AreEqual(1, negativePriorityTestModule.VersionNumbers.Count);
            Assert.AreEqual(1, highPriorityTestModule.VersionNumbers.Count);
            Assert.AreEqual(1, badDownloadLinkTestModule.VersionNumbers.Count);
            Assert.IsNull(noVersionModule.VersionNumbers);

            Assert.IsNotNull(normalModule.RecommendedVersion);
            Assert.AreEqual(normalModule.RecommendedVersionNumber, normalModule.RecommendedVersion.Version);
            Assert.IsNotNull(negativePriorityTestModule.RecommendedVersion);
            Assert.AreEqual(negativePriorityTestModule.RecommendedVersionNumber, normalModule.RecommendedVersion.Version);
            Assert.IsNotNull(highPriorityTestModule.RecommendedVersion);
            Assert.AreEqual(highPriorityTestModule.RecommendedVersionNumber, normalModule.RecommendedVersion.Version);
            Assert.IsNotNull(badDownloadLinkTestModule.RecommendedVersion);
            Assert.AreEqual(badDownloadLinkTestModule.RecommendedVersionNumber, normalModule.RecommendedVersion.Version);

            // Assert DownloadModuleImageFiles for "example" module
            string tmpSourceFolder = Path.Combine(sourceManager.SokuModSourceTempDirPath, "TestSource");

            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "Normal"), "banner.png")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "Normal"), "icon.jpg")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "NoVersionMod"), "banner.png")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "NoVersionMod"), "icon.png")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "NegativePriorityTest"), "banner.jpg")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "NegativePriorityTest"), "icon.gif")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "HighPriorityTest"), "banner.png")));
            Assert.IsTrue(File.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "HighPriorityTest"), "icon.jpg")));
            Assert.IsFalse(Directory.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "BadDownloadLinkTest"), "banner.png")));
            Assert.IsFalse(Directory.Exists(Path.Combine(Path.Combine(tmpSourceFolder, "BadDownloadLinkTest"), "icon.png")));
        }
    }

}
