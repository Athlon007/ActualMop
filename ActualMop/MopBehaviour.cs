using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using HutongGames.PlayMaker;

namespace ActualMop
{
    class MopBehaviour : MonoBehaviour
    {
        public static Vector3 DefaultPosition =new Vector3(-12.8f, 0.2f, 2.4f);

        PlayMakerFSM pissAreas;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        float lastUrineValue;
        bool isHeld;

        // Import the user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // Declare some keyboard keys as constants with its respective code
        // See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        const int VK_RCONTROL = 0x50; // P key

        public MopBehaviour()
        {
            // Initialize the game object
            gameObject.name = "mop(Clone)";
            gameObject.layer = LayerMask.NameToLayer("Parts");
            gameObject.tag = "PART";
            gameObject.transform.parent = null;

            // Get PissAreas PlayMakerFSM
            pissAreas = GameObject.Find("PissAreas").GetComponent<PlayMakerFSM>();

            // Get player's piss particle renderer
            GameObject playerPiss = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/Fluid").gameObject;
            pissRenderer = playerPiss.GetComponent<ParticleRenderer>();

            // Get item pivot
            itemPivot = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/ItemPivot");
            GetComponent<Rigidbody>().isKinematic = false;
            transform.position = DefaultPosition;
        }

        public void Initialize(MopSaveData mopSaveData)
        {
            transform.position = mopSaveData.Position;
            transform.rotation = mopSaveData.Rotation;
        }

        void Update()
        {
            if (transform.parent == itemPivot)
            {
                ToggleCleaningMode(true);

                // Simulate the P key press
                keybd_event(VK_RCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(VK_RCONTROL, 0, KEYEVENTF_KEYUP, 0);

                // Hold the urine level
                PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue <= 0 ? 1 : lastUrineValue;
            }
            else
            { 
                ToggleCleaningMode(false);
            }
        }

        void ToggleCleaningMode(bool enabled)
        {
            if (enabled && pissAreas.FsmVariables.FindFsmFloat("PissRate").Value < 0)
                return;

            if (!enabled && pissAreas.FsmVariables.FindFsmFloat("PissRate").Value > 0)
                return;

            pissAreas.FsmVariables.FindFsmFloat("PissRate").Value *= -1;
            isHeld = enabled;
            pissRenderer.enabled = !enabled;

            if (enabled)
            {
                lastUrineValue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value;
            }
            else
            {
                StartCoroutine(DisableRoutine());
            }
        }

        IEnumerator DisableRoutine()
        {
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = 0;
            yield return null;
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue;
            pissRenderer.enabled = true;

            // Remove the dirtiness given by the game for finishing pissing
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerDirtiness").Value -= 5;
        }

        public MopSaveData GetSaveInfo()
        {
            return new MopSaveData(transform.position, transform.rotation);
        }
    }
}
