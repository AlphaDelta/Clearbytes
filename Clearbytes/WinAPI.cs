using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Clearbytes
{
    class WinAPI
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int memcmp(byte[] b1, byte[] b2, long count);
    }
}
