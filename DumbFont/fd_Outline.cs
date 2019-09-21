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
        internal fd_Rect bbox;
        //internal int num_of_contours;
        //internal int num_of_points;

        internal List<fd_ContourRange> contours;
        internal List<Vector2> points;
        internal uint cell_count_x;
        internal uint cell_count_y;
        internal uint[] cells;

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

        private void add_outline_point(Vector2 p)
        {
            points.Add(p);
        }

        private Vector2 convert_point_4(FT_Vector4 v)
        {
            return new Vector2(v.x / 64.0f,  v.y / 64.0f);
        }

        private void add_outline_contour(fd_ContourRange range)
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

        private Vector2 convert_point_8(FT_Vector8 v)
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

        internal static fd_Outline fd_outline_fix_thin_lines(fd_Outline src)
        {
            var dst = new fd_Outline
            {
                bbox = src.bbox,
             };

            for (var contour_index = 0; contour_index < src.contours.Count; contour_index++)
            {
                var contour_begin = src.contours[contour_index].begin;
                var contour_end = src.contours[contour_index].end;

                dst.outline_add_odd_point();

                var urange = new fd_ContourRange
                {
                    begin = (uint) dst.points.Count,
                    end = uint.MaxValue
                };
                dst.add_outline_contour(urange);

                for (int i = (int)contour_begin; i < contour_end; i += 2)
                {
                    var p0 = src.points[i];
                    var p1 = src.points[i + 1];
                    var p2 = src.points[i + 2];

                    Vector2 mid, midp1;
                    mid = Vector2.Lerp(p0, p2, 0.5f);
                    midp1 = p1 - mid;

                    var bezier = new List<Vector2> {
                    p0,
                    p1,
                    p2
                };

                    bezier[1] += midp1;
                    /*
                    bool subdivide = false;
                    if (i > 2)
                    {
                        uint jbegin = contour_begin;
                        if (i == contour_end - 2) jbegin += 2;

                        for (uint j = jbegin; j < i - 2; j += 2)
                        {
                            float *q0 = o.points[j];
                            float *q2 = o.points[j + 2];

                            if (fd_bezier2_line_is_intersecting(bezier, q0, q2))
                                subdivide = true;
                        }
                    }

                    uint jend = contour_end;
                    if (i == contour_begin) jend -= 2;

                    for (uint j = i + 2; j < jend; j += 2)
                    {
                        float *q0 = o.points[j];
                        float *q2 = o.points[j + 2];

                        if (fd_bezier2_line_is_intersecting(bezier, q0, q2))
                            subdivide = true;
                    }
                    */
                    bool subdivide = false;
                    for (int j = (int)contour_begin; j < contour_end; j += 2)
                    {
                        if (i == contour_begin && j == contour_end - 2) continue;
                        if (i == contour_end - 2 && j == contour_begin) continue;
                        if (j + 2 >= i && j <= i + 2) continue;

                        var q0 = src.points[j];
                        //float *q1 = o.points[j + 1];
                        var q2 = src.points[j + 2];

                        if (Geometry.fd_bezier2_line_is_intersecting(bezier, 0, q0, q2))
                            subdivide = true;
                    }

                    if (subdivide)
                    {
                        var newp = new Vector2[3];
                        Geometry.fd_bezier2_split_3p(newp, src.points, i, 0.5f);

                        dst.add_outline_point(p0);
                        dst.add_outline_point(newp[0]);
                        dst.add_outline_point(newp[1]);
                        dst.add_outline_point(newp[2]);
                    }
                    else
                    {
                        dst.add_outline_point(p0);
                        dst.add_outline_point(p1);
                    }
                }

                var temp = dst.contours[(int)contour_index];
                temp.end = (uint) dst.points.Count;
                dst.contours[(int)contour_index] = temp;
                dst.add_outline_point(src.points[(int)contour_end]);
            }

            src.fd_outline_destroy();
            return dst;
        }

        private void fd_outline_destroy()
        {
            if (contours.Count > 0)
                contours.Clear();
            if (points.Count > 0)
                points.Clear();

            if (cells != null)
                cells = null;
        }

        private void outline_add_odd_point()
        {
            if (points.Count % 2 != 0)
            {
                add_outline_point(new Vector2(bbox.min_x, bbox.min_y));
            }
        }

        const int FD_OUTLINE_MAX_POINTS = (255 * 2);

        static uint uint32_to_pow2(uint v)
        {
            v--;
            v |= v >> 1;
            v |= v >> 2;
            v |= v >> 4;
            v |= v >> 8;
            v |= v >> 16;
            v++;
            return v;
        }

        internal static fd_Outline fd_outline_make_cells(fd_Outline original)
        {
            int no_of_points = original.points.Count;
            if (no_of_points > FD_OUTLINE_MAX_POINTS)
                return original;

            float w = original.bbox.max_x - original.bbox.min_x;
            float h = original.bbox.max_y - original.bbox.min_y;

            float multiplier = 0.5f;
            if (h > w * 1.8f || w > h * 1.8f)
                multiplier = 1.0f;

            uint c = uint32_to_pow2((uint)Math.Sqrt(no_of_points * 0.75f));

            original.cell_count_x = c;
            original.cell_count_y = c;

            if (h > w * 1.8f) original.cell_count_x /= 2;
            if (w > h * 1.8f) original.cell_count_y /= 2;

            while (true)
            {
                if (try_to_fit_in_cell_count(original, out fd_Outline replacement))
                {
                    return replacement; 
                }

                if (original.cell_count_x > 64 || original.cell_count_y > 64)
                {
                    original.cell_count_x = 0;
                    original.cell_count_y = 0;
                    return original;
                }

                if (original.cell_count_x == original.cell_count_y)
                {
                    if (w > h) original.cell_count_x *= 2;
                    else original.cell_count_y *= 2;
                }
                else
                {
                    if (original.cell_count_x < original.cell_count_y)
                        original.cell_count_x *= 2;
                    else original.cell_count_y *= 2;
                }
            }
        }

        static bool try_to_fit_in_cell_count(fd_Outline original, out fd_Outline replacement)
        {
            bool ret = true;

            var cells = new fd_WIPCell[original.cell_count_x * original.cell_count_y];
            init_wipcells(original, cells);

            var temp = new fd_Outline
            {
                bbox = original.bbox,
                cell_count_x = original.cell_count_x,
                cell_count_y = original.cell_count_y,
            };

            var num_of_points = (uint)original.points.Count;
            var num_of_contours = (uint) original.contours.Count;

            for (uint contour_index = 0; contour_index < num_of_contours; contour_index++)
            {
                uint contour_begin = original.contours[(int)contour_index].begin;
                uint contour_end = original.contours[(int)contour_index].end;

                temp.outline_add_odd_point();

                var urange = new fd_ContourRange
                {
                    begin = num_of_points,
                    end = num_of_points + contour_end - contour_begin
                };
                temp.add_outline_contour(urange);

                for (uint i = contour_begin; i < contour_end; i += 2)
                {
                    var p0 = original.points[(int)i];
                    var p1 = original.points[(int)(i + 1)];
                    //float *p2 = o.points[i + 2];

                    uint j = num_of_points;
                    temp.add_outline_point(p0);
                    temp.add_outline_point(p1);

                    ret &= for_each_wipcell_add_bezier(original, temp, i, j, contour_index, cells);
                }

                uint max_start_len = 0;
                ret &= for_each_wipcell_finish_contour(original, temp, contour_index, cells, ref max_start_len);

                uint continuation_end = contour_begin + max_start_len * 2;
                for (uint i = contour_begin; i < continuation_end; i += 2)
                {
                    temp.add_outline_point(original.points[(int)i]);
                    temp.add_outline_point(original.points[(int)(i + 1)]);
                }

                var plast = original.points[(int)continuation_end];
                temp.add_outline_point(plast);

            }

            if (!ret)
            {
                temp.fd_outline_destroy();
                replacement = null;
                return ret;
            }

            uint filled_line = outline_add_filled_line(temp);
            uint filled_cell = make_cell_from_single_edge(filled_line);
            set_filled_cells(temp, cells, filled_cell);

            copy_wipcell_values(temp, ref cells);
            original.fd_outline_destroy();
            replacement = temp;
            return ret;
        }

        static void copy_wipcell_values(fd_Outline u, ref fd_WIPCell[] cells)
        {
            u.cells = new uint[u.cell_count_x * u.cell_count_y];

            for (uint y = 0; y < u.cell_count_y; y++)
            {
                for (uint x = 0; x < u.cell_count_x; x++)
                {
                    uint i = y * u.cell_count_x + x;
                    u.cells[i] = cells[i].value;
                }
            }

        }

        static void set_filled_cells(fd_Outline u, fd_WIPCell[] cells, uint filled_cell)
        {
            for (uint y = 0; y < u.cell_count_y; y++)
            {
                for (uint x = 0; x < u.cell_count_x; x++)
                {
                    uint i = y * u.cell_count_x + x;
                    var cell = cells[i];

                    if (cell.value == 0 && is_cell_filled(u, cell.bbox))
                        cell.value = filled_cell;
                }
            }
        }

        // TODO: optimize
        static bool is_cell_filled(fd_Outline o, fd_Rect bbox)
        {
            var p = new Vector2
            {
                X = (bbox.max_x + bbox.min_x) / 2.0f,
                Y = (bbox.max_y + bbox.min_y) / 2.0f,
            };

            float mindist = float.MaxValue;
            float v = float.MaxValue;
            uint j = uint.MaxValue;

            var num_of_contours = o.contours.Count;

            for (int contour_index = 0; contour_index < num_of_contours; contour_index++)
            {
                uint contour_begin = o.contours[contour_index].begin;
                uint contour_end = o.contours[contour_index].end;

                for (int i = (int)contour_begin; i < contour_end; i += 2)
                {
                    var p0 = o.points[i];
                    var p1 = o.points[i + 1];
                    var p2 = o.points[i + 2];

                    float t = Geometry.fd_line_calculate_t(p0, p2, p);

                    Vector2 p02;
                    p02 = Vector2.Lerp(p0, p2, t);

                    float udist = Vector2.Distance(p02, p);

                    if (udist < mindist + 0.0001f)
                    {
                        float d = Geometry.fd_line_signed_distance(p0, p2, p);

                        if (udist >= mindist && i > contour_begin)
                        {
                            float lastd = i == contour_end - 2 && j == contour_begin
                                ? Geometry.fd_line_signed_distance(p0, p2, o.points[(int)(contour_begin + 2)])
                                : Geometry.fd_line_signed_distance(p0, p2, o.points[i - 2]);

                            if (lastd < 0.0) v = Math.Max(d, v);
                            else v = Math.Min(d, v);
                        }
                        else
                        {
                            v = d;
                        }

                        mindist = Math.Min(mindist, udist);
                        j = (uint)i;
                    }
                }
            }

            return v > 0.0f;
        }

        static uint make_cell_from_single_edge(uint e)
        {
            Debug.Assert(e % 2 == 0);
            return e << 7 | 1;
        }

        static uint outline_add_filled_line(fd_Outline o)
        {
            o.outline_add_odd_point();

            uint i = (uint) o.points.Count;
            float y = o.bbox.max_y + 1000.0f;
            var f0 = new Vector2 { X = o.bbox.min_x, Y = y };
            var f1 = new Vector2 { X = o.bbox.min_x + 10.0f, Y = y };
            var f2 = new Vector2 { X = o.bbox.min_x + 20.0f, Y = y };
            o.add_outline_point(f0);
            o.add_outline_point(f1);
            o.add_outline_point(f2);

            return i;
        }

        static bool for_each_wipcell_add_bezier(fd_Outline o, fd_Outline u, uint i, uint j, uint contour_index, fd_WIPCell[] cells)
        {
            fd_Rect bezier_bbox;
            Geometry.fd_bezier2_bbox(o.points, (int)i, out bezier_bbox);

            float outline_bbox_w = o.bbox.max_x - o.bbox.min_x;
            float outline_bbox_h = o.bbox.max_y - o.bbox.min_y;

            uint min_x = (uint)((bezier_bbox.min_x - o.bbox.min_x) / outline_bbox_w * o.cell_count_x);
            uint min_y = (uint)((bezier_bbox.min_y - o.bbox.min_y) / outline_bbox_h * o.cell_count_y);
            uint max_x = (uint)((bezier_bbox.max_x - o.bbox.min_x) / outline_bbox_w * o.cell_count_x);
            uint max_y = (uint)((bezier_bbox.max_y - o.bbox.min_y) / outline_bbox_h * o.cell_count_y);

            if (max_x >= o.cell_count_x) max_x = o.cell_count_x - 1;
            if (max_y >= o.cell_count_y) max_y = o.cell_count_y - 1;

            bool ret = true;
            for (uint y = min_y; y <= max_y; y++)
            {
                for (uint x = min_x; x <= max_x; x++)
                {
                    var cell = cells[y * o.cell_count_x + x];
                    if (Geometry.fd_bbox_bezier2_intersect(cell.bbox, o.points, (int)i))
                        ret &= wipcell_add_bezier(o, u, i, j, contour_index, cell);
                }
            }

            return ret;
        }
        static bool wipcell_add_bezier(fd_Outline o, fd_Outline u, uint i, uint j, uint contour_index, fd_WIPCell cell)
        {
            bool ret = true;
            uint ucontour_begin = u.contours[(int)contour_index].begin;

            if (cell.to != uint.MaxValue && cell.to != j)
            {
                Debug.Assert(cell.to < j);

                if (cell.from == ucontour_begin)
                {
                    Debug.Assert(cell.to % 2 == 0);
                    Debug.Assert(cell.from % 2 == 0);

                    cell.start_len = (cell.to - cell.from) / 2;
                }
                else
                {
                    cell.value = cell_add_range(cell.value, cell.from, cell.to);
                    if (cell.value == 0) ret = false;
                }

                cell.from = j;
            }
            else
            {
                if (cell.from == uint.MaxValue)
                    cell.from = j;
            }

            cell.to = j + 2;
            return ret;
        }

        static uint cell_add_range(uint cell, uint from, uint to)
        {
            Debug.Assert(from % 2 == 0 && to % 2 == 0);

            from /= 2;
            to /= 2;

            if (from >= byte.MaxValue) return 0;
            if (to >= byte.MaxValue) return 0;

            uint length = to - from;
            if (length <= 3 && (cell & 0x03) == 0)
            {
                cell |= from << 8;
                cell |= length;
                return cell;
            }

            if (length > 7)
                return 0;

            if ((cell & 0x1C) == 0)
            {
                cell |= from << 16;
                cell |= length << 2;
                return cell;
            }

            if ((cell & 0xE0) == 0)
            {
                cell |= from << 24;
                cell |= length << 5;
                return cell;
            }

            return 0;
        }

        static bool for_each_wipcell_finish_contour(fd_Outline o, fd_Outline u, uint contour_index, fd_WIPCell[] cells, ref uint max_start_len)
        {
            bool ret = true;
            for (uint y = 0; y < o.cell_count_y; y++)
            {
                for (uint x = 0; x < o.cell_count_x; x++)
                {
                    fd_WIPCell cell = cells[y * o.cell_count_x + x];
                    ret &= wipcell_finish_contour(o, u, contour_index, cell, ref max_start_len);
                }
            }

            return ret;
        }

        static bool wipcell_finish_contour(fd_Outline o, fd_Outline u, uint contour_index, fd_WIPCell cell, ref uint max_start_len)
        {
            bool ret = true;
            uint ucontour_begin = u.contours[(int)contour_index].begin;
            uint ucontour_end = u.contours[(int)contour_index].end;

            // max_start_len = uint.MinValue;

            if (cell.to < ucontour_end)
            {
                cell.value = cell_add_range(cell.value, cell.from, cell.to);
                if (cell.value == 0) ret = false;

                cell.from = uint.MaxValue;
                cell.to = uint.MaxValue;
            }

            Debug.Assert(cell.to == uint.MaxValue || cell.to == ucontour_end);
            cell.to = uint.MaxValue;

            if (cell.from != uint.MaxValue && cell.start_len != 0)
            {
                cell.value = cell_add_range(cell.value, cell.from, ucontour_end + cell.start_len * 2);
                if (cell.value == 0) ret = false;

                max_start_len = Math.Max(max_start_len, cell.start_len);
                cell.from = uint.MaxValue;
                cell.start_len = 0;
            }

            if (cell.from != uint.MaxValue)
            {
                cell.value = cell_add_range(cell.value, cell.from, ucontour_end);
                if (cell.value == 0) ret = false;

                cell.from = uint.MaxValue;
            }

            if (cell.start_len != 0)
            {
                cell.value = cell_add_range(cell.value, ucontour_begin, ucontour_begin + cell.start_len * 2);
                if (cell.value == 0) ret = false;

                cell.start_len = 0;
            }

            Debug.Assert(cell.from == uint.MaxValue && cell.to == uint.MaxValue);
            return ret;
        }

        static void init_wipcells(fd_Outline o, fd_WIPCell[] cells)
        {
            float w = o.bbox.max_x - o.bbox.min_x;
            float h = o.bbox.max_y - o.bbox.min_y;

            for (uint y = 0; y < o.cell_count_y; y++)
            {
                for (uint x = 0; x < o.cell_count_x; x++)
                {
                    var bbox = new fd_Rect
                    {
                        min_x = o.bbox.min_x + ((float)x / o.cell_count_x) * w,
                        min_y = o.bbox.min_y + ((float)y / o.cell_count_y) * h,
                        max_x = o.bbox.min_x + ((float)(x + 1) / o.cell_count_x) * w,
                        max_y = o.bbox.min_y + ((float)(y + 1) / o.cell_count_y) * h,
                    };

                    uint i = y * o.cell_count_x + x;
                    cells[i].bbox = bbox;
                    cells[i].from = uint.MaxValue;
                    cells[i].to = uint.MaxValue;
                    cells[i].value = 0;
                    cells[i].start_len = 0;
                }
            }
        }
    }
    
}
