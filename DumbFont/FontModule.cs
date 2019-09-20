using System;
using System.Runtime.InteropServices;

namespace DumbFont
{
    // typedef signed long  FT_Long;
    internal class FontModule
    {
        /// <summary>
        /// Defines the location of the FreeType DLL. Update SharpFont.dll.config if you change this!
        /// </summary>
        /// TODO: Use the same name for all platforms.
        internal const string FreetypeDll = "freetype";

        /// <summary>
        /// Defines the calling convention for P/Invoking the native freetype methods.
        /// </summary>
        internal const CallingConvention CallConvention = CallingConvention.Cdecl;
    }
}
