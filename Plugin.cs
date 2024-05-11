using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using FluffyUnderware.Curvy;

namespace TheLagFixer;

[BepInPlugin("GOI.Core.LagFixer", "The Lag Fixer", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"The lag fixer has loaded.");

        SceneManager.sceneLoaded += OnSceneLoaded;
        Harmony.CreateAndPatchAll(typeof(GamePatches));
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "Mian") {}
    }
}

public static class GamePatches {
    [HarmonyPatch(typeof(CameraControl), "FixedUpdate")]
    [HarmonyPrefix]
    public static bool FixedUpdate() {
        return false; // Don't run the original camera movement code
    }
}
