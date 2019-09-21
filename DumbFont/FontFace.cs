using System;
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
    }
}
