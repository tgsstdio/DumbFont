using System.Runtime.InteropServices;

namespace DumbFont
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct fd_WIPCell
    {
        internal fd_Rect bbox;
        internal uint value;
        internal uint from;
        internal uint to;
        internal uint start_len;
    }
}
