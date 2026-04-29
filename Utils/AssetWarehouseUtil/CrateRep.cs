using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer.Extensions;

namespace LabUtils.Utils.AssetWarehouseUtil
{
    [Serializable]
    public struct CrateRep
    {
        public CrateType type;
        public string pallet_Title;
        public string pallet_Author;
        public string barcode;
        public string title;
        public string description;
        public string[] tags;
        public bool unlockable;
        public bool redacted;
        public override string ToString()
        {
            return $"Title: {title}\nPallet: {pallet_Title}\nAuthor: {pallet_Author}\nDescription: {description}\nUnlockable: {unlockable}\nRedacted: {redacted}\nType: {type.ToString()}\nTags:{tags}";
        }
        public static CrateRep CreateCrateRep(Crate crate)
        {
            CrateRep rep = new CrateRep();
            rep.pallet_Title = crate.Pallet.Title;
            rep.barcode = crate.Barcode.ID;
            rep.title = crate.Title;
            rep.description = crate.Description;
            rep.unlockable = crate.Unlockable;
            rep.redacted = crate.Redacted;
            rep.pallet_Author = crate.Pallet.Author;
            rep.tags = crate.Tags.ToList().ToArray();
            if(crate.TryCast<AvatarCrate>()) rep.type = CrateType.Avatar;
            else if (crate.TryCast<SpawnableCrate>()) rep.type = CrateType.Spawnable;
            if(crate.TryCast<LevelCrate>()) rep.type = CrateType.Level;
            return rep;
        }
    }
}
