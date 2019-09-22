using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace DumbFont
{
    // typedef signed long  FT_Long;
    // typedef signed long FT_Pos;
    //   typedef signed long  FT_Fixed;

    // typedef signed long  FT_F26Dot6;

    // typedef signed char  FT_Char; (FT_Char*)

    // typedef signed int  FT_Int;
    // typedef unsigned int  FT_UInt;

    // typedef unsigned short  FT_UShort;

    // typedef signed short  FT_Short;

    //  typedef unsigned char  FT_Bool;

    public class fd_Outline
    {
        public fd_Rect bbox;
        //internal int num_of_contours;
        //internal int num_of_points;

        internal List<fd_ContourRange> contours;
        public List<Vector2> points;
        public uint cell_count_x;
        public uint cell_count_y;
        public uint[] cells;

        public fd_Outline()
        {
            contours = new List<fd_ContourRange>();
            points = new List<Vector2>();
        }

        internal int MoveTo_4(IntPtr to, IntPtr user)
        {
            var to_0 = Marshal.PtrToStructure<FT_Vector4>(to);

            var p = Vector2.Zero;

            if (contours.Count > 0)
            {
                contours[contours.Count - 1].end = (uint) points.Count - 1;
                add_outline_point(p);
            }

            Debug.Assert(points.Count % 2 == 0);

            var range = new fd_ContourRange
            {
                begin = (uint) points.Count,
                end = uint.MaxValue,
            };
            add_outline_contour(range);

            add_outline_point(convert_point_4(to_0));
            return 0;
        }

        internal void add_outline_point(Vector2 p)
        {
            points.Add(p);
        }

        internal Vector2 convert_point_4(FT_Vector4 v)
        {
            return new Vector2(v.x / 64.0f,  v.y / 64.0f);
        }

        internal void add_outline_contour(fd_ContourRange range)
        {
            contours.Add(range);
        }

        internal int LineTo_4(IntPtr to, IntPtr user)
        {
            var toPt = convert_point_4(Marshal.PtrToStructure<FT_Vector4>(to));
            var lastPt = points[points.Count - 1];
            var p = Vector2.Lerp(lastPt, toPt, 0.5f);
            add_outline_point(p);
            add_outline_point(toPt);
            return 0;
        }

        internal int ConicTo_4(IntPtr control, IntPtr to, IntPtr user)
        {
            var control_Pt = convert_point_4(Marshal.PtrToStructure<FT_Vector4>(control));
            var to_Pt = convert_point_4(Marshal.PtrToStructure<FT_Vector4>(to));

            add_outline_point(control_Pt);
            add_outline_point(to_Pt);
            return 0;
        }

        internal int CubicTo_4(IntPtr control1, IntPtr control2, IntPtr to, IntPtr user)
        {
            return LineTo_4(to, user);
        }

        internal int MoveTo_8(IntPtr to, IntPtr user)
        {
            var to_0 = Marshal.PtrToStructure<FT_Vector8>(to);

            var p = Vector2.Zero;

            if (contours.Count > 0)
            {
                contours[contours.Count - 1].end = (uint)points.Count - 1;
                add_outline_point(p);
            }

            Debug.Assert(points.Count % 2 == 0);

            var range = new fd_ContourRange
            {
                begin = (uint) points.Count,
                end = uint.MaxValue,
            };
            add_outline_contour(range);

            add_outline_point(convert_point_8(to_0));
            return 0;
        }

        internal Vector2 convert_point_8(FT_Vector8 v)
        {
            return new Vector2(v.x / 64.0f, v.y / 64.0f);
        }

        internal int LineTo_8(IntPtr to, IntPtr user)
        {
            var toPt = convert_point_8(Marshal.PtrToStructure<FT_Vector8>(to));
            var lastPt = points[points.Count - 1];
            var p = Vector2.Lerp(lastPt, toPt, 0.5f);
            add_outline_point(p);
            add_outline_point(toPt);
            return 0;
        }

        internal int ConicTo_8(IntPtr control, IntPtr to, IntPtr user)
        {
            var control_Pt = convert_point_8(Marshal.PtrToStructure<FT_Vector8>(control));
            var to_Pt = convert_point_8(Marshal.PtrToStructure<FT_Vector8>(to));

            add_outline_point(control_Pt);
            add_outline_point(to_Pt);
            return 0;
        }

        internal int CubicTo_8(IntPtr control, IntPtr control2, IntPtr to, IntPtr user)
        {
            return LineTo_8(to, user);
        }

        public void fd_outline_destroy()
        {
            if (contours.Count > 0)
                contours.Clear();
            if (points.Count > 0)
                points.Clear();

            if (cells != null)
                cells = null;
        }

        internal void outline_add_odd_point()
        {
            if (points.Count % 2 != 0)
            {
                add_outline_point(new Vector2(bbox.min_x, bbox.min_y));
            }
        }
                
        public uint num_of_points {
            get
            {
                return (uint) points.Count;
            }
        }

    }
    
}
