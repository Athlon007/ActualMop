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
        public static Vector3 DefaultPosition = new Vector3(-13.5f, -0.6f, 2.8f);

        PlayMakerFSM pissAreas;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        // Determs if player intends to use the mop
        bool useItem;

        // In hand object
        GameObject mopInHand;

        // This object's renderer
        GameObject renderer;

        float lastUrineValue;
        float lastPissRate;

        // Import the user32.dll
        // We're using keybd_event function in Win32 API to simulate the keyboard click
        // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-keybd_event
        [DllImport("user32.dll", SetLastError = true)]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        // Declare some keyboard keys as constants with its respective code
        // See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
        const int KEYEVENTF_KEYUP = 0x0002; //Key up flag
        byte virtualKey = 0x50;

        public MopBehaviour()
        {
            mopInHand = GameObject.Instantiate(this.gameObject);

            // Initialize the game object
            gameObject.name = "mop(Clone)";
            gameObject.layer = LayerMask.NameToLayer("Parts");
            gameObject.tag = "PART";
            gameObject.transform.parent = null;

            renderer = transform.Find("node_id4").gameObject;

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

            // Setting up in hand model
            Object.Destroy(mopInHand.GetComponent<Rigidbody>());
            Object.Destroy(mopInHand.GetComponent<MopBehaviour>());
            mopInHand.name = "MopInHand";
            mopInHand.transform.parent = GameObject.Find("PLAYER").transform.Find("Pivot/AnimPivot/Camera/FPSCamera");
            mopInHand.transform.localPosition = new Vector3(0.25f, -0.4f, 1);
            mopInHand.transform.localRotation = Quaternion.Euler(-80, -720, -720);
            mopInHand.SetActive(false);
        }

        public void Initialize(MopSaveData mopSaveData)
        {
            transform.position = mopSaveData.Position;
            transform.rotation = mopSaveData.Rotation;
        }

        void Update()
        {
            if (transform.parent == itemPivot && cInput.GetButtonDown("Use"))
            {
                useItem ^= true;
            }

            if (useItem)
            { 
                ToggleCleaningMode(true);

                // Simulate the P key press
                // Player HAS to have pissing button binded to P
                keybd_event(virtualKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
                keybd_event(virtualKey, 0, KEYEVENTF_KEYUP, 0);

                // Hold the urine level
                PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue <= 0 ? 1 : lastUrineValue;
                pissAreas.FsmVariables.FindFsmFloat("PissRate").Value = -400;
            }
            else
            { 
                ToggleCleaningMode(false);
            }

            if (transform.parent != itemPivot && useItem)
            {
                useItem = false;
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

            if (enabled)
            {
                // Get current key binded to urinating
                virtualKey = HexManager.instance.GetHex();

                lastPissRate = pissAreas.FsmVariables.FindFsmFloat("PissRate").Value;
                lastUrineValue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value;
                pissRenderer.enabled = !enabled;

                mopInHand.SetActive(true);
                renderer.SetActive(false);
            }
            else
            {
                StartCoroutine(DisableRoutine());
                mopInHand.SetActive(false);
                renderer.SetActive(true);
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
            // Restore last PissRate value
            pissAreas.FsmVariables.FindFsmFloat("PissRate").Value = lastPissRate;
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
