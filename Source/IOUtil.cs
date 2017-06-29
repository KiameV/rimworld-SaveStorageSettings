/*
 * MIT License
 * 
 * Copyright (c) [2017] [Travis Offtermatt]
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
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
        public static bool LoadFilters(ThingFilter filter, FileInfo fi)
        {
            ThingFilterReflection tfr = new ThingFilterReflection(filter);
            bool changed = false;
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
                                            /*case "specialFiltersToAllow":
                                                try
                                                {
                                                    if (kv[1] != null && kv[1].Length > 0)
                                                    {
                                                        tfr.SpecialFiltersToAllow = new List<string>(kv[1].Split('/'));
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    LogException("Problem while loading Allowed Filters.", e);
                                                }
                                                break;
                                            case "specialFiltersToDisallow":
                                                try
                                                {
                                                    if (kv[1] != null && kv[1].Length > 0)
                                                    {
                                                        tfr.SpecialFiltersToDisallow = new List<string>(kv[1].Split('/'));
                                                    }
                                                }
                                                catch (Exception e)
                                                {
                                                    LogException("Problem while loading Allowed Filters.", e);
                                                }
                                                break;*/
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
                return false;
            }
            finally
            {
                if (changed)
                    tfr.SettingsChangedCallback();
            }
            return true;
        }

        static void LogException(string msg, Exception e)
        {
            Log.Warning(msg + Environment.NewLine + e.GetType().Name + " " + e.Message + " " + e.StackTrace);
            Messages.Message(msg, MessageSound.Negative);
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
                        ThingFilterReflection tfr = new ThingFilterReflection(filter);
                        WriteField(sw, "Version", "1");

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

                        if (filter.allowedHitPointsConfigurable)
                        {
                            sb = new StringBuilder(filter.AllowedHitPointsPercents.min.ToString("N4"));
                            sb.Append(":");
                            sb.Append(filter.AllowedHitPointsPercents.max.ToString("N4"));
                            WriteField(sw, "allowedHitPointsPercents", sb.ToString());
                        }

                        if (filter.allowedQualitiesConfigurable)
                        {
                            sb = new StringBuilder(filter.AllowedQualityLevels.min.ToString());
                            sb.Append(":");
                            sb.Append(filter.AllowedQualityLevels.max.ToString());
                            WriteField(sw, "allowedQualities", sb.ToString());
                        }

                        sb = new StringBuilder();
                        foreach (SpecialThingFilterDef def in tfr.DisallowedSpecialFilters)
                        {
                            if (sb.Length > 0)
                                sb.Append("/");
                            sb.Append(def.defName);
                        }
                        WriteField(sw, "disallowedSpecialFilters", sb.ToString());

                        /*if (tfr.SpecialFiltersToAllow != null && tfr.SpecialFiltersToAllow.Count > 0)
                        {
                            sb = new StringBuilder();
                            foreach (string s in tfr.SpecialFiltersToAllow)
                            {
                                if (sb.Length > 0)
                                    sb.Append("/");
                                sb.Append(s);
                            }
                            WriteField(sw, "specialFiltersToAllow", sb.ToString());
                        }

                        if (tfr.SpecialFiltersToDisallow != null && tfr.SpecialFiltersToDisallow.Count > 0)
                        {
                            sb = new StringBuilder();
                            foreach (string s in tfr.SpecialFiltersToDisallow)
                            {
                                if (sb.Length > 0)
                                    sb.Append("/");
                                sb.Append(s);
                            }
                            WriteField(sw, "specialFiltersToDisallow", sb.ToString());
                        }*/
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

        private static bool ReadField(StreamReader sr, out String[] nameValue)
        {
            string line = sr.ReadLine();
            if (line != null)
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
    }
}
