using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class SaveDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal SaveDialog(string storageTypeName, ThingFilter thingFilter) : base(storageTypeName)
        {
            this.ThingFilter = thingFilter;
            this.interactButLabel = "OverwriteButton".Translate();
        }

        protected override bool ShouldDoTypeInField
        {
            get
            {
                return true;
            }
        }

        protected override void DoFileInteraction(FileInfo fi)
        {
            IOUtil.SaveStorageSettings(this.ThingFilter, fi);
            base.Close();
        }
    }
}
