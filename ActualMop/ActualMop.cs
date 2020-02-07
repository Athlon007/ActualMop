// Actual Mop
// Copyright(C) 2020 Athlon

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
        public override string Name => "Actual Mop (Beta)"; //You mod name
        public override string Author => "Athlon"; //Your Username
        public override string Version => "0.1"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;

        // Mop object
        GameObject mop;

        // Called once, when mod is loading after game is fully loaded
        public override void OnLoad()
        {
            // Load dem assets
            AssetBundle ab = LoadAssets.LoadBundle(this, "mop.unity3d");
            GameObject originalMop = ab.LoadAsset<GameObject>("mop.prefab");
            mop = GameObject.Instantiate<GameObject>(originalMop);
            ab.Unload(false);

            // Add MopBehaviour component
            MopBehaviour behaviour = mop.AddComponent<MopBehaviour>();

            // Load save data
            MopSaveData mopSaveData = SaveLoad.DeserializeSaveFile<MopSaveData>(this, "mop.cfg");
            if (mopSaveData != null)
            {
                behaviour.Initialize(mopSaveData);
            }
        }

        public override void OnSave()
        {
            SaveLoad.SerializeSaveFile(this, mop.GetComponent<MopBehaviour>().GetSaveInfo(), "mop.cfg");
        }
    }
}
