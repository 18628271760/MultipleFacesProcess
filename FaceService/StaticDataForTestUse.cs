using System;
using System.Collections.Concurrent;

namespace Proprietor.InformationService
{
    static public class StaticDataForTestUse
    {
        public static ConcurrentDictionary<IntPtr, string> dbFaceInfor = new ConcurrentDictionary<IntPtr, string>();
    }
}
