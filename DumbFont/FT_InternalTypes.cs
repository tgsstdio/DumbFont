using System;
using System.Runtime.InteropServices;

namespace DumbFont
{
    // typedef signed long  FT_Long;
    using FT_Long4 = System.Int32;
    using FT_Long8 = System.Int64;
    // typedef signed long FT_Pos;
    using FT_Pos4 = System.Int32;
    using FT_Pos8 = System.Int64;
    //   typedef signed long  FT_Fixed;
    using FT_Fixed4 = System.Int32;
    using FT_Fixed8 = System.Int64;

    // typedef signed long  FT_F26Dot6;
    using FT_F26Dot6_4 = System.Int32;
    using FT_F26Dot6_8 = System.Int64;

    // typedef signed char  FT_Char; (FT_Char*)
    using FT_String = System.IntPtr;

    // typedef signed int  FT_Int;
    using FT_Int = System.Int32;
    // typedef unsigned int  FT_UInt;
    using FT_UInt = System.UInt32;

    // typedef unsigned short  FT_UShort;
    using FT_UShort = System.UInt16;

    // typedef signed short  FT_Short;
    using FT_Short = System.Int16;

    using FT_Int32 = System.Int32;

    //  typedef unsigned char  FT_Bool;
    using FT_Bool = System.Byte;

    using FT_ListNode = System.IntPtr;

    using FT_ULong4 = System.UInt32;
    using FT_ULong8 = System.UInt64;

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Bitmap_Size4
    {
        FT_Short height;
        FT_Short width;

        FT_Pos4 size;

        FT_Pos4 x_ppem;
        FT_Pos4 y_ppem;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Bitmap_Size8
    {
        FT_Short height;
        FT_Short width;

        FT_Pos8 size;

        FT_Pos8 x_ppem;
        FT_Pos8 y_ppem;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_BBox4
    {
        FT_Pos4 xMin;
        FT_Pos4 yMin;
        FT_Pos4 xMax;
        FT_Pos4 yMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_BBox8
    {
        FT_Pos8 xMin;
        FT_Pos8 yMin;
        FT_Pos8 xMax;
        FT_Pos8 yMax;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_CharMap
    {
        IntPtr face; // (FT_FaceRec4 *)
        FT_Encoding encoding; // UInt32 
        FT_UShort platform_id;
        FT_UShort encoding_id;
    }

    // typedef void (* FT_Generic_Finalizer) (void*  object );
    delegate void FT_Generic_Finalizer(IntPtr @object);

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Generic
    {
        IntPtr data;
        IntPtr finalizer; // FT_Generic_Finalizer
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_FaceRec4
    {
        FT_Long4 num_faces;
        FT_Long4 face_index;

        FT_Long4 face_flags;
        FT_Long4 style_flags;

        FT_Long4 num_glyphs;

        FT_String family_name;
        FT_String style_name;

        FT_Int num_fixed_sizes;
        IntPtr available_sizes; // (FT_Bitmap_Size[4|8] *)

        FT_Int num_charmaps;
        IntPtr charmaps; // FT_CharMap* => FT_CharMapRec_

        FT_Generic generic;

        /*# The following member variables (down to `underline_thickness`) */
        /*# are only relevant to scalable outlines; cf. @FT_Bitmap_Size    */
        /*# for bitmap fonts.                                              */
        FT_BBox4 bbox;

        FT_UShort units_per_EM;
        FT_Short ascender;
        FT_Short descender;
        FT_Short height;

        FT_Short max_advance_width;
        FT_Short max_advance_height;

        FT_Short underline_position;
        FT_Short underline_thickness;

        // typedef struct FT_GlyphSlotRec_*  FT_GlyphSlot;
        IntPtr glyph;
        //   typedef struct FT_SizeRec_*  FT_Size_[4|8];
        IntPtr size;
        FT_CharMap charmap;

        /*@private begin */


        IntPtr driver; // typedef struct FT_DriverRec_*  FT_Driver;
        IntPtr memory; //   typedef struct FT_MemoryRec_*  FT_Memory;
        IntPtr stream; //   typedef struct FT_StreamRec_*  FT_Stream;

        FT_ListRec sizes_list;

        FT_Generic autohint;   /* face-specific auto-hinter data */
        IntPtr extensions; /* unused   void*                      */

        IntPtr @internal; // typedef struct FT_Face_InternalRec_*  FT_Face_Internal
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_FaceRec8
    {
        FT_Long8 num_faces;
        FT_Long8 face_index;

        FT_Long8 face_flags;
        FT_Long8 style_flags;

        FT_Long8 num_glyphs;

        FT_String family_name;
        FT_String style_name;

        FT_Int num_fixed_sizes;
        IntPtr available_sizes; // (FT_Bitmap_Size[4|8] *)

        FT_Int num_charmaps;
        IntPtr charmaps; // FT_CharMap* => FT_CharMapRec_

        FT_Generic generic;

        /*# The following member variables (down to `underline_thickness`) */
        /*# are only relevant to scalable outlines; cf. @FT_Bitmap_Size    */
        /*# for bitmap fonts.                                              */
        FT_BBox8 bbox;

        FT_UShort units_per_EM;
        FT_Short ascender;
        FT_Short descender;
        FT_Short height;

        FT_Short max_advance_width;
        FT_Short max_advance_height;

        FT_Short underline_position;
        FT_Short underline_thickness;

        // typedef struct FT_GlyphSlotRec_*  FT_GlyphSlot;
        IntPtr glyph;
        //   typedef struct FT_SizeRec_*  FT_Size_[4|8];
        IntPtr size;
        FT_CharMap charmap;

        /*@private begin */


        IntPtr driver; // typedef struct FT_DriverRec_*  FT_Driver;
        IntPtr memory; //   typedef struct FT_MemoryRec_*  FT_Memory;
        IntPtr stream; //   typedef struct FT_StreamRec_*  FT_Stream;

        FT_ListRec sizes_list;

        FT_Generic autohint;   /* face-specific auto-hinter data */
        IntPtr extensions; /* unused   void*                      */

        IntPtr @internal; // typedef struct FT_Face_InternalRec_*  FT_Face_Internal
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_ListRec
    {
        FT_ListNode head;
        FT_ListNode tail;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_4
    {
        /* parent face object              */
        IntPtr face;
        /* generic pointer for client uses */
        FT_Generic generic;
        /* size metrics                    */
        FT_Size_Metrics_4 metrics;
        // typedef struct FT_Size_InternalRec_*  FT_Size_Internal;
        IntPtr @internal;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_8
    {
        /* parent face object              */
        IntPtr face;
        /* generic pointer for client uses */
        FT_Generic generic;
        /* size metrics                    */
        FT_Size_Metrics_8 metrics;
        // typedef struct FT_Size_InternalRec_*  FT_Size_Internal;
        IntPtr @internal;
    };

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_InternalRec_4
    {
        IntPtr module_data; // void*

        FT_RenderMode autohint_mode; // FT_Render_Mode
        FT_Size_Metrics_4 autohint_metrics;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_InternalRec_8
    {
        IntPtr module_data; // void*

        FT_RenderMode autohint_mode;
        FT_Size_Metrics_8 autohint_metrics;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_Metrics_4
    {
        /* horizontal pixels per EM               */
        FT_UShort x_ppem;
        /* vertical pixels per EM                 */
        FT_UShort y_ppem;

        /* scaling values used to convert font    */
        FT_Fixed4 x_scale;
        /* units to 26.6 fractional pixels        */
        FT_Fixed4 y_scale;

        /* ascender in 26.6 frac. pixels          */
        FT_Pos4 ascender;
        /* descender in 26.6 frac. pixels         */
        FT_Pos4 descender;
        /* text height in 26.6 frac. pixels       */
        FT_Pos4 height;
        /* max horizontal advance, in 26.6 pixels */
        FT_Pos4 max_advance;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Size_Metrics_8
    {
        /* horizontal pixels per EM               */
        FT_UShort x_ppem;
        /* vertical pixels per EM                 */
        FT_UShort y_ppem;

        /* scaling values used to convert font    */
        FT_Fixed8 x_scale;
        /* units to 26.6 fractional pixels        */
        FT_Fixed8 y_scale;

        /* ascender in 26.6 frac. pixels          */
        FT_Pos8 ascender;
        /* descender in 26.6 frac. pixels         */
        FT_Pos8 descender;
        /* text height in 26.6 frac. pixels       */
        FT_Pos8 height;
        /* max horizontal advance, in 26.6 pixels */
        FT_Pos8 max_advance;
    }


    [StructLayout(LayoutKind.Sequential)]
    struct FT_GlyphSlotRec_4
    {
        IntPtr library; // IntPtr
        IntPtr face; // FT_Face
                     // typedef struct FT_GlyphSlotRec_*  FT_GlyphSlot;
        IntPtr next;
        FT_UInt glyph_index; /* new in 2.10; was reserved previously */
        FT_Generic generic;

        FT_Glyph_Metrics_4 metrics;
        FT_Fixed4 linearHoriAdvance;
        FT_Fixed4 linearVertAdvance;
        FT_Vector4 advance;

        FT_GlyphFormat format; // Uint32

        FT_Bitmap bitmap;
        FT_Int bitmap_left;
        FT_Int bitmap_top;

        FT_Outline outline;

        FT_UInt num_subglyphs;
        IntPtr subglyphs; // FT_SubGlyph => FT_SubGlyphRec_[4|8]

        IntPtr control_data; // IntPtr
        FT_Long4 control_len; // long => FT_Long4

        FT_Pos4 lsb_delta;
        FT_Pos4 rsb_delta;

        IntPtr other; // void*

        //   typedef struct FT_Slot_InternalRec_*  FT_Slot_Internal;
        IntPtr @internal;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Glyph_Metrics_4
    {
        FT_Pos4 width;
        FT_Pos4 height;

        FT_Pos4 horiBearingX;
        FT_Pos4 horiBearingY;
        FT_Pos4 horiAdvance;

        FT_Pos4 vertBearingX;
        FT_Pos4 vertBearingY;
        FT_Pos4 vertAdvance;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Glyph_Metrics_8
    {
        FT_Pos8 width;
        FT_Pos8 height;

        FT_Pos8 horiBearingX;
        FT_Pos8 horiBearingY;
        FT_Pos8 horiAdvance;

        FT_Pos8 vertBearingX;
        FT_Pos8 vertBearingY;
        FT_Pos8 vertAdvance;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Vector4
    {
        FT_Pos4 x;
        FT_Pos4 y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Vector8
    {
        FT_Pos8 x;
        FT_Pos8 y;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Bitmap
    {
        uint rows; // unsigned int
        uint width; // unsigned int
        int pitch;
        IntPtr buffer; //  unsigned char*
        ushort num_grays; // unsigned short
        byte pixel_mode; // unsigned char
        byte palette_mode; // unsigned char
        IntPtr palette; // void*
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Outline
    {
        /* number of contours in glyph        */
        short n_contours;
        /* number of points in the glyph      */
        short n_points;

        /* the outline's points               */
        IntPtr points;          // FT_Vector*
        /* the points flags                   */
        IntPtr tags;             // char*
        /* the contour end points             */
        IntPtr contours;      // short*
        /* outline masks                      */
        int flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_SubGlyphRec_4
    {
        FT_Int index;
        FT_UShort flags;
        FT_Int arg1;
        FT_Int arg2;
        FT_Matrix_4 transform;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_SubGlyphRec_8
    {
        FT_Int index;
        FT_UShort flags;
        FT_Int arg1;
        FT_Int arg2;
        FT_Matrix_8 transform;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Matrix_4
    {
        FT_Fixed4 xx;
        FT_Fixed4 xy;
        FT_Fixed4 yx;
        FT_Fixed4 yy;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Matrix_8
    {
        FT_Fixed8 xx;
        FT_Fixed8 xy;
        FT_Fixed8 yx;
        FT_Fixed8 yy;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Slot_InternalRec_4
    {
        //   typedef struct  FT_GlyphLoaderRec_, ... , *FT_GlyphLoader;
        IntPtr loader;
        FT_UInt flags;
        FT_Bool glyph_transformed;
        FT_Matrix_4 glyph_matrix;
        FT_Vector4 glyph_delta;
        IntPtr glyph_hints; // void*

        FT_Int32 load_flags;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FT_Slot_InternalRec_8
    {
        //   typedef struct  FT_GlyphLoaderRec_, ... , *FT_GlyphLoader;
        IntPtr loader;
        FT_UInt flags;
        FT_Bool glyph_transformed;
        FT_Matrix_8 glyph_matrix;
        FT_Vector8 glyph_delta;
        IntPtr glyph_hints; // void*

        FT_Int32 load_flags;
    }
}
