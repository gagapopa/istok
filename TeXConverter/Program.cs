using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeXConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2)
            {
                try
                {
                    NativeMethods.CreateGifFromEq(args[0], args[1]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
            else
                Console.WriteLine("Usage: texconverter.exe expression filename");
        }
    }

    [System.Security.SuppressUnmanagedCodeSecurity()]
    internal class NativeMethods
    {
        private NativeMethods()
        { //all methods in this class would be static
        }

        [System.Runtime.InteropServices.DllImport(
            "MimeTex.dll",
            CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
        internal static extern int CreateGifFromEq(string expr, string fileName);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        internal extern static IntPtr GetModuleHandle(string lpModuleName);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal extern static bool FreeLibrary(IntPtr hLibModule);
    }
}
