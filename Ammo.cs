using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Weapons;
using T90A;
using UnityEngine;

namespace T90A
{
    public class Ammo
    {
        static AmmoType ammo_3bm32;
        static AmmoType ammo_kobra;

        public static AmmoClipCodexScriptable clip_codex_3bm42;
        public static AmmoType.AmmoClip clip_3bm42;
        public static AmmoCodexScriptable ammo_codex_3bm42;
        public static AmmoType ammo_3bm42;
        public static GameObject ammo_3bm42_vis = null;

        public static AmmoClipCodexScriptable clip_codex_3bm46;
        public static AmmoType.AmmoClip clip_3bm46;
        public static AmmoCodexScriptable ammo_codex_3bm46;
        public static AmmoType ammo_3bm46;
        public static GameObject ammo_3bm46_vis = null;

        public static AmmoClipCodexScriptable clip_codex_3bm44m;
        public static AmmoType.AmmoClip clip_3bm44m;
        public static AmmoCodexScriptable ammo_codex_3bm44m;
        public static AmmoType ammo_3bm44m;
        public static GameObject ammo_3bm44m_vis = null;

        public static AmmoClipCodexScriptable clip_codex_3bm32;
        public static AmmoClipCodexScriptable clip_codex_3bk18m;

        public static AmmoClipCodexScriptable clip_codex_9m119;
        public static AmmoType.AmmoClip clip_9m119;
        public static AmmoCodexScriptable ammo_codex_9m119;
        public static AmmoType ammo_9m119;
        public static GameObject ammo_9m119_vis = null;

        public static Dictionary<string, AmmoClipCodexScriptable> ap;

        public static void Init()
        {
            if (ammo_3bm42 == null)
            {
                var composite_optimizations_3bm42 = new List<AmmoType.ArmorOptimization>() { };
                var composite_optimizations_3bm44m = new List<AmmoType.ArmorOptimization>() { };

                string[] composite_names = new string[] {
                    "Abrams special armor gen 1 hull front",
                    "Abrams special armor gen 1 mantlet",
                    "Abrams special armor gen 1 turret cheeks",
                    "Abrams special armor gen 1 turret sides",
                    "Abrams special armor gen 0 turret cheeks",
                    "Corundum ball armor",
                    "Kvartz"
                };

                foreach (ArmorCodexScriptable s in Resources.FindObjectsOfTypeAll<ArmorCodexScriptable>())
                {
                    if (composite_names.Contains(s.name) || (s.name.Contains("Abrams") && s.name.Contains("composite")))
                    {
                        AmmoType.ArmorOptimization optimization_3bm42 = new AmmoType.ArmorOptimization();
                        optimization_3bm42.Armor = s;
                        optimization_3bm42.RhaRatio = 0.75f;
                        composite_optimizations_3bm42.Add(optimization_3bm42);

                        AmmoType.ArmorOptimization optimization_3bm44m = new AmmoType.ArmorOptimization();
                        optimization_3bm44m.Armor = s;
                        optimization_3bm44m.RhaRatio = 0.67f;
                        composite_optimizations_3bm44m.Add(optimization_3bm44m);

                    }

                }

                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.AmmoType.Name == "3BM32 APFSDS-T") { ammo_3bm32 = s.AmmoType; }
                    if (s.AmmoType.Name == "9M112M Kobra") { ammo_kobra = s.AmmoType; }

                    if (ammo_kobra != null && ammo_3bm32 != null) break;
                }

                foreach (AmmoClipCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoClipCodexScriptable)))
                {
                    if (s.name == "clip_3BM32") { clip_codex_3bm32 = s; }
                    if (s.name == "clip_3BK18M") { clip_codex_3bk18m = s; }

                    if (clip_codex_3bm32 != null && clip_codex_3bk18m != null) break;
                }

                ammo_3bm42 = new AmmoType();
                Util.ShallowCopy(ammo_3bm42, ammo_3bm32);
                ammo_3bm42.Name = "3BM42 APFSDS-T";
                ammo_3bm42.Coeff = ammo_3bm42.Coeff / 2f;
                ammo_3bm42.Caliber = 125;
                ammo_3bm42.RhaPenetration = 540f;
                ammo_3bm42.Mass = 4.85f;
                ammo_3bm42.MuzzleVelocity = 1700f;
                ammo_3bm42.SpallMultiplier = 0.9f;
                ammo_3bm42.MaxSpallRha = 24f;
                ammo_3bm42.MinSpallRha = 6f;

                ammo_3bm42.ArmorOptimizations = composite_optimizations_3bm42.ToArray<AmmoType.ArmorOptimization>();

                ammo_codex_3bm42 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_3bm42.AmmoType = ammo_3bm42;
                ammo_codex_3bm42.name = "ammo_3bm42";

                clip_3bm42 = new AmmoType.AmmoClip();
                clip_3bm42.Capacity = 1;
                clip_3bm42.Name = "3BM42 APFSDS-T";
                clip_3bm42.MinimalPattern = new AmmoCodexScriptable[1];
                clip_3bm42.MinimalPattern[0] = ammo_codex_3bm42;

                clip_codex_3bm42 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_3bm42.name = "clip_3bm42";
                clip_codex_3bm42.ClipType = clip_3bm42;

                ammo_3bm42_vis = GameObject.Instantiate(ammo_3bm32.VisualModel);
                ammo_3bm42_vis.name = "3bm42 visual";
                ammo_3bm42.VisualModel = ammo_3bm42_vis;
                ammo_3bm42.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_3bm42;
                ammo_3bm42.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_3bm42;

                ammo_3bm44m = new AmmoType();
                Util.ShallowCopy(ammo_3bm44m, ammo_3bm42);
                ammo_3bm44m.Name = "3BM44M APFSDS-T";
                ammo_3bm44m.Caliber = 125;
                ammo_3bm44m.RhaPenetration = 560f;
                ammo_3bm44m.Mass = 4.6f;
                ammo_3bm44m.MuzzleVelocity = 1750f;
                ammo_3bm44m.SpallMultiplier = 0.9f;
                ammo_3bm44m.MaxSpallRha = 10f;
                ammo_3bm44m.MinSpallRha = 6f;

                ammo_3bm44m.ArmorOptimizations = composite_optimizations_3bm44m.ToArray<AmmoType.ArmorOptimization>();

                ammo_codex_3bm44m = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_3bm44m.AmmoType = ammo_3bm44m;
                ammo_codex_3bm44m.name = "ammo_3bm44m";

                clip_3bm44m = new AmmoType.AmmoClip();
                clip_3bm44m.Capacity = 1;
                clip_3bm44m.Name = "3BM44M APFSDS-T";
                clip_3bm44m.MinimalPattern = new AmmoCodexScriptable[1];
                clip_3bm44m.MinimalPattern[0] = ammo_codex_3bm44m;

                clip_codex_3bm44m = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_3bm44m.name = "clip_3bm44m";
                clip_codex_3bm44m.ClipType = clip_3bm44m;

                ammo_3bm44m_vis = GameObject.Instantiate(ammo_3bm32.VisualModel);
                ammo_3bm44m_vis.name = "3bm44m visual";
                ammo_3bm44m.VisualModel = ammo_3bm44m_vis;
                ammo_3bm44m.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_3bm44m;
                ammo_3bm44m.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_3bm44m;

                ammo_3bm46 = new AmmoType();
                Util.ShallowCopy(ammo_3bm46, ammo_3bm32);
                ammo_3bm46.Name = "3BM46 APFSDS-T";
                ammo_3bm46.Caliber = 125;
                ammo_3bm46.RhaPenetration = 650f;
                ammo_3bm46.Mass = 4.85f;
                ammo_3bm46.MuzzleVelocity = 1700f;
                ammo_3bm46.SpallMultiplier = 1f;
                ammo_3bm46.MaxSpallRha = 24f;
                ammo_3bm46.MinSpallRha = 6f;

                ammo_codex_3bm46 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_3bm46.AmmoType = ammo_3bm46;
                ammo_codex_3bm46.name = "ammo_3bm46";

                clip_3bm46 = new AmmoType.AmmoClip();
                clip_3bm46.Capacity = 1;
                clip_3bm46.Name = "3BM46 APFSDS-T";
                clip_3bm46.MinimalPattern = new AmmoCodexScriptable[1];
                clip_3bm46.MinimalPattern[0] = ammo_codex_3bm46;

                clip_codex_3bm46 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_3bm46.name = "clip_3bm46";
                clip_codex_3bm46.ClipType = clip_3bm46;

                ammo_3bm46_vis = GameObject.Instantiate(ammo_3bm32.VisualModel);
                ammo_3bm46_vis.name = "3bm46 visual";
                ammo_3bm46.VisualModel = ammo_3bm46_vis;
                ammo_3bm46.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_3bm46;
                ammo_3bm46.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_3bm46;

                ammo_9m119 = new AmmoType();
                Util.ShallowCopy(ammo_9m119, ammo_kobra);
                ammo_9m119.Name = "9M119 Refleks";
                ammo_9m119.Caliber = 125;
                ammo_9m119.RhaPenetration = 600f;
                ammo_9m119.MuzzleVelocity = 300f;
                ammo_9m119.RangedFuseTime = 17.7f;
                ammo_9m119.TntEquivalentKg = 5.72f;

                ammo_codex_9m119 = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_9m119.AmmoType = ammo_9m119;
                ammo_codex_9m119.name = "ammo_9m119_refleks";

                clip_9m119 = new AmmoType.AmmoClip();
                clip_9m119.Capacity = 1;
                clip_9m119.Name = "9M119 Refleks";
                clip_9m119.MinimalPattern = new AmmoCodexScriptable[1];
                clip_9m119.MinimalPattern[0] = ammo_codex_9m119;

                clip_codex_9m119 = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_9m119.name = "clip_9m119_refleks";
                clip_codex_9m119.ClipType = clip_9m119;

                ammo_9m119_vis = GameObject.Instantiate(ammo_kobra.VisualModel);
                ammo_9m119_vis.name = "9m119 visual";
                ammo_9m119.VisualModel = ammo_9m119_vis;
                ammo_9m119.VisualModel.GetComponent<AmmoStoredVisual>().AmmoType = ammo_9m119;
                ammo_9m119.VisualModel.GetComponent<AmmoStoredVisual>().AmmoScriptable = ammo_codex_9m119;

                ap = new Dictionary<string, AmmoClipCodexScriptable>()
                {
                    ["3BM32"] = clip_codex_3bm32,
                    ["3BM42"] = clip_codex_3bm42,
                    ["3BM46"] = clip_codex_3bm46,
                    ["3BM44M"] = clip_codex_3bm44m,

                };
            }
        }
    }
}