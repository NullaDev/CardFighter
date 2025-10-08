using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

namespace Editor
{
    public static class CopyGameDataOnBuild
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            var buildDir = Path.GetDirectoryName(pathToBuiltProject);
            var sourceDir = Path.Combine(Application.dataPath, "../GameData");
            var destDir = Path.Combine(buildDir, "GameData");

            if (!Directory.Exists(sourceDir))
            {
                Debug.LogWarning($"[BuildPostProcess] Not found GameData path: {sourceDir}");
                return;
            }

            if (Directory.Exists(destDir))
            {
                Directory.Delete(destDir, true);
            }

            CopyDirectory(sourceDir, destDir);
            Debug.Log($"[BuildPostProcess] successfully copy GameData dir to: {destDir}");
        }

        private static void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }

}