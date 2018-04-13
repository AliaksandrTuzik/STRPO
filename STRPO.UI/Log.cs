using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace STRPO.UI
{
    static class Log
    {
        public static void WithTime(string message = null, [CallerMemberName] string methodName = null)
        {
            Debug.WriteLine($"{message} at {methodName} ({DateTime.Now})");
        }
    }
}
