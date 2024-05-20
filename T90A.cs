using GHPC.Effects.Voices;
using GHPC.Equipment.Optics;
using GHPC.State;
using GHPC.Utility;
using GHPC.Vehicle;
using GHPC.Weapons;
using GHPC;
using MelonLoader.Utils;
using MelonLoader;
using NWH.VehiclePhysics;
using System.Collections.Generic;
using System.IO;
using System;
using T90A;
using Thermals;
using UnityEngine;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using Reticle;
using TMPro;
using System.Linq;

namespace T90A
{
    public class T90A
    {
        static GameObject soviet_crew_voice;
        static Mesh hull_cleaned_mesh;

        static Material t72m1_material;
        static GameObject t72m1_composite_cheeks;
        static Mesh t72m1_turret_mesh;
        static GameObject _1g42_ui;

        static GameObject t90_turret;
        static GameObject t90_hull;
        static WeaponSystemCodexScriptable gun_2a46m5;

        static Dictionary<string, int> ammo_racks = new Dictionary<string, int>()
        {
            ["Hull Wet"] = 1,
            ["Hull Rear"] = 2,
            ["Hull Front"] = 3,
            ["Turret Spare"] = 4
        };

        public class UpdateAmmoTypeUI : MonoBehaviour
        {
            GameObject ap;
            GameObject heat;
            GameObject he;
            GameObject glatgm;
            GameObject current_display;
            public FireControlSystem fcs;

            Dictionary<AmmoType.AmmoShortName, GameObject> displays;

            void Awake()
            {
                Transform canvas = fcs.transform.Find("GPS/1G42 Canvas(Clone)/GameObject");
                ap = canvas.transform.Find("ammo text APFSDS (TMP)").gameObject;
                heat = canvas.transform.Find("ammo text HEAT (TMP)").gameObject;
                he = canvas.transform.Find("ammo text HE (TMP)").gameObject;
                glatgm = canvas.transform.Find("ammo text GLATGM (TMP)").gameObject;

                current_display = ap;

                displays = new Dictionary<AmmoType.AmmoShortName, GameObject>()
                {
                    [AmmoType.AmmoShortName.Sabot] = ap,
                    [AmmoType.AmmoShortName.Heat] = heat,
                    [AmmoType.AmmoShortName.He] = he,
                    [AmmoType.AmmoShortName.Missile] = glatgm,
                };
            }
            void Update()
            {
                if (displays[fcs.CurrentAmmoType.ShortName] != current_display)
                {
                    current_display.SetActive(false);
                    current_display = displays[fcs.CurrentAmmoType.ShortName];
                    current_display.SetActive(true);
                }
            }
        }

        public static IEnumerator Convert(GameState _)
        {
            foreach (Vehicle vic in Mod.vics)
            {
                GameObject vic_go = vic.gameObject;

                if (vic == null) continue;
                if (vic.FriendlyName != "T-72M1") continue;
                if (vic_go.GetComponent<AlreadyConverted>() != null) continue;

                vic_go.AddComponent<AlreadyConverted>();

                vic._friendlyName = "T-90A";

                // SOVIET CREW 
                vic.transform.Find("DE Tank Voice").gameObject.SetActive(false);
                GameObject crew_voice = GameObject.Instantiate(soviet_crew_voice, vic.transform);
                crew_voice.transform.localPosition = new Vector3(0, 0, 0);
                crew_voice.transform.localEulerAngles = new Vector3(0, 0, 0);
                CrewVoiceHandler handler = crew_voice.GetComponent<CrewVoiceHandler>();
                handler._chassis = vic._chassis as NwhChassis;
                handler._reloadType = CrewVoiceHandler.ReloaderType.AutoLoaderAZ;
                vic._crewVoiceHandler = handler;
                crew_voice.SetActive(true);

                // HIDE STUFF
                vic.AimablePlatforms[1].transform.parent.Find("T72_markings").Find("roundels_72M1").gameObject.SetActive(false);
                vic.AimablePlatforms[1].transform.parent.Find("T72_markings").Find("roundels_72M").gameObject.SetActive(false);
                vic.AimablePlatforms[1].transform.Find("optic cover parent").gameObject.SetActive(false);

                WeaponSystem weapon = vic.GetComponent<WeaponsManager>().Weapons[0].Weapon;
                FireControlSystem fcs = vic.GetComponentInChildren<FireControlSystem>();
                LoadoutManager loadout_manager = vic.GetComponent<LoadoutManager>();
                UsableOptic night_optic = fcs.NightOptic;
                UsableOptic day_optic = Util.GetDayOptic(fcs);

                weapon.CodexEntry = gun_2a46m5;
                Transform tpdk1 = vic.transform.Find("---MESH---/HULL/TURRET/GUN/---MAIN GUN SCRIPTS---/2A46/TPD-K1 gunner's sight");
                Transform laser = vic.transform.Find("---MESH---/HULL/TURRET/GUN/---MAIN GUN SCRIPTS---/2A46/TPD-K1 gunner's sight/laser");
                tpdk1.localPosition = new Vector3(-0.5161f, 0.3482f, -5.6541f);
                laser.localEulerAngles = Vector3.zero;
                laser.localPosition = new Vector3(0f, 0f, 0.4f);

                GameObject hud = GameObject.Instantiate(_1g42_ui, tpdk1.Find("GPS"));
                hud.SetActive(true);

                UpdateAmmoTypeUI update_ammo_type_ui = day_optic.gameObject.AddComponent<UpdateAmmoTypeUI>();
                update_ammo_type_ui.fcs = fcs;

                fcs._autoModeOnLase = true;
                fcs._horizontalLeadOnly = true;
                fcs.InertialCompensation = false;
                fcs._manualModeOnRangeSet = true;
                fcs._originalMainOpticAlign = OpticAlignment.BoresightStabilized;
                fcs._originalSuperelevationMode = true;
                fcs._originalSuperleadMode = true;
                fcs.AutoModeTriggers = new GHPC.Equipment.FcsManualModeCancelTrigger[] { GHPC.Equipment.FcsManualModeCancelTrigger.Lase };
                fcs.DynamicLead = true;
                fcs.FireGateAngle = 0.1f;
                fcs.ForceReticleToZeroRange = true;
                fcs.SuperelevateFireGating = true;
                fcs.HasManualMode = true;
                fcs.IgnoreHorizontalForFireGating = true;
                fcs.LaserAim = LaserAimMode.Fixed;
                fcs.MaxLaserRange = 4000f;
                fcs.ManualModeLead = GHPC.Equipment.FcsManualModeLead.DisableLeadCompensation;
                fcs.ManualModeSuperelevation = GHPC.Equipment.FcsManualModeSuperelevation.DisableSuperelevation;
                fcs.RecordTraverseRateBuffer = true;
                fcs.SuperelevateWeapon = true;
                fcs.SuperleadWeapon = true;
                fcs.TraverseBufferSeconds = 1f;
                fcs.ZeroReticleRangeInAutoMode = true;
                fcs.ManualModeOpticAlign = OpticAlignment.BoresightStabilized;

                day_optic.RotateAzimuth = true;
                day_optic.slot.DefaultFov = 20f;
                day_optic.slot.OtherFovs = new float[] {
                    18.55f, 17.1f, 15.65f, 14.2f, 12.75f,
                    11.3f, 9.85f, 8.4f
                };
                day_optic.LocalElevationLimits = new Vector2(-15f, 25f);
                day_optic.slot.VibrationBlurScale = 0.0f;
                day_optic.slot.VibrationShakeMultiplier = 0.0f;
                day_optic.slot.fovAspect = true;
                day_optic.RangeTextPrefix = "<mspace=0.9em>";
                day_optic.RangeTextQuantize = 5;
                day_optic.RangeText = hud.transform.Find("GameObject/range text (TMP)").GetComponent<TMP_Text>();
                day_optic.OverridingObject = hud.transform.Find("override indicator").gameObject;
                day_optic.ReadyToFireObject = hud.transform.Find("ready indicator").gameObject;

                day_optic.reticleMesh.reticleSO = ReticleMesh.cachedReticles["1G42"].tree;
                day_optic.reticleMesh.reticle = ReticleMesh.cachedReticles["1G42"];
                day_optic.reticleMesh.SMR = null;
                day_optic.reticleMesh.Load();

                AmmoClipCodexScriptable codex = Ammo.ap["3BM44M"];
                loadout_manager.LoadedAmmoTypes[0] = codex;
                loadout_manager.LoadedAmmoTypes[1] = Ammo.clip_codex_3bk18m;
                loadout_manager.LoadedAmmoTypes = Util.AppendToArray(loadout_manager.LoadedAmmoTypes, Ammo.clip_codex_9m119);
                for (int i = 0; i <= 4; i++)
                {
                    GHPC.Weapons.AmmoRack rack = loadout_manager.RackLoadouts[i].Rack;
                    rack.ClipTypes[0] = codex.ClipType;
                    rack.ClipTypes[1] = Ammo.clip_codex_3bk18m.ClipType;

                    if (i == 0 || i == 2) {
                        rack.ClipTypes = Util.AppendToArray(rack.ClipTypes, Ammo.clip_9m119);

                        loadout_manager.RackLoadouts[i].FixedChoices = new LoadoutManager.RackLoadoutFixedChoice[] {
                            new LoadoutManager.RackLoadoutFixedChoice() {
                                AmmoClipIndex = 3,
                                RackSlotIndex = i == 0 ? 21 : 10,
                            },
                            new LoadoutManager.RackLoadoutFixedChoice() {
                                AmmoClipIndex = 3,
                                RackSlotIndex = i == 0 ? 20 : 9,
                            }
                        };
                    }

                    Util.EmptyRack(rack);
                }
                loadout_manager._totalAmmoTypes = 4;
                loadout_manager.TotalAmmoCounts = new int[] { 28, 8, 4, 4 };

                loadout_manager.SpawnCurrentLoadout();
                weapon.Feed.AmmoTypeInBreech = null;
                weapon.Feed.Start();
                loadout_manager.RegisterAllBallistics();

                var to_empty = new string[] { "Turret Spare" };
                foreach (string rack in to_empty)
                {
                    int idx = ammo_racks[rack];
                    Util.EmptyRack(loadout_manager.RackLoadouts[idx].Rack);
                }

                Transform turret = vic.transform.Find("---MESH---/HULL/TURRET");
                turret.Find("T72M1_turret").gameObject.SetActive(false);
                turret.Find("convoylight_1").gameObject.SetActive(false);
                turret.Find("Object002").gameObject.SetActive(false);
                turret.Find("CUPOLA").GetComponent<LateFollowTarget>()._lateFollowers[0].gameObject.SetActive(false);
                turret.Find("LUNA").localScale = Vector3.zero;
                turret.Find("nsv_mount").localScale = Vector3.zero;
                turret.Find("CUPOLA").localScale = Vector3.zero;
                turret.Find("turret_hatch").localScale = Vector3.zero;
                turret.Find("EJECTOR").transform.localScale = Vector3.zero;
                vic.transform.Find("T72A_interior/T72/MESHES/HULL/TURRET/Turret.002").localScale = Vector3.zero;
                vic.transform.Find("T72A_interior/T72/MESHES/HULL/TURRET/CUPOLA").localScale = Vector3.zero;

                SkinnedMeshRenderer skinned_rend = vic.transform.Find("T72M1_mesh (1)/T72M1_skin_turret").GetComponent<SkinnedMeshRenderer>();
               
                Transform new_turret_transform = new GameObject().transform;
                new_turret_transform.parent = turret;
                new_turret_transform.localPosition = new Vector3(-0.0353f + 0.04f, 0f, -0.0153f);
                new_turret_transform.localScale = new Vector3(0.9f, 1f, 1f);
                new_turret_transform.localEulerAngles = Vector3.zero;

                Transform new_canvas_transform = new GameObject().transform;
                new_canvas_transform.parent = turret.Find("GUN");
                new_canvas_transform.localPosition = new Vector3(-0.0353f + 0.04f, 0f, -0.0473f);
                new_canvas_transform.localScale = new Vector3(0.72f, 1f, 1f);
                new_canvas_transform.localEulerAngles = Vector3.zero;

                Transform[] bones = skinned_rend.bones;
                bones[5] = new_turret_transform;
                bones[6] = new_canvas_transform;

                skinned_rend.bones = bones;

                turret.Find("GUN/muzzle identity").localPosition = new Vector3(-0.0353f + 0.04f, 0f, 5.3109f);
                turret.Find("GUN/---MAIN GUN SCRIPTS---/2A46/Animation ---TODO get actual gun recoil amount/Start").localPosition = new Vector3(-0.0353f + 0.04f, 0f, 0f);
                turret.Find("GUN/---MAIN GUN SCRIPTS---/2A46/Animation ---TODO get actual gun recoil amount/End").localPosition = new Vector3(-0.0353f + 0.04f, 0f, -0.25f);

                Transform hull_rend = vic.transform.Find("T72M1_mesh (1)/T72M1_hull");
                hull_rend.GetComponent<MeshFilter>().sharedMesh = hull_cleaned_mesh;
                hull_rend.GetComponent<MeshRenderer>().materials[1].color = new Color(0, 0, 0, 0);
                hull_rend.gameObject.AddComponent<HeatSource>();

                turret.Find("smoke rack").localScale = Vector3.zero;
                vic.transform.Find("---MESH---/equipment").gameObject.SetActive(false);

                GameObject _t90_turret = GameObject.Instantiate(t90_turret, turret);
                _t90_turret.transform.localPosition = new Vector3(0.04f, 0.3953f, -0.1647f);
                _t90_turret.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
                _t90_turret.AddComponent<LateFollowTarget>();

                Transform[] hatches = new Transform[] {
                    _t90_turret.transform.Find("CMDR HATCH"),
                    _t90_turret.transform.Find("GUNNER HATCH"),
                    _t90_turret.transform.Find("EJECTOR")
                };

                Transform[] hatches_armour = new Transform[] {
                    hatches[0].GetChild(0),
                    hatches[1].GetChild(0),
                    hatches[2].GetChild(0)
                };

                for (int i = 0; i < hatches.Length; i++) {
                    hatches[i].gameObject.AddComponent<LateFollowTarget>();
                    LateFollow hatch_armour = hatches_armour[i].gameObject.AddComponent<LateFollow>();

                    hatch_armour.gameObject.SetActive(true);
                    hatch_armour.FollowTarget = hatches[i].transform;
                    hatch_armour.ForceToRoot = true;
                    hatch_armour.enabled = true;
                    hatch_armour.Awake();
                    hatch_armour.gameObject.AddComponent<Reparent>();
                }

                Transform turret_followers = turret.GetComponent<LateFollowTarget>()._lateFollowers[0].transform;
                turret_followers.Find("ARMOR/Composite Armor Array").gameObject.SetActive(false);
                turret_followers.Find("ARMOR/Turret.002").gameObject.SetActive(false);
                turret_followers.Find("ARMOR/Loaders Hatch.002").gameObject.SetActive(false);
                turret_followers.Find("ARMOR/Ejector Door").gameObject.SetActive(false);
                turret_followers.Find("ARMOR/Turret Storage box").gameObject.SetActive(false);

                LateFollow t90_turret_armor = _t90_turret.transform.Find("ARMOR").gameObject.AddComponent<LateFollow>();
                t90_turret_armor.gameObject.SetActive(true);
                t90_turret_armor.FollowTarget = _t90_turret.transform;
                t90_turret_armor.ForceToRoot = true;
                t90_turret_armor.enabled = true;
                t90_turret_armor.Awake();
                _t90_turret.transform.Find("ARMOR").gameObject.AddComponent<Reparent>();

                Transform ejector_script = turret.Find("---TURRET SCRIPTS---/Ejector");
                ejector_script.parent = _t90_turret.transform;
                ejector_script.localPosition = new Vector3(0.1375f, 0.0554f, -0.0494f);
                ejector_script.localScale = new Vector3(1f, 1f, 1f);
                ejector_script.localEulerAngles = Vector3.zero;

                Transform start = ejector_script.Find("Start");
                start.localPosition = Vector3.zero;
                start.localEulerAngles = new Vector3(0f, 0f, 344.8222f);

                Transform end = ejector_script.Find("End");
                end.localPosition = Vector3.zero;
                end.localEulerAngles = new Vector3(281.0823f, 0f, 344.8222f);

                ejector_script.GetComponent<AnimatedPart>().Transform = _t90_turret.transform.Find("EJECTOR");

                Transform cmdr_hatch_script = turret.Find("CUPOLA/Cupola/commander's hatch");
                cmdr_hatch_script.parent = _t90_turret.transform;
                cmdr_hatch_script.localPosition = new Vector3(0.0178f, 0.1023f, 0.1096f);
                cmdr_hatch_script.localScale = new Vector3(1f, 1f, 1f);
                cmdr_hatch_script.localEulerAngles = Vector3.zero;

                Transform cmdr_start = cmdr_hatch_script.Find("Start transform");
                cmdr_start.localPosition = Vector3.zero;

                Transform cmdr_end = cmdr_hatch_script.Find("End trasnform");
                cmdr_end.localPosition = Vector3.zero;
                cmdr_end.localEulerAngles = new Vector3(0f, 0f, 101.0319f);

                cmdr_hatch_script.GetComponent<AnimatedPart>().Transform = hatches[0];
                cmdr_hatch_script.GetComponent<AnimatedPart>().AnimationTime = 1.3f;

                Transform gunner_hatch_script = turret.Find("---TURRET SCRIPTS---/Gunner's Hatch");
                gunner_hatch_script.parent = _t90_turret.transform;
                gunner_hatch_script.localPosition = new Vector3(0.0223f, 0.057f, -0.103f);
                gunner_hatch_script.localScale = new Vector3(1f, 1f, 1f);
                gunner_hatch_script.localEulerAngles = Vector3.zero;

                Transform gunner_start = gunner_hatch_script.Find("Start");
                gunner_start.localPosition = Vector3.zero;
                gunner_start.localEulerAngles = new Vector3(350.7226f, 0f, 0f);

                Transform gunner_end = gunner_hatch_script.Find("End");
                gunner_end.localPosition = Vector3.zero;
                gunner_end.localEulerAngles = new Vector3(350.7226f, 0f, 93.6874f);

                gunner_hatch_script.GetComponent<AnimatedPart>().Transform = hatches[1];

                Transform hull = vic.transform.Find("---MESH---/HULL");
                GameObject hull_stuff = GameObject.Instantiate(t90_hull, hull);
                hull_stuff.transform.localPosition = new Vector3(-0.3063f, -1.1033f, 0.302f);

                LateFollow t90_hull_armor = hull_stuff.transform.Find("ARMOR").gameObject.AddComponent<LateFollow>();
                t90_hull_armor.gameObject.SetActive(true);
                t90_hull_armor.FollowTarget = hull.transform;
                t90_hull_armor.ForceToRoot = true;
                t90_hull_armor.enabled = true;
                t90_hull_armor.Awake();
                hull_stuff.transform.Find("ARMOR").gameObject.AddComponent<Reparent>();

                Transform og_hull_colliders = vic.GetComponent<LateFollowTarget>()._lateFollowers[0].transform;
                og_hull_colliders.Find("AAR/Turret Ring").gameObject.SetActive(false);

                //Shtora.Add(turret);
         
                weapon.Feed.ReloadDuringMissileTracking = true;
                weapon.FireWhileGuidingMissile = false;
                GameObject guidance_computer_obj = GameObject.Instantiate(new GameObject("guidance computer"), vic.AimablePlatforms.Where(o => o.name == "---TURRET SCRIPTS---").First().transform.parent);
                MissileGuidanceUnit computer = guidance_computer_obj.AddComponent<MissileGuidanceUnit>();

                computer.AimElement = fcs.AimTransform;
                weapon.GuidanceUnit = computer;              
            }

            yield break;
        }

        private static void LoadTurretAssets()
        {
            Transform t90_turret_armor = t90_turret.transform.Find("ARMOR");

            UniformArmor turret_ring = t90_turret_armor.transform.Find("TURRET RING").gameObject.AddComponent<UniformArmor>();
            turret_ring._armorType = Armour.ru_welded_armor;
            turret_ring.gameObject.tag = "Penetrable";
            turret_ring.gameObject.layer = 8;
            turret_ring._name = "turret ring";
            turret_ring.PrimaryHeatRha = 30f;
            turret_ring.PrimarySabotRha = 30f;

            UniformArmor cmdr_hatch = t90_turret.transform.Find("CMDR HATCH/CMDR HATCH ARMOR").gameObject.AddComponent<UniformArmor>();
            cmdr_hatch.gameObject.tag = "Penetrable";
            cmdr_hatch.gameObject.layer = 8;
            cmdr_hatch._name = "commander's hatch";
            cmdr_hatch.PrimaryHeatRha = 20f;
            cmdr_hatch.PrimarySabotRha = 20f;

            UniformArmor gunner_hatch = t90_turret.transform.Find("GUNNER HATCH/GUNNER HATCH ARMOR").gameObject.AddComponent<UniformArmor>();
            gunner_hatch.gameObject.tag = "Penetrable";
            gunner_hatch.gameObject.layer = 8;
            gunner_hatch._name = "gunner's hatch";
            gunner_hatch.PrimaryHeatRha = 15f;
            gunner_hatch.PrimarySabotRha = 15f;

            UniformArmor ejector = t90_turret.transform.Find("EJECTOR/EJECTOR ARMOR").gameObject.AddComponent<UniformArmor>();
            ejector.gameObject.tag = "Penetrable";
            ejector.gameObject.layer = 8;
            ejector._name = "ejector door";
            ejector.PrimaryHeatRha = 15f;
            ejector.PrimarySabotRha = 15f;

            foreach (MeshRenderer mesh_renderer in t90_turret.transform.GetComponentsInChildren<MeshRenderer>())
            {
                if (!mesh_renderer.enabled) continue;

                mesh_renderer.material.shader = Shader.Find("Standard (FLIR)");
                HeatSource heat = mesh_renderer.gameObject.AddComponent<HeatSource>();
                heat.Heat = 2f;
                heat._heat = 2f;
                heat.heat = 2f;
            }

            foreach (Transform t in t90_turret.transform.Find("ARMOR").GetComponentsInChildren<Transform>())
            {
                t.gameObject.tag = "Penetrable";
                t.gameObject.layer = 8;
            }

            Transform storage_boxes = t90_turret.transform.Find("ARMOR/STORAGE BOXES");
            foreach (Transform storage_box in storage_boxes)
            {
                UniformArmor armour = storage_box.gameObject.AddComponent<UniformArmor>();
                armour._name = "storage container";
                armour.PrimaryHeatRha = 10f;
                armour.PrimarySabotRha = 10f;
            }

            Transform smokes = t90_turret.transform.Find("ARMOR/SMOKES");
            foreach (Transform smoke in smokes)
            {
                UniformArmor armour = smoke.gameObject.AddComponent<UniformArmor>();
                armour._name = "smoke launcher array";
                armour.PrimaryHeatRha = 3f;
                armour.PrimarySabotRha = 3f;
            }

            Transform shtoras = t90_turret.transform.Find("ARMOR/SHTORA");
            foreach (Transform shtora in shtoras)
            {
                UniformArmor armour = shtora.gameObject.AddComponent<UniformArmor>();
                if (shtora.name.Contains("THING"))
                {
                    armour._name = "laser warning receiver";
                    armour.PrimaryHeatRha = 3f;
                    armour.PrimarySabotRha = 3f;
                }
                else
                {
                    armour._name = "Shtora-1 emitter";
                    armour.PrimaryHeatRha = 8f;
                    armour.PrimarySabotRha = 8f;
                }
            }

            UniformArmor cmdr_cupola = t90_turret.transform.Find("ARMOR/CMDR CUPOLA ARMOR").gameObject.AddComponent<UniformArmor>();
            cmdr_cupola.gameObject.tag = "Penetrable";
            cmdr_cupola.gameObject.layer = 8;
            cmdr_cupola._name = "commander's cupola";
            cmdr_cupola.PrimaryHeatRha = 20f;
            cmdr_cupola.PrimarySabotRha = 20f;

            GameObject turret_front = t90_turret_armor.transform.Find("TURRET FRONT").gameObject;
            VariableArmor armor_turret_front = turret_front.AddComponent<VariableArmor>();
            armor_turret_front.SetName("turret");
            armor_turret_front.AverageRha = 70f;
            armor_turret_front._spallForwardRatio = 0.2f;
            armor_turret_front._armorType = Armour.ru_welded_armor;

            GameObject turret_side = t90_turret_armor.transform.Find("TURRET SIDES").gameObject;
            VariableArmor armor_turret_side = turret_side.AddComponent<VariableArmor>();
            armor_turret_side.SetName("turret side");
            armor_turret_side.AverageRha = 70f;
            armor_turret_side._spallForwardRatio = 0.2f;
            armor_turret_side._armorType = Armour.ru_welded_armor;

            GameObject turret_rear = t90_turret_armor.transform.Find("TURRET REAR").gameObject;
            VariableArmor armor_turret_rear = turret_rear.AddComponent<VariableArmor>();
            armor_turret_rear.SetName("turret rear");
            armor_turret_rear.AverageRha = 50f;
            armor_turret_rear._spallForwardRatio = 0.2f;
            armor_turret_rear._armorType = Armour.ru_welded_armor;

            GameObject turret_roof = t90_turret_armor.transform.Find("TURRET ROOF").gameObject;
            VariableArmor armor_turret_roof = turret_roof.AddComponent<VariableArmor>();
            armor_turret_roof.SetName("turret roof");
            armor_turret_roof.AverageRha = 45f;
            armor_turret_roof._armorType = Armour.ru_welded_armor;
            armor_turret_roof._spallForwardRatio = 0.2f;

            GameObject turret_left_cheek = t90_turret_armor.transform.Find("LEFT COMP CHEEK").gameObject;
            VariableArmor armor_turret_l_cheek = turret_left_cheek.AddComponent<VariableArmor>();
            armor_turret_l_cheek.SetName("turret cheek composite array");
            armor_turret_l_cheek._armorType = Armour.composite_armor;
            armor_turret_l_cheek._spallForwardRatio = 0.2f;
            AarVisual aar_l_cheek = turret_left_cheek.AddComponent<AarVisual>();
            aar_l_cheek.SwitchMaterials = false;
            aar_l_cheek.HideUntilAar = true;

            GameObject turret_right_cheek = t90_turret_armor.transform.Find("RIGHT COMP CHEEK").gameObject;
            VariableArmor armor_turret_r_cheek = turret_right_cheek.AddComponent<VariableArmor>();
            armor_turret_r_cheek.SetName("turret cheek composite array");
            armor_turret_r_cheek._armorType = Armour.composite_armor;
            armor_turret_r_cheek._spallForwardRatio = 0.2f;
            AarVisual aar_r_cheek = turret_right_cheek.AddComponent<AarVisual>();
            aar_r_cheek.SwitchMaterials = false;
            aar_r_cheek.HideUntilAar = true;

            GameObject turret_left_cheek_backing = t90_turret_armor.transform.Find("LEFT BACKING PLATE").gameObject;
            VariableArmor armor_turret_l_cheek_backing = turret_left_cheek_backing.AddComponent<VariableArmor>();
            armor_turret_l_cheek_backing.SetName("turret cheek backing plate");
            armor_turret_l_cheek_backing._armorType = Armour.ru_welded_armor;
            armor_turret_l_cheek_backing._spallForwardRatio = 0.2f;
            AarVisual aar_l_cheek_backing = turret_left_cheek_backing.AddComponent<AarVisual>();
            aar_l_cheek_backing.SwitchMaterials = false;
            aar_l_cheek_backing.HideUntilAar = true;

            GameObject turret_right_cheek_backing = t90_turret_armor.transform.Find("RIGHT BACKING PLATE").gameObject;
            VariableArmor armor_turret_r_cheek_backing = turret_right_cheek_backing.AddComponent<VariableArmor>();
            armor_turret_r_cheek_backing.SetName("turret cheek backing plate");
            armor_turret_r_cheek_backing._armorType = Armour.ru_welded_armor;
            armor_turret_r_cheek_backing._spallForwardRatio = 0.2f;
            AarVisual aar_r_cheek_backing = turret_right_cheek_backing.AddComponent<AarVisual>();
            aar_r_cheek_backing.SwitchMaterials = false;
            aar_r_cheek_backing.HideUntilAar = true;

            Kontakt5.ERA_Setup(
                t90_turret.transform.Find("ARMOR/K5").GetComponentsInChildren<Transform>(),
                t90_turret.transform.Find("K5 BRICKS").GetComponentsInChildren<MeshRenderer>()
            );

            t90_turret_armor.gameObject.SetActive(false);
        }

        private static void LoadHullAssets() {
            foreach (MeshRenderer mesh_renderer in t90_hull.transform.GetComponentsInChildren<MeshRenderer>())
            {
                if (!mesh_renderer.enabled) continue;

                mesh_renderer.material.shader = Shader.Find("Standard (FLIR)");
                HeatSource heat = mesh_renderer.gameObject.AddComponent<HeatSource>();
                heat.Heat = 2f;
                heat._heat = 2f;
                heat.heat = 2f;
            }

            foreach (Transform t in t90_hull.transform.Find("ARMOR").GetComponentsInChildren<Transform>())
            {
                t.gameObject.tag = "Penetrable";
                t.gameObject.layer = 8;
            }

            Kontakt5.ERA_Setup(
                t90_hull.transform.Find("ARMOR/K5").GetComponentsInChildren<Transform>(),
                t90_hull.transform.Find("K5 BRICKS").GetComponentsInChildren<MeshRenderer>()
            );
        }

        public static void Init()
        {
            if (soviet_crew_voice == null)
            {
                foreach (CrewVoiceHandler obj in Resources.FindObjectsOfTypeAll(typeof(CrewVoiceHandler)))
                {
                    if (obj.name != "RU Tank Voice") continue;
                    soviet_crew_voice = obj.gameObject;
                    break;
                }
            }

            if (t72m1_material == null)
            {
                foreach (Vehicle obj in Resources.FindObjectsOfTypeAll(typeof(Vehicle)))
                {
                    if (obj.gameObject.name == "T72M1")
                    {
                        t72m1_turret_mesh = obj.transform.Find("T72M1_mesh (1)/T72M1_turret").GetComponent<MeshFilter>().sharedMesh;
                        t72m1_material = obj.transform.Find("T72M1_mesh (1)/T72M1_turret").GetComponent<MeshRenderer>().materials[0];
                        t72m1_composite_cheeks = obj.transform.Find("T-72 TURRET COLLIDERS/ARMOR/Composite Armor Array").gameObject;
                    }
                }
            }

            if (_1g42_ui == null) {
                foreach (Vehicle obj in Resources.FindObjectsOfTypeAll(typeof(Vehicle)))
                {
                    if (obj.gameObject.name == "T80B")
                    {
                        _1g42_ui = obj.transform.Find("---MAIN GUN SCRIPTS---/2A46-2/1G42 gunner's sight/GPS/1G42 Canvas").gameObject;
                        _1g42_ui.SetActive(false);

                        if (!ReticleMesh.cachedReticles.ContainsKey("1G42"))
                        {
                            obj.transform.Find("---MAIN GUN SCRIPTS---/2A46-2/1G42 gunner's sight/GPS/Reticle Mesh").GetComponent<ReticleMesh>().Load();
                        }

                        break;
                    }
                }
            }

            if (gun_2a46m5 == null)
            {
                gun_2a46m5 = ScriptableObject.CreateInstance<WeaponSystemCodexScriptable>();
                gun_2a46m5.name = "gun_2a46m5";
                gun_2a46m5.CaliberMm = 125;
                gun_2a46m5.FriendlyName = "125mm Gun 2A46M-5";
                gun_2a46m5.Type = WeaponSystemCodexScriptable.WeaponType.LargeCannon;
            }

            if (t90_turret == null)
            {
                t90_turret = Mod.t90_bundle.LoadAsset<GameObject>("T90TURRET.prefab");
                t90_turret.hideFlags = HideFlags.DontUnloadUnusedAsset;

                t90_hull = Mod.t90_bundle.LoadAsset<GameObject>("T90HULL.prefab");
                t90_hull.hideFlags = HideFlags.DontUnloadUnusedAsset;

                hull_cleaned_mesh = Mod.t90_bundle.LoadAsset<Mesh>("t90_hull_cleaned.asset");
                hull_cleaned_mesh.hideFlags = HideFlags.DontUnloadUnusedAsset;

                LoadHullAssets();
                LoadTurretAssets();
            }

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Medium);
        }
    }
}