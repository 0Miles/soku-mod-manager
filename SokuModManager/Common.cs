﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace SokuModManager
{
    public class Common
    {
        public static string ExecutableFileName { get; internal set; } = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string ExecutableDir { get; internal set; } = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? AppContext.BaseDirectory;

        public static string GetFilePath(string filename)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, filename);
        }

        public static async Task<string?> DownloadStringAsync(string url)
        {
            using var httpClient = new HttpClient();
            try
            {
                return await httpClient.GetStringAsync(url);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error downloading data from {url}: {ex.Message}");
                return null;
            }
        }

        public static async Task DownloadAndSaveFileAsync(string baseUrl, string relativeUrl, string saveFolder, string fileName)
        {
            string fileUrl = Path.Combine(baseUrl, relativeUrl);
            string filePath = Path.Combine(saveFolder, fileName);

            using HttpClient client = new();
            using HttpResponseMessage response = await client.GetAsync(fileUrl);
            if (response.IsSuccessStatusCode)
            {
                using Stream contentStream = await response.Content.ReadAsStreamAsync();
                using FileStream fileStream = File.Create(filePath);
                await contentStream.CopyToAsync(fileStream);
                fileStream.Flush();
            }
            else
            {
                Logger.LogInformation($"Download {fileUrl} failed: {response.StatusCode}");
            }
        }

        public static string GetRelativePath(string absolutePath, string baseDirPath)
        {
            Directory.SetCurrentDirectory(ExecutableDir);
            if (absolutePath == baseDirPath)
            {
                return ".";
            }
            else if (absolutePath == Path.GetFullPath(Path.Combine(baseDirPath, "..")))
            {
                return "..";
            }

            if (!baseDirPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                baseDirPath += Path.DirectorySeparatorChar;
            }
            Uri baseUri = new(baseDirPath);
            Uri absoluteUri = new(absolutePath);
            Uri relativeUri = baseUri.MakeRelativeUri(absoluteUri);
            string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            return relativePath;
        }
    }
}
