using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer;
using LabUtils.Developer.Extensions;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabUtils.Utils.AssetWarehouseUtil
{
    public static class RetrievingTask
    {
        public static bool isRunning;
        public static void RetrieveResults(string query, Action<CrateRep[]> onComplete)
        {
            if (isRunning) return;
            isRunning = true;
            bool showRedacted = AssetWarehouseUtility.ShowRedacted.Value;
            bool showUnlockable = AssetWarehouseUtility.ShowUnlockable.Value;

            bool includeTags = AssetWarehouseUtility.IncludeTags.Value;
            List<Crate> crates = LabData.crates;
            Task.Factory.StartNew(() =>
            {
                var foundCrates = new List<Crate>();
                foreach (var crate in crates) {
                    if(crate.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundCrates.Add(crate);
                    }
                    foreach(var tag in crate.Tags)
                    {
                        if (tag.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foundCrates.Add(crate);
                        }
                    }
                }
                foreach (var crate in foundCrates.ToList()) {
                    if (showRedacted && crate.Redacted) foundCrates.Remove(crate);
                    if (showUnlockable && crate.Unlockable) foundCrates.Remove(crate);
                }
                CrateRep[] crateReps = new CrateRep[crates.Count];
                int i = 0;
                foreach(var crate in foundCrates.ToList())
                {
                    CrateRep rep = CrateRep.CreateCrateRep(crate);
                    crateReps[i] = rep;
                    i++;
                }
                MelonCoroutines.Start(InvokeOnMainThread(crateReps, onComplete));
            });
        }
        private static System.Collections.IEnumerator InvokeOnMainThread(CrateRep[] result, Action<CrateRep[]> onComplete)
        {
            yield return null; // Wait one frame to ensure we're on main thread
            onComplete(result);
        }
    }
}
