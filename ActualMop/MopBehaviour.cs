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
        PlayMakerFSM pissAreas;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        Rigidbody rb;

        float lastUrineValue;
        bool isHeld;

        // Import the user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // Declare some keyboard keys as constants with its respective code
        // See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        public const int VK_RCONTROL = 0x50; // P key

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
            //rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if (transform.parent == itemPivot)
            {
                ToggleCleaningMode(true);
                keybd_event(VK_RCONTROL, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(VK_RCONTROL, 0, KEYEVENTF_KEYUP, 0);
                PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = 1;
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

            if (enabled)
            {
                lastUrineValue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value;
                pissRenderer.enabled = !enabled;
                //rb.freezeRotation = true;
            }
            else
            {
                StartCoroutine(DisableRoutine());
            }
        }

        IEnumerator DisableRoutine()
        {
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = 0;
            yield return new WaitForSeconds(1);
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue;
            pissRenderer.enabled = true;
        }

        public MopSaveData GetSaveInfo()
        {
            return new MopSaveData(transform.position, transform.rotation);
        }
    }
}
