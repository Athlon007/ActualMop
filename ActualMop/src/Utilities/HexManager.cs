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

using System.Collections.Generic;
using System.Linq;
using MSCLoader;

namespace ActualMop
{
    class HexManager
    {
        // This class manages the hexadecimal values corresponding to appropriate keyboard keys.
        // See Virtual Code Keys: https://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx

        public static HexManager instance;

        Dictionary<string, byte> keys;
        const byte VK_DEFAULT = 0x50; // P key

        public HexManager()
        {
            instance = this;

            // Initialize and populate the dictionary
            keys = new Dictionary<string, byte>();
            keys.Add("0", 0x30);
            keys.Add("1", 0x31);
            keys.Add("2", 0x32);
            keys.Add("3", 0x33);
            keys.Add("4", 0x34);
            keys.Add("5", 0x35);
            keys.Add("6", 0x36);
            keys.Add("7", 0x37);
            keys.Add("8", 0x38);
            keys.Add("9", 0x39);
            keys.Add("A", 0x41);
            keys.Add("B", 0x42);
            keys.Add("C", 0x43);
            keys.Add("D", 0x44);
            keys.Add("E", 0x45);
            keys.Add("F", 0x46);
            keys.Add("G", 0x47);
            keys.Add("H", 0x48);
            keys.Add("I", 0x49);
            keys.Add("J", 0x4A);
            keys.Add("K", 0x4B);
            keys.Add("L", 0x4C);
            keys.Add("M", 0x4D);
            keys.Add("N", 0x4E);
            keys.Add("O", 0x4F);
            keys.Add("P", 0x50);
            keys.Add("Q", 0x51);
            keys.Add("R", 0x52);
            keys.Add("S", 0x53);
            keys.Add("T", 0x54);
            keys.Add("U", 0x55);
            keys.Add("V", 0x56);
            keys.Add("W", 0x57);
            keys.Add("X", 0x58);
            keys.Add("Y", 0x59);
            keys.Add("Z", 0x5A);
        }

        /// <summary>
        /// Returns currently binded key to urinating
        /// </summary>
        string GetCurrentlyBindedButton()
        {
            return cInput.GetText("Urinate");
        }

        /// <summary>
        /// Returns the hexadecimal value corresponding to currently binded button for urinating
        /// </summary>
        public byte GetHex()
        {
            string bind = GetCurrentlyBindedButton();
            
            // Test if binded is valid 
            // Return default value (P) if invalid
            if (!IsValidKey(bind))
            {
                ModConsole.LogError("[Actual Mop] Urinate key has to be binded to letter or a number on the keyboard!");
                return VK_DEFAULT;
            }

            return keys.First(t => t.Key == bind).Value;
        }

        /// <summary>
        /// Checks if the bind is a valid key.
        /// Bind has to be not longer than 1 character, and has to be digit or letter.
        /// </summary>
        bool IsValidKey(string bind)
        {
            // Binded key is too long for a number or character
            if (bind.Length > 1)
                return false;

            // Char has to be a digit or letter
            char c = bind.ToCharArray()[0];
            return char.IsLetter(c) || char.IsDigit(c);
        }
    }
}
