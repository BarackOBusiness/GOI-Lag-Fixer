using HarmonyLib;
using UnityEngine;
using FluffyUnderware.Curvy;

namespace TheLagFixer.LateUpdate;

public class CameraControlAssist : MonoBehaviour {
	private Rigidbody2D player;

	private ProgressMeter progressMeter;
	private CurvySpline spline;
	private Camera mainCam;

	
private Vector3 lookaheadPos;
	// Wouldn't it be funny if I just called it pi but actually assigned it to like 3.33?
	private const float pi = 3.1415926535897932384626433f;

    private Vector3 target;
    private Vector3 vel;
    private float waterLevel;
    private float lastTF;

    private void Awake() {
        CameraControl cameraControl = gameObject.GetComponent<CameraControl>();

        player = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        progressMeter = cameraControl.progressMeter;
        spline = cameraControl.spline;
        mainCam = Camera.main;

        lookaheadPos = Vector3.zero;
        target = player.position;
        vel = Vector3.zero;
        waterLevel = cameraControl.water.GetComponent<MeshRenderer>().bounds.max.y;
    }
    
    private void LateUpdate() {
        Vector3 playerPos = new Vector3(player.position.x, player.position.y, 0f);
        
		lastTF = Math.ExpDecay(lastTF, progressMeter.currentTF, 36f, Time.deltaTime);
		spline.Interpolate(lastTF);
		Vector3 vector = spline.Interpolate(lastTF + 0.01f);
		Vector3 vector2 = spline.Interpolate(lastTF + 0.02f);
		Vector3 vector3 = spline.Interpolate(lastTF + 0.03f);
		Vector3 b = 0.3333f * (vector + vector2 + vector3);
		lookaheadPos = Math.ExpDecay(lookaheadPos, b, 36f, Time.deltaTime);
		Vector3 vector4 = lookaheadPos - playerPos;
		vector4.z = 0f;
		target = playerPos + vector4.normalized * 2f;
		target.y = Mathf.Max(waterLevel + mainCam.orthographicSize - 2f, target.y);
		target.z = -20f;
		Vector3 vector5 = new Vector3(0.001f * Mathf.Sin(Time.time), 0.001f * Mathf.Sin(Time.time), 0f);
		target += vector5;
		// Using pi rn, but it's (pi/e)*e don't ask man it did in fact serve a purpose
		transform.position = Math.ExpDecay(transform.position, target, pi, Time.deltaTime);
		// For reference
		// Vector3 vector6 = target - transform.position;
		// vel += 60f * vector6 * Time.fixedDeltaTime;
		// transform.position = transform.position + vel * Time.deltaTime;
    }
}

public static class Math {
	public static float ExpDecay(float a, float b, float decay, float dt) {
		return b+(a-b)*Mathf.Exp(-decay*dt);
	}

	// Now we'll see if this is the incorrect approach or not
	public static Vector3 ExpDecay(Vector3 a, Vector3 b, float decay, float dt) {
		return b+(a-b)*Mathf.Exp(-decay*dt);
	}
}

public static class Patches {
	[HarmonyPatch(typeof(CameraControl), "FixedUpdate")]
	[HarmonyPrefix]
	public static bool FixedUpdate() {
		return false; // Don't run original camera control code at all
	}
}
