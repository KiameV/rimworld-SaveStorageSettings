using HarmonyLib;
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
            var harmony = new Harmony("com.savestoragesettings.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            DeleteXTexture = ContentFinder<Texture2D>.Get("UI/Buttons/Delete", true);
            SaveTexture = ContentFinder<Texture2D>.Get("UI/save", true);
            LoadTexture = ContentFinder<Texture2D>.Get("UI/load", true);
            AppendTexture = ContentFinder<Texture2D>.Get("UI/append", true);
        }
    }

    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    static class Patch_Pawn_GetGizmos
    {
        static FieldInfo OnOperationTab = null;
        static Patch_Pawn_GetGizmos()
        {
            OnOperationTab = typeof(HealthCardUtility).GetField("onOperationTab", BindingFlags.Static | BindingFlags.NonPublic);
        }
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            foreach (var aGizmo in __result)
            {
                yield return aGizmo;
            }

            if (!(bool)OnOperationTab.GetValue(null))
                yield break;

            if (!__instance.IsColonist && !__instance.IsPrisoner)
                yield break;

            string type = "OperationHuman";
            if (__instance.RaceProps.Animal)
                type = "OperationAnimal";

            yield return new Command_Action
            {
                icon = HarmonyPatches.SaveTexture,
                defaultLabel = "SaveStorageSettings.SaveOperations".Translate(),
                defaultDesc = "SaveStorageSettings.SaveOperations".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate {
                    Find.WindowStack.Add(new SaveOperationDialog(type, __instance));
                },
                groupKey = 987764552
            };

            yield return new Command_Action
            {
                icon = HarmonyPatches.AppendTexture,
                defaultLabel = "SaveStorageSettings.LoadOperations".Translate(),
                defaultDesc = "SaveStorageSettings.LoadOperations".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate
                {
                    Find.WindowStack.Add(new LoadOperationDialog(__instance, type));
                },
                groupKey = 987764553
            };
        }
    }

    [HarmonyPatch(typeof(Building), "GetGizmos")]
    static class Patch_Building_GetGizmos
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building __instance)
        {
            foreach (var aGizmo in __result)
            {
                yield return aGizmo;
            }

            if (__instance.def.IsWorkTable)
            {
                string type = GetType(__instance.def.defName);
                if (type == null)
                    yield break;

                yield return new Command_Action
                {
                    icon = HarmonyPatches.SaveTexture,
                    defaultLabel = "SaveStorageSettings.SaveBills".Translate(),
                    defaultDesc = "SaveStorageSettings.SaveBillsDesc".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate {
                        Find.WindowStack.Add(new SaveCraftingDialog(type, ((Building_WorkTable)__instance).billStack));
                    },
                    groupKey = 987767552
                };

                yield return new Command_Action
                {
                    icon = HarmonyPatches.AppendTexture,
                    defaultLabel = "SaveStorageSettings.AppendBills".Translate(),
                    defaultDesc = "SaveStorageSettings.AppendBillsDesc".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate {
                        Find.WindowStack.Add(new LoadCraftingDialog(type, ((Building_WorkTable)__instance).billStack, LoadCraftingDialog.LoadType.Append));
                    },
                    groupKey = 987767553
                };

                yield return new Command_Action
                {
                    icon = HarmonyPatches.LoadTexture,
                    defaultLabel = "SaveStorageSettings.LoadBills".Translate(),
                    defaultDesc = "SaveStorageSettings.LoadBillsDesc".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate
                    {
                        Find.WindowStack.Add(new LoadCraftingDialog(type, ((Building_WorkTable)__instance).billStack, LoadCraftingDialog.LoadType.Replace));
                    },
                    groupKey = 987767554
                };
            }
        }

        private static string GetType(string defName)
        {
            switch (defName)
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

    [HarmonyPatch(typeof(Building_Storage), "GetGizmos")]
    static class Patch_BuildingStorage_GetGizmos
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Building __instance)
        {
            foreach (var aGizmo in __result)
            {
                yield return aGizmo;
            }

            if (__instance is Building_Storage)
            {
                string type = GetType(__instance.def.defName);
                string dn = __instance.def.defName;
                if (dn == "ChangeDresser")
                {
                    type = "Apparel_Management";
                }
                else if (dn == "WeaponStorage")
                {
                    type = "Weapon_Management";
                }

                yield return new Command_Action
                {
                    icon = HarmonyPatches.SaveTexture,
                    defaultLabel = "SaveStorageSettings.SaveZoneSettings".Translate(),
                    defaultDesc = "SaveStorageSettings.SaveZoneSettingsDesc".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate {
                        Find.WindowStack.Add(new SaveFilterDialog(type, ((Building_Storage)__instance).settings.filter));
                    },
                    groupKey = 987767552
                };

                yield return new Command_Action
                {
                    icon = HarmonyPatches.LoadTexture,
                    defaultLabel = "SaveStorageSettings.LoadZoneSettings".Translate(),
                    defaultDesc = "SaveStorageSettings.LoadZoneSettingsDesc".Translate(),
                    activateSound = SoundDef.Named("Click"),
                    action = delegate
                    {
                        Find.WindowStack.Add(new LoadFilterDialog(type, ((Building_Storage)__instance).settings.filter));
                    },
                    groupKey = 987767553

                };
            }
        }

        private static string GetType(string defName)
        {
            string s = defName.ToLower();
            if (s.Contains("shelf"))
            {
                return "shelf";
            }
            if (s.Contains("clothing"))
            {
                return "Apparel_Management";
            }
            return "Zone_Stockpile";
        }
    }

    [HarmonyPatch(typeof(Zone_Stockpile), "GetGizmos")]
    static class Patch_Zone_Stockpile_GetGizmos
    {
        static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Zone_Stockpile __instance)
        {
            foreach (var aGizmo in __result)
            {
                yield return aGizmo;
            }

            yield return new Command_Action
            {
                icon = HarmonyPatches.SaveTexture,
                defaultLabel = "SaveStorageSettings.SaveZoneSettings".Translate(),
                defaultDesc = "SaveStorageSettings.SaveZoneSettingsDesc".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate
                {
                    Find.WindowStack.Add(new SaveFilterDialog("Zone_Stockpile", __instance.settings.filter));
                },
                groupKey = 987767552
            };

            yield return new Command_Action
            {
                icon = HarmonyPatches.LoadTexture,
                defaultLabel = "SaveStorageSettings.LoadZoneSettings".Translate(),
                defaultDesc = "SaveStorageSettings.LoadZoneSettingsDesc".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate
                {
                    Find.WindowStack.Add(new LoadFilterDialog("Zone_Stockpile", __instance.settings.filter));
                },
                groupKey = 987767553
            };
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
                if (Widgets.ButtonText(new Rect(0f, 0f, 150f, 32f), "SaveStorageSettings.LoadOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new LoadFilterDialog("Apparel_Management", selectedOutfit.filter));
                }
                if (Widgets.ButtonText(new Rect(160f, 0f, 150f, 32f), "SaveStorageSettings.SaveOutfit".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new SaveFilterDialog("Apparel_Management", selectedOutfit.filter));
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

    [HarmonyPatch(typeof(Dialog_ManageDrugPolicies), "DoWindowContents")]
    static class Patch_Dialog_Dialog_ManageDrugPolicies
    {
        static void Postfix(Dialog_ManageDrugPolicies __instance, Rect inRect)
        {
            float x = 500;
            if (Widgets.ButtonText(new Rect(x, 0, 150f, 35f), "SaveStorageSettings.LoadAsNew".Translate(), true, false, true))
            {
                DrugPolicy policy = Current.Game.drugPolicyDatabase.MakeNewDrugPolicy();
                SetDrugPolicy(__instance, policy);

                Find.WindowStack.Add(new LoadPolicyDialog("DrugPolicy", policy));
            }
            x += 160;

            DrugPolicy selectedPolicy = GetDrugPolicy(__instance);
            if (selectedPolicy != null)
            {
                Text.Font = GameFont.Small;
                if (Widgets.ButtonText(new Rect(x, 0f, 75, 35f), "LoadGameButton".Translate(), true, false, true))
                {
                    string label = selectedPolicy.label;
                    Find.WindowStack.Add(new LoadPolicyDialog("DrugPolicy", selectedPolicy));
                    selectedPolicy.label = label;
                }
                x += 80;
                if (Widgets.ButtonText(new Rect(x, 0f, 75, 35f), "SaveGameButton".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new SavePolicyDialog("DrugPolicy", selectedPolicy));
                }
            }
        }

        private static DrugPolicy GetDrugPolicy(Dialog_ManageDrugPolicies dialog)
        {
            return (DrugPolicy)typeof(Dialog_ManageDrugPolicies).GetProperty("SelectedPolicy", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).GetValue(dialog, null);
        }

        private static void SetDrugPolicy(Dialog_ManageDrugPolicies dialog, DrugPolicy selectedPolicy)
        {
            typeof(Dialog_ManageDrugPolicies).GetProperty("SelectedPolicy", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).SetValue(dialog, selectedPolicy, null);
        }
    }

    [HarmonyPatch(typeof(Dialog_ManageFoodRestrictions), "DoWindowContents")]
    static class Patch_Dialog_ManageFoodRestrictions
    {
        static void Postfix(Dialog_ManageFoodRestrictions __instance, Rect inRect)
        {
            float x = 500;
            if (Widgets.ButtonText(new Rect(x, 0, 150f, 35f), "SaveStorageSettings.LoadAsNew".Translate(), true, false, true))
            {
                FoodRestriction restriction = Current.Game.foodRestrictionDatabase.MakeNewFoodRestriction();
                SetFoodRestriction(__instance, restriction);

                Find.WindowStack.Add(new LoadFoodRestrictionDialog("FoodRestriction", restriction));
            }

            FoodRestriction selected = GetFoodRestriction(__instance);
            if (selected != null)
            {
                Text.Font = GameFont.Small;
                if (Widgets.ButtonText(new Rect(x, 50f, 72, 35f), "LoadGameButton".Translate(), true, false, true))
                {
                    string label = selected.label;
                    Find.WindowStack.Add(new LoadFoodRestrictionDialog("FoodRestriction", selected));
                    selected.label = label;
                }
                x += 77;
                if (Widgets.ButtonText(new Rect(x, 50f, 73, 35f), "SaveGameButton".Translate(), true, false, true))
                {
                    Find.WindowStack.Add(new SaveFoodRestrictionDialog("FoodRestriction", selected));
                }
            }
        }

        private static FoodRestriction GetFoodRestriction(Dialog_ManageFoodRestrictions dialog)
        {
            return (FoodRestriction)typeof(Dialog_ManageFoodRestrictions).GetProperty("SelectedFoodRestriction", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).GetValue(dialog, null);
        }

        private static void SetFoodRestriction(Dialog_ManageFoodRestrictions dialog, FoodRestriction selectedRestriction)
        {
            typeof(Dialog_ManageFoodRestrictions).GetProperty("SelectedFoodRestriction", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty).SetValue(dialog, selectedRestriction, null);
        }
    }
}