using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace FrostweepGames.Plugins
{
    [InitializeOnLoad]
    public class DefineProcessing : Editor
    {
        public static void AddOrRemoveDefines(bool add, bool allTargets, params string[] definesToChange)
        {
            BuildTargetGroup[] buildTargets;

            if (allTargets)
            {
                var targets = new List<BuildTargetGroup>();
                foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
                {
                    var group = BuildPipeline.GetBuildTargetGroup(target);

                    if (group == BuildTargetGroup.Unknown)
                        continue;
                    targets.Add(group);
                }

                buildTargets = targets.ToArray();
            }
            else
            {
                buildTargets = new[] { EditorUserBuildSettings.selectedBuildTargetGroup };
            }

            for (var i = 0; i < buildTargets.Length; i++)
            {
                var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargets[i]);
                var defines = definesString.Split(';').ToList();

                if (add)
                    defines.AddRange(definesToChange.Except(defines));
                else
                    defines.RemoveAll(item => definesToChange.Contains(item));
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargets[i], string.Join(";", defines.ToArray()));
            }
        }
    }
}