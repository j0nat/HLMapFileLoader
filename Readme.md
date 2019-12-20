# Half-Life .map File Loader

[Map-Files](https://github.com/stefanha/map-files) .map parser, written by Stefan Hajnoczi, partly ported to C#.

### Ported
* Parse .map file
* Turn brush into polygon
* Calculate texture coordinates
* Sort vertices

### Not ported
* CSG Union 
* CMF Export
* WAD Code (Uses MonoGame Content.Load<Texture2D>)

### Extra
* Uses MonoGame Framework (textures, vectors etc...)
* Inverse function to rotate output correctly

### Prerequisites
* Microsoft Visual Studio 2012+
* MonoGame 




**Check out the [example project](https://github.com/j0nat/HLMapFileLoader.Example)**
