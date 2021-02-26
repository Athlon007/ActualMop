// Actual Mop
// Copyright(C) 2020-2021 Athlon

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.If not, see<http://www.gnu.org/licenses/>.

using MSCLoader;
using UnityEngine;

namespace ActualMop
{
    public class ActualMop : Mod
    {
        public override string ID => "ActualMop"; //Your mod ID (unique)
        public override string Name => "ACTUAL MOP"; //You mod name
        public override string Author => "Athlon"; //Your Username
        public override string Version => "1.1"; //Version
        public override string UpdateLink => "https://github.com/Athlon007/ActualMop";
        public override byte[] Icon => Properties.Resources.icon;

        // Mop object
        GameObject mop;

        const string SaveFile = "mop.cfg";

        // Called once, when mod is loading after game is fully loaded
        public override void OnLoad()
        {
            // Load dem assets
            AssetBundle ab = ModAssets.LoadBundle(this, "mop.unity3d");
            GameObject originalMop = ab.LoadAsset<GameObject>("mop.prefab");
            mop = GameObject.Instantiate<GameObject>(originalMop);
            ab.Unload(false);

            // Add MopBehaviour component
            MopBehaviour behaviour = mop.AddComponent<MopBehaviour>();

            // Load save data
            MopSaveData mopSaveData = ModSave.Load<MopSaveData>(SaveFile);
            if (mopSaveData != null)
            {
                behaviour.Initialize(mopSaveData);
            }

            GameObject actualMopManager = new GameObject("ActualMopManager");
            MopOptimization optimization = actualMopManager.AddComponent<MopOptimization>();
            optimization.Initialize(mop.transform);
        }

        public override void OnSave()
        {
            ModSave.Save(SaveFile, mop.GetComponent<MopBehaviour>().GetSaveInfo());
        }

        public override void OnNewGame()
        {
            // Delete save data on new game
            ModConsole.Log("[Actual Mop] Resetting save data.");
            ModSave.Delete(SaveFile);
        }

        public override void ModSettings()
        {
            modSettings.AddButton("resetMopPosition", "RESET MOP POSITION", ResetMopPosition);

            modSettings.AddHeader("CHANGELOG");
            modSettings.AddText(GetChangelog());
        }

        /// <summary>
        /// Resets mop position
        /// </summary>
        static void ResetMopPosition()
        {
            if (ModLoader.CurrentScene == CurrentScene.Game)
            {
                GameObject mop = GameObject.Find("mop(Clone)");
                mop.GetComponent<Rigidbody>().velocity = Vector3.zero;
                mop.transform.position = MopBehaviour.DefaultPosition;
                mop.transform.eulerAngles = MopBehaviour.DefaultEuler;
            }
        }

        /// <summary>
        /// Gets changelog from changelog.txt and adds rich text elements.
        /// </summary>
        /// <returns></returns>
        string GetChangelog()
        {
            string[] changelog = Properties.Resources.changelog.Split('\n');
            string output = "";
            for (int i = 0; i < changelog.Length; i++)
            {
                string line = changelog[i];

                // If line starts with ###, make it look like a header of section.
                if (line.StartsWith("###"))
                {
                    line = line.Replace("###", "");
                    line = $"<color=yellow><size=24>{line}</size></color>";
                }

                // Replace - with bullet.
                if (line.StartsWith("-"))
                {
                    line = line.Substring(1);
                    line = $"• {line}";
                }

                // Similar to the bullet, but also increase the tab.
                if (line.StartsWith("  -"))
                {
                    line = line.Substring(3);
                    line = $"    • {line}";
                }

                if (line.Contains("(Development)"))
                {
                    line = line.Replace("(Development)", "<color=orange>Development: </color>");
                }

                output += line + "\n";
            }

            return output;
        }
    }
}
