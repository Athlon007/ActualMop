using UnityEngine;

namespace ActualMop
{
    class MopOptimization : MonoBehaviour
    {
        Transform player;
        Transform mop;

        void Initialize(Transform mop)
        {
            this.mop = mop;
            player = GameObject.Find("PLAYER").transform;
        }

        void Update()
        {
            mop.gameObject.SetActive(Vector3.Distance(player.position, mop.position) < 200);
        }
    }
}
