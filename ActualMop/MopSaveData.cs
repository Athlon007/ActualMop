using UnityEngine;

namespace ActualMop
{
    public class MopSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public MopSaveData()
        {
            this.Position = MopBehaviour.DefaultPosition;
            this.Rotation = new Quaternion();
        }

        public MopSaveData(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
