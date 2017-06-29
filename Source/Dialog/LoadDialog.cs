using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class LoadDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal LoadDialog(Zone_Stockpile zone) : base(SaveTypeEnum.Zone_Stockpile)
        {
            this.ThingFilter = zone.settings.filter;
            this.interactButLabel = "LoadGameButton".Translate();
        }

        internal LoadDialog(Outfit outfit) : base(SaveTypeEnum.Apparel_Management)
        {
            this.ThingFilter = outfit.filter;
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
