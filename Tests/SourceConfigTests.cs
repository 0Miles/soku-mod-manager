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

            var sourceUrl = "https://sokuexample.github.io/testsource/";

            sourceConfig.AddSource(sourceUrl);
            var configs = sourceConfig.GetSourceConfigs();

            Assert.IsNotNull(configs);
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("testsource", configs[0].Name);
        }

        [TestMethod]
        public void RemoveSource_ShouldRemoveExistingSource()
        {
            // Arrange
            var sourceConfig = new SourceConfig();
            sourceConfig.ClearSource();

            sourceConfig.AddSource("https://sokuexample.github.io/testsource/");
            sourceConfig.AddSource("https://sokuexample.github.io/testsource/");

            // Act
            sourceConfig.RemoveSource("testsource");
            var configs = sourceConfig.GetSourceConfigs();

            // Assert
            Assert.IsNotNull(configs);
            Assert.AreEqual(1, configs.Count);
            Assert.AreEqual("testsource1", configs[0].Name);
        }
    }
}