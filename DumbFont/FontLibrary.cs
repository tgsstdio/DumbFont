using System;
using System.Runtime.InteropServices;

namespace DumbFont
{
    public class FontLibrary
    {
        [DllImport(FontModule.FreetypeDll, CallingConvention = FontModule.CallConvention)]
        internal static extern FT_Error FT_Init_FreeType(ref IntPtr alibrary);

        [DllImport(FontModule.FreetypeDll, CallingConvention = FontModule.CallConvention)]
        internal static extern FT_Error FT_Done_FreeType(IntPtr library);

        internal IntPtr Handle { get; set; } = IntPtr.Zero;

        public FT_Error Init_FreeType()
        {
            IntPtr handle = IntPtr.Zero;
            var err = FT_Init_FreeType(ref handle);
            Handle = handle;
            return err;
        }

        public FT_Error Done_FreeType()
        {
            if (Handle != IntPtr.Zero)
            {
                return FT_Done_FreeType(Handle);
            }
            return FT_Error.Ok;
        }
    }
}
