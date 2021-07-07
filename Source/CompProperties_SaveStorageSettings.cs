using Verse;

namespace SaveStorageSettings
{
    class CompProperties_SaveStorageSettings : CompProperties
    {
        public string name;

        public CompProperties_SaveStorageSettings()
        {
            this.compClass = typeof(CompSaveStorageSettings);
        }
    }

    class CompSaveStorageSettings : ThingComp { }
}
