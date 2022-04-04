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

using UnityEngine;

namespace ActualMop
{
    public class MopSaveData
    {
        //public Vector3 Position;
        //public Vector3 Euler;

        public float PosX, PosY, PosZ, RotX, RotY, RotZ;

        public MopSaveData()
        {
            PosX = MopBehaviour.DefaultPosition.x;
            PosY = MopBehaviour.DefaultPosition.y;
            PosZ = MopBehaviour.DefaultPosition.z;

            RotX = MopBehaviour.DefaultEuler.x;
            RotY = MopBehaviour.DefaultEuler.y;
            RotZ = MopBehaviour.DefaultEuler.z;
        }

        public MopSaveData(Vector3 position, Vector3 euler)
        {
            PosX = position.x;
            PosY = position.y;
            PosZ = position.z;

            RotX= euler.x;
            RotY = euler.y;
            RotZ = euler.z;
        }

        public Vector3 Position() { return new Vector3(PosX, PosY, PosZ); }
        public Vector3 Euler() { return new Vector3(RotX, RotY, RotZ); }
    }
}
