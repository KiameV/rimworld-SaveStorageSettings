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
            Command_Action a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/save", true);
            a.defaultLabel = "SaveStorageSettings.SaveZoneSettings".Translate();
            a.defaultDesc = "SaveStorageSettings.SaveZoneSettingsDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new SaveDialog(storageTypeName, thingFilter)); };
            a.groupKey = groupKey;
            gizmos.Add(a);

            a = new Command_Action();
            a.icon = ContentFinder<UnityEngine.Texture2D>.Get("UI/load", true);
            a.defaultLabel = "SaveStorageSettings.LoadZoneSettings".Translate();
            a.defaultDesc = "SaveStorageSettings.LoadZoneSettingsDesc".Translate();
            a.activateSound = SoundDef.Named("Click");
            a.action = delegate { Find.WindowStack.Add(new LoadDialog(storageTypeName, thingFilter)); };
            a.groupKey = groupKey + 1;
            gizmos.Add(a);

            return gizmos;
        }
    }
}
