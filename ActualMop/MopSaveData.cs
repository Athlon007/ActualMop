using UnityEngine;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ActualMop
{
    public class MopSaveData
    {
        public Vector3 Position;
        public Quaternion Rotation;

        public MopSaveData()
        {

        }

        public MopSaveData(Vector3 position, Quaternion rotation)
        {
            this.Position = position;
            this.Rotation = rotation;
        }
    }
}
