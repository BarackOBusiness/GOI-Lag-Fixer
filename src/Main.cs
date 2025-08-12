using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;

namespace TheLagFixer;

[BepInPlugin("goi.core.lagfixer", "The Lag Fixer", "1.2.1")]
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
            "When this option is left default, the camera gains a rigidbody that is moved on physics updates and interpolated like the player. When it's on, the camera movement is moved to LateUpdate instead. It's recommended to keep this setting turned off for vanilla parity. Speedruns submitted to speedrun.com may be rejected when this option is turned on, usage is only advised for developmental purposes."
        );

        smooth = Config.Bind(
            "",
            "Smoothed timestep",
            true,
            "Whether or not to use a smoothed value for multiplying the camera velocity every frame, only applies with alternate camera logic turned on."
        );
    
        // Plugin startup logic
        Logger.LogInfo($"All forms of physics engine induced stutter may have been fixed!");
        SceneManager.sceneLoaded += OnSceneLoad;
        if (alternate.Value) {
            Harmony.CreateAndPatchAll(typeof(LateUpdate.Patches));
        } else {
            Harmony.CreateAndPatchAll(typeof(Rigidbody.Patches));
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
                Camera.main.gameObject.AddComponent<LateUpdate.CameraControlAssist>();
            } else {
                // Setup the camera for the rigidbody path; the component
                // must be a Rigidbody or else it conflicts with the BoxCollider
                // which is not 2D.
                // Destroying the BoxCollider has adverse effects in the level loader.
                UnityEngine.Rigidbody rb = Camera.main.gameObject.AddComponent<UnityEngine.Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.isKinematic = true;
            }
        }
    }
}

