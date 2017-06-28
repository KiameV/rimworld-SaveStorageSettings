using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class LoadDialog : FileListDialog
    {
        private readonly Zone_Stockpile zone;

        internal LoadDialog(Zone_Stockpile zone)
        {
            this.zone = zone;
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
            IOUtil.LoadFilters(this.zone.settings.filter, fi);
            base.Close();
        }
    }
}
