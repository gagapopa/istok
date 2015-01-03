using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COTES.ISTOK.Block;

namespace COTES.ISTOK.Tests.Block
{
    class TestValueReceiver : ValueReceiver
    {
        Dictionary<int, List<Package>> packages;

        public TestValueReceiver()
            : base(null)
        {
            packages = new Dictionary<int, List<Package>>();
            SavePackageLog = new List<Package>();
        }

        public List<Package> SavePackageLog { get; set; }

        public override void SavePackage(Package package)
        {
            List<Package> list;

            if (!packages.TryGetValue(package.Id, out list))
            {
                packages[package.Id] = list = new List<Package>();
            }
            if (!list.Contains(package))
            {
                list.Add(package);
            }
            // add to log
            SavePackageLog.Add(package);
        }

        public override Package LoadPackage(int parameterID, DateTime Time)
        {
            List<Package> list;

            if (packages.TryGetValue(parameterID, out list))
            {
                return (from p in list
                        where p.DateFrom <= Time && Time < p.DateTo
                        select p).FirstOrDefault();
            }
            return null;
        }

        public override Package LoadPrevPackage(int parameterID, DateTime Time)
        {
            List<Package> list;

            if (packages.TryGetValue(parameterID, out list))
            {
                return (from p in list
                        where p.DateTo < Time
                        orderby p.DateTo descending
                        select p).FirstOrDefault();
            }
            return null;
        }

        public override Package LoadNextPackage(int parameterID, DateTime Time)
        {
            List<Package> list;

            if (packages.TryGetValue(parameterID, out list))
            {
                return (from p in list
                        where p.DateFrom > Time
                        orderby p.DateFrom ascending
                        select p).FirstOrDefault();
            }
            return null;
        }
    }
}
