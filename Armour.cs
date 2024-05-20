using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Equipment;
using UnityEngine;

namespace T90A
{
    public class Armour
    {
        public static ArmorCodexScriptable ru_welded_armor;
        public static ArmorCodexScriptable composite_armor;

        public static void Init() {
            if (ru_welded_armor == null)
            {
                ru_welded_armor = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                ru_welded_armor.name = "ru welded armor";

                ArmorType ru_welded = new ArmorType();
                ru_welded.Name = "welded steel";
                ru_welded.CanRicochet = true;
                ru_welded.CanShatterLongRods = true;
                ru_welded.NormalizesHits = true;
                ru_welded.ThicknessSource = ArmorType.RhaSource.Multipliers;
                ru_welded.SpallAngleMultiplier = 1f;
                ru_welded.SpallPowerMultiplier = 1f;
                ru_welded.RhaeMultiplierCe = 0.97f;
                ru_welded.RhaeMultiplierKe = 0.97f;
                ru_welded.CrushThicknessModifier = 1f;
                ru_welded_armor.ArmorType = ru_welded;

                composite_armor = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                composite_armor.name = "ru composite";

                ArmorType composite = new ArmorType();
                composite.Name = "composite";
                composite.CanRicochet = true;
                composite.CanShatterLongRods = true;
                composite.NormalizesHits = true;
                composite.ThicknessSource = ArmorType.RhaSource.Multipliers;
                composite.SpallAngleMultiplier = 1f;
                composite.SpallPowerMultiplier = 0.2f;
                composite.RhaeMultiplierCe = 1.67f;
                composite.RhaeMultiplierKe = 1.05f;
                composite.CrushThicknessModifier = 1f;
                composite_armor.ArmorType = composite;
            }
        }
    }
}
