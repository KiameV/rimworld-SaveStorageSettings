using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace SaveStorageSettings
{
    internal class ThingFilterReflection
    {
        private readonly ThingFilter filter;

        internal ThingFilterReflection(ThingFilter filter)
        {
            this.filter = filter;
        }

        internal IEnumerable<ThingDef> AllStorableThingDefs
        {
            get
            {
                return from def in DefDatabase<ThingDef>.AllDefs
                       where def.EverStoreable
                       select def;
                //return (List<ThingDef>)typeof(ThingDef).GetProperty("AllStorableThingDefs", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.GetProperty).GetValue(null, null);
            }
        }

        internal HashSet<ThingDef> AllowedDefs
        {
            get
            {
                return (HashSet<ThingDef>)this.GetPrivateFieldInfo("allowedDefs").GetValue(this.filter);
            }
            set
            {
                this.GetPrivateFieldInfo("allowedDefs").SetValue(this.filter, value);
            }
        }

        internal List<SpecialThingFilterDef> DisallowedSpecialFilters
        {
            get
            {
                return (List<SpecialThingFilterDef>)this.GetPrivateFieldInfo("disallowedSpecialFilters").GetValue(this.filter);
            }
            set
            {
                this.GetPrivateFieldInfo("disallowedSpecialFilters").SetValue(this.filter, value);
            }
        }

        internal QualityRange AllowedQualities
        {
            get
            {
                return (QualityRange)this.GetPrivateFieldInfo("allowedQualities").GetValue(this.filter);
            }
        }

        internal void SettingsChangedCallback()
        {
            Action a = ((Action)this.GetPrivateFieldInfo("settingsChangedCallback").GetValue(this.filter));
            if (a != null)
                a.Invoke();
        }

        private FieldInfo GetPrivateFieldInfo(string name)
        {
            return typeof(ThingFilter).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /*internal List<string> SpecialFiltersToAllow
        {
            get
            {
                return (List<string>)this.GetPrivateFieldInfo("specialFiltersToAllow").GetValue(this.filter);
            }
            set
            {
                this.GetPrivateFieldInfo("specialFiltersToAllow").SetValue(this.filter, value);
            }
        }

        internal List<string> SpecialFiltersToDisallow
        {
            get
            {
                return (List<string>)this.GetPrivateFieldInfo("specialFiltersToDisallow").GetValue(this.filter);
            }
            set
            {
                this.GetPrivateFieldInfo("specialFiltersToDisallow").SetValue(this.filter, value);
            }
        }*/
    }
}
