using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

namespace DumbFont
{
    public static class Geometry
    {
        static FD_QUADRATIC_SOLUTION solve_quadratic(float a, float b, float c, ref float x1, ref float x2)
        {
            float discriminant = b * b - 4.0f * a * c;

            if (discriminant > 0.0f)
            {
                float sqrt_d = (float) Math.Sqrt(discriminant);
                float common = b >= 0.0f ? -b - sqrt_d : -b + sqrt_d;

                x1 = 2.0f * c / common;

                if (a == 0.0f)
                    return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_ONE;

                x2 = common / (2.0f * a);
                return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_TWO;
            }
            else if (discriminant == 0.0f)
            {
                if (b == 0.0f)
                {
                    if (a == 0.0f)
                    {
                        if (c == 0.0f) return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_ALL;
                        else return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_NONE;
                    }
                    else
                    {
                        x1 = 0.0f;
                        return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_TOUCH;
                    }
                }
                else
                {
                    x1 = 2.0f * c / -b;
                    return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_TOUCH;
                }
            }

            return FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_NONE;
        }

        static float line_vertical_intersect(float x, Vector2 p1, Vector2 p2)
        {
            float m = (p2.Y - p1.Y) / (p2.X - p1.X);
	        return p1.Y - m* (p1.X - x);
        }

        static float line_horizontal_intersect(float y, Vector2 p1, Vector2 p2)
        {
            float n = (p2.X - p1.X) / (p2.Y - p1.Y);
            return p1.X - n * (p1.Y - y);
        }

        static bool is_between(float value, float min, float max)
        {
            return value >= min && value <= max;
        }

        static bool is_between_exclusive(float value, float min, float max)
        {
            return value > min && value < max;
        }

        static bool is_point_inside_bbox(fd_Rect bbox, Vector2 p)
        {
            return is_between(p.X, bbox.min_x, bbox.max_x) && is_between(p.Y, bbox.min_y, bbox.max_y);
        }

        static bool is_point_inside_bbox_exclusive(fd_Rect bbox, Vector2 p)
        {
            return is_between_exclusive(p.X, bbox.min_x, bbox.max_x) && is_between_exclusive(p.Y, bbox.min_y, bbox.max_y);
        }

        static bool is_intersection_in_line_segment(Vector2 p1, Vector2 p2, Vector2 i)
        {
            float px_min = Math.Min(p1.X, p2.X);
            float px_max = Math.Max(p1.X, p2.X);
            float py_min = Math.Min(p1.Y, p2.Y);
            float py_max = Math.Max(p1.Y, p2.Y);

            return is_between(i.X, px_min, px_max) && is_between(i.Y, py_min, py_max);
        }

        static bool is_line_segment_intersecting_bbox(fd_Rect bbox, Vector2 p1, Vector2 p2)
        {
            if (is_point_inside_bbox_exclusive(bbox, p1)) return true;
            if (is_point_inside_bbox_exclusive(bbox, p2)) return true;

            float x_top = line_horizontal_intersect(bbox.max_y, p1, p2);
            float x_bottom = line_horizontal_intersect(bbox.min_y, p1, p2);
            float y_left = line_vertical_intersect(bbox.min_x, p1, p2);
            float y_right = line_vertical_intersect(bbox.max_x, p1, p2);

            var top = new Vector2(x_top, bbox.max_y);
            var bottom =  new Vector2( x_bottom, bbox.min_y );
            var left = new Vector2( bbox.min_x, y_left );
            var right = new Vector2( bbox.max_x, y_right );

            if (is_between(x_top, bbox.min_x, bbox.max_x) &&
                is_intersection_in_line_segment(p1, p2, top))
            {
                return true;
            }

            if (is_between(x_bottom, bbox.min_x, bbox.max_x) &&
                is_intersection_in_line_segment(p1, p2, bottom))
            {
                return true;
            }

            if (is_between(y_left, bbox.min_y, bbox.max_y) &&
                is_intersection_in_line_segment(p1, p2, left))
            {
                return true;
            }

            if (is_between(y_right, bbox.min_y, bbox.max_y) &&
                is_intersection_in_line_segment(p1, p2, right))
            {
                return true;
            }

            return false;
        }

        public static bool fd_bbox_bezier2_intersect(fd_Rect bbox, List<Vector2> bezier, int offset)
        {
            if (is_point_inside_bbox_exclusive(bbox, bezier[offset + 0])) return true;
            if (is_point_inside_bbox_exclusive(bbox, bezier[offset + 2])) return true;

            var bl = new Vector2( bbox.min_x, bbox.min_y );
            var br = new Vector2(bbox.max_x, bbox.min_y );
            var tl = new Vector2(bbox.min_x, bbox.max_y );
            var tr = new Vector2( bbox.max_x, bbox.max_y );

            return fd_bezier2_line_is_intersecting(bezier, offset, bl, br) ||
                   fd_bezier2_line_is_intersecting(bezier, offset, br, tr) ||
                   fd_bezier2_line_is_intersecting(bezier, offset, tr, tl) ||
                   fd_bezier2_line_is_intersecting(bezier, offset, tl, bl);
        }

        public static float fd_line_signed_distance(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 line_dir;
            line_dir =  b - a;
            Debug.Assert(line_dir.Length() > 0.0f);

            var perp_dir = new Vector2(-line_dir.Y, line_dir.X );
            perp_dir = Vector2.Normalize(perp_dir);

            Vector2 dir_to_a;
            dir_to_a =  a - p;

            return Vector2.Dot(perp_dir, dir_to_a);
        }

        public static float fd_line_calculate_t(Vector2 a, Vector2 b, Vector2 p)
        {
            Vector2 ab, ap;
            ab =  b - a;
            ap = p - a;

            float t = Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab);
            return NormalizedClamp(t);
        }

        static float NormalizedClamp(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            else if (value > 1f)
            {
                return 1f;
            }

            return value;
        }

        static void bezier2_points(out Vector2 q0, out Vector2 q1, out Vector2 r, List<Vector2> bezier, int offset, float t)
        {
            q0 = Vector2.Lerp(bezier[offset], bezier[offset + 1], t);
            q1 = Vector2.Lerp(bezier[offset + 1], bezier[offset + 2], t);
            r = Vector2.Lerp(q0, q1, t);
        }

        static void fd_bezier2_point(out Vector2 r, List<Vector2> bezier, int offset, float t)
        {
            bezier2_points(out _, out _, out r, bezier, offset, t);
        }

        static void fd_bezier2_split_lr(Vector2[] left, Vector2[] right, List<Vector2> bezier, int offset, float t)
        {
            Vector2 q0, q1, r;
            bezier2_points(out q0, out q1, out r, bezier, offset, t);

            left[0] =  bezier[0];
            left[1] =  q0;
            left[2] =  r;

            right[0] = r;
            right[1] =  q1;
            right[2] = bezier[2];
        }

        static void fd_bezier2_split_5p(Vector2[] ret, List<Vector2> bezier, int offset, float t)
        {
            Vector2 q0, q1, r;
            bezier2_points(out q0, out q1, out r, bezier, offset, t);

            ret[0] =  bezier[0];
            ret[1] =  q0;
            ret[2] = r;
            ret[3] = q1;
            ret[4] = bezier[2];
        }

        public static void fd_bezier2_split_3p(Vector2[] ret, List<Vector2> bezier, int offset, float t)
        {
            Vector2 q0, q1, r;
            bezier2_points(out q0, out q1, out r, bezier, offset, t);

            ret[0] =  q0;
            ret[1] =  r;
            ret[2] = q1;
        }

        static void fd_bezier2_derivative(List<Vector2> bezier, int offset, Vector2[] derivative)
        {
            derivative[0] =  bezier[offset + 1] - bezier[offset];
            derivative[0] *= 2;

            derivative[1] = bezier[offset + 2] - bezier[offset + 1];
            derivative[1] *= 2;
        }
        static float lerpf(float x, float y, float t)
        {
            return x * (1 - t) + y * t;
        }

        static float bezier2_component(float p0, float p1, float p2, float t)
        {
            return lerpf(lerpf(p0, p1, t), lerpf(p1, p2, t), t);
        }

        public static void fd_bezier2_bbox(List<Vector2> bezier, int offset, out fd_Rect bbox)
        {
            Vector2[] deriv = new Vector2[2];
            fd_bezier2_derivative(bezier, offset, deriv);

            float tx = deriv[0].X / (deriv[0].X - deriv[1].X);
            float ty = deriv[0].Y / (deriv[0].Y - deriv[1].Y);

            bbox.min_x = Math.Min(bezier[0].X, bezier[2].X);
            bbox.min_y = Math.Min(bezier[0].Y, bezier[2].Y);
            bbox.max_x = Math.Max(bezier[0].X, bezier[2].X);
            bbox.max_y = Math.Max(bezier[0].Y, bezier[2].Y);

            if (0.0f <= tx && tx <= 1.0f)
            {
                float x = bezier2_component(bezier[0].X, bezier[1].X, bezier[2].X, tx);

                if (deriv[0].X < deriv[1].X)
                    bbox.min_x = Math.Min(bbox.min_x, x);
                else bbox.max_x = Math.Max(bbox.max_x, x);
            }

            if (0.0f <= ty && ty <= 1.0f)
            {
                float y = bezier2_component(bezier[0].Y, bezier[1].Y, bezier[2].Y, ty);

                if (deriv[0].Y < deriv[1].Y)
                    bbox.min_y = Math.Min(bbox.min_y, y);
                else bbox.max_y = Math.Max(bbox.max_y, y);
            }
        }

        static void align_point(ref Vector2 r, Vector2 p, Vector2 t, float s, float c)
        {
            Vector2 tmp;
            tmp = p - t;

            r = new Vector2(
                tmp.X * c - tmp.Y * s,
                tmp.X * s + tmp.Y * c
            );
        }

        static void align_lsc(Vector2 p0, Vector2 p1, out float l, out float s, out float c)
        {
            Vector2 v;
            v =  p1 - p0;

            l = v.Length();
            s = -v.Y / l;
            c = v.X / l;
        }

        static void fd_bezier2_align_to_self(Vector2[] r, Vector2[] bezier)
        {
            float l, s, c;
            align_lsc(bezier[0], bezier[2], out l, out s, out c);

            r[0] = new Vector2(0.0f, 0.0f);
            align_point(ref r[1], bezier[1], bezier[0], s, c);
            r[2] = new Vector2(l, 0.0f);
        }

        static void fd_bezier2_align_to_line(Vector2[] r, Vector2[] bezier, Vector2 line0, Vector2 line1)
        {
            float l, s, c;
            align_lsc(line0, line1, out l, out s, out c);

            align_point(ref r[0], bezier[0], line0, s, c);
            align_point(ref r[1], bezier[1], line0, s, c);
            align_point(ref r[2], bezier[2], line0, s, c);
        }

        public static bool fd_bezier2_line_is_intersecting(List<Vector2> bezier, int offset, Vector2 line0, Vector2 line1)
        {
            float l, si, co;
            align_lsc(line0, line1, out l, out si, out co);

            Vector2[] bez = new Vector2[3];
            align_point(ref bez[0], bezier[offset], line0, si, co);
            align_point(ref bez[1], bezier[offset + 1], line0, si, co);
            align_point(ref bez[2], bezier[offset + 2], line0, si, co);

            float x0 = bez[0].X, y0 = bez[0].Y;
            float x1 = bez[1].X, y1 = bez[1].Y;
            float x2 = bez[2].X, y2 = bez[2].Y;

            float a = y0 - 2 * y1 + y2;
            float b = 2 * (y1 - y0);
            float c = y0;

            float t0 = 0;
            float t1 = 0;
            float xt0, xt1;
            var sol = solve_quadratic(a, b, c, ref t0, ref t1);

            switch (sol)
            {
                case FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_NONE:
                case FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_ALL:
                    return false;

                case FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_TOUCH:
                case FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_ONE:
                    xt0 = bezier2_component(x0, x1, x2, t0);
                    return is_between(t0, 0, 1) && is_between(xt0, 0, l);

                case FD_QUADRATIC_SOLUTION.FD_QUADRATIC_SOLUTION_TWO:
                    xt0 = bezier2_component(x0, x1, x2, t0);
                    xt1 = bezier2_component(x0, x1, x2, t1);

                    return is_between(t0, 0, 1) && is_between(xt0, 0, l) ||
                           is_between(t1, 0, 1) && is_between(xt1, 0, l);

                default:
                    Debug.Assert(false);
                    return false;
            }
        }
    }
}
