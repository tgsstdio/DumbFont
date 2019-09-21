using System.Runtime.InteropServices;

namespace DumbFont
{
    [StructLayout(LayoutKind.Sequential)]
    public struct fd_Rect
    {
        public float min_x;
        public float min_y;
        public float max_x;
        public float max_y;
    }
}
