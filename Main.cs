using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.Audio;
using GHPC.Camera;
using GHPC.Player;
using GHPC.State;
using GHPC.Vehicle;
using MelonLoader;
using MelonLoader.Utils;
using NWH.VehiclePhysics;
using T90A;
using UnityEngine;

[assembly: MelonInfo(typeof(Mod), "T-90A", "1.0.0", "ATLAS")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace T90A
{
    public class Mod : MelonMod
    {
        public static Vehicle[] vics;
        public static MelonPreferences_Category cfg;

        private GameObject game_manager;
        public static AudioSettingsManager audio_settings_manager;
        public static PlayerInput player_manager;
        public static CameraManager camera_manager;
        public static AssetBundle t90_bundle;

        public IEnumerator GetVics(GameState _)
        {
            vics = GameObject.FindObjectsByType<Vehicle>(FindObjectsSortMode.None);

            yield break;
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (Util.menu_screens.Contains(sceneName)) return;

            if (t90_bundle == null) {
                t90_bundle = AssetBundle.LoadFromFile(Path.Combine(MelonEnvironment.ModsDirectory + "/t90a", "t90"));
            }

            game_manager = GameObject.Find("_APP_GHPC_");
            audio_settings_manager = game_manager.GetComponent<AudioSettingsManager>();
            player_manager = game_manager.GetComponent<PlayerInput>();
            camera_manager = game_manager.GetComponent<CameraManager>();

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            Ammo.Init();
            Armour.Init();
            Kontakt5.Init();
            Shtora.Init();
            T90A.Init();
        }
    }
}