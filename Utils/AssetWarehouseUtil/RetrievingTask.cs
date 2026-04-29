using Il2CppSLZ.Marrow.Warehouse;
using LabUtils.Developer;
using LabUtils.Developer.Extensions;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LabUtils.Utils.AssetWarehouseUtil
{
    public static class RetrievingTask
    {
        public static List<Crate> Crates = new();
        public static bool isRunning;
        private static readonly ReaderWriterLockSlim lockSlim = new ReaderWriterLockSlim();
        public static void RetrieveResults(string query, Action<CrateRep[]> onComplete)
        {
            if (isRunning) return;
            isRunning = true;
            bool showRedacted = AssetWarehouseUtility.ShowRedacted.Value;
            bool showUnlockable = AssetWarehouseUtility.ShowUnlockable.Value;

            bool includeTags = AssetWarehouseUtility.IncludeTags.Value;
            var foundCrates = new List<Crate>();
            ThreadPool.QueueUserWorkItem(_ =>
            {
                lockSlim.EnterReadLock();
                try
                {
                    if (string.IsNullOrWhiteSpace(query) || Crates.Count < 1 || !AssetWarehouse.ready)
                    {
                        onComplete(Array.Empty<CrateRep>());
                        return;
                    }
                    foreach (var crate in Crates)
                    { 
                        if (crate.Title.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                        {
                            foundCrates.Add(crate);
                        }
                        foreach (var tag in crate.Tags)
                        {
                            if (tag.Contains(query, StringComparison.InvariantCultureIgnoreCase))
                            {
                                foundCrates.Add(crate);
                            }
                        }
                    }
                    foreach (var crate in foundCrates.ToList())
                    {
                        if (!showRedacted && crate.Redacted) foundCrates.Remove(crate);
                        if (!showUnlockable && crate.Unlockable) foundCrates.Remove(crate);
                    }
                    CrateRep[] crateReps = new CrateRep[foundCrates.Count];
                    int i = 0;
                    foreach (var crate in foundCrates)
                    {
                        CrateRep rep = CrateRep.CreateCrateRep(crate);
                        crateReps[i] = rep;
                        i++;
                    }
                    MelonCoroutines.Start(InvokeOnMainThread(crateReps, onComplete));
                }
                finally
                {
                    lockSlim.ExitReadLock();
                }

            });
        }
        private static System.Collections.IEnumerator InvokeOnMainThread(CrateRep[] result, Action<CrateRep[]> onComplete)
        {
            yield return new WaitForEndOfFrame(); // Wait one frame to ensure we're on main thread
            onComplete(result);
        }
    }
}
