﻿
using SokuModManager.Models.Mod;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SokuModManager
{
    public class ModManager
    {
        private const string MOD_VERSION_FILENAME_SUFFIX = ".version.txt";
        private readonly string sokuDirFullPath;

        public List<ModInfoModel> ModInfoList { get; private set; } = new();

        public List<string> ToBeDeletedDirList { get; private set; } = new();

        public string DefaultModsDir { get; private set; }
        public bool SWRSToysD3d9Exist { get { return File.Exists(Path.Combine(sokuDirFullPath, "d3d9.dll")); } }

        public ModManager(string? sokuDirFullPath = null)
        {
            this.sokuDirFullPath = sokuDirFullPath ?? Common.ExecutableDir;
            DefaultModsDir = Path.Combine(this.sokuDirFullPath, "modules");
        }

        private ModInfoModel? GetModInfo(string dllFilePath)
        {
            var dirName = Path.GetDirectoryName(dllFilePath)!;
            var fullPath = dllFilePath;
            var relativePath = Common.GetRelativePath(dllFilePath, sokuDirFullPath);

            var modInfoJsonFilename = Path.Combine(dirName, "mod.json");

            ModInfoModel result;

            if (File.Exists(modInfoJsonFilename))
            {
                var json = File.ReadAllText(modInfoJsonFilename, Encoding.UTF8);
                result = JsonSerializer.Deserialize<ModInfoModel>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) ?? new ModInfoModel();
                if (Path.GetFileName(dllFilePath).ToLower() != result.Main?.ToLower()) return null;
                
                result.ConfigFiles = result.ConfigFiles?.Select(x => Path.Combine(dirName, x)).ToList();
                if (result.Icon != null)
                {
                    result.Icon = Path.Combine(dirName, result.Icon);
                }
                if (result.Banner != null)
                {
                    result.Banner = Path.Combine(dirName, result.Banner);
                }

                result.IsFromRegularJson = true;
            }
            else
            {
                // 相容無mod.json模組
                result = new()
                {
                    Name = Path.GetFileNameWithoutExtension(dllFilePath),
                    Main = Path.GetFileName(dllFilePath),
                    ConfigFiles = Directory.GetFiles(dirName, "*.ini").Concat(Directory.GetFiles(dirName, "*.json")).ToList(),
                    Version = GetVersionFromDllFileOrVersionTxt(fullPath).ToString(),
                    IsFromRegularJson = false,
                };

                string icon = Path.Combine(dirName, "icon.png");
                if (File.Exists(icon))
                {
                    result.Icon = icon;
                }
            }

            result.DirName = dirName;
            result.RelativePath = relativePath;
            result.FullPath = fullPath;

            string[] dllFiles = Directory.GetFiles(dirName, "*.dll");
            result.SameDirModPathList = dllFiles.Where(filePath => filePath != fullPath).ToList();
            
            return result;
        }

        private static Version GetVersionFromDllFileOrVersionTxt(string fileName)
        {
            var fileVersionInfo = File.Exists(fileName) ? FileVersionInfo.GetVersionInfo(fileName) : null;

            string modCurrentVersion = "0.0.0.0";

            if (fileVersionInfo?.FileVersion != null)
            {
                modCurrentVersion = fileVersionInfo.FileVersion;
            }
            else
            {
                string modName = Path.GetFileNameWithoutExtension(fileName);
                string modDir = Path.GetDirectoryName(fileName)!;
                string modVersionFileName = Path.Combine(modDir, $"{modName}{MOD_VERSION_FILENAME_SUFFIX}");

                if (File.Exists(modVersionFileName))
                {
                    modCurrentVersion = File.ReadAllText(modVersionFileName);
                }
            }

            var (major, minor, build, revision) = (0, 0, 0, 0);
            var m = new Regex("\\d+").Matches(modCurrentVersion);

            if (m.Count > 0)
            {
                _ = int.TryParse(m[0]?.Value ?? "0", out major);
            }
            if (m.Count > 1)
            {
                _ = int.TryParse(m[1]?.Value ?? "0", out minor);
            }
            if (m.Count > 2)
            {
                _ = int.TryParse(m[2]?.Value ?? "0", out build);
            }
            if (m.Count > 3)
            {
                _ = int.TryParse(m[3]?.Value ?? "0", out revision);
            }

            return new Version(major, minor, build, revision);
        }

        public void Refresh()
        {
            ModInfoList.Clear();
            ToBeDeletedDirList.Clear();

            if (!Directory.Exists(DefaultModsDir))
            {
                return;
            }

            string[] moduleDirectories = Directory.GetDirectories(DefaultModsDir);

            foreach (string moduleDir in moduleDirectories)
            {
                string[] dllFiles = Directory.GetFiles(moduleDir, "*.dll");

                foreach (string dllFilePath in dllFiles)
                {
                    var modInfo = GetModInfo(dllFilePath);
                    if (modInfo != null)
                    {
                        ModInfoList.Add(modInfo);
                    }
                }
            }
        }

        public void LoadSWRSToysSetting()
        {
            string modLoaderSettingsPath = Path.Combine(sokuDirFullPath, "ModLoaderSettings.json");
            if (File.Exists(modLoaderSettingsPath))
            {
                try
                {
                    var json = File.ReadAllText(modLoaderSettingsPath);
                    var modLoaderSettings = JsonSerializer.Deserialize<ModLoaderSettingsModel>(json, new JsonSerializerOptions(){PropertyNameCaseInsensitive = true});

                    foreach (var module in modLoaderSettings?.Modules ?? new())
                    {
                        string fullPath = Path.Combine(sokuDirFullPath, module.Key.Trim().Replace('/', Path.DirectorySeparatorChar));
                        if (!File.Exists(fullPath))
                        {
                            continue;
                        }
                        var modInfo = ModInfoList.FirstOrDefault(x => x.FullPath.ToLower() == fullPath.ToLower());
                        if (modInfo == null)
                        {
                            modInfo = GetModInfo(fullPath);
                            if (modInfo != null)
                            {
                                ModInfoList.Add(modInfo);
                            }
                        }

                        if (modInfo != null)
                        {
                            modInfo.Enabled = module.Value.Enabled;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Read ModLoaderSettings.json Failed", ex);
                }
            }
            else
            {
                try
                {
                    string iniFilePath = Path.Combine(sokuDirFullPath, "SWRSToys.ini");

                    if (!File.Exists(iniFilePath))
                    {
                        return;
                    }

                    string[] lines = File.ReadAllLines(iniFilePath);

                    bool isModuleSectionsStart = false;

                    foreach (string line in lines)
                    {
                        string trimmedLine = line.Trim();

                        if (trimmedLine.ToLower() == "[module]")
                        {
                            isModuleSectionsStart = true;
                        }
                        else if (trimmedLine.StartsWith("[") && trimmedLine.ToLower() != "[module]")
                        {
                            break;
                        }

                        if (isModuleSectionsStart && !string.IsNullOrEmpty(trimmedLine))
                        {
                            string[] splitedLine = trimmedLine.Split('=');
                            if (splitedLine.Length > 1)
                            {
                                bool enabled = !splitedLine[0].StartsWith(";");

                                string[] splitedPath = splitedLine[1].Split(';');
                                string fullPath = Path.Combine(sokuDirFullPath, splitedPath[0].Trim().Replace('/', Path.DirectorySeparatorChar));

                                if (!File.Exists(fullPath))
                                {
                                    continue;
                                }

                                var modInfo = ModInfoList.FirstOrDefault(x => x.FullPath.ToLower() == fullPath.ToLower());

                                if (modInfo == null)
                                {
                                    modInfo = GetModInfo(fullPath);
                                    if (modInfo != null)
                                    {
                                        ModInfoList.Add(modInfo);
                                    }
                                }

                                if (modInfo != null)
                                {
                                    modInfo.Enabled = enabled;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Read SWRSToys.ini Failed", ex);
                }
            }
        }
    
        public ModInfoModel? GetModInfoByModName(string modName)
        {
            return ModInfoList.FirstOrDefault(x => x.Name.ToLower() == modName.ToLower() || x.DirName.ToLower() == modName.ToLower() && x.SameDirModPathList.Count == 0);
        }

        public ModInfoModel? GetModInfoByModFileName(string modFileName)
        {
            return ModInfoList.FirstOrDefault(x => x.Name.ToLower() == Path.GetFileNameWithoutExtension(modFileName).ToLower());
        }

        public void ChangeModEnabled(string modName, bool enabled)
        {
            var modInfo = GetModInfoByModName(modName);
            if (modInfo != null) {
                modInfo.Enabled = enabled;
            }
        }

        public void ChangeModIniSetting(string modName, IniSettingModel modIniSetting)
        {
            var modInfo = GetModInfoByModName(modName);
            if (modInfo != null)
            {
                string iniFilePath = Path.Combine(modInfo.DirName, modIniSetting.FileName!);
                if (File.Exists(iniFilePath))
                {
                    IniFile iniFile = new(iniFilePath);
                    if (modIniSetting.Enabled == false)
                    {
                        iniFile.DeleteKey(modIniSetting.Key, modIniSetting.Section);
                    }
                    else
                    {
                        iniFile.Write(modIniSetting.Key, modIniSetting.Value, modIniSetting.Section);
                    }
                }
            }
        }

        public void SaveSWRSToysIni()
        {
            Directory.SetCurrentDirectory(sokuDirFullPath);
            
            string modLoaderSettingsPath = Path.Combine(sokuDirFullPath, "ModLoaderSettings.json");
            if (File.Exists(modLoaderSettingsPath))
            {
                File.Delete(modLoaderSettingsPath);
            }

            string iniFilePath = Path.Combine(sokuDirFullPath, "SWRSToys.ini");

            using StreamWriter writer = new(iniFilePath);

            writer.WriteLine("[Module]");

            var sortedModInfoList = ModInfoList.OrderByDescending(mod => mod.Priority).ToList();
            foreach (var modInfo in sortedModInfoList)
            {
                writer.WriteLine((modInfo.Enabled ? "" : ";") + $"{modInfo.Name}={modInfo.RelativePath}");
            }
            writer.Close();
        }

        public void AddModToBeDeleted(ModInfoModel modInfo)
        {
            ToBeDeletedDirList.Add(modInfo.DirName);
        }

        public void AddModToBeDeleted(string modName)
        {
            var modInfo = GetModInfoByModName(modName);
            if (modInfo != null)
            {
                ToBeDeletedDirList.Add(modInfo.DirName);
            }
        }

        public void ExecuteDelete()
        {
            foreach(string path in ToBeDeletedDirList)
            {
                Directory.Delete(path, true);
            }
            ToBeDeletedDirList.Clear();
        }

        public void ApplyModSettingGroup(ModSettingGroupModel settingGroup)
        {
            if (settingGroup.EnableMods != null)
            {
                foreach (var enableMod in settingGroup.EnableMods)
                {
                    ChangeModEnabled(enableMod, true);
                    if (settingGroup.IniSettingsOverride != null)
                    {
                        settingGroup.IniSettingsOverride.TryGetValue(enableMod.ToLower(), out List<IniSettingModel>? iniSettings);
                        if (iniSettings != null)
                        {
                            foreach (var iniSetting in iniSettings)
                            {
                                ChangeModIniSetting(enableMod, iniSetting);
                            }
                        }
                    }
                }
            }
            if (settingGroup.DisableMods != null)
            {
                foreach (var disableMod in settingGroup.DisableMods)
                {
                    ChangeModEnabled(disableMod, false);
                }
            }
            SaveSWRSToysIni();
        }
    }
}
