using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Verse;

namespace SaveStorageSettings
{
    static class IOUtil
    {
        private const string BREAK = "---";
        public static bool LoadFilters(ThingFilter filter, FileInfo fi)
        {
            try
            {
                if (fi.Exists)
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        string[] kv = null;
                        if (sr.EndOfStream ||
                            !ReadField(sr, out kv))
                        {
                            throw new Exception("Trying to read from an empty file");
                        }

                        if (kv != null && "1".Equals(kv[1]))
                        {
                            ReadFiltersFromFile(filter, sr);
                        }
                        else
                        {
                            throw new Exception("Unsupported version: " + kv[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem loading storage settings file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }

        public static List<Bill> LoadCraftingBills(FileInfo fi)
        {
            List<Bill> bills = new List<Bill>();
            try
            {
                if (fi.Exists)
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        string[] kv = null;
                        if (sr.EndOfStream ||
                            !ReadField(sr, out kv))
                        {
                            throw new Exception("Trying to read from an empty file");
                        }

                        if (kv != null && "2".Equals(kv[1]))
                        {
                            string line = "";
                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    line = sr.ReadLine().Trim();
                                    if (line != null && line.StartsWith("bill:"))
                                    {
                                        Bill_Production b;
                                        if (TryCreateBill(sr, out b))
                                        {
                                            if (b != null)
                                            {
                                                bills.Add(b);
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                    Log.Error("Unable to load a bill for [" + line + "]");
                                }
                            }
                        }
                        else
                        {
                            Log.Error("This version of save files is not supported Please create a new one. File: " + kv[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem loading storage settings file '" + fi.Name + "'.", e);
            }
            return bills;
        }

        internal static List<Bill> LoadOperationBills(FileInfo fi, Pawn pawn)
        {
            List<Bill> bills = new List<Bill>();
            try
            {
                if (fi.Exists)
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        string[] kv = null;
                        if (sr.EndOfStream ||
                            !ReadField(sr, out kv))
                        {
                            throw new Exception("Trying to read from an empty file");
                        }

                        if (kv != null && "2".Equals(kv[1]))
                        {
                            string line = "";
                            while (!sr.EndOfStream)
                            {
                                try
                                {
                                    line = sr.ReadLine().Trim();
                                    if (line != null && line.StartsWith("bill:"))
                                    {
                                        if (TryCreateBill(sr, pawn, out Bill_Medical b))
                                        {
                                            bills.Add(b);
                                        }
                                        else
                                        {
                                            Log.Warning("Unable to load operation bill [" + line.Split(':')[1] + "]");
                                        }
                                    }
                                }
                                catch
                                {
                                    Log.Error("Unable to load a bill for [" + line + "]");
                                }
                            }
                        }
                        else
                        {
                            Log.Error("This version of save files is not supported Please create a new one. File: " + kv[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem loading storage settings file '" + fi.Name + "'.", e);
            }
            return bills;
        }

        internal static void LoadPolicy(DrugPolicy drugPolicy, FileInfo fi)
        {
            try
            {
                if (fi.Exists)
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        string[] kv = null;
                        if (sr.EndOfStream ||
                            !ReadField(sr, out kv))
                        {
                            throw new Exception("Trying to read from an empty file");
                        }

                        if (kv != null && "1".Equals(kv[1]))
                        {
                            while (!sr.EndOfStream)
                            {
                                if (ReadField(sr, out kv))
                                {
                                    switch (kv[0])
                                    {
                                        case "name":
                                            drugPolicy.label = kv[1];
                                            break;
                                        case "drug":
                                            DrugPolicyEntry e;
                                            if (TryCreateDrugPolicyEntry(sr, out e))
                                            {
                                                for (int i = 0; i < drugPolicy.Count; ++i)
                                                {
                                                    if (drugPolicy[i].drug.defName.Equals(e.drug.defName))
                                                    {
                                                        drugPolicy[i] = e;
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unsupported version: " + kv[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem loading storage settings file '" + fi.Name + "'.", e);
            }
        }

        internal static void LoadFoodRestriction(FoodRestriction fr, FileInfo fi)
        {
            try
            {
                if (fi.Exists)
                {
                    // Load Data
                    using (StreamReader sr = new StreamReader(fi.FullName))
                    {
                        string[] kv = null;
                        if (sr.EndOfStream ||
                            !ReadField(sr, out kv))
                        {
                            throw new Exception("Trying to read from an empty file");
                        }

                        if (kv != null && "1".Equals(kv[1]))
                        {
                            while (!sr.EndOfStream)
                            {
                                if (ReadField(sr, out kv))
                                {
                                    switch (kv[0])
                                    {
                                        case "name":
                                            fr.label = kv[1];
                                            ReadFiltersFromFile(fr.filter, sr);
                                            break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unsupported version: " + kv[1]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem loading storage settings file '" + fi.Name + "'.", e);
            }
        }

        static void LogException(string msg, Exception e)
        {
            Log.Warning(msg + Environment.NewLine + e.GetType().Name + " " + e.Message + " " + e.StackTrace);
            Messages.Message(msg, MessageTypeDefOf.NegativeEvent);
        }

        public static bool SaveStorageSettings(ThingFilter filter, FileInfo fi)
        {
            try
            {
                // Write Data
                using (FileStream fileStream = File.Open(fi.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        WriteField(sw, "Version", "1");

                        WriteFiltersToFile(filter, sw);
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem saving storage settings file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }

        public static bool SaveOperationSettings(Pawn pawn, FileInfo fi)
        {
            try
            {
                // Write Data
                using (FileStream fileStream = File.Open(fi.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        WriteField(sw, "Version", "2");

                        foreach (Bill b in pawn.BillStack)
                        {
                            if (b is Bill_Medical m)
                            {
                                WriteField(sw, "bill", b.recipe.defName);
                                WriteField(sw, "recipeDefName", b.recipe.defName);
                                if (m.recipe.targetsBodyPart)
                                {
                                    WriteField(sw, "part", m.Part.Label + ":" + m.Part.def.defName);
                                }
                                WriteField(sw, BREAK, BREAK);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem saving operation bills file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }

        public static bool SaveCraftingSettings(BillStack bills, FileInfo fi)
        {
            try
            {
                // Write Data
                using (FileStream fileStream = File.Open(fi.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        WriteField(sw, "Version", "2");

                        foreach (Bill b in bills)
                        {
                            if (b is Bill_Production)
                            {
                                Bill_Production p = b as Bill_Production;

                                BillStoreModeDef storeMode       = p.GetStoreMode();
                                Zone_Stockpile   storeZone       = p.GetStoreZone();
                                Zone_Stockpile   includeFromZone = p.includeFromZone;

                                WriteField(sw, "bill", p.recipe.defName);
                                if (b is Bill_ProductionWithUft)
                                {
                                    WriteField(sw, "recipeDefNameUft", p.recipe.defName);
                                }
                                else
                                {
                                    WriteField(sw, "recipeDefName", p.recipe.defName);
                                }
                                WriteField(sw, "skillRange", p.allowedSkillRange.ToString());
                                WriteField(sw, "suspended", p.suspended.ToString());
                                WriteField(sw, "ingSearchRadius", p.ingredientSearchRadius.ToString());
                                WriteField(sw, "storeMode", storeMode == BillStoreModeDefOf.SpecificStockpile ?
                                    string.Join("/", storeMode.defName, storeZone.GetUniqueLoadID()) :
                                    storeMode.defName
                                );
                                WriteField(sw, "repeatMode", p.repeatMode.defName);
                                WriteField(sw, "repeatCount", p.repeatCount.ToString());
                                WriteField(sw, "targetCount", p.targetCount.ToString());
                                WriteField(sw, "includeEquipped", p.includeEquipped.ToString());
                                WriteField(sw, "includeTainted", p.includeTainted.ToString());
                                if (includeFromZone != null) WriteField(sw, "includeFromZone", includeFromZone.GetUniqueLoadID());
                                WriteField(sw, "limitToAllowedStuff", p.limitToAllowedStuff.ToString());
                                WriteField(sw, "pauseWhenSatisfied", p.pauseWhenSatisfied.ToString());
                                WriteField(sw, "unpauseWhenYouHave", p.unpauseWhenYouHave.ToString());
                                WriteField(sw, "hpRange", p.hpRange.ToString());
                                WriteField(sw, "qualityRange", p.qualityRange.ToString());
                                WriteField(sw, "ingredientFilter", "");
                                WriteFiltersToFile(p.ingredientFilter, sw);
                            }
                            WriteField(sw, BREAK, BREAK);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem saving crafting bills file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }

        public static bool SavePolicySettings(DrugPolicy policy, FileInfo fi)
        {
            try
            {
                // Write Data
                using (FileStream fileStream = File.Open(fi.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        WriteField(sw, "Version", "1");

                        WriteField(sw, "name", policy.label);
                        for (int i = 0; i < policy.Count; ++i)
                        {
                            DrugPolicyEntry e = policy[i];
                            WriteField(sw, "drug", i.ToString());
                            WriteField(sw, "defName", e.drug.defName);
                            WriteField(sw, "allowedForAddiction", e.allowedForAddiction.ToString());
                            WriteField(sw, "allowedForJoy", e.allowedForJoy.ToString());
                            WriteField(sw, "allowScheduled", e.allowScheduled.ToString());
                            WriteField(sw, "daysFrequency", e.daysFrequency.ToString());
                            WriteField(sw, "onlyIfJoyBelow", e.onlyIfJoyBelow.ToString());
                            WriteField(sw, "onlyIfMoodBelow", e.onlyIfMoodBelow.ToString());
                            WriteField(sw, "takeToInventory", e.takeToInventory.ToString());
                            WriteField(sw, "takeToInventoryTempBuffer", e.takeToInventoryTempBuffer);
                            WriteField(sw, BREAK, BREAK);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem saving storage settings file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }
        public static bool SaveFoodRestrictionSettings(FoodRestriction fr, FileInfo fi)
        {
            try
            {
                // Write Data
                using (FileStream fileStream = File.Open(fi.FullName, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        WriteField(sw, "Version", "1");

                        WriteField(sw, "name", fr.label);
                        WriteFiltersToFile(fr.filter, sw);
                    }
                }
            }
            catch (Exception e)
            {
                LogException("Problem saving storage settings file '" + fi.Name + "'.", e);
                return false;
            }
            return true;
        }

        private static bool TryCreateBill(StreamReader sr, out Bill_Production bill)
        {
            bill = null;
            string[] kv = null;
            try
            {
                while (!sr.EndOfStream)
                {
                    if (ReadField(sr, out kv))
                    {
                        RecipeDef def;
                        switch (kv[0])
                        {
                            case BREAK:
                                return true;
                            case "recipeDefName":
                                def = DefDatabase<RecipeDef>.GetNamed(kv[1]);
                                if (def == null)
                                {
                                    string msg = "SaveStorageSettings.UnableToLoadRecipeDef".Translate().Replace("%s", kv[1]);
                                    Messages.Message(msg, MessageTypeDefOf.SilentInput);
                                    Log.Warning(msg);
                                    return false;
                                }
                                if (def.researchPrerequisite != null && !def.researchPrerequisite.IsFinished)
                                {
                                    string msg = "SaveStorageSettings.ResearchNotDoneForRecipeDef".Translate().Replace("%s", def.label);
                                    Messages.Message(msg, MessageTypeDefOf.SilentInput);
                                    Log.Warning(msg);
                                    return false;
                                }
                                bill = new Bill_Production(def);
                                break;
                            case "recipeDefNameUft":
                                def = DefDatabase<RecipeDef>.GetNamed(kv[1]);
                                if (def == null)
                                {
                                    string msg = "SaveStorageSettings.UnableToLoadRecipeDef".Translate().Replace("%s", kv[1]);
                                    Messages.Message(msg, MessageTypeDefOf.SilentInput);
                                    Log.Warning(msg);
                                    return false;
                                }
                                if (def.researchPrerequisite != null && !def.researchPrerequisite.IsFinished)
                                {
                                    string msg = "SaveStorageSettings.ResearchNotDoneForRecipeDef".Translate().Replace("%s", def.label);
                                    Messages.Message(msg, MessageTypeDefOf.SilentInput);
                                    Log.Warning(msg);
                                    return false;
                                }
                                bill = new Bill_ProductionWithUft(def);
                                break;
                            case "skillRange":
                                kv = kv[1].Split('~');
                                bill.allowedSkillRange = new IntRange(int.Parse(kv[0]), int.Parse(kv[1]));
                                break;
                            case "suspended":
                                bill.suspended = bool.Parse(kv[1]);
                                break;
                            case "ingSearchRadius":
                                bill.ingredientSearchRadius = float.Parse(kv[1]);
                                break;
                            case "repeatMode":
                                bill.repeatMode = null;
                                if (BillRepeatModeDefOf.Forever.defName.Equals(kv[1]))
                                    bill.repeatMode = BillRepeatModeDefOf.Forever;
                                else if (BillRepeatModeDefOf.RepeatCount.defName.Equals(kv[1]))
                                    bill.repeatMode = BillRepeatModeDefOf.RepeatCount;
                                else if (BillRepeatModeDefOf.TargetCount.defName.Equals(kv[1]))
                                    bill.repeatMode = BillRepeatModeDefOf.TargetCount;
                                else if ("TD_ColonistCount".Equals(kv[1]))
                                    EverybodyGetsOneUtil.TryGetRepeatModeDef("TD_ColonistCount", out bill.repeatMode);
                                else if ("TD_XPerColonist".Equals(kv[1]))
                                    EverybodyGetsOneUtil.TryGetRepeatModeDef("TD_XPerColonist", out bill.repeatMode);
                                else if ("TD_WithSurplusIng".Equals(kv[1]))
                                    EverybodyGetsOneUtil.TryGetRepeatModeDef("TD_WithSurplusIng", out bill.repeatMode);

                                if (bill.repeatMode == null)
                                {
                                    Log.Warning("Unknown repeatMode of [" + kv[1] + "] for bill " + bill.recipe.defName);
                                    bill = null;
                                    return false;
                                }
                                break;
                            case "repeatCount":
                                bill.repeatCount = int.Parse(kv[1]);
                                break;
                            case "storeMode":
                                string[] storeSplit = kv[1].Split('/');

                                BillStoreModeDef storeMode = DefDatabase<BillStoreModeDef>.GetNamedSilentFail(storeSplit[0]);
                                if (storeMode == null)
                                {
                                    Log.Message("Bill [" + bill.recipe.defName + "] storeMode [" + kv[1] + "] cannot be found. Defaulting to [" + BillStoreModeDefOf.BestStockpile.ToString() + "].");
                                    storeMode = BillStoreModeDefOf.BestStockpile;
                                }

                                Zone_Stockpile storeZone = null;
                                if (storeMode == BillStoreModeDefOf.SpecificStockpile)
                                {
                                    if (storeSplit.Length > 1) storeZone = (Zone_Stockpile)Find.CurrentMap?.zoneManager.AllZones.FirstOrFallback(z => z.GetUniqueLoadID() == storeSplit[1]);

                                    if (storeZone == null)
                                    {
                                        Log.Message("Bill [" + bill.recipe.defName + "] storeZone [" + kv[1] + "] cannot be found. Defaulting to storeMode [" + BillStoreModeDefOf.BestStockpile.ToString() + "].");
                                        storeMode = BillStoreModeDefOf.BestStockpile;
                                    }
                                }

                                bill.SetStoreMode(storeMode, storeZone);
                                break;
                            case "targetCount":
                                bill.targetCount = int.Parse(kv[1]);
                                break;
                            case "includeEquipped":
                                bill.includeEquipped = bool.Parse(kv[1]);
                                break;
                            case "includeTainted":
                                bill.includeTainted = bool.Parse(kv[1]);
                                break;
                            case "includeFromZone":
                                Zone_Stockpile zone = (Zone_Stockpile)Find.CurrentMap?.zoneManager.AllZones.FirstOrFallback(z => z.GetUniqueLoadID() == kv[1]);

                                if (zone == null)
                                {
                                    Log.Message("Bill [" + bill.recipe.defName + "] includeFromZone [" + kv[1] + "] cannot be found. Defaulting to Everywhere (null).");
                                }

                                bill.includeFromZone = zone;
                                break;
                            case "limitToAllowedStuff":
                                bill.limitToAllowedStuff = bool.Parse(kv[1]);
                                break;
                            case "pauseWhenSatisfied":
                                bill.pauseWhenSatisfied = bool.Parse(kv[1]);
                                break;
                            case "unpauseWhenYouHave":
                                bill.unpauseWhenYouHave = int.Parse(kv[1]);
                                break;
                            case "hpRange":
                                kv = kv[1].Split('~');
                                bill.hpRange = new FloatRange(float.Parse(kv[0]), float.Parse(kv[1]));
                                break;
                            case "qualityRange":
                                bill.qualityRange = QualityRange.FromString(kv[1]);
                                break;
                            case "ingredientFilter":
                                ReadFiltersFromFile(bill.ingredientFilter, sr);
                                return true;
                        }
                    }
                }
            }
            catch
            {
                string error = "";
                if (bill != null && bill.recipe != null)
                {
                    error = "Unable to load bill [" + bill.recipe.defName + "].";
                }
                else
                {
                    error = "Unable to load a bill.";
                }

                if (kv == null || kv.Length < 2)
                {
                    error += " Current line: [" + kv[0] + ":" + kv[1] + "]";
                }
                Log.Warning(error);
                bill = null;
                return false;
            }
            return true;
        }

        private static bool TryCreateBill(StreamReader sr, Pawn pawn, out Bill_Medical bill)
        {
            bill = null;
            string[] kv = null;
            try
            {
                while (!sr.EndOfStream)
                {
                    if (ReadField(sr, out kv))
                    {
                        switch (kv[0])
                        {
                            case BREAK:
                                return true;
                            case "recipeDefName":
                                var def = DefDatabase<RecipeDef>.GetNamed(kv[1]);
                                if (def == null)
                                {
                                    Log.Warning("Unable to load bill with RecipeDef of [" + kv[1] + "]");
                                    return false;
                                }
                                bill = new Bill_Medical(def);
                                break;
                            case "part":
                                var pv = kv[1].Split(':');
                                bill.Part = null;
                                string partToFind = pv[0];
                                foreach (BodyPartRecord part in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null))
                                {
                                    if (part.Label.Equals(partToFind))
                                    {
                                        bill.Part = part;
                                        break;
                                    }
                                }
                                if (bill.Part == null)
                                {
                                    if (bill.recipe.defName.StartsWith("Remove"))
                                        Log.Warning("Pawn [" + pawn.Name.ToStringShort + "] does not have body part [" + partToFind + "] to have removed.");
                                    else if (bill.recipe.defName.StartsWith("Install"))
                                        Log.Warning("Unknown body part to install part to [" + partToFind + "].");
                                    else
                                        Log.Warning("Unknown body part [" + partToFind + "].");
                                    return false;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                string error = "";
                if (bill != null && bill.recipe != null)
                {
                    error = "Unable to load bill [" + bill.recipe.defName + "].";
                }
                else
                {
                    error = "Unable to load a bill.";
                }

                if (kv == null || kv.Length < 2)
                {
                    error += " Current line: [" + kv[0] + ":" + kv[1] + "]";
                }
                Log.Error(e.GetType().Name + " - " + e.Message);
                Log.Warning(error);
                bill = null;
                return false;
            }
            return true;
        }

        private static bool TryCreateDrugPolicyEntry(StreamReader sr, out DrugPolicyEntry entry)
        {
            entry = null;
            string[] kv = null;
            try
            {
                while (!sr.EndOfStream)
                {
                    if (ReadField(sr, out kv))
                    {
                        switch (kv[0])
                        {
                            case BREAK:
                                return true;
                            case "defName":
                                ThingDef def = DefDatabase<ThingDef>.GetNamed(kv[1]);
                                if (def == null)
                                {
                                    Log.Warning("Unable to load drug policy with Drug of [" + kv[1] + "]");
                                    return false;
                                }
                                entry = new DrugPolicyEntry();
                                entry.drug = def;
                                break;
                            case "allowedForAddiction":
                                entry.allowedForAddiction = bool.Parse(kv[1]);
                                break;
                            case "allowedForJoy":
                                entry.allowedForJoy = bool.Parse(kv[1]);
                                break;
                            case "allowScheduled":
                                entry.allowScheduled = bool.Parse(kv[1]);
                                break;
                            case "daysFrequency":
                                entry.daysFrequency = int.Parse(kv[1]);
                                break;
                            case "onlyIfJoyBelow":
                                entry.onlyIfJoyBelow = float.Parse(kv[1]);
                                break;
                            case "onlyIfMoodBelow":
                                entry.onlyIfMoodBelow = float.Parse(kv[1]);
                                break;
                            case "takeToInventory":
                                entry.takeToInventory = int.Parse(kv[1]);
                                break;
                            case "takeToInventoryTempBuffer":
                                entry.takeToInventoryTempBuffer = kv[1];
                                break;
                        }
                    }
                }
            }
            catch
            {
                string error = "";
                if (entry != null && entry.drug != null)
                {
                    error = "Unable to load drug policy [" + entry.drug.defName + "].";
                }
                else
                {
                    error = "Unable to load a drug policy.";
                }

                if (kv == null || kv.Length < 2)
                {
                    error += " Current line: [" + kv[0] + ":" + kv[1] + "]";
                }
                Log.Warning(error);
                entry = null;
                return false;
            }
            return true;
        }

        private static bool ReadField(StreamReader sr, out String[] nameValue)
        {
            string line = sr.ReadLine();
            if (line != null && !line.Equals(BREAK))
            {
                int i = line.IndexOf(':');
                if (i > 0)
                {
                    string name = line.Substring(0, i);
                    string value = "";
                    if (i < line.Length - 1)
                    {
                        value = line.Substring(i + 1, line.Length - i - 1);
                        if ("null".Equals(value))
                            value = null;
                    }
                    nameValue = new string[] { name, value };
                    return true;
                }
            }
            nameValue = null;
            return false;
        }

        private static void WriteField(StreamWriter sw, string name, string value)
        {
            StringBuilder sb = new StringBuilder(name);
            sb.Append(":");
            if (value == null)
                sb.Append("null");
            else
                sb.Append(value);
            sb.Append(Environment.NewLine);
            sw.Write(sb.ToString());
        }

        public static bool TryGetFileInfo(string storageTypeName, string fileName, out FileInfo fi)
        {
            string path;
            if (TryGetDirectoryPath(storageTypeName, out path))
            {
                fi = new FileInfo(Path.Combine(path, fileName.ToString() + ".txt"));
                return true;
            }
            fi = null;
            return false;
        }

        public static bool TryGetDirectoryPath(string storageTypeName, out string path)
        {
            if (TryGetDirectoryName(storageTypeName, out path))
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                if (!directoryInfo.Exists)
                {
                    directoryInfo.Create();
                }
                return true;
            }
            return false;
        }

        private static bool TryGetDirectoryName(string storageTypeName, out string path)
        {
            try
            {
                path = (string)typeof(GenFilePaths).GetMethod("FolderUnderSaveData", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[]
                {
                    "SaveStorageSettings/" + storageTypeName
                });
                return true;
            }
            catch (Exception ex)
            {
                Log.Error("SaveStorageSettings: Failed to get folder name - " + ex);
                path = null;
                return false;
            }
        }

        private static void ReadFiltersFromFile(ThingFilter filter, StreamReader sr)
        {
            ThingFilterReflection tfr = new ThingFilterReflection(filter);
            bool changed = false;
            try
            {
                string[] kv;
                while (!sr.EndOfStream)
                {
                    if (ReadField(sr, out kv))
                    {
                        switch (kv[0])
                        {
                            case BREAK:
                                return;
                            case "allowedDefs":
                                try
                                {
                                    HashSet<ThingDef> allowedDefs = tfr.AllowedDefs;
                                    allowedDefs.Clear();
                                    if (kv[1] != null && kv[1].Length > 0)
                                    {
                                        HashSet<string> expected = new HashSet<string>(kv[1].Split('/'));
                                        foreach (ThingDef thing in tfr.AllStorableThingDefs)
                                        {
                                            if (expected.Contains(thing.defName))
                                            {
                                                allowedDefs.Add(thing);
                                            }
                                        }
                                    }
                                    changed = true;
                                }
                                catch (Exception e)
                                {
                                    LogException("Problem while loading Allowed Filters.", e);
                                }
                                break;
                            case "allowedHitPointsPercents":
                                if (kv[1] != null && kv[1].IndexOf(':') != -1)
                                {
                                    kv = kv[1].Split(':');
                                    try
                                    {
                                        filter.AllowedHitPointsPercents = new FloatRange(
                                            float.Parse(kv[0]), float.Parse(kv[1]));
                                    }
                                    catch (Exception e)
                                    {
                                        LogException("Problem while loading Hit Point Percents.", e);
                                    }
                                }
                                changed = true;
                                break;
                            case "allowedQualities":
                                if (kv[1] != null && kv[1].IndexOf(':') != -1)
                                {
                                    kv = kv[1].Split(':');
                                    try
                                    {
                                        filter.AllowedQualityLevels = new QualityRange(
                                            (QualityCategory)Enum.Parse(typeof(QualityCategory), kv[0], true),
                                            (QualityCategory)Enum.Parse(typeof(QualityCategory), kv[1], true));
                                        changed = true;
                                    }
                                    catch (Exception e)
                                    {
                                        LogException("Problem while loading Allowed Qualities.", e);
                                    }
                                }
                                break;
                            case "disallowedSpecialFilters":
                                if (kv[1] != null)
                                {
                                    try
                                    {
                                        if (kv[1].Length > 0)
                                        {
                                            HashSet<string> expected = new HashSet<string>(kv[1].Split('/'));
                                            List<SpecialThingFilterDef> l = new List<SpecialThingFilterDef>();
                                            foreach (SpecialThingFilterDef def in DefDatabase<SpecialThingFilterDef>.AllDefs)
                                            {
                                                if (def != null && def.configurable && expected.Contains(def.defName))
                                                {
                                                    l.Add(def);
                                                }
                                            }

                                            tfr.DisallowedSpecialFilters = l;
                                        }
                                        else
                                            tfr.DisallowedSpecialFilters.Clear();
                                        changed = true;
                                    }
                                    catch (Exception e)
                                    {
                                        LogException("Problem while loading Disallowed Special Filters.", e);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
            finally
            {
                if (changed)
                    tfr.SettingsChangedCallback();
            }
        }

        private static void WriteFiltersToFile(ThingFilter filter, StreamWriter sw)
        {
            ThingFilterReflection tfr = new ThingFilterReflection(filter);
            StringBuilder sb = new StringBuilder();
            foreach (ThingDef thing in tfr.AllStorableThingDefs)
            {
                if (filter.Allows(thing))
                {
                    if (sb.Length > 0)
                        sb.Append("/");
                    sb.Append(thing.defName);
                }
            }
            WriteField(sw, "allowedDefs", sb.ToString());
            sb = null;

            if (filter.allowedHitPointsConfigurable)
            {
                sb = new StringBuilder(filter.AllowedHitPointsPercents.min.ToString("N4"));
                sb.Append(":");
                sb.Append(filter.AllowedHitPointsPercents.max.ToString("N4"));
                WriteField(sw, "allowedHitPointsPercents", sb.ToString());
                sb = null;
            }

            if (filter.allowedQualitiesConfigurable)
            {
                sb = new StringBuilder(filter.AllowedQualityLevels.min.ToString());
                sb.Append(":");
                sb.Append(filter.AllowedQualityLevels.max.ToString());
                WriteField(sw, "allowedQualities", sb.ToString());
                sb = null;
            }

            sb = new StringBuilder();
            foreach (SpecialThingFilterDef def in tfr.DisallowedSpecialFilters)
            {
                if (sb.Length > 0)
                    sb.Append("/");
                sb.Append(def.defName);
            }
            WriteField(sw, "disallowedSpecialFilters", sb.ToString());
            sb = null;
        }
    }

    class EverybodyGetsOneUtil
    {
        private static Assembly ego = null;
        private static bool initialized = false;
        public static bool Exists
        {
            get
            {
                if (!initialized)
                {
                    foreach (ModContentPack pack in LoadedModManager.RunningMods)
                    {
                        foreach (Assembly assembly in pack.assemblies.loadedAssemblies)
                        {
                            if (
                                assembly.GetName().Name.Equals("Everybody_Gets_One") && (
                                    assembly.GetType("Everybody_Gets_One.RepeatModeDefOf")  != null ||
                                    assembly.GetType("TD_Enhancement_Pack.RepeatModeDefOf") != null
                                )
                            )
                            {
                                initialized = true;
                                ego = assembly;
                                break;
                            }
                        }
                        if (initialized)
                        {
                            break;
                        }
                    }
                    initialized = true;
                }
                return ego != null;
            }
        }

        public static bool TryGetRepeatModeDef(string defName, out BillRepeatModeDef def)
        {
            Log.Warning("Try get " + defName);
            def = null;
            if (Exists) {
                Type repeatModeType = ego.GetType("Everybody_Gets_One.RepeatModeDefOf") ?? ego.GetType("TD_Enhancement_Pack.RepeatModeDefOf");
                def = repeatModeType.GetField(defName, BindingFlags.Static | BindingFlags.Public).GetValue(null) as BillRepeatModeDef;
            }
            return def != null;
        }
    }
}
