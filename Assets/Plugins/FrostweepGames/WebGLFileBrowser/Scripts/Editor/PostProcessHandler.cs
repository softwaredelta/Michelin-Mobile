using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace FrostweepGames.Plugins.WebGLFileBrowser.PostProcess
{
    public class PostProcessHandler
    {
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (!GeneralConfig.Config.usePostProcess)
                return;

            if (target == BuildTarget.WebGL)
            {
                var indexPath = $"{pathToBuiltProject}/index.html";
                var indexDataPath = GetIndexFormDataPath();
                var contentDataPath = GetContentDataPath();

                if (System.IO.File.Exists(indexPath) && System.IO.File.Exists(indexDataPath))
                {
                    var indexData = System.IO.File.ReadAllText(indexPath);
                    var indexFBContent = System.IO.File.ReadAllText(indexDataPath);

                    if (!indexData.Contains(indexFBContent))
                        indexData = indexData.Insert(indexData.IndexOf("</body>"), "\n" + indexFBContent + "\n");

#if UNITY_2020_1_OR_NEWER
                    if (!indexData.Contains("gameInstance = unityInstance"))
                    {
                        //set variables
                        var pattern = "}).then((unityInstance) => {";

                        if (indexData.IndexOf(pattern) != -1)
                        {
                            indexData = indexData.Insert(indexData.IndexOf(pattern) + pattern.Length + 1,
                                "\n          gameInstance = unityInstance;\n");
                        }
                        else
                        {
                            pattern = "createUnityInstance(document.querySelector(";

                            var indexOfCreatingInstance = indexData.IndexOf(pattern);

                            pattern = "});";

                            var indexOfEndOFCreatingInstance = indexData.IndexOf(pattern, indexOfCreatingInstance);

                            if (indexOfEndOFCreatingInstance != -1)
                                indexData = indexData.Insert(indexOfEndOFCreatingInstance + 2,
                                    ".then((unityInstance) => { gameInstance = unityInstance; })");
                        }
                    }

                    if (!indexData.Contains("var gameInstance = null"))
                    {
                        var pattern = "document.querySelector(\"#unity-container\");";

                        if (indexData.IndexOf(pattern) != -1)
                        {
                            indexData = indexData.Insert(indexData.IndexOf(pattern) + pattern.Length + 1,
                                "\n      var gameInstance = null;\n");
                        }
                        else
                        {
                            pattern = "container = document.querySelector(\"#gameContainer\")";
                            if (indexData.Contains(pattern))
                            {
                                indexData = indexData.Insert(indexData.IndexOf(pattern) + pattern.Length + 1,
                                    "\n      var gameInstance = null;\n");
                            }
                            else
                            {
                                pattern = "createUnityInstance(document.querySelector(";
                                indexData = indexData.Insert(indexData.IndexOf(pattern) - 1,
                                    "\n      var gameInstance = null;\n");
                            }
                        }
                    }
#elif UNITY_2019_4_OR_NEWER
                    if(!indexData.Contains("var unityInstance") && indexData.Contains("UnityLoader.instantiate"))
                    {
                        string pattern = "UnityLoader.instantiate";

                        if (indexData.IndexOf(pattern) != -1)
                        {
                            indexData = indexData.Insert(indexData.IndexOf(pattern), "var unityInstance = ");
                        }
                    }

                    if (!indexData.Contains("var gameInstance = unityInstance") && (indexData.Contains("var unityInstance = UnityLoader.instantiate") || 
                                                                                    indexData.Contains("var unityInstance=UnityLoader.instantiate")))
                    {
                        string pattern = "</head>";

                        if (indexData.IndexOf(pattern) != -1)
                        {
                            indexData =
 indexData.Insert(indexData.IndexOf(pattern), "  <script>var gameInstance = unityInstance;</script>\n");
                        }
                    }
#endif
                    System.IO.File.WriteAllText(indexPath, indexData);

                    if (Directory.Exists(contentDataPath))
                        DirectoryCopy(contentDataPath, $"{pathToBuiltProject}/FileBrowser");
                }
                else
                {
                    Debug.LogError("Process of File Browser Plugin failed due to: files not found!");
                }
            }
        }

        private static string GetIndexFormDataPath()
        {
            return $"{GetPluginFolderPath()}/Scripts/Editor/HTMLIndexData.html";
        }

        private static string GetContentDataPath()
        {
            return $"{GetPluginFolderPath()}/Scripts/Editor/Content";
        }

        private static string GetPluginFolderPath()
        {
            return SearchFolder(Application.dataPath, "WebGLFileBrowser");
        }

        private static string SearchFolder(string path, string name)
        {
            var directories = Directory.GetDirectories(path);

            for (var i = 0; i < directories.Length; i++)
                if (directories[i].EndsWith(name))
                {
                    return directories[i];
                }
                else
                {
                    var exportPath = SearchFolder(directories[i], name);

                    if (!string.IsNullOrEmpty(exportPath))
                        return exportPath;
                }

            return null;
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs = true)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);

            var dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension == ".meta")
                    continue;
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
                foreach (var subdir in dirs)
                {
                    var tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
        }
    }
}