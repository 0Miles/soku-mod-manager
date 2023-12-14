using SokuModManager;

namespace Tests
{
    [TestClass]
    public class SourceConfigTests
    {
        [TestMethod]
        public void AddSource_ShouldAddNewSource()
        {
            var sourceConfig = new SourceConfig();

            sourceConfig.ClearSource();

            var sourceUrl = "https://github.com/example/repo.git";

            sourceConfig.AddSource(sourceUrl);
            var configs = sourceConfig.GetSourceConfigs();

            Assert.IsNotNull(configs);
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("repo", configs[0].Name);
        }

        [TestMethod]
        public void RemoveSource_ShouldRemoveExistingSource()
        {
            // Arrange
            var sourceConfig = new SourceConfig();
            sourceConfig.ClearSource();

            sourceConfig.AddSource("https://github.com/example/repo.git");
            sourceConfig.AddSource("https://github.com/example/repo.git");

            // Act
            sourceConfig.RemoveSource("repo");
            var configs = sourceConfig.GetSourceConfigs();

            // Assert
            Assert.IsNotNull(configs);
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("repo1", configs[0].Name);
        }
    }
}