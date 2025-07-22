using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using Game.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace ALittleNoahFix;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("LittleNoah.exe")]
public partial class ALittleNoahFix : BasePlugin
{
    private static ManualLogSource? LogSource { get; set; }

    public override void Load()
    {
        LogSource = Log;

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        
        // Initializes our configuration file, alongside loading graphics options.
        InitConfig();
        LoadGraphicsSettings();
        
        // Finally, load our patches.
        Harmony.CreateAndPatchAll(typeof(MousePatches));
        Harmony.CreateAndPatchAll(typeof(UIPatches));
        Harmony.CreateAndPatchAll(typeof(GraphicsPatches));
    }

    [HarmonyPatch]
    public class MousePatches
    {
        // NOTE: This doesn't seem to work at the moment. If it did, that would be great, so I could see where my cursor is in UnityExplorer.
        [HarmonyPatch(typeof(WindowsPlatformService), nameof(WindowsPlatformService.SetupTransparentCursor)), HarmonyPrefix]
        public static bool NOPTransparentCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            return false;
        }
    }

    [HarmonyPatch]
    public class GraphicsPatches
    {
        private static int m_PreviousRefreshRate;
        private static bool m_PreviousVSyncEnabled;
        
        [HarmonyPatch(typeof(Engine), nameof(Engine.DelayFrame)), HarmonyPrefix]
        public static bool PatchFramerateLimiter()
        {
            var refreshRate = Screen.currentResolution.m_RefreshRate;
            var vsync = _bvSync.Value;
            
            // Apply changes only when target display refresh rate is changed.
            // For example, new screen settings was 1920x1080@144hz, but old one was 1280x720@60hz
            // then the patch will be re-applied once
            if (m_PreviousRefreshRate == refreshRate && m_PreviousVSyncEnabled == vsync)
                return false;
            
            var fixedDt = 1.0f / refreshRate;
            // Let us adjust VSync.
            QualitySettings.vSyncCount = vsync ? 1 : 0;
            // Unlock the Framerate.
            Application.targetFrameRate = vsync ? refreshRate : -1;
            // FixedDeltaTime is seemingly being used by most things instead of DeltaTime, which is why I assume the shitty framelimiter has been added. Anyways, after this, smooth as butter.
            Time.fixedDeltaTime = fixedDt;
            
            Debug.Log($"FPS Patch status:\r\nRefreshRate: {refreshRate}\r\nV-Sync: {vsync}\r\nV-Sync count: {QualitySettings.vSyncCount}");
            m_PreviousRefreshRate = refreshRate;
            m_PreviousVSyncEnabled = vsync;
            return false;
        }
    }

    [HarmonyPatch]
    public class UIPatches
    {
        [HarmonyPatch(typeof(CanvasScaler), "OnEnable")]
        [HarmonyPostfix]
        public static void CanvasScalerFixes(CanvasScaler __instance)
        {
            __instance.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        }

        [HarmonyPatch(typeof(Game.BattleMenuCompornent), nameof(Game.BattleMenuCompornent.Start)), HarmonyPostfix]
        public static void AddAspectRatioFitterToGameUI(Game.BattleMenuCompornent __instance)
        {
            if (__instance.gameObject.GetComponent<AspectRatioFitter>() != null) return;
            var arf = __instance.gameObject.AddComponent<AspectRatioFitter>();
            if (arf == null) return;
            AdjustAspectRatioFitter(arf);
        }
        
        private static void AdjustAspectRatioFitter(AspectRatioFitter arf)
        {
            arf.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            arf.enabled = true;
            // Check if the display aspect ratio is less than 16:9, and if so, disable the AspectRatioFitter and use the old transforms.
            if (Screen.currentResolution.m_Width / Screen.currentResolution.m_Height >= 1920.0f / 1080.0f) {
                arf.aspectRatio = 1920.0f / 1080.0f;
            }
            else {
                arf.aspectRatio = Screen.currentResolution.m_Width / (float)Screen.currentResolution.m_Height;
            }
        }
    }
}