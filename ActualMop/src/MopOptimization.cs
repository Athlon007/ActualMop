using UnityEngine;

namespace ActualMop
{
    class MopOptimization : MonoBehaviour
    {
        Transform player;
        Transform mop;

        MopBehaviour behaviour;

        public void Initialize(Transform mop)
        {
            this.mop = mop;
            player = GameObject.Find("PLAYER").transform;

            behaviour = mop.gameObject.GetComponent<MopBehaviour>();
        }

        void Update()
        {
            if (!behaviour.IsLoaded) return;
            mop.gameObject.SetActive(Vector3.Distance(player.position, mop.position) < 200);
        }
    }
}
