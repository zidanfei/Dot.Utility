using System;
using System.Collections.Generic;
using System.Text;

namespace Dot.Utility
{
    public class RuntimeInformation
    {
        public bool IsWindowsPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);
        }
        public bool IsLinuxPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux);
        }
        public bool IsOSPlatform()
        {
            return System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
        }


    }
}
