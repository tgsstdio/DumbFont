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
    /// <summary>
    ///  Handle "long" with Interops duplication
    ///  https://software.intel.com/en-us/articles/size-of-long-integer-type-on-different-architecture-and-os
    ///  
    /// OS	   | Architecture| Size of "long" type
    //  Windows| IA-32       | 4 bytes
    //         | Intel® 64	 | 4 bytes
    //  Linux  | IA-32 	     | 4 bytes
    //         | Intel® 64	 | 8 bytes
    //  mac OS | Intel® 64	 | 8 bytes
    /// 
    ///  Therefore long on Windows for Intel is always 4 bytes
    ///  Linux (for Intel CPUs) is either 4 or 8 bytes
    ///  MacOS is always 8 bytes
    /// </summary>

    public enum LongForm
    {
        Long8,
        Long4
    }
}
