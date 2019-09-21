using DumbFont;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    class Program
    {
        static void FT_CHECK(FT_Error check)
        {

        }

        struct fd_Render
        {
            internal fd_Outline[] outlines;
            internal fd_HostGlyphInfo[] glyph_infos;
        }

        struct fd_Outline
        {

        }

        struct fd_HostGlyphInfo
        {

        }


        const uint NUMBER_OF_GLYPHS = 96;

        static void load_font(FontLibrary library, string path, LongForm form)
        {
            var face = new FontFace(form);
            FT_CHECK(face.NewFace(library, path, 0));

            FT_CHECK(face.SetCharSize(
                form.FromInt32(0),
                form.FromInt32(1000),
                96,
                96));

            uint total_points = 0;
            uint total_cells = 0;

            for (uint i = 0; i< NUMBER_OF_GLYPHS; i++)
            {
                char c = Convert.ToChar(Convert.ToInt32(' ') + i);
                //Console.WriteLine("{0}", c);

                //fd_Outline o = r.outlines[i];
                //fd_HostGlyphInfo hgi = r.glyph_infos[i];

                var glyph_index = face.GetCharIndex(c);
                FT_CHECK(face.LoadGlyph(glyph_index, LoadFlags.NoHinting, LoadTarget.Default));

                //fd_outline_convert(&face->glyph->outline, o, c);

                //hgi->bbox = o->bbox;
                //hgi->advance = face->glyph->metrics.horiAdvance / 64.0f;


                //total_points += o->num_of_points;
                //total_cells += o->cell_count_x* o->cell_count_y;
            }

            //r->glyph_info_size = sizeof(fd_DeviceGlyphInfo) * NUMBER_OF_GLYPHS;
            //r->glyph_cells_size = sizeof(uint32_t) * total_cells;
            //r->glyph_points_size = sizeof(vec2) * total_points;

            //uint32_t alignment = r->device_properties.limits.minStorageBufferOffsetAlignment;
            //r->glyph_info_offset = 0;
            //r->glyph_cells_offset = align_uint32(r->glyph_info_size, alignment);
            //r->glyph_points_offset = align_uint32(r->glyph_info_size + r->glyph_cells_size, alignment);
            //r->glyph_data_size = r->glyph_points_offset + r->glyph_points_size;

            //r->glyph_data = malloc(r->glyph_data_size);

            //fd_DeviceGlyphInfo* device_glyph_infos = (fd_DeviceGlyphInfo*)((char*)r->glyph_data + r->glyph_info_offset);
            //uint32_t* cells = (uint32_t*)((char*)r->glyph_data + r->glyph_cells_offset);
            //vec2* points = (vec2*)((char*)r->glyph_data + r->glyph_points_offset);

            UInt32 point_offset = 0;
            UInt32 cell_offset = 0;

            //for (uint32_t i = 0; i<NUMBER_OF_GLYPHS; i++)
            //{
            //    fd_Outline* o = &r->outlines[i];
            //fd_DeviceGlyphInfo* dgi = &device_glyph_infos[i];

            //dgi->cell_info.cell_count_x = o->cell_count_x;
            //    dgi->cell_info.cell_count_y = o->cell_count_y;
            //    dgi->cell_info.point_offset = point_offset;
            //    dgi->cell_info.cell_offset = cell_offset;
            //    dgi->bbox = o->bbox;

            //    uint32_t cell_count = o->cell_count_x * o->cell_count_y;
            //memcpy(cells + cell_offset, o->cells, sizeof(uint32_t) * cell_count);
            //memcpy(points + point_offset, o->points, sizeof(vec2) * o->num_of_points);

            ////fd_outline_u16_points(o, &dgi->cbox, points + point_offset);

            //point_offset += o->num_of_points;
            //    cell_offset += cell_count;
            //}

            Debug.Assert(point_offset == total_points);
            Debug.Assert(cell_offset == total_cells);

            //for (uint i = 0; i<NUMBER_OF_GLYPHS; i++)
            //    fd_outline_destroy(&r->outlines[i]);

            FT_CHECK(face.DoneFace());

            //Console.WriteLine("\n");
            //Console.WriteLine("Avarage glyph size: {0} bytes", dst.glyph_data_size / NUMBER_OF_GLYPHS);
            //Console.WriteLine("    points size: {0} bytes", dst.glyph_points_size / NUMBER_OF_GLYPHS);
            //Console.WriteLine("    cells size: {0} bytes", dst.glyph_cells_size / NUMBER_OF_GLYPHS);
        }

        static void Main(string[] args)
        {
            try
            {
                var library = new DumbFont.FontLibrary();
                FT_CHECK(library.Init_FreeType());
                var longForm = DetrimineLongForm();
                load_font(library, "", longForm);
                FT_CHECK(library.Done_FreeType());
            }
            catch(Exception e)
            {

                var foregroundColor  = Console.ForegroundColor;
                var backgroundColor = Console.BackgroundColor;

                var exceptionType = e.GetType();
                if (exceptionType == typeof(BadImageFormatException))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                Console.BackgroundColor = ConsoleColor.Cyan;
                Console.WriteLine(exceptionType);
                Console.BackgroundColor = backgroundColor;
                Console.ForegroundColor = foregroundColor;

                Console.WriteLine(e);
            }
        }

        private static LongForm DetrimineLongForm()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LongForm.Long4;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LongForm.Long8;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (IntPtr.Size == 4)
                {
                    return LongForm.Long4;
                }
                else
                {
                    return LongForm.Long8;
                }
            }
            throw new PlatformNotSupportedException();
        }
    }
}
