using System.IO;
using UnityEditor;
using UnityEngine;

namespace FrostweepGames.Plugins.WebGLFileBrowser
{
    //[CreateAssetMenu(fileName = "GeneralConfig", menuName = "FrostweepGames/MicrophonePro/GeneralConfig", order = 3)]
    public class GeneralConfig : ScriptableObject
    {
        private static GeneralConfig _Config;

        public bool showWelcomeDialogAtStartup = true;

        [Tooltip("Auto modify index.html temlate (useful for default templates)")]
        public bool usePostProcess;

        public static GeneralConfig Config
        {
            get
            {
                if (_Config == null)
                    _Config = GetConfig();
                return _Config;
            }
        }

        private static GeneralConfig GetConfig()
        {
            var path = "WebGLFileBrowser/GeneralConfig";
            var config = Resources.Load<GeneralConfig>(path);

            if (config == null)
            {
                Debug.LogError(
                    $"WebGL File Browser General Config not found in {path} Resources folder. Will use default.");

                config = (GeneralConfig)CreateInstance("GeneralConfig");

#if UNITY_EDITOR
                var pathToFolder = "Assets/Plugins/FrostweepGames/WebGLFileBrowser/Resources/WebGLFileBrowser";
                var filename = "GeneralConfig.asset";

                if (!Directory.Exists(Application.dataPath + "/../" + pathToFolder))
                {
                    Directory.CreateDirectory(pathToFolder);
                    AssetDatabase.ImportAsset(pathToFolder);
                }

                if (!System.IO.File.Exists(Application.dataPath + "/../" + pathToFolder + "/" + filename))
                    AssetDatabase.CreateAsset(config, pathToFolder + "/" + filename);
                AssetDatabase.SaveAssets();
#endif
            }

            return config;
        }
    }
}