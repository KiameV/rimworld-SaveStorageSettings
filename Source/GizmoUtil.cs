using SaveStorageSettings.Dialog;
using System.Collections.Generic;
using Verse;

namespace SaveStorageSettings
{
    public static class GizmoUtil
    {
        internal static IEnumerable<Gizmo> AddSaveLoadGizmos(IEnumerable<Gizmo> gizmos, string storageTypeName, ThingFilter thingFilter, int groupKey = 987767552)
        {
            List<Gizmo> l;
            if (gizmos != null)
            {
                l = new List<Gizmo>(gizmos);
            }
            else
            {
                l = new List<Gizmo>(2);
            }
            return AddSaveLoadGizmos(l, storageTypeName, thingFilter);
        }

        public static List<Gizmo> AddSaveLoadGizmos(List<Gizmo> gizmos, string storageTypeName, ThingFilter thingFilter, int groupKey = 987767552)
        {
            gizmos.Add(new Command_Action
            {
                icon = HarmonyPatches.SaveTexture,
                defaultLabel = "SaveStorageSettings.SaveZoneSettings".Translate(),
                defaultDesc = "SaveStorageSettings.SaveZoneSettingsDesc".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate { Find.WindowStack.Add(new SaveFilterDialog(storageTypeName, thingFilter)); },
                groupKey = groupKey
            });

            gizmos.Add(new Command_Action
            {
                icon = HarmonyPatches.LoadTexture,
                defaultLabel = "SaveStorageSettings.LoadZoneSettings".Translate(),
                defaultDesc = "SaveStorageSettings.LoadZoneSettingsDesc".Translate(),
                activateSound = SoundDef.Named("Click"),
                action = delegate { Find.WindowStack.Add(new LoadFilterDialog(storageTypeName, thingFilter)); },
                groupKey = groupKey + 1
            });

            return gizmos;
        }
    }
}
