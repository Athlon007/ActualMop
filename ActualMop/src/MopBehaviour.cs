// Actual Mop
// Copyright(C) 2020-2022 Athlon

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
using UnityEngine;

namespace ActualMop
{
    class MopBehaviour : MonoBehaviour
    {
        bool isLoaded;
        public bool IsLoaded => isLoaded;

        public static Vector3 DefaultPosition = new Vector3(-13.5f, -0.5f, 3f);
        public static Vector3 DefaultEuler = new Vector3(341, 91.5f, 359);

        Rigidbody rb;

        PlayMakerFSM pissAreas, pissLogic;
        ParticleRenderer pissRenderer;
        Transform itemPivot;

        // Determs if player intends to use the mop
        bool isEquipped;

        // In hand object
        GameObject mopInHand, player;

        // This object's renderer
        GameObject renderer;

        float lastUrineValue, lastPissRate;

        // FSM
        FsmState itemPickedState;
        FsmEvent equipEvent, proceedDropEvent, proceedThrowEvent;

        MopSaveData mopSaveData;

        // SFX
        AudioSource waterSplashFloor;

        // Animation
        const float MopAnimationMin = 0.7f;
        const float MopAnimationMax = 1.3f;
        const float MopAnimationSpeed = 2f;
        bool moveUp;

        void Start()
        {
            player = GameObject.Find("PLAYER");

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

            pissLogic = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss").gameObject.GetComponents<PlayMakerFSM>().First(f => f.FsmName == "Logic");

            // Get player's piss particle renderer
            GameObject playerPiss = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/Piss/Fluid/Fluid").gameObject;
            pissRenderer = playerPiss.GetComponent<ParticleRenderer>();

            // Get item pivot
            itemPivot = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/ItemPivot");
            GetComponent<Rigidbody>().isKinematic = false;

            // Setting up in hand model
            // Get rid of Rigidbody and MopBehaviour
            UnityEngine.Object.Destroy(mopInHand.GetComponent<Rigidbody>());
            UnityEngine.Object.Destroy(mopInHand.GetComponent<MopBehaviour>());
            mopInHand.name = "MopInHand";
            mopInHand.transform.parent = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera");
            mopInHand.transform.localPosition = new Vector3(0.25f, -0.4f, 1);
            mopInHand.transform.localRotation = Quaternion.Euler(-80, -720, -720);
            mopInHand.SetActive(false);

            // Setting up "anti-drop" script
            itemPickedState = player.transform.Find("Pivot/AnimPivot/Camera/FPSCamera/1Hand_Assemble/Hand")
                            .GetComponents<PlayMakerFSM>()[0].FsmStates.FirstOrDefault(state => state.Name == "Item picked");
            equipEvent = (itemPickedState.Actions[4] as GetButtonDown).sendEvent;
            proceedDropEvent = (itemPickedState.Actions[1] as GetMouseButtonDown).sendEvent;
            proceedThrowEvent = (itemPickedState.Actions[2] as GetMouseButtonDown).sendEvent;

            waterSplashFloor = GameObject.Find("MasterAudio/HouseFoley/water_splash_floor").GetComponent<AudioSource>();

            //transform.Find("node_id4").gameObject.layer = LayerMask.NameToLayer("HingedObjects");
            //transform.Find("node_id4/node_id4 1").gameObject.layer = LayerMask.NameToLayer("HingedObjects");
            transform.Find("node_id4").GetComponent<MeshCollider>().enabled = false;
            //transform.Find("node_id4/node_id4 1").GetComponent<MeshCollider>().enabled = false;

            InitializationWait();

            if (mopSaveData != null)
            {
                transform.position = mopSaveData.Position();
                transform.eulerAngles = mopSaveData.Euler();
            }
            else
            {
                transform.position = DefaultPosition;
                transform.eulerAngles = DefaultEuler;
            }
            rb.velocity = Vector3.zero;
        }

        public void Initialize(MopSaveData mopSaveData)
        {
            this.mopSaveData = mopSaveData;
            transform.position = mopSaveData.Position();
            transform.eulerAngles = mopSaveData.Euler();
        }

        void InitializationWait()
        {
            StartCoroutine(WaitRoutine());
        }

        IEnumerator WaitRoutine()
        {
            rb.isKinematic = false;
            yield return new WaitForSeconds(2);
            transform.position = mopSaveData.Position();
            transform.eulerAngles = mopSaveData.Euler();
            rb.isKinematic = true;

            isLoaded = true;
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

                pissLogic.SendEvent("FINISHED");

                // Hold the urine level
                PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value = lastUrineValue <= 0 ? 1 : lastUrineValue;
                pissAreas.FsmVariables.FindFsmFloat("PissRate").Value = -400;

                PlaySound();
                waterSplashFloor.transform.position = player.transform.position;
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
                StartCoroutine(Animation());

                lastPissRate = pissAreas.FsmVariables.FindFsmFloat("PissRate").Value;
                lastUrineValue = PlayMakerGlobals.Instance.Variables.FindFsmFloat("PlayerUrine").Value;
                pissRenderer.enabled = !enabled;

                // Enable in hand model and disable this object's renderer
                mopInHand.SetActive(true);
                renderer.SetActive(false);

                // This piece of code prevents player from dropping the mop and lets him open doors, etc.
                (itemPickedState.Actions[1] as GetMouseButtonDown).sendEvent = equipEvent;
                (itemPickedState.Actions[2] as GetMouseButtonDown).sendEvent = equipEvent;
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
                StopSound();
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

        void PlaySound()
        {
            if (!waterSplashFloor.isPlaying)
            {
                waterSplashFloor.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
                waterSplashFloor.Play();
            }
        }

        void StopSound()
        {
            waterSplashFloor.pitch = 1;
            waterSplashFloor.Stop();
        }

        IEnumerator Animation()
        {
            while (isEquipped)
            {
                yield return null;
                Vector3 pos = mopInHand.transform.localPosition;
                pos.z = moveUp ? MopAnimationMax : MopAnimationMin;
                mopInHand.transform.localPosition = Vector3.Lerp(mopInHand.transform.localPosition, pos, Time.deltaTime * MopAnimationSpeed);

                if (!moveUp)
                {
                    if (mopInHand.transform.localPosition.z < MopAnimationMin + 0.1f)
                    {
                        moveUp = true;
                    }
                }
                else
                {
                    if (mopInHand.transform.localPosition.z > MopAnimationMax - 0.1f)
                    {
                        moveUp = false;
                    }
                }
            }
        }
    }
}