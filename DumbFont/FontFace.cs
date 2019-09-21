using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DumbFont
{
    // typedef signed long  FT_Long;
    using FT_Long4 = System.Int32;
    using FT_Long8 = System.Int64;
    // typedef signed long FT_Pos;
    //   typedef signed long  FT_Fixed;

    // typedef signed long  FT_F26Dot6;
    using FT_F26Dot6_4 = System.Int32;
    using FT_F26Dot6_8 = System.Int64;

    // typedef signed char  FT_Char; (FT_Char*)

    // typedef signed int  FT_Int;
    // typedef unsigned int  FT_UInt;
    using FT_UInt = System.UInt32;

    // typedef unsigned short  FT_UShort;

    // typedef signed short  FT_Short;

    //  typedef unsigned char  FT_Bool;

    using FT_ULong4 = System.UInt32;
    using FT_ULong8 = System.UInt64;

    using FT_Face = System.IntPtr;

    public class FontFace
    {
        private FT_Face mHandle;
        private FT_FaceRec4 mFace4;
        private FT_FaceRec8 mFace8;

        public FontFace(LongForm form)
        {
            Form = form;
        }

        public LongForm Form { get; }

        static FT_Long8 ToLong8(int num)
        {
            return Convert.ToInt64(num);
        }

        #region NewFace methods

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_New_Face",
            CallingConvention = FontModule.CallConvention,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        static extern FT_Error FT_New_Face_4(IntPtr library, string filepathname, FT_Long4 face_index, ref FT_Face aface);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_New_Face",
            CallingConvention = FontModule.CallConvention,
            CharSet = CharSet.Ansi,
            BestFitMapping = false,
            ThrowOnUnmappableChar = true)]
        static extern FT_Error FT_New_Face_8(IntPtr library, string filepathname, FT_Long8 face_index, ref FT_Face aface);

        public FT_Error NewFace(FontLibrary library, string path, int face_index)
        {           
            if (Form == LongForm.Long4)
            {
                return FT_New_Face_4(library.Handle, path, face_index, ref mHandle);
            }
            else
            {
                return FT_New_Face_8(library.Handle, path, ToLong8(face_index), ref mHandle);
            }
        }

        #endregion

        #region DoneFace methods 

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Done_Face",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Done_Face_4(FT_Face face);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Done_Face",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Done_Face_8(FT_Face face);

        public FT_Error DoneFace()
        {
            if (Form == LongForm.Long4)
            {
                return FT_Done_Face_4(mHandle);
            }
            else
            {
                return FT_Done_Face_8(mHandle);
            }
        }

        #endregion

        #region SetCharSize methods

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Set_Char_Size",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Set_Char_Size_4(
            FT_Face face,
            FT_F26Dot6_4 char_width,
            FT_F26Dot6_4 char_height,
            FT_UInt horz_resolution,
            FT_UInt vert_resolution);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Set_Char_Size",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Set_Char_Size_8(
            FT_Face face,
            FT_F26Dot6_8 char_width,
            FT_F26Dot6_8 char_height,
            FT_UInt horz_resolution,
            FT_UInt vert_resolution);

        public FT_Error SetCharSize(
            Fixed26Dot6 char_width,
            Fixed26Dot6 char_height,
            uint horz_resolution,
            uint vert_resolution)
        {
            if (Form == LongForm.Long4)
            {
                return FT_Set_Char_Size_4(
                    mHandle,
                    char_width.value,
                    char_height.value,
                    horz_resolution,
                    vert_resolution);
            }
            else
            {
                return FT_Set_Char_Size_8(
                    mHandle,
                    char_width.value8,
                    char_height.value8,
                    horz_resolution,
                    vert_resolution);
            }
        }

        #endregion

        #region GetCharIndex methods

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Get_Char_Index",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_UInt FT_Get_Char_Index_4(FT_Face face, FT_ULong4 charcode);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Get_Char_Index",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_UInt FT_Get_Char_Index_8(FT_Face face, FT_ULong8 charcode);

        static FT_ULong4 ToULong4(UInt64 value)
        {
            return Convert.ToUInt32(value);
        }

        public FT_UInt GetCharIndex(UInt64 charcode)
        {
            if (Form == LongForm.Long4)
            {
                if (charcode > FT_ULong4.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(charcode));
                }

                return FT_Get_Char_Index_4(mHandle, ToULong4(charcode));
            }
            else
            {
                return FT_Get_Char_Index_8(mHandle, charcode);
            }
        }

        #endregion

        #region LoadGlyph methods 

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Load_Glyph",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Load_Glyph_4(FT_Face face, FT_UInt glyph_index, Int32 load_flags);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Load_Glyph",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Load_Glyph_8(FT_Face face, FT_UInt glyph_index, Int32 load_flags);

        public FT_Error LoadGlyph(uint glyph_index, LoadFlags flags, LoadTarget target)
        {
            if (Form == LongForm.Long4)
            {
                return FT_Load_Glyph_4(mHandle, glyph_index, ((Int32) flags | (Int32)target));
            }
            else
            {
                return FT_Load_Glyph_8(mHandle, glyph_index, ((Int32)flags | (Int32)target));
            }
        }

        #endregion

        #region GetGlyphOutline methods

        class fd_Outline
        {
            internal float min_x;
            internal float min_y;
            internal float max_x;
            internal float max_y;
            internal int num_of_contours;
            internal int num_of_points;

            internal List<fd_ContourRange> contours;
            public fd_Outline()
            {
                contours = new List<fd_ContourRange>();
            }

            internal int MoveTo_4(IntPtr to, IntPtr user)
            {
                var to_0 = Marshal.PtrToStructure<FT_Vector4>(to);

                vec2 p = { 0 };

                if (num_of_contours > 0)
                {
                    contours[num_of_contours - 1].end = (uint) num_of_points - 1;
                    add_outline_point(p);
                }

                Debug.Assert(num_of_points % 2 == 0);

                var range = new fd_ContourRange
                {
                    begin = num_of_points,
                    end = uint.MaxValue,
                };
                add_outline_contour(range);

                convert_point(to_0, p);
                add_outline_point(p);
                return 0;
            }

            private void add_outline_point(vec2 p)
            {
                throw new NotImplementedException();
            }

            private void convert_point(FT_Vector4 to_0, vec2 p)
            {
                throw new NotImplementedException();
            }

            private void add_outline_contour(fd_ContourRange range)
            {
                throw new NotImplementedException();
            }

            internal int LineTo_4(IntPtr to, IntPtr user)
            {
                var to_0 = Marshal.PtrToStructure<FT_Vector4>(to);
            }

            internal int ConicTo_4(IntPtr control, IntPtr to, IntPtr user)
            {
                var control_0 = Marshal.PtrToStructure<FT_Vector4>(control);
                var to_0 = Marshal.PtrToStructure<FT_Vector4>(to);
            }

            internal int CubicTo_4(IntPtr control, IntPtr control2, IntPtr to, IntPtr user)
            {
                var control_0 = Marshal.PtrToStructure<FT_Vector4>(control);
                var control2_0 = Marshal.PtrToStructure<FT_Vector4>(control2);
                var to_0 = Marshal.PtrToStructure<FT_Vector4>(to);
            }

            internal int MoveTo_8(IntPtr to, IntPtr user)
            {
                throw new NotImplementedException();
            }

            internal int LineTo_8(IntPtr to, IntPtr user)
            {
                throw new NotImplementedException();
            }

            internal int ConicTo_8(IntPtr control, IntPtr to, IntPtr user)
            {
                throw new NotImplementedException();
            }

            internal int CubicTo_8(IntPtr control, IntPtr control2, IntPtr to, IntPtr user)
            {
                throw new NotImplementedException();
            }

            internal void fd_outline_fix_thin_lines()
            {
                throw new NotImplementedException();
            }

            internal void fd_outline_make_cells()
            {
                throw new NotImplementedException();
            }
        }
        fd_Outline fd_outline_convert(FT_Outline outline, char c)
        {
            if (c == '&')
            {
                Console.WriteLine("");
            }

            var o = fd_outline_decompose(outline);
            //fd_outline_fix_corners(o);
            //fd_outline_subdivide(o);
            o.fd_outline_fix_thin_lines();
            o.fd_outline_make_cells();
            //}

            return o;
            //printf("  %d ms\n", clock() - t);
        }

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Outline_Get_BBox",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Outline_Get_BBox_4(ref FT_Outline face, ref FT_BBox4 bbox);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Outline_Get_BBox",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Outline_Get_BBox_8(ref FT_Outline face, ref FT_BBox8 bbox);

        // EVEN IF SAME 
        delegate int MoveToFunc_4(IntPtr to, IntPtr user);
        delegate int LineToFunc_4(IntPtr to, IntPtr user);
        delegate int ConicToFunc_4(IntPtr control, IntPtr to, IntPtr user);
        delegate int CubicToFunc_4(IntPtr control, IntPtr control2, IntPtr to, IntPtr user);

        // EVEN IF SAME
        delegate int MoveToFunc_8(IntPtr to, IntPtr user);
        delegate int LineToFunc_8(IntPtr to, IntPtr user);
        delegate int ConicToFunc_8(IntPtr control, IntPtr to, IntPtr user);
        delegate int CubicToFunc_8(IntPtr control, IntPtr control2, IntPtr to, IntPtr user);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Outline_Decompose",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Outline_Decompose_4(
            ref FT_Outline face, 
            ref FT_Outline_Funcs_4 func_interface,
            IntPtr user);

        [DllImport(FontModule.FreetypeDll,
            EntryPoint = "FT_Outline_Decompose",
            CallingConvention = FontModule.CallConvention)]
        static extern FT_Error FT_Outline_Decompose_8(
            ref FT_Outline face, 
            ref FT_Outline_Funcs_8 func_interface,
            IntPtr user);

        fd_Outline fd_outline_decompose(FT_Outline src)
        {
            var o = new fd_Outline();

            if (Form == LongForm.Long4)
            {
                var bbox4 = new FT_BBox4();
                var err = FT_Outline_Get_BBox_4(ref src, ref bbox4);
                if (err != FT_Error.Ok)
                {
                    throw new InvalidOperationException("FT_Outline_Get_BBox_4 failed");
                }

                o.min_x = (float)bbox4.xMin / 64.0f;
                o.min_y = (float)bbox4.yMin / 64.0f;
                o.max_x = (float)bbox4.xMax / 64.0f;
                o.max_y = (float)bbox4.yMax / 64.0f;

                var funcs = new FT_Outline_Funcs_4
                {
                    move_to = Marshal.GetFunctionPointerForDelegate<MoveToFunc_4>(o.MoveTo_4),
                    line_to = Marshal.GetFunctionPointerForDelegate<LineToFunc_4>(o.LineTo_4),
                    conic_to = Marshal.GetFunctionPointerForDelegate<ConicToFunc_4>(o.ConicTo_4),
                    cubic_to = Marshal.GetFunctionPointerForDelegate<CubicToFunc_4>(o.CubicTo_4),
                };

                err = FT_Outline_Decompose_4(ref src, ref funcs, IntPtr.Zero);
                if (err != FT_Error.Ok)
                {
                    throw new InvalidOperationException("FT_Outline_Decompose_4 failed");
                }

            }
            else
            {
                var bbox8 = new FT_BBox8();
                var err = FT_Outline_Get_BBox_8(ref src, ref bbox8);
                if (err != FT_Error.Ok)
                {
                    throw new InvalidOperationException("FT_Outline_Get_BBox_8 failed");
                }

                o.min_x = (float)bbox8.xMin / 64.0f;
                o.min_y = (float)bbox8.yMin / 64.0f;
                o.max_x = (float)bbox8.xMax / 64.0f;
                o.max_y = (float)bbox8.yMax / 64.0f;

                var funcs = new FT_Outline_Funcs_8
                {
                    move_to = Marshal.GetFunctionPointerForDelegate<MoveToFunc_8>(o.MoveTo_8),
                    line_to = Marshal.GetFunctionPointerForDelegate<LineToFunc_8>(o.LineTo_8),
                    conic_to = Marshal.GetFunctionPointerForDelegate<ConicToFunc_8>(o.ConicTo_8),
                    cubic_to = Marshal.GetFunctionPointerForDelegate<CubicToFunc_8>(o.CubicTo_8),
                };

                err = FT_Outline_Decompose_8(ref src, ref funcs, o);

                if (err != FT_Error.Ok)
                {
                    throw new InvalidOperationException("FT_Outline_Decompose_4 failed");
                }
            }

            if (o.num_of_contours > 0)
            {
                o.contours[o.num_of_contours - 1].end = (uint)o.num_of_points - 1;
            }

            return o;
        }

        public void GetGlyphOutline(char c)
        {
            if (mHandle == IntPtr.Zero)
                return;

            if (Form == LongForm.Long4)
            {
                FT_FaceRec4 face4 = Marshal.PtrToStructure<FT_FaceRec4>(mHandle);
                // OR use OffsetOf() and ReadPointer()
                var glyphData = Marshal.PtrToStructure<FT_GlyphSlotRec_4>(face4.glyph);
                var outline = glyphData.outline;

                var output = fd_outline_convert(outline, c);
            }
            else // if (Form == LongForm.Long8)
            {
                var face8= Marshal.PtrToStructure<FT_FaceRec8>(mHandle);
                var glyphPtr8 = face8.glyph;
                var glyphData = Marshal.PtrToStructure<FT_GlyphSlotRec_8>(face8.glyph);
                var outline = glyphData.outline;

                var output = fd_outline_convert(outline, c);
            }
        }

        #endregion
    }
}
