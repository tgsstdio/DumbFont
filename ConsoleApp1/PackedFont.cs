using DumbFont;

namespace ConsoleApp1
{
    class PackedFont
    {
        internal uint glyph_info_size;
        internal uint glyph_cells_size;
        internal uint glyph_points_size;
        internal uint glyph_info_offset;
        internal uint glyph_cells_offset;
        internal uint glyph_points_offset;
        internal uint glyph_data_size;
        internal byte[] glyphData;
        internal fd_HostGlyphInfo[] glyph_infos;
        internal uint alignment;
        internal uint no_of_glyphs;
    }
}
