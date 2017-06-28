using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class SaveDialog : FileListDialog
    {
        private readonly Zone_Stockpile zone;

        internal SaveDialog(Zone_Stockpile zone)
        {
            this.zone = zone;
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
            IOUtil.SaveStorageSettings(this.zone.settings.filter, fi);
            base.Close();
        }
    }
}
