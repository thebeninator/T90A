using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Audio;
using GHPC.Effects;
using UnityEngine;
using HarmonyLib;
using GHPC.Utility;
using MelonLoader;

namespace T90A
{
    public class Shtora
    {
        static GameObject sensor;

        public static void Add(Transform origin) {
            GameObject _sensor = GameObject.Instantiate(sensor, origin);
            LateFollow follow = _sensor.AddComponent<LateFollow>();
            follow.FollowTarget = origin;
            follow.enabled = true;
            follow.Awake();
            follow._localRotShift = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            follow._localPosShift = new Vector3(0f, 0f, 15f);

            _sensor.AddComponent<Reparent>();
        }
        /*
        [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "penCheck")]
        public static class Dazzle
        {
            private static bool Prefix(GHPC.Weapons.LiveRound __instance, object[] __args)
            {
                if (__instance.IsSpall) return true;
                if (!__instance.Guided) return true;

                Collider collider = (Collider)__args[0];

                if (!collider.gameObject.name.Contains("SHTORA SENSOR")) return true;

                Vector3 impact_point = (Vector3)__args[3];
                Vector3 impact_path = (Vector3)__args[2];
                Vector3 normal = (Vector3)__args[1];

                bool hit_front_face = Vector3.Angle(normal, collider.gameObject.transform.forward) == 0;
                bool hit_side_face = Vector3.Angle(normal, collider.gameObject.transform.forward) == 90;
                // float angle_of_impact = Vector3.SignedAngle(impact_path, drozd.unit.transform.position - impact_point, Vector3.up);
                //&& Math.Abs(angle_of_impact) >= MIN_ENGAGEMENT_ANGLE && Math.Abs(angle_of_impact) <= MAX_ENGAGEMENT_ANGLE
                //if ((hit_front_face || hit_side_face))
                //{
                    __instance.Info = AmmoType.CopyOf(__instance.Info);
                    __instance.Info.SpiralPower = 50f;
                    __instance.Info.SpiralAngularRate = 5f;
                    MelonLogger.Msg("fghfghfg");
                //}

                return true;
            }
        }
        */
        public static void Init()
        {
            sensor = Mod.t90_bundle.LoadAsset<GameObject>("SHTORA SENSOR.prefab");
            sensor.hideFlags = HideFlags.DontUnloadUnusedAsset;
            sensor.transform.localScale = new Vector3(6f, 6f, 30f);
            sensor.layer = 7;
        }
    }
}
