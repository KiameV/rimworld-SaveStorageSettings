using Harmony;
using RimWorld;
using SaveStorageSettings.Dialog;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

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
            Log.Message("SaveStorageSettings: Adding Harmony Postfix to Dialog_ManageOutfits.DoWindowContents(Rect)");
            
            DeleteX = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);

            new GameObject("SaveStorageSettings_Loaded");
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_Zone_GetGizmos
    {
        static void Postfix(Zone_Stockpile __instance, ref IEnumerable<Gizmo> __result)
        {
            __result = GizmoUtil.AddSaveLoadGizmos(__result, "Zone_Stockpile", __instance.settings.filter);
        }
    }

    [HarmonyPatch(typeof(Dialog_ManageOutfits), "DoWindowContents")]
    static class Patch_Dialog_ManageOutfits_DoWindowContents
    {
        static void Postfix(Dialog_ManageOutfits __instance, Rect inRect)
        {
            if (Widgets.ButtonText(new Rect(480f, 0f, 150f, 35f), "SaveStorageSettings.LoadAsNew".Translate(), true, false, true))
            {
                Outfit outfit = Current.Game.outfitDatabase.MakeNewOutfit();
                SetSelectedOutfit(__instance, outfit);

                LoadDialog dialog = new LoadDialog("Apparel_Management", outfit.filter);
                Find.WindowStack.Add(dialog);
            }

            Outfit selectedOutfit = GetSelectedOutfit(__instance);
            if (selectedOutfit != null)
            {
                Text.Font = GameFont.Small;
                GUI.BeginGroup(new Rect(220f, 49f, 300, 32f));
                if (Widgets.ButtonText(new Rect(0f, 0f, 150f, 32f), "SaveStorageSettings.SaveOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new SaveDialog("Apparel_Management", selectedOutfit.filter));
                }
                if (Widgets.ButtonText(new Rect(160f, 0f, 150f, 32f), "LoadStorageSettings.SaveOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new LoadDialog("Apparel_Management", selectedOutfit.filter));
                }
                GUI.EndGroup();
            }
        }

        private static Outfit GetSelectedOutfit(Dialog_ManageOutfits dialog)
        {
            return (Outfit)typeof(Dialog_ManageOutfits).GetProperty("SelectedOutfit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).GetValue(dialog, null);
        }

        private static void SetSelectedOutfit(Dialog_ManageOutfits dialog, Outfit selectedOutfit)
        {
            typeof(Dialog_ManageOutfits).GetProperty("SelectedOutfit", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).SetValue(dialog, selectedOutfit, null);
        }
    }
}
