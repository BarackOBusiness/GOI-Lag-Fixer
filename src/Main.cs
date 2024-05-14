using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace TheLagFixer;

[BepInPlugin("goi.core.lagfixer", "The Lag Fixer", "0.1.0")]
public class TheLagFixer : BaseUnityPlugin
{
    public ConfigEntry<bool> alternate;
    public ConfigEntry<bool> smooth;
    
    private void Awake()
    {
        alternate = Config.Bind(
            "",
            "Use alternative camera follow logic?",
            false,
            "With this option on, the camera gains a rigidbody that is moved on physics updates and interpolated like the player, with it off, the camera moves in LateUpdate"
        );

        smooth = Config.Bind(
            "",
            "Smoothed timestep",
            true,
            "Whether or not to use a smoothed value for multiplying the camera velocity every frame, only applies with alternate camera logic turned off."
        );
    
        // Plugin startup logic
        Logger.LogInfo($"All forms of physics engine induced stutter may have been fixed!");
        SceneManager.sceneLoaded += OnSceneLoad;
        if (alternate.Value) {
            Harmony.CreateAndPatchAll(typeof(Rigidbody.Patches));
        } else {
            Harmony.CreateAndPatchAll(typeof(LateUpdate.Patches));
        }
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Mian") {
            Rigidbody2D[] bodies = GameObject.Find("Player").GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D body in bodies) {
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
            string[] propNames = new string[]{ "Coffee+Cup+Takeaway", "SnowHat", "Orange" };
            foreach (string prop in propNames) {
                Rigidbody2D body = GameObject.Find($"Props/{prop}").GetComponent<Rigidbody2D>();
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            bodies = GameObject.Find("Rope4/Bone1/").GetComponentsInChildren<Rigidbody2D>();
            foreach (Rigidbody2D body in bodies) {
                body.interpolation = RigidbodyInterpolation2D.Interpolate;
            }

            if (alternate.Value) {
                StartCoroutine(Rigidbody.Rigidbody.SetupAlternateCamera());
            } else {
                Camera.main.gameObject.AddComponent<LateUpdate.CameraControlAssist>();
            }
        }
    }
}

