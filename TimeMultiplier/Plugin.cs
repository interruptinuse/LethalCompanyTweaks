using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace TimeMultiplier {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin: BaseUnityPlugin
	{
		public static Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
		public static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource("TimeMultiplier");
		public static MethodInfo SyncGlobalTimeOnNetwork = typeof(TimeOfDay)
			.GetMethod("SyncGlobalTimeOnNetwork", BindingFlags.NonPublic | BindingFlags.Instance);
		public static Plugin Instance;
		public ConfigEntry<float> mul;

		private void Awake()
		{
			mul = Config.Bind("General", "Time multiplier", 1.0f, "Clock speed multiplier.");
			Instance = this;
			harmony.PatchAll(typeof(Patches));
		}

		public static void UpdateMultiplier()
		{
			SetMultiplier(Instance.mul.Value);
		}

		public static void ResetMultiplier()
		{
			SetMultiplier(1.0f);
		}

		internal static void SetMultiplier(float mul)
		{
			TimeOfDay tod = TimeOfDay.Instance;
			mls.LogInfo($"");
			tod.globalTimeSpeedMultiplier = mul;
			SyncGlobalTimeOnNetwork.Invoke(tod, new object[] {});
		}
	}

	[HarmonyPatch]
	internal class Patches
	{
		[HarmonyPatch(typeof(StartOfRound), "StartGame")]
		[HarmonyPostfix]
		public static void StartGamePostfix(Terminal __instance)
		{
			Plugin.UpdateMultiplier();
		}

		[HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
		[HarmonyPostfix]
		public static void ReviveDeadPlayersPostfix(StartOfRound __instance)
		{
			Plugin.ResetMultiplier();
			TimeOfDay tod = TimeOfDay.Instance;
			tod.globalTime = Mathf.Clamp(tod.globalTime, 0f, tod.globalTimeAtEndOfDay);
		}
	}
}
