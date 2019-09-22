using System.Runtime.InteropServices;

namespace DumbFont
{
    [StructLayout(LayoutKind.Sequential)]
    public struct fd_HostGlyphInfo
    {
        public fd_Rect bbox;
        public float advance;
    }
}
