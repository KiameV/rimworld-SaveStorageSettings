using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class LoadDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal LoadDialog(string storageTypeName, ThingFilter thingFilter) : base(storageTypeName)
        {
            this.ThingFilter = thingFilter;
            this.interactButLabel = "LoadGameButton".Translate();
        }

        protected override bool ShouldDoTypeInField
        {
            get
            {
                return false;
            }
        }

        protected override void DoFileInteraction(FileInfo fi)
        {
            IOUtil.LoadFilters(this.ThingFilter, fi);
            base.Close();
        }
    }
}
