using MSCLoader;
using System.Linq;
using UnityEngine;
using System.IO;

namespace ActualMop
{
    public class ActualMop : Mod
    {
        public override string ID => "Actual Mop"; //Your mod ID (unique)
        public override string Name => "ActualMop"; //You mod name
        public override string Author => "Athlon"; //Your Username
        public override string Version => "0.1"; //Version

        // Set this to true if you will be load custom assets from Assets folder.
        // This will create subfolder in Assets folder for your mod.
        public override bool UseAssetsFolder => true;

        GameObject mop;

        const string SaveFileName = "mop.cfg";

        // Called once, when mod is loading after game is fully loaded
        public override void OnLoad()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "mop.unity3d");
            GameObject originalMop = ab.LoadAsset<GameObject>("mop.prefab");
            mop = GameObject.Instantiate<GameObject>(originalMop);
            originalMop = null;
            ab.Unload(false);

            MopBehaviour behaviour = mop.AddComponent<MopBehaviour>();
            MopSaveData mopSaveData = SaveLoad.DeserializeSaveFile<MopSaveData>(this, SaveFileName);
        }

        public override void OnSave()
        {
            SaveLoad.SerializeSaveFile(this, mop.GetComponent<MopBehaviour>().GetSaveInfo(), SaveFileName);
        }
    }
}
