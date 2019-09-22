# DumbFont

## Version 2.10
.net core [FreeType][6] interop demo using FreeType's [official Windows builds][5] (>= 2.10)

## Credits 

[SharpFont][4]

Utility classes taken from library.  

## Developer Note

#### Handling "long" with Interops duplication

Interops structs & external bindings are duplicating to handle C ___long___ data type (either 4 or 8 bytes) varies based on OS and CPU used throughout FreeType.

| OS      | CPU    | sizeof(long) | 
|---      | -----  | -----------  |
|  Windows| IA-32       | 4 bytes
|         | Intel® 64	 | 4 bytes
|  Linux  | IA-32 	     | 4 bytes
|         | Intel® 64	 | 8 bytes
|  mac OS | Intel® 64	 | 8 bytes

- Therefore on Windows for Intel, ___long___ are always 4 bytes
- Linux (for Intel CPUs) ___long___ are either 4 or 8 bytes
- On MacOS, ___long___ is always 8 bytes

### Links

- See [SharpFont.Dependencies][3]'s custom FreeType [Win64 binaries][1] with [custom patch][2]
- See ["Size of 'long integer' data type (C++) on various architectures and OS"][7]

[1]: https://github.com/Robmaister/SharpFont.Dependencies/blob/master/freetype2/README.md
[2]: https://github.com/tgsstdio/DumbFont/blob/master/win64.patch
[3]: https://github.com/Robmaister/SharpFont.Dependencies
[4]: https://github.com/Robmaister/SharpFont
[5]: https://github.com/ubawurinna/freetype-windows-binaries
[6]: https://www.freetype.org/
[7]: https://software.intel.com/en-us/articles/size-of-long-integer-type-on-different-architecture-and-os