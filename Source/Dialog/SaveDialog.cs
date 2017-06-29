using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class SaveDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal SaveDialog(Zone_Stockpile zone) : base(SaveTypeEnum.Zone_Stockpile)
        {
            this.ThingFilter = zone.settings.filter;
            this.interactButLabel = "OverwriteButton".Translate();
        }

        internal SaveDialog(Outfit outfit) : base(SaveTypeEnum.Apparel_Management)
        {
            this.ThingFilter = outfit.filter;
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
