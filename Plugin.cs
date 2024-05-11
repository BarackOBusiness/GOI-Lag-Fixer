using BepInEx;


namespace TheLagFixer;

[BepInPlugin("GOI.Core.LagFixer", "The Lag Fixer", "0.1.0")]
public class Plugin : BaseUnityPlugin
{
    private void Awake()
    {
        // Plugin startup logic
        Logger.LogInfo($"The lag fixer has loaded.");
    }
}
