using System.Collections;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (scene.name == "Mian") {
            // Setup the camera, has to be a Rigidbody or else
            // it conflicts with the BoxCollider, which is not 2D
            Rigidbody body = Camera.main.gameObject.AddComponent<Rigidbody>();
            body.isKinematic = true;
            body.interpolation = RigidbodyInterpolation.Interpolate;

            GameObject player = GameObject.Find("Player");
            Rigidbody2D[] bodies = player.GetComponentsInChildren<Rigidbody2D>();

            foreach (Rigidbody2D rb in bodies) {
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            }
        }
    }
}

public static class GamePatches {
    [HarmonyPatch(typeof(CameraControl), "FixedUpdate")]
    [HarmonyPrefix]
    public static bool FixedUpdate(
        CameraControl __instance,
        ref Camera ___mainCam,
        ref Vector3 ___lookaheadPos,
        ref Vector3 ___target,
        ref Vector3 ___vel,
        ref float ___waterLevel,
        ref float ___lastTF
    ) {
        Rigidbody camBody = __instance.GetComponent<Rigidbody>();
        Rigidbody2D body = __instance.player.GetComponent<Rigidbody2D>();

        if (__instance.loadFinished && Application.isPlaying)
		{
			if (__instance.player == null)
			{
				__instance.player = GameObject.Find("Player");
			}
			___lastTF = Mathf.Lerp(___lastTF, __instance.progressMeter.currentTF, 0.3f);
			__instance.spline.Interpolate(___lastTF);
			Vector3 vector = __instance.spline.Interpolate(___lastTF + 0.01f);
			Vector3 vector2 = __instance.spline.Interpolate(___lastTF + 0.02f);
			Vector3 vector3 = __instance.spline.Interpolate(___lastTF + 0.03f);
			Vector3 b = 0.3333f * (vector + vector2 + vector3);
			___lookaheadPos = Vector3.Lerp(___lookaheadPos, b, 0.3f);
			Vector3 vector4 = ___lookaheadPos - new Vector3(body.position.x, body.position.y, 0f);
			vector4.z = 0f;
			___target = new Vector3(body.position.x, body.position.y, 0f) + vector4.normalized * 2f;
			___target.y = Mathf.Max(___waterLevel + ___mainCam.orthographicSize - 2f, ___target.y);
			___target.z = -20f;
			Vector3 vector5 = new Vector3(0.001f * Mathf.Sin(Time.time), 0.001f * Mathf.Sin(Time.time), 0f);
			___target += vector5;
			Vector3 vector6 = ___target - __instance.transform.position;
			___vel += 60f * vector6 * Time.fixedDeltaTime - 0.12f * ___vel;
            camBody.MovePosition(new Vector3(camBody.position.x, camBody.position.y, -20f) + ___vel * Time.fixedDeltaTime);
		}

        return false; // Don't run the original camera movement code
    }
}
