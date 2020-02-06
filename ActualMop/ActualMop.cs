using MSCLoader;
using System.Linq;
using UnityEngine;

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
        public override bool UseAssetsFolder => false;

        // Called once, when mod is loading after game is fully loaded
        public override void OnLoad()
        {
            GameObject mop = GameObject.Instantiate(GameObject.Find("sledgehammer(itemx)"));
            mop.transform.position = new Vector3(-16, 2, 14);
            mop.name = "Mop";
            mop.AddComponent<Mop>();
        }        
    }
}
