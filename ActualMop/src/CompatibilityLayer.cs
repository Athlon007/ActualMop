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

using MSCLoader;
using UnityEngine;

namespace ActualMop
{
#if PRO
    // This class' purpose is to add a settings compatibility for mods
    sealed class Settings : MSCLoader.Settings
    {
        public Settings(string id, string name, object value) : base(id, name, value) { }

        public static void AddButton(Mod mod, string id, string name, UnityEngine.Events.UnityAction onClick)
        {
            mod.modSettings.AddButton(id, name, onClick);
        }

        public static void AddButton(Mod mod, string id, string name, UnityEngine.Events.UnityAction onClick, Color32 foreground, Color32 background)
        {
            mod.modSettings.AddButton(id, name, onClick);
        }

        public new static void AddHeader(Mod mod, string text)
        {
            mod.modSettings.AddHeader(text);
        }

        public new static void AddText(Mod mod, string text)
        {
            mod.modSettings.AddText(text);
        }
    }
#endif
}
