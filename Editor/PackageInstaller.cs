using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace DevelopKit.Editor
{
    [InitializeOnLoad]
    public class PackageInstaller : UnityEditor.Editor
    {
        public static readonly string AssetSampleRoot = "Assets/DevelopKit/Samples";
        private const string PackageName = "com.cheonnyang.developKit";
        
        static PackageInstaller()
        {
            EditorApplication.delayCall -= OnPackagesRegistered;
            EditorApplication.delayCall += OnPackagesRegistered;
        }

        private static void OnPackagesRegistered()
        {
            // 패키지 설치 후 초기 window 창 띄우기
            if (PlayerPrefs.GetInt("DevelopKitInit", 0) == 0)
            {
                PlayerPrefs.SetInt("DevelopKitInit", 1);
                DevelopKitEditor.ShowWindow();
            }
        }

        public static void SolvePackageDependencies()
        {
            foreach (var hubButton in DevelopKitEditor.HubButtons.Values)
            {
                var sampleName = hubButton.Name;
                if (!CheckSampleInstalled(sampleName))
                    continue;
                
                if (hubButton.Dependencies == null)
                    continue;
                
                foreach (var dependency in hubButton.Dependencies)
                {
                    if (!CheckPackageInstalled(dependency.Name))
                    {
                        AddPackage(dependency.Name, dependency.URL);
                    }
                }
            }
        }

        private static void AddPackage(string name, string url)
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), "Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"manifest.json not found at '{manifestPath}'");
                return;
            }
            
            string manifestText = File.ReadAllText(manifestPath);
            if (!manifestText.Contains(name))
            {
                Debug.Log($"{name} not found in manifest.json");
                var modifiedText = manifestText.Insert(manifestText.IndexOf("dependencies") + 17, $"\t\"{name}\": \"{url}\",\n");
                File.WriteAllText(manifestPath, modifiedText);
                Debug.Log($"Added {name} to manifest.json");
            }
            Client.Resolve();
        }

        private static void RemovePackage(string name)
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), "Packages/manifest.json");
            if (!File.Exists(manifestPath))
            {
                Debug.LogError($"manifest.json not found at '{manifestPath}'");
                return;
            }
            
            string manifestText = File.ReadAllText(manifestPath);
            if (manifestText.Contains(name))
            {
                Debug.Log($"{name} found in manifest.json");
                string[] lines = File.ReadAllLines(manifestPath);
                string modifiedText = string.Join("\n", Array.FindAll(lines, line => !line.Contains(name)));
                File.WriteAllText(manifestPath, modifiedText);
                Debug.Log($"Removed {name} to manifest.json");
            }
            Client.Resolve();
        }

        private static bool CheckPackageInstalled(string packageName)
        {
            string manifestPath = Path.Combine(Application.dataPath.Replace("Assets", string.Empty), "Packages/manifest.json");
            string manifestText = File.ReadAllText(manifestPath);
            return manifestText.Contains(packageName);
        }

        public static void CreateSample(string sampleName)
        {
            string folderPath = Path.Combine("Packages", PackageName, "Samples~", sampleName);

            if (CheckSampleInstalled(sampleName))
            {
                Debug.LogError($"Samples is already installed : {folderPath}");
                return;
            }
            
            string destinationPath = Path.Combine(AssetSampleRoot, sampleName);
            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }
            
            CopyDirectory(folderPath, destinationPath);
            Debug.Log($"Samples installed to: {destinationPath}");

            // 의존성 Package 추가
            List<PackageDependency> dependencies = null;
            foreach (var button in DevelopKitEditor.HubButtons.Values)
            {
                if (button.Name == sampleName)
                {
                    dependencies = button.Dependencies;
                    break;
                }
            }
            
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    if (!CheckPackageInstalled(dependency.Name))
                        AddPackage(dependency.Name, dependency.URL);
                }
            }
            
            AssetDatabase.Refresh();
        }

        public static void RemoveSample(string sampleName)
        {
            string folderPath = Path.Combine(AssetSampleRoot, sampleName);
            
            if (!CheckSampleInstalled(sampleName))
            {
                Debug.LogWarning($"Folder not found: {folderPath}");
                return;
            }
            
            Directory.Delete(folderPath, true); // true는 하위 파일 및 폴더 삭제 포함
            Debug.Log($"Folder deleted: {folderPath}");

            // 의존성 Package 제거
            List<PackageDependency> dependencies = null;
            foreach (var button in DevelopKitEditor.HubButtons.Values)
            {
                if (button.Name == sampleName)
                {
                    dependencies = button.Dependencies;
                    break;
                }
            }
            
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    RemovePackage(dependency.Name);
                }
            }
            
            SolvePackageDependencies();
            AssetDatabase.Refresh();
        }

        public static bool CheckSampleInstalled(string sampleName)
        {
            string folderPath = Path.Combine(AssetSampleRoot, sampleName);
            return Directory.Exists(folderPath);
        }
        
        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);
            
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }
            
            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                var destDir = Path.Combine(destinationDir, Path.GetFileName(dir));
                CopyDirectory(dir, destDir);
            }
        }
    }
}