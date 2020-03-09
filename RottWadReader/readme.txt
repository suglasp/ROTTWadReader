
-------------------------------
 ROTT2D WAD Reader Readme file
-------------------------------

project by Pieter De Ridder
web: www.rott2d.net
Distributed under the gpl 3.0 license.
Project started 12-2011. First initial basic version on 10-01-2012.
Last updated on 08-12-2013
Version : v3m

Introduction:
==================
This is the "ROTT2D WAD Reader"-tool.
With this tool, you can read (most-) data inside one of the original ROTT WAD files.


WAD file testing:
===================
I tested this tool with the following ROTT WAD files:
- huntbgin.wad from the shareware v1.0
- huntbgin.wad from the shareware v1.3
- darkwar.wad  from full version v1.3 (GOG.Com edition)


About WAD files:
===================
There is one WAD file included in this project under \data\wad\. This WAD file is huntbgin.wad from ROTT shareware v1.3.
If you want a darkwar.wad file, order a full copy of ROTT through www.gog.com or copy it from your original ROTT CD-ROM.
You can open a ROTT wad file from any location, by clicking on the "..." button and browse to the correct location.

Notice that you can only ready ROTT WAD files, Doom or other game related WAD files can't be read.


About the C# code:
===================
There are a few lump types i did't spend much time on to reverse engineer.
Since ROTT2D will not use these lump types, i didn't spend time on these lumps types.

Types left to be decoded:
- First of all, ROTT's WAD files contains fonts.
All fonts in ROTT are stored in the early aplha Doom font format.
In short, you could say a ROTT font is an array of small pictures that represent a ASCII character set.

- Secondly, ROTT also uses a format called LBM. This is the Amiga IFF/EA 1985 standard format,
and is written with RLE compression. ROTT uses this for exporting screenshots and also
(i suppose) in the main save/load screen the small thumbnail images where you last saved the game.
I did a little research on this format since i first figured that some lumps where
stored in this format. But later they appeared to be an other format.

You can always take a look in ROTT's "lumpy.h" c-header source code file for some interesting
leads on all lump data types.

Those i don't use have following header structs (copy from the file "lumpy.h"):
//small fonts
typedef struct
{
   short height;
   char  width[256];
   short charofs[256];
   byte  data;       // as much as required
} font_t;

//large fonts
typedef struct
{
   byte  color;
   short height;
   char  width[256];
   short charofs[256];
   byte  pal[0x300];
   byte  data;       // as much as required
} cfont_t;

//did some research on this LBM data type
//it uses it's own build-in 256 color palette and not the WAD file colormap
typedef struct
{
   short width;
   short height;
   byte palette[768];
   byte data;
} lbm_t;



Not related to ROTT WAD files:
================================
- RTL/RTC files contain all de game map's. You could say that one such file can act as an episode.

RTL = Single player levels
RTC = Comm-Bat (Multiplayer) : exit arches act as teleports

Both files are "compressed" with a default RLE compression method.

The maximum number of maps (or levels) in one RT(C/L) file is 100.

I've created a tool in C# (like this one) to read and decompress the RT(C/L) files.
But in order to render or generate a visually map of ROTT, it still needs some work.




Other references for reading:
================================
- Original ROTT source code (ftp.3drealms.com/source/rottsource.zip)
- The ROTTen Editor (www.3drealms.com/stuff/rotten.zip)
- "Hacker.txt" file from Apogee (included in the Extreme ROTT package from www.3drealms.com/rott)
- WinROTT (www.riseofthetriad.dk)
- Original Doom source code (ftp.idsoftware.com)
- Open Source Project DoomBuilder (www.doombuilder.com)
- Doom Unofficial Specs (aiforge.net/test/wadview/dmspec16.txt)


Many Thanks to:
=================
- Birger N. A. from the WinROTT project (www.riseofthetriad.dk)
    -> For reference help on the TransMasked Textures


This Project LICENSE:
======================
See gpl-3.0.txt for the GNU GENERAL PUBLIC LICENSE

This file is part of ROTT2D.

Notes:
All C# written source code in this project was generated with Visual Studio,
or self written by Pieter. None of the C# source code in this project is copy/pasted
from other authors or programmers. Primary download source can be found on www.rott2d.net.

ROTT2D is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

ROTT2D is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with ROTT2D.  If not, see <http://www.gnu.org/licenses/>.

 