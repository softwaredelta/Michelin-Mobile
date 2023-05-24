using UnityEditor;

namespace FrostweepGames.Plugins.WebGLFileBrowser
{
    [InitializeOnLoad]
    public class DefineProcessing : Plugins.DefineProcessing
    {
        private static readonly string[] _Defines =
        {
            "FG_WEBGLFB"
        };

        static DefineProcessing()
        {
            AddOrRemoveDefines(true, true, _Defines);
        }
    }
}