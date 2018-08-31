using Harmony;
using RimWorld;
using SaveStorageSettings.Dialog;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;
using System;

namespace SaveStorageSettings
{
    [StaticConstructorOnStartup]
    class HarmonyPatches
    {
        public static readonly Texture2D DeleteXTexture;
        public static readonly Texture2D SaveTexture;
        public static readonly Texture2D LoadTexture;
        public static readonly Texture2D AppendTexture;

        static HarmonyPatches()
        {
            var harmony = HarmonyInstance.Create("com.savestoragesettings.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message(
                "SaveStorageSettings: Harmony Patches:\n" +
                "    Postfix:\n" +
                "        Building.GetGizmos(IEnumerable<Gizmo>)\n" +
                "        Zone_Stockpile.GetGizmos(IEnumerable<Gizmo>)\n" +
                "        Dialog_ManageOutfits.DoWindowContents(Rect)\n");

            DeleteXTexture = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            SaveTexture = ContentFinder<Texture2D>.Get("UI/save", true);
            LoadTexture = ContentFinder<Texture2D>.Get("UI/load", true);
            AppendTexture = ContentFinder<Texture2D>.Get("UI/append", true);
        }
    }

    [HarmonyPatch(typeof(Building), "GetGizmos")]
    static class Patch_Building_GetGizmos
    {
        static void Postfix(Building __instance, ref IEnumerable<Gizmo> __result)
        {
            if (__instance.def.IsWorkTable)
            {
                string type = GetType(__instance.def.defName);
                if (type == null)
                    return;

                List<Gizmo> gizmos = new List<Gizmo>(__result)
                {
                    new Command_Action
                    {
                        icon = HarmonyPatches.SaveTexture,
                        defaultLabel = "SaveStorageSettings.SaveBills".Translate(),
                        defaultDesc = "SaveStorageSettings.SaveBillsDesc".Translate(),
                        activateSound = SoundDef.Named("Click"),
                        action = delegate {
                            Find.WindowStack.Add(new SaveCraftingDialog(type, ((Building_WorkTable)__instance).billStack));
                        },
                        groupKey = 987767552
                    },

                    new Command_Action
                    {
                        icon = HarmonyPatches.AppendTexture,
                        defaultLabel = "SaveStorageSettings.AppendBills".Translate(),
                        defaultDesc = "SaveStorageSettings.AppendBillsDesc".Translate(),
                        activateSound = SoundDef.Named("Click"),
                        action = delegate {
                            Find.WindowStack.Add(new LoadCraftingDialog(type, ((Building_WorkTable)__instance).billStack, LoadCraftingDialog.LoadType.Append));
                        },
                        groupKey = 987767553
                    },

                    new Command_Action
                    {
                        icon = HarmonyPatches.LoadTexture,
                        defaultLabel = "SaveStorageSettings.LoadBills".Translate(),
                        defaultDesc = "SaveStorageSettings.LoadBillsDesc".Translate(),
                        activateSound = SoundDef.Named("Click"),
                        action = delegate {
                            Find.WindowStack.Add(new LoadCraftingDialog(type, ((Building_WorkTable)__instance).billStack, LoadCraftingDialog.LoadType.Replace));
                        },
                        groupKey = 987767554
                    }
                };

                __result = gizmos;
            }
        }

        private static string GetType(string defName)
        {
            switch(defName)
            {
                case "ButcherSpot":
                case "TableButcher":
                    return "Butcher";
                case "HandTailoringBench":
                case "ElectricTailoringBench":
                    return "TailoringBench";
                case "FueledSmithy":
                case "ElectricSmithy":
                    return "Smithy";
                case "FueledStove":
                case "ElectricStove":
                    return "Stove";
                case "SimpleResearchBench":
                case "HiTechResearchBench":
                    return null;
            }
            return defName;
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_Zone_Stockpile_GetGizmos
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
                
                Find.WindowStack.Add(new LoadFilterDialog("Apparel_Management", outfit.filter));
            }

            Outfit selectedOutfit = GetSelectedOutfit(__instance);
            if (selectedOutfit != null)
            {
                Text.Font = GameFont.Small;
                GUI.BeginGroup(new Rect(220f, 49f, 300, 32f));
                if (Widgets.ButtonText(new Rect(0f, 0f, 150f, 32f), "SaveStorageSettings.SaveOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new LoadFilterDialog("Apparel_Management", selectedOutfit.filter));
                }
                if (Widgets.ButtonText(new Rect(160f, 0f, 150f, 32f), "LoadStorageSettings.SaveOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new LoadFilterDialog("Apparel_Management", selectedOutfit.filter));
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
