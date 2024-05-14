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
    private Vector3 target;
    private Vector3 vel;
    private float waterLevel;
    private float lastTF;

    private bool smooth;

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

        smooth = Resources.FindObjectsOfTypeAll<TheLagFixer>()[0].smooth.Value;
    }
    
    private void LateUpdate() {
        Vector3 playerPos = new Vector3(player.position.x, player.position.y, 0f);
        float deltaTime = smooth? Time.smoothDeltaTime : Time.deltaTime;
        
        lastTF = Mathf.Lerp(lastTF, progressMeter.currentTF, 0.3f);
		spline.Interpolate(lastTF);
		Vector3 vector = spline.Interpolate(lastTF + 0.01f);
		Vector3 vector2 = spline.Interpolate(lastTF + 0.02f);
		Vector3 vector3 = spline.Interpolate(lastTF + 0.03f);
		Vector3 b = 0.3333f * (vector + vector2 + vector3);
		lookaheadPos = Vector3.Lerp(lookaheadPos, b, 0.3f);
		Vector3 vector4 = lookaheadPos - playerPos;
		vector4.z = 0f;
		target = playerPos + vector4.normalized * 2f;
		target.y = Mathf.Max(waterLevel + mainCam.orthographicSize - 2f, target.y);
		target.z = -20f;
		Vector3 vector5 = new Vector3(0.001f * Mathf.Sin(Time.time), 0.001f * Mathf.Sin(Time.time), 0f);
		target += vector5;
		Vector3 vector6 = target - transform.position;
		vel += 60f * vector6 * Time.fixedDeltaTime - 0.12f * vel;
		transform.position = transform.position + vel * Time.deltaTime;
    }
}

public static class Patches {
	[HarmonyPatch(typeof(CameraControl), "FixedUpdate")]
	[HarmonyPrefix]
	public static bool FixedUpdate() {
		return false; // Don't run original camera control code at all
	}
}
