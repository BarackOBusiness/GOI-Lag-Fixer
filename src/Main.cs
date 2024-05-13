using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            false,
            "Whether or not to use a smoothed value for multiplying the camera velocity every frame, only applies with alternate camera logic turned off."
        );
    
        // Plugin startup logic
        Logger.LogInfo($"All forms of physics engine induced stutter may have been fixed!");
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
        
    }
}

public class CameraControlAssist : MonoBehaviour {
}
