using DumbFont;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{

    class Program
    {
        static void FT_CHECK(FT_Error check)
        {
            if (check != FT_Error.Ok)
            {
                throw new InvalidOperationException(check.ToString());
            }
        }

        static uint align_uint32(uint value, uint alignment)
        {
            return (value + alignment - 1) / alignment * alignment;
        }

        const uint NUMBER_OF_GLYPHS = 96;

        static PackedFont load_font(FontLibrary library, string path, LongForm form, uint alignment)
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


            var glyph_infos = new fd_HostGlyphInfo[NUMBER_OF_GLYPHS];
           

            var outlines = new fd_Outline[NUMBER_OF_GLYPHS];
            for (uint i = 0; i < NUMBER_OF_GLYPHS; i++)
            {
                char c = Convert.ToChar(Convert.ToInt32(' ') + i);
                Console.WriteLine("{0}", c);

                //fd_Outline o = r.outlines[i];
                //fd_HostGlyphInfo hgi = r.glyph_infos[i];

                var glyph_index = face.GetCharIndex(c);
                FT_CHECK(face.LoadGlyph(glyph_index, LoadFlags.NoHinting, LoadTarget.Default));

                var g = face.GetGlyph(c);
                outlines[i] = g.Outline;
                glyph_infos[i] = new fd_HostGlyphInfo
                {
                    bbox = outlines[i].bbox,
                    advance = g.Advance,
                };                

                total_points += outlines[i].num_of_points;
                total_cells += outlines[i].cell_count_x * outlines[i].cell_count_y;                
            }

            uint glyph_info_size = (UInt32)Marshal.SizeOf<fd_DeviceGlyphInfo>() * NUMBER_OF_GLYPHS;
            uint glyph_cells_size = sizeof(UInt32) * total_cells;

            var result = new PackedFont
            {                
                no_of_glyphs = NUMBER_OF_GLYPHS,
                glyph_infos = glyph_infos,
                glyph_info_size = glyph_info_size,
                glyph_cells_size = glyph_cells_size,
                glyph_points_size = (UInt32)Marshal.SizeOf<Vector2>() * total_points,

                alignment = alignment,
                glyph_info_offset = 0U,
                glyph_cells_offset = align_uint32(glyph_info_size, alignment),
                glyph_points_offset = align_uint32(glyph_info_size + glyph_cells_size, alignment),
            };

           // UInt32 alignment = r->device_properties.limits.minStorageBufferOffsetAlignment; 
            result.glyph_data_size = align_uint32(result.glyph_points_offset + result.glyph_points_size, alignment);
            result.glyphData = new byte[result.glyph_data_size];

            Span<byte> dstBuffer = result.glyphData;

            var dstGlyphInfo = MemoryMarshal.Cast<byte, fd_DeviceGlyphInfo>(
                dstBuffer.Slice((int) result.glyph_info_offset, (int) result.glyph_info_size)
            );
            var dstCells = MemoryMarshal.Cast<byte, uint>(
                dstBuffer.Slice((int) result.glyph_cells_offset, (int) result.glyph_cells_size)
                );
            var dstPoints = MemoryMarshal.Cast<byte, Vector2>(
                dstBuffer.Slice((int) result.glyph_points_offset, (int) result.glyph_points_size)
            );

            UInt32 point_offset = 0;
            UInt32 cell_offset = 0;

            for (var i = 0; i < NUMBER_OF_GLYPHS; i++)
            {
                dstGlyphInfo[i] = new fd_DeviceGlyphInfo
                { 
                    bbox = outlines[i].bbox,
                    cell_info = new fd_CellInfo
                    {
                         cell_count_x = outlines[i].cell_count_x,
                         cell_count_y = outlines[i].cell_count_y,
                         point_offset = point_offset,
                         cell_offset = cell_offset,
                    }
                };

                uint cell_count = outlines[i].cell_count_x * outlines[i].cell_count_y;
                // memcpy(dstCells + cell_offset, o->cells, sizeof(uint32_t) * cell_count);
                {
                    var dst = dstCells.Slice((int) cell_offset, (int)cell_count);
                    Span<uint> src = outlines[i].cells;
                    src.CopyTo(dst);
                }

                // memcpy(dstPoints + point_offset, o->points, sizeof(vec2) * o->num_of_points);
                {
                    var num_of_points = (int)outlines[i].num_of_points;
                    var dst = dstPoints.Slice((int) point_offset, num_of_points);
                    for (var j = 0; j < num_of_points; j += 1)
                    {
                        dst[j] = outlines[i].points[j];
                    }
                }

                point_offset += outlines[i].num_of_points;
                cell_offset += cell_count;
            }

            Debug.Assert(point_offset == total_points);
            Debug.Assert(cell_offset == total_cells);

            for (uint i = 0; i < NUMBER_OF_GLYPHS; i++)
            {
                outlines[i].fd_outline_destroy();
            }

            FT_CHECK(face.DoneFace());

            return result;
        }

        static void Main(string[] args)
        {
            try
            {
                var library = new DumbFont.FontLibrary();
                FT_CHECK(library.Init_FreeType());
                var longForm = DetrimineLongForm();
                var packed = load_font(library, "Noto Sans 700.ttf", longForm, 4U);

                Console.WriteLine("Average glyph size: {0} bytes", packed.glyph_data_size / packed.no_of_glyphs);
                Console.WriteLine("    points size: {0} bytes", packed.glyph_points_size / packed.no_of_glyphs);
                Console.WriteLine("    cells size: {0} bytes", packed.glyph_cells_size / packed.no_of_glyphs);


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
