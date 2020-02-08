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

using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ActualMop
{
    class MopBehaviour : MonoBehaviour
    {
        public static Vector3 DefaultPosition =new Vector3(-13.5f, -0.6f, 2.8f);

        PlayMakerFSM pissAreas;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        float lastUrineValue;

        // Import the user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // Declare some keyboard keys as constants with its respective code
        // See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        const int VK_P = 0x50; // P key
        byte virtualKey = 0x50;

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

            virtualKey = HexManager.instance.GetHex();
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
                // Player HAS to have pissing button binded to P
                keybd_event(virtualKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(virtualKey, 0, KEYEVENTF_KEYUP, 0);

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
            // Ignore if PissRate is already set to < 0 and enabled == true
            if (enabled && pissAreas.FsmVariables.FindFsmFloat("PissRate").Value < 0)
                return;

            // Ignore if PissRate is set to > 0 and enabled == false
            if (!enabled && pissAreas.FsmVariables.FindFsmFloat("PissRate").Value > 0)
                return;

            // Multiple the PissRate by -1, so it will decrease/increase the piss stain.
            pissAreas.FsmVariables.FindFsmFloat("PissRate").Value *= -1;

            if (enabled)
            {
                lastUrineValue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value;
                pissRenderer.enabled = !enabled;
            }
            else
            {
                StartCoroutine(DisableRoutine());
            }
        }

        /// <summary>
        /// Used when player drops the mop.
        /// Sets the PlayerUrine value to 0, so the player stops pissing, waits a single frame,
        /// then resets PlayerUrine to lastUrineValue, and reenables pissRenderer
        /// </summary>
        /// <returns></returns>
        IEnumerator DisableRoutine()
        {
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = 0;
            yield return null;
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue;
            pissRenderer.enabled = true;

            // Remove the dirtiness given by the game for finishing pissing
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerDirtiness").Value -= 5;
        }

        /// <summary>
        /// Generates the game save info
        /// </summary>
        /// <returns></returns>
        public MopSaveData GetSaveInfo()
        {
            return new MopSaveData(transform.position, transform.rotation);
        }
    }
}
