using System.Runtime.InteropServices;

namespace DumbFont
{
    [StructLayout(LayoutKind.Sequential)]
    public struct fd_CellInfo
    {
        public uint point_offset;
        public uint cell_offset;
        public uint cell_count_x;
        public uint cell_count_y;
    }
}
