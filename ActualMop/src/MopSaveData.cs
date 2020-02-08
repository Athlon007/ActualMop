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
