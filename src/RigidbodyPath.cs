using System.Collections;
using UnityEngine;
using HarmonyLib;

namespace TheLagFixer.Rigidbody;

public static class Rigidbody {
    public static IEnumerator SetupAlternateCamera() {
        while (Camera.main.GetComponent<BoxCollider>()) {
            Component.Destroy(Camera.main.GetComponent<BoxCollider>());
            yield return null;
        }

        Rigidbody2D rb = Camera.main.gameObject.AddComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.isKinematic = true;
    }
}

public static class Patches {
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
		Rigidbody2D rb = __instance.GetComponent<Rigidbody2D>();
        Rigidbody2D player = __instance.player.GetComponent<Rigidbody2D>();

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
			Vector3 vector4 = ___lookaheadPos - new Vector3(player.position.x, player.position.y, 0f);
			vector4.z = 0f;
			___target = new Vector3(player.position.x, player.position.y, 0f) + vector4.normalized * 2f;
			___target.y = Mathf.Max(___waterLevel + ___mainCam.orthographicSize - 2f, ___target.y);
			___target.z = -20f;
			Vector3 vector5 = new Vector3(0.001f * Mathf.Sin(Time.time), 0.001f * Mathf.Sin(Time.time), 0f);
			___target += vector5;
			Vector3 vector6 = ___target - __instance.transform.position;
			___vel += 60f * vector6 * Time.fixedDeltaTime - 0.12f * ___vel;
            rb.MovePosition(new Vector3(rb.position.x, rb.position.y, -20f) + ___vel * Time.fixedDeltaTime);
		}

        return false;
	}
}
