using System.Runtime.InteropServices;

namespace DumbFont
{
    [StructLayout(LayoutKind.Sequential)]
    public struct fd_DeviceGlyphInfo
    {
        public fd_Rect bbox;
        //fd_Rect cbox;
        public fd_CellInfo cell_info;
    }
}
