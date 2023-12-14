namespace SokuModManager.Models.Source
{
    public class SourceModel
    {
        public string? Name { get; set; }
        public string Url { get; set; } = "";
        public List<SourceModuleSummaryModel> ModuleSummaries { get; set; } = new();
        public List<SourceModuleModel> Modules { get; set; } = new();
    }
}
