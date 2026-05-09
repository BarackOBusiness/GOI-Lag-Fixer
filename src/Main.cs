using BepInEx;
using BepInEx.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheLagFixer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class LagFixerPlugin : BaseUnityPlugin {
    private void Awake() {
        SceneManager.sceneLoaded += OnSceneLoad;
        Hooks.Hook(Logger);
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoad;
        Hooks.Unhook();
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        if (scene.name != "Mian") return;
        // Add a rigidbody to the camera; the component must not be a
        // Rigidbody2D or else it conflicts with the regular BoxCollider.
        // Destroying the BoxCollider has adverse effects in the level loader.
        Rigidbody cameraRB = Camera.main.gameObject.AddComponent<Rigidbody>();
        cameraRB.interpolation = RigidbodyInterpolation.Interpolate;
        cameraRB.isKinematic = true;

        // For all other rigidbodies that exist at the time of Mian scene load
        Rigidbody2D[] bodies = Resources.FindObjectsOfTypeAll<Rigidbody2D>();
        foreach (var body in bodies) {
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
}
