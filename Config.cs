// BepInEx and Harmony Stuff
using BepInEx.Configuration;
// Unity and System Stuff
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ALittleNoahFix;

public partial class ALittleNoahFix
{
        // Graphics Config
        public static ConfigEntry<float>  _fLodBias;                    // Default is 1.00, but this can be adjusted for an increased or decreased draw distance. 4.00 is the max I'd personally recommend for performance reasons.
        public static ConfigEntry<int>    _iForcedLodQuality;           // Default is 0, goes up to LOD #3 without cutting insane amounts of level geometry.
        public static ConfigEntry<int>    _iForcedTextureQuality;       // Default is 0, goes up to 1/14th resolution.
        public static ConfigEntry<int>    _anisotropicFiltering;        // 0: Off, 2: 2xAF, 4: 4xAF, 8: 8xAF, 16: 16xAF.

        // Framelimiter Config
        public static ConfigEntry<bool> _bvSync; // Self Explanatory. Prevents the game's framerate from going over the screen refresh rate, as that can cause screen tearing or increased energy consumption.
        
        // Misc Config
        //public static ConfigEntry<bool> _bSkipSplashScreenSequence; // True: Skips Opening Splash Screen Logos and Movie, False: Default Behavior.
        
        private void InitConfig()
        {

            // Graphics Config
            _fLodBias = Config.Bind("Graphics", "Draw Distance (Lod Bias)", (float)2.00, new ConfigDescription("Default is 2.00, but this can be adjusted for an increased or decreased draw distance. 4.00 is the max I'd personally recommend for performance reasons."));
            _iForcedLodQuality = Config.Bind("Graphics", "LOD Quality", 0, new ConfigDescription("0: No Forced LODs (Default), 1: Forces LOD # 1, 2: Forces LOD # 2, 3: Forces LOD # 3. Higher the value, the less mesh detail.", new AcceptableValueRange<int>(0, 3)));
            _iForcedTextureQuality = Config.Bind("Graphics", "Texture Quality", 0, new ConfigDescription("0: Full Resolution (Default), 1: Half-Res, 2: Quarter Res. Goes up to 1/14th res (14).", new AcceptableValueRange<int>(0, 14)));
            _anisotropicFiltering = Config.Bind("Graphics", "Anisotropic Filtering", 0, new ConfigDescription("0: Off, 2: 2xAF, 4: 4xAF, 8: 8xAF, 16: 16xAF", new AcceptableValueRange<int>(0, 16)));

            // Framelimiter Config
            _bvSync = Config.Bind("Framerate", "VSync", true, "Self Explanatory. Prevents the game's framerate from going over the screen refresh rate, as that can cause screen tearing or increased energy consumption.");

            // Misc Config
            //_bSkipSplashScreenSequence = Config.Bind("Misc", "Skip Splash Screens and Opening Video", false, "True: Skips Splash Screen and Opening Videos for faster startup times, False: Default Functionality.");
        }
        
        private static void LoadGraphicsSettings()
        {
            // TODO:
            // 1. Figure out why the texture filtering is not working correctly. Despite our patches, the textures are still blurry as fuck and has visible seams.
            // 2. Find a way of writing to the shadow resolution variables in the UniversalRenderPipelineAsset.
            
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            Texture.SetGlobalAnisotropicFilteringLimits(_anisotropicFiltering.Value, _anisotropicFiltering.Value);
            Texture.masterTextureLimit      = _iForcedTextureQuality.Value; // Can raise this to force lower the texture size. Goes up to 14.
            QualitySettings.maximumLODLevel = _iForcedLodQuality.Value; // Can raise this to force lower the LOD settings. 3 at max if you want it to look like a blockout level prototype.
            QualitySettings.lodBias         = _fLodBias.Value;
        }
}