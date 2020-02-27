﻿using RimWorld;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Verse;

namespace SaveStorageSettings.Dialog
{
    class LoadFilterDialog : FileListDialog
    {
        private readonly ThingFilter ThingFilter;

        internal LoadFilterDialog(string storageTypeName, ThingFilter thingFilter) : base(storageTypeName)
        {
            this.ThingFilter = thingFilter;
            base.interactButLabel = "LoadGameButton".Translate();
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

    class LoadCraftingDialog : FileListDialog
    {
        public enum LoadType
        {
            Replace,
            Append,
        }

        private readonly LoadType Type;
        private readonly BillStack BillStack;

        internal LoadCraftingDialog(string storageTypeName, BillStack billStack, LoadType type) : base(storageTypeName)
        {
            this.Type = type;
            this.BillStack = billStack;
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
            int maxBillCount = 15;
            if (ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name.Contains("No Max Bills")))
            {
                maxBillCount = 125;
            }

            List<Bill> bills = IOUtil.LoadCraftingBills(fi);
            if (bills != null && bills.Count > 0)
            {
                if (this.Type == LoadType.Replace)
                {
                    this.BillStack.Clear();
                }

                foreach (Bill b in bills)
                {
                    if (this.BillStack.Count < maxBillCount)
                    {
                        this.BillStack.AddBill(b);
                    }
                    else
                    {
                        Log.Warning("Work Table has too many bills. Bill for [" + b.recipe.defName + "] will not be added.");
                    }
                }
                bills.Clear();
                bills = null;
            }
            base.Close();
        }
    }

    class LoadOperationDialog : FileListDialog
    {
        private readonly Pawn Pawn;

        internal LoadOperationDialog(Pawn pawn, string storageTypeName) : base(storageTypeName)
        {
            this.Pawn = pawn;
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
            List<Bill> bills = IOUtil.LoadOperationBills(fi, this.Pawn);
            if (bills != null && bills.Count > 0)
            {
                this.Pawn.BillStack.Clear();
                foreach (Bill b in bills)
                {
                    this.Pawn.BillStack.AddBill(b);
                    //Log.Warning("Bills Count: " + this.Pawn.BillStack.Count);
                }
                bills.Clear();
                bills = null;
            }
            base.Close();
        }
    }

    class LoadPolicyDialog : FileListDialog
    {
        private readonly DrugPolicy DrugPolicy;

        internal LoadPolicyDialog(string storageTypeName, DrugPolicy drugPolicy) : base(storageTypeName)
        {
            this.DrugPolicy = drugPolicy;
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
            IOUtil.LoadPolicy(this.DrugPolicy, fi);
            base.Close();
        }
    }
}
