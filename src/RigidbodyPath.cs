using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace TheLagFixer.Rigidbody;

public static class Patches {
	[HarmonyPatch(typeof(CameraControl), "FixedUpdate")]
	[HarmonyPrefix]
	public static bool FixedUpdate(
		CameraControl __instance,
		ref Camera ___mainCam,
		ref Camera ___backgroundCam,
		ref Vector3 ___lookaheadPos,
		ref Vector3 ___target,
		ref Vector3 ___vel,
		ref float ___waterLevel,
		ref float ___lastTF
	) {
		UnityEngine.Rigidbody rb = __instance.GetComponent<UnityEngine.Rigidbody>();
        Rigidbody2D player = __instance.player.GetComponent<Rigidbody2D>();

        if (__instance.loadFinished && Application.isPlaying)
		{
			bool customMap = UnityEngine.SceneManagement.SceneManager.sceneCount > 1;
			if (___backgroundCam == null) {
				___backgroundCam = __instance.transform.Find("BGCamera").GetComponent<Camera>();
			}
			if (__instance.player == null)
			{
				__instance.player = GameObject.Find("Player");
			}
			Vector3 vector = Vector3.zero;
			if (!customMap) {
				___lastTF = Mathf.Lerp(___lastTF, __instance.progressMeter.currentTF, 0.3f);
				__instance.spline.Interpolate(___lastTF);
				Vector3 vector2 = __instance.spline.Interpolate(___lastTF + 0.01f);
				Vector3 vector3 = __instance.spline.Interpolate(___lastTF + 0.02f);
				Vector3 vector4 = __instance.spline.Interpolate(___lastTF + 0.03f);
				Vector3 b = 0.3333f * (vector + vector2 + vector3);
				___lookaheadPos = Vector3.Lerp(___lookaheadPos, b, 0.3f);
				vector = ___lookaheadPos - player.transform.position;
			}
			vector.z = 0f;
			___target = new Vector3(player.position.x, player.position.y, 0f) + vector.normalized * 2f;
			if (!customMap) {
				___target.y = Mathf.Max(___waterLevel + ___mainCam.orthographicSize - 2f, ___target.y);
				___target.z = -20f;
			} else {
				___target.z = GOILevelImporter.Base.metadata.zPlane;
				___mainCam.farClipPlane = GOILevelImporter.Base.metadata.Farplane;
				___backgroundCam.farClipPlane = GOILevelImporter.Base.metadata.BGFarplane;
				if (GOILevelImporter.Base.metadata.CameraMode == 1) {
					___mainCam.orthographic = false;
					___backgroundCam.fieldOfView = 60f;
					___backgroundCam.transform.localPosition = Vector3.zero;
				}
			}
			Vector3 vector5 = new Vector3(0.001f * Mathf.Sin(Time.time), 0.001f * Mathf.Sin(Time.time), 0f);
			___target += vector5;
			Vector3 vector6 = ___target - __instance.transform.position;
			___vel += 60f * vector6 * Time.fixedDeltaTime - 0.12f * ___vel;
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, -20f) + ___vel * Time.fixedDeltaTime);
		}

        return false;
	}
}
