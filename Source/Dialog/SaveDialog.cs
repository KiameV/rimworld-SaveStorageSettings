using RimWorld;
using System.IO;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class SaveFilterDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal SaveFilterDialog(string storageTypeName, ThingFilter thingFilter) : base(storageTypeName)
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

    class SaveCraftingDialog : FileListDialog
    {
        private readonly BillStack BillStack;

        internal SaveCraftingDialog(string type, BillStack billStack) : base(type)
        {
            this.BillStack = billStack;
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
            IOUtil.SaveCraftingSettings(this.BillStack, fi);
            base.Close();
        }
    }

    class SavePolicyDialog : FileListDialog
    {
        private readonly DrugPolicy DrugPolicy;

        internal SavePolicyDialog(string type, DrugPolicy drugPolicy) : base(type)
        {
            this.DrugPolicy = drugPolicy;
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
            IOUtil.SavePolicySettings(this.DrugPolicy, fi);
            base.Close();
        }
    }
}
