using SokuModManager.Models.Source;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SokuModManager
{
    public class SourceConfig
    {
        private const string SOURCE_FILE_NAME = "sources.json";
        private List<SourceConfigModel> sourceConfigs = new();

        public SourceConfig()
        {
            RefreshSourceConfigs();
        }

        public List<SourceConfigModel> GetSourceConfigs()
        {
            return sourceConfigs;
        }

        public string GetRecommendedSourceNameByUrl(string sourceUrl)
        {
            var regex = new Regex("http(?:s?):\\/\\/([^/]+?)\\.github\\.io\\/([^/]+)");
            var match = regex.Match(sourceUrl);
            string? recommendName;
            if (match.Success)
            {
                recommendName = match.Groups[2].Value;
            }
            else
            {
                recommendName = "New Source";
            }
            return GetSafeSourceName(recommendName);
        }

        public string GetSafeSourceName(string sourceName)
        {
            if (sourceConfigs.Any(x => x.Name == sourceName))
            {
                var match = new Regex("^(.*?)(\\d+)?$").Match(sourceName);
                string name = match.Groups[1].Value;
                _ = int.TryParse(match.Groups[2].Value, out int num);
                return GetSafeSourceName(name + (num + 1));
            }
            else
            {
                return sourceName;
            }
        }

        public void AddSource(string sourceUrl)
        {
            AddSource(new SourceConfigModel
            {
                Name = GetRecommendedSourceNameByUrl(sourceUrl),
                Url = sourceUrl
            });
        }

        public void AddSource(SourceConfigModel newSource)
        {
            if (!sourceConfigs.Contains(newSource))
            {
                sourceConfigs.Add(newSource);
                SaveSourceConfigs();
            }
            else
            {
                Logger.LogInformation("Source already exists.");
            }
        }

        public void RemoveSource(string sourceName)
        {
            var targetSource = sourceConfigs.FirstOrDefault(x => x.Name == sourceName);
            if (targetSource != null)
            {
                sourceConfigs.Remove(targetSource);
                SaveSourceConfigs();
            }
            else
            {
                Logger.LogInformation("Source not found.");
            }
        }


        public void ClearSource()
        {
            sourceConfigs.Clear();
            SaveSourceConfigs();
        }

        public void RefreshSourceConfigs()
        {
            try
            {
                string filePath = Common.GetFilePath(SOURCE_FILE_NAME);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    sourceConfigs = JsonSerializer.Deserialize<List<SourceConfigModel>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new List<SourceConfigModel>();
                }
                else
                {
                    sourceConfigs = new List<SourceConfigModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading sources: {ex.Message}");
                sourceConfigs = new List<SourceConfigModel>();
            }
        }

        private void SaveSourceConfigs()
        {
            try
            {
                string filePath = Common.GetFilePath(SOURCE_FILE_NAME);
                string json = JsonSerializer.Serialize(sourceConfigs);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error saving sources", ex);
            }
        }
    }
}
