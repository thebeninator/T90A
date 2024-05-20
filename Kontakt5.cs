using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GHPC;
using MelonLoader;
using MelonLoader.Utils;
using Thermals;
using UnityEngine;
using GHPC.Equipment;
using HarmonyLib;
using System.Reflection.Emit;
using static HarmonyLib.Tools.Logger;
using GHPC.Effects;
using GHPC.Audio;

namespace T90A
{
    public class Kontakt5
    {
        public class OnDestroy : MonoBehaviour
        {
            private bool done = false;
            private bool completely_destroyed = false;
            public MeshRenderer renderer;

            void Update()
            {
                if (!GetComponent<UniformArmor>().IsDetonated || done) return;
                renderer.material.color = new Color(0.55f, 0.55f, 0.55f);
                ParticleEffectsManager.Instance.CreateImpactEffectOfType(dummy_he, ParticleEffectsManager.FusedStatus.Fuzed, ParticleEffectsManager.SurfaceMaterial.Steel, false, transform.position);
                ImpactSFXManager.Instance.PlaySimpleImpactAudio(ImpactAudioType.MainGunHeat, transform.position);

                int rand = UnityEngine.Random.Range(0, 2);
                if (rand == 1 && !completely_destroyed)
                {
                    GetComponent<UniformArmor>()._isDetonated = false;
                    completely_destroyed = true;
                }
                else
                {
                    done = true;
                }
            }
        }

        public static GameObject k5_hull_array;
        public static GameObject k5_side_array;

        public static ArmorCodexScriptable kontakt5_so_big = null;
        public static ArmorType kontakt5_armour_big = new ArmorType();

        public static ArmorCodexScriptable kontakt5_so_small = null;
        public static ArmorType kontakt5_armour_small = new ArmorType();

        private static AmmoType dummy_he;

        public static void ERA_Setup(Transform[] era_transforms, MeshRenderer[] renderers)
        {
            for (int i = 1; i < era_transforms.Length; i++)
            {
                Transform transform = era_transforms[i];

                if (!transform.name.Contains("K5 SMALL") && !transform.name.Contains("K5 BIG")) continue;

                transform.gameObject.AddComponent<UniformArmor>();
                transform.gameObject.tag = "Penetrable";
                transform.gameObject.layer = 8;
                UniformArmor armor = transform.gameObject.GetComponent<UniformArmor>();
                armor.SetName("Kontakt-5");
                armor.PrimaryHeatRha = 50f;
                armor.PrimarySabotRha = 50f;
                armor.SecondaryHeatRha = 2f;
                armor.SecondarySabotRha = 2f;
                armor.ThicknessListed = UniformArmor.ThicknessMode.ActualThickness;
                armor._canShatterLongRods = true;
                armor._crushThicknessModifier = 1f;
                armor._normalizesHits = true;
                armor._isEra = true;
                armor.AngleMatters = true;

                if (kontakt5_so_big == null)
                {
                    kontakt5_so_big = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                    kontakt5_so_big.name = "kontakt-5 armour big";
                    kontakt5_armour_big.RhaeMultiplierKe = 4f;
                    kontakt5_armour_big.RhaeMultiplierCe = 5.5f;
                    kontakt5_armour_big.CanRicochet = true;
                    kontakt5_armour_big.CrushThicknessModifier = 1f;
                    kontakt5_armour_big.NormalizesHits = true;
                    kontakt5_armour_big.CanShatterLongRods = true;
                    kontakt5_armour_big.ThicknessSource = ArmorType.RhaSource.Multipliers;

                    kontakt5_so_big.ArmorType = kontakt5_armour_big;

                    kontakt5_so_small = ScriptableObject.CreateInstance<ArmorCodexScriptable>();
                    kontakt5_so_small.name = "kontakt-5 armour small";
                    kontakt5_armour_small.RhaeMultiplierKe = 1.5f;
                    kontakt5_armour_small.RhaeMultiplierCe = 2.5f;
                    kontakt5_armour_small.CanRicochet = true;
                    kontakt5_armour_small.CrushThicknessModifier = 1f;
                    kontakt5_armour_small.NormalizesHits = true;
                    kontakt5_armour_small.CanShatterLongRods = true;
                    kontakt5_armour_small.ThicknessSource = ArmorType.RhaSource.Multipliers;

                    kontakt5_so_small.ArmorType = kontakt5_armour_small;
                }

                armor._armorType = transform.name.Contains("BIG") ? kontakt5_so_big : kontakt5_so_small;
                OnDestroy on_destroy = transform.gameObject.AddComponent<OnDestroy>();
                on_destroy.renderer = renderers[i-1];
            }
        }

        public static void Init()
        {
            if (k5_hull_array == null)
            {
                dummy_he = new AmmoType();
                dummy_he.DetonateEffect = Resources.FindObjectsOfTypeAll<GameObject>().Where(o => o.name == "HEAT Impact").First();
                dummy_he.ImpactEffectDescriptor = new ParticleEffectsManager.ImpactEffectDescriptor()
                {
                    HasImpactEffect = true,
                    ImpactCategory = ParticleEffectsManager.Category.HighExplosive,
                    EffectSize = ParticleEffectsManager.EffectSize.Autocannon,
                    RicochetType = ParticleEffectsManager.RicochetType.None,
                    Flags = ParticleEffectsManager.ImpactModifierFlags.Medium,
                    MinFilterStrictness = ParticleEffectsManager.FilterStrictness.Low
                };
            }
        }
    }

    [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "penCheck")]
    public class InsensitiveERA
    {
        private static float pen_threshold = 40f;
        private static float caliber_threshold = 25f;

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var detonate_era = AccessTools.Method(typeof(GHPC.IArmor), "Detonate");
            var is_era = AccessTools.PropertyGetter(typeof(GHPC.IArmor), nameof(GHPC.IArmor.IsEra));
            var pen_rating = AccessTools.PropertyGetter(typeof(GHPC.Weapons.LiveRound), nameof(GHPC.Weapons.LiveRound.CurrentPenRating));
            var debug = AccessTools.Field(typeof(GHPC.Weapons.LiveRound), nameof(GHPC.Weapons.LiveRound.Debug));
            var shot_info = AccessTools.Field(typeof(GHPC.Weapons.LiveRound), nameof(GHPC.Weapons.LiveRound.Info));
            var caliber = AccessTools.Field(typeof(AmmoType), nameof(AmmoType.Caliber));

            var instr = new List<CodeInstruction>(instructions);
            int idx = -1;
            int debug_count = 0;
            Label endof = il.DefineLabel();
            Label exec = il.DefineLabel();

            // find location of if-statement for ERA det code 
            for (int i = 0; i < instr.Count; i++)
            {
                if (instr[i].opcode == OpCodes.Callvirt && instr[i].operand == (object)is_era)
                {
                    // ??????????? need to find out how to peek into the stack at runtime 
                    idx = i + 5; break;
                }
            }

            // find start of the next if-statement
            for (int i = idx; i < instr.Count; i++)
            {
                if (instr[i].opcode == OpCodes.Ldsfld && instr[i].operand == (object)debug)
                {
                    debug_count++;

                    // IL_0C26
                    if (debug_count == 1) instr[i].labels.Add(exec);


                    // IL_0C6C
                    if (debug_count == 2) { instr[i].labels.Add(endof); break; }
                }
            }

            var custom_instr = new List<CodeInstruction>() {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, shot_info),
                new CodeInstruction(OpCodes.Ldfld, caliber),
                new CodeInstruction(OpCodes.Ldc_R4, caliber_threshold),
                new CodeInstruction(OpCodes.Bge_S, exec),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, pen_rating),
                new CodeInstruction(OpCodes.Ldc_R4, pen_threshold),
                new CodeInstruction(OpCodes.Ble_Un_S, endof)
            };
            instr.InsertRange(idx, custom_instr);
        
            return instr;
        }
    }
}