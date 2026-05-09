using System.Reflection;
using BepInEx.Logging;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;

namespace TheLagFixer;

internal static class Hooks {
	private const BindingFlags Public = BindingFlags.Public;
	private const BindingFlags Instance = BindingFlags.Instance;

	private static ManualLogSource _logger;
	private static ILHook _hook;
	
	internal static void Hook(ManualLogSource logger) {
		_logger = logger;
		_logger.LogInfo("Hooking CameraControl::FixedUpdate");
		_hook = new ILHook(GetMethod<CameraControl>("FixedUpdate", Public | Instance), FixedUpdate);
	}

	internal static void Unhook() {
		_hook?.Dispose();
	}

	private static void FixedUpdate(ILContext il) {
		ILCursor cursor = new ILCursor(il).Goto(0);

		if (cursor.TryGotoNext(
			x => x.MatchLdarg(0),
			x => x.MatchCall<Component>("get_transform"),
			x => x.MatchLdarg(0),
			x => x.MatchCall<Component>("get_transform"),
			x => x.MatchCallvirt<Transform>("get_position"),
			x => x.MatchLdarg(0),
			x => x.MatchLdfld<CameraControl>("vel"),
			x => x.MatchCall<Time>("get_fixedDeltaTime"),
			x => x.MatchCall<Vector3>("op_Multiply"),
			x => x.MatchCall<Vector3>("op_Addition"),
			x => x.MatchCallvirt<Transform>("set_position")
		)) {
			_logger.LogInfo("CameraControl::FixedUpdate hooked");
		}
	}

	private static MethodInfo GetMethod<T>(string name, BindingFlags flags) {
		return typeof(T).GetMethod(name, flags);
	}
}
