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

using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ActualMop
{
    class MopBehaviour : MonoBehaviour
    {
        public static Vector3 DefaultPosition = new Vector3(-13.5f, -0.5f, 3f);
        public static Vector3 DefaultEuler = new Vector3(341, 91.5f, 359);

        Rigidbody rb;

        PlayMakerFSM pissAreas;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        // Determs if player intends to use the mop
        bool isEquipped;

        // In hand object
        GameObject mopInHand;

        GameObject player;

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

        FsmState itemPickedState;
        FsmEvent equipEvent, proceedDropEvent, proceedThrowEvent;

        bool isPaused;

        void Start()
        {
            // Clone this game object to be used later for in hand object
            mopInHand = GameObject.Instantiate(this.gameObject);

            rb = GetComponent<Rigidbody>();

            // Initialize the game object
            gameObject.name = "mop(Clone)";
            gameObject.layer = LayerMask.NameToLayer("Parts");
            gameObject.tag = "ITEM";
            gameObject.transform.parent = null;

            // Get this object's renderer
            renderer = transform.Find("node_id4").gameObject;

            // Get PissAreas PlayMakerFSM
            pissAreas = GameObject.Find("PissAreas").GetComponent<PlayMakerFSM>();

            // Get player's piss particle renderer
            player = GameObject.Find("PLAYER");
            GameObject playerPiss = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/Fluid").gameObject;
            pissRenderer = playerPiss.GetComponent<ParticleRenderer>();

            // Get item pivot
            itemPivot = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/ItemPivot");
            GetComponent<Rigidbody>().isKinematic = false;

            virtualKey = HexManager.instance.GetHex();

            // Setting up in hand model
            // Get rid of Rigidbody and MopBehaviour
            Object.Destroy(mopInHand.GetComponent<Rigidbody>());
            Object.Destroy(mopInHand.GetComponent<MopBehaviour>());
            mopInHand.name = "MopInHand";
            mopInHand.transform.parent = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera");
            mopInHand.transform.localPosition = new Vector3(0.25f, -0.4f, 1);
            mopInHand.transform.localRotation = Quaternion.Euler(-80, -720, -720);
            mopInHand.SetActive(false);

            // Setting up "anti-drop" script
            itemPickedState = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand")
                            .GetComponents<PlayMakerFSM>()[0].FsmStates.FirstOrDefault(state => state.Name == "Item picked");
            equipEvent = (itemPickedState.Actions[5] as GetButtonDown).sendEvent;
            proceedDropEvent = (itemPickedState.Actions[1] as GetMouseButtonDown).sendEvent;
            proceedThrowEvent = (itemPickedState.Actions[2] as GetMouseButtonDown).sendEvent;

            InitializationWait();
            transform.position = DefaultPosition;
            transform.eulerAngles = DefaultEuler;
        }

        public void Initialize(MopSaveData mopSaveData)
        {
            transform.position = mopSaveData.Position;
            transform.eulerAngles = mopSaveData.Euler;
        }

        void InitializationWait()
        {
            StartCoroutine(WaitRoutine());
        }

        IEnumerator WaitRoutine()
        {
            rb.isKinematic = false;
            yield return new WaitForSeconds(2);
            rb.isKinematic = true;
        }

        void Update()
        {
            // If player holds the object and presses Use button, toggle isEquipped
            if (transform.parent == itemPivot)
            {
                if (!isEquipped)
                    PlayMakerGlobals.Instance.Variables.GetFsmBool("GUIuse").Value = true;

                if (cInput.GetButtonDown("Use"))
                    isEquipped ^= true;
            }

            // If is equipped, equip the mop
            if (isEquipped)
            {
                ToggleCleaningMode(true);

                // Simulate the P key press
                // Player HAS to have pissing button binded to P
                if (!isPaused)
                {
                    keybd_event(virtualKey, 0, KEYEVENTF_EXTENDEDKEY, 0);
                    keybd_event(virtualKey, 0, KEYEVENTF_KEYUP, 0);
                }

                // Hold the urine level
                PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue <= 0 ? 1 : lastUrineValue;
                pissAreas.FsmVariables.FindFsmFloat("PissRate").Value = -400;
            }
            else
            {
                ToggleCleaningMode(false);
            }

            // Throwing the mop while it's equipped means it will also remove the mop from hand
            if (transform.parent != itemPivot && isEquipped)
            {
                isEquipped = false;
                ToggleCleaningMode(false);
            }
        }

        /// <summary>
        /// Manages the cleaning mode toggling.
        /// </summary>
        /// <param name="enabled"></param>
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

                // Enable in hand model and disable this object's renderer
                mopInHand.SetActive(true);
                renderer.SetActive(false);

                // This piece of code prevents player from dropping the mop and lets him open doors, etc.
                (itemPickedState.Actions[1] as GetMouseButtonDown).sendEvent = equipEvent;
                (itemPickedState.Actions[2] as GetMouseButtonDown).sendEvent = equipEvent;
                (itemPickedState.Actions[3] as GetKeyDown).sendEvent = equipEvent;
                (itemPickedState.Actions[4] as GetKeyDown).sendEvent = equipEvent;
            }
            else
            {
                StartCoroutine(DisableRoutine());

                // Disable in hand model and toggle back on this object's renderer
                mopInHand.SetActive(false);
                renderer.SetActive(true);

                // Deactivate the previos script
                (itemPickedState.Actions[1] as GetMouseButtonDown).sendEvent = proceedDropEvent;
                (itemPickedState.Actions[2] as GetMouseButtonDown).sendEvent = proceedThrowEvent;
                (itemPickedState.Actions[3] as GetKeyDown).sendEvent = proceedDropEvent;
                (itemPickedState.Actions[4] as GetKeyDown).sendEvent = proceedDropEvent;
            }
        }

        /// <summary>
        /// Used when player drops the mop.
        /// Sets the PlayerUrine value to 0, so the player stops pissing, waits a single frame,
        /// then resets PlayerUrine to lastUrineValue, and reenables pissRenderer
        /// </summary>
        IEnumerator DisableRoutine()
        {
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = 0;
            yield return null;
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue;
            pissRenderer.enabled = true;

            // Remove the dirtiness given by the game for finishing pissing
            PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerDirtiness").Value -= 2.5f;
            // Restore last PissRate value
            pissAreas.FsmVariables.FindFsmFloat("PissRate").Value = lastPissRate;
        }

        /// <summary>
        /// Generates the game save info
        /// </summary>
        /// <returns></returns>
        public MopSaveData GetSaveInfo()
        {
            return new MopSaveData(transform.position, transform.eulerAngles);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            isPaused = !hasFocus;
        }

        void OnApplicationPause(bool pauseStatus)
        {
            isPaused = pauseStatus;
        }
    }
}