using MSCLoader;
using System.Linq;
using UnityEngine;
using System.IO;

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
