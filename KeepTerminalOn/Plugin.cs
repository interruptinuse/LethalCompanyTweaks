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

namespace KeepTerminalOn {
	[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	public class Plugin: BaseUnityPlugin
	{
		public static Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
		public static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource("KeepTerminalOn");

		private void Awake()
		{
			harmony.PatchAll(typeof(Patches));
		}
	}

	[HarmonyPatch]
	internal class Patches
	{
		[HarmonyPatch(typeof(Terminal), "Update")]
		[HarmonyPostfix]
		public static void TerminalUpdatePostfix(Terminal __instance)
		{
			__instance.terminalUIScreen.gameObject.SetActive(true);
		}
	}
}
