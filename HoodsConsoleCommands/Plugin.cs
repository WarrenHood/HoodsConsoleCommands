using BepInEx;
using HarmonyLib;
using HoodsConsoleCommands.Patches;

namespace HoodsConsoleCommands
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        public static BepInEx.Logging.ManualLogSource logger;
        private void Awake() {
            logger = BepInEx.Logging.Logger.CreateLogSource("HoodsConsoleCommands");
            // Plugin startup logic
            logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");

            logger.LogInfo("Applying patches...");
            Harmony.CreateAndPatchAll(typeof(Patches.Patches));
            logger.LogInfo("Done patching");
        }
    }
}