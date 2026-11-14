using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
using CompressionLevel = System.IO.Compression.CompressionLevel;

namespace Rehawk.Foundation.Build
{
    public sealed class BuildPostprocessor : IPreprocessBuildWithReport
    {
        private const string VERSION_FILE_NAME = "version.txt";

        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            string buildPath = NormalizePath(report.summary.outputPath);

            if (Directory.Exists(buildPath))
            {
                Directory.Delete(buildPath, true);
                return;
            }

            if (File.Exists(buildPath))
            {
                File.Delete(buildPath);
            }
        }

        private static readonly string[] _requiredBuildDirectories =
        {
            "D3D12",
            "MonoBleedingEdge"
        };

        [PostProcessBuild(0)]
        private static void OnPostprocessBuild(BuildTarget target, string buildPath)
        {
            string versionFileDirectory = GetVersionFileDirectory(buildPath);
            string versionFilePath = Path.Combine(versionFileDirectory, VERSION_FILE_NAME);

            Directory.CreateDirectory(versionFileDirectory);
            File.WriteAllText(versionFilePath, PlayerSettings.bundleVersion + System.Environment.NewLine);

            if (!IsMonoWindowsBuild(target))
            {
                Debug.Log($"Build version file written to: {versionFilePath}. ZIP creation skipped because the build does not use Mono.");
                return;
            }

            string zipFilePath = GetArchivePath(buildPath);
            buildPath = NormalizePath(buildPath);
            CreateBuildArchive(buildPath, versionFilePath, zipFilePath, versionFileDirectory);

            Debug.Log($"Build version file written to: {versionFilePath}");
            Debug.Log($"Build archive written to: {zipFilePath}");
        }

        private static string GetVersionFileDirectory(string buildPath)
        {
            if (Path.HasExtension(buildPath))
            {
                string directory = Path.GetDirectoryName(buildPath);
                return string.IsNullOrEmpty(directory) ? Directory.GetCurrentDirectory() : directory;
            }

            return Directory.GetParent(buildPath)?.FullName ?? Directory.GetCurrentDirectory();
        }

        private static string GetArchivePath(string buildPath)
        {
            string archiveDirectory = GetVersionFileDirectory(buildPath);
            string projectTitle = Slugify(PlayerSettings.productName);
            string projectVersion = Slugify(PlayerSettings.bundleVersion, true);
            string archiveName = $"{projectTitle}-{projectVersion}.zip";

            return Path.Combine(archiveDirectory, archiveName);
        }

        private static void CreateBuildArchive(
            string buildPath,
            string versionFilePath,
            string zipFilePath,
            string buildDirectory)
        {
            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            using ZipArchive archive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
            string projectName = Directory.Exists(buildPath)
                ? Path.GetFileName(buildPath)
                : Path.GetFileNameWithoutExtension(buildPath);

            foreach (string directoryName in _requiredBuildDirectories)
            {
                AddDirectoryToArchive(archive, Path.Combine(buildDirectory, directoryName), directoryName);
            }

            AddDirectoryToArchive(archive, Path.Combine(buildDirectory, $"{projectName}_Data"), $"{projectName}_Data");
            AddFileToArchive(archive, Path.Combine(buildDirectory, $"{projectName}.exe"), $"{projectName}.exe");
            AddFileToArchive(archive, Path.Combine(buildDirectory, "UnityCrashHandler64.exe"), "UnityCrashHandler64.exe");
            AddFileToArchive(archive, Path.Combine(buildDirectory, "UnityPlayer.dll"), "UnityPlayer.dll");
            AddFileToArchive(archive, versionFilePath, VERSION_FILE_NAME);
        }

        private static bool IsMonoWindowsBuild(BuildTarget target)
        {
            if (target != BuildTarget.StandaloneWindows64)
            {
                return false;
            }

            NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(BuildPipeline.GetBuildTargetGroup(target));
            return PlayerSettings.GetScriptingBackend(namedBuildTarget) == ScriptingImplementation.Mono2x;
        }

        private static void AddDirectoryToArchive(ZipArchive archive, string directoryPath, string archiveDirectory)
        {
            if (!Directory.Exists(directoryPath))
            {
                Debug.LogWarning($"Build archive directory not found and was skipped: {directoryPath}");
                return;
            }

            foreach (string filePath in Directory.GetFiles(directoryPath, "*", SearchOption.AllDirectories))
            {
                string relativePath = GetRelativePath(directoryPath, filePath);
                archive.CreateEntryFromFile(
                    filePath,
                    $"{archiveDirectory}/{relativePath}",
                    CompressionLevel.Optimal);
            }
        }

        private static void AddFileToArchive(ZipArchive archive, string filePath, string archivePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"Build archive file not found and was skipped: {filePath}");
                return;
            }

            archive.CreateEntryFromFile(filePath, archivePath, CompressionLevel.Optimal);
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path);
        }

        private static string GetRelativePath(string rootPath, string filePath)
        {
            string relativePath = filePath.Substring(rootPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relativePath.Replace(Path.DirectorySeparatorChar, '/');
        }

        private static string Slugify(string value, bool preservePeriods = false)
        {
            StringBuilder slug = new();
            bool separatorPending = false;

            foreach (char character in value)
            {
                if (char.IsLetterOrDigit(character) || preservePeriods && character == '.')
                {
                    if (separatorPending && slug.Length > 0)
                    {
                        slug.Append('-');
                    }

                    slug.Append(char.ToLowerInvariant(character));
                    separatorPending = false;
                    continue;
                }

                separatorPending = true;
            }

            return slug.Length > 0 ? slug.ToString() : "build";
        }
    }
}