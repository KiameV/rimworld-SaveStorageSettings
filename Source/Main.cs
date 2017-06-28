using Harmony;
using RimWorld;
using SaveStorageSettings.Dialog;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace SaveStorageSettings
{

    [StaticConstructorOnStartup]
    class Main
    {
        public static readonly Texture2D DeleteX;

        static Main()
        {
            var harmony = HarmonyInstance.Create("com.savestoragesettings.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("SaveStorageSettings: Adding Harmony Postfix to Zone_Stockpile.GetGizmos(Pawn)");

            DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_Zone_GetGizmos
    {
        static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            List<Gizmo> l;
            if (__result != null)
                l = new List<Gizmo>(__result);
            else
                l = new List<Gizmo>(2);

            Command_Action a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/save", true);
            a.defaultLabel = "SaveStorageSettings.SaveZoneSettings".Translate();
            a.defaultDesc = "SaveStorageSettings.SaveZoneSettingsDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new SaveDialog(__instance)); };
            a.groupKey = 887767542;
            l.Add(a);

            a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/load", true);
            a.defaultLabel = "SaveStorageSettings.LoadZoneSettings".Translate();
            a.defaultDesc = "SaveStorageSettings.LoadZoneSettingsDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new LoadDialog(__instance)); };
            a.groupKey = 887767543;
            l.Add(a);

            __result = l;
        }
    }
}
