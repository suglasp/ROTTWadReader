ROTT WAD Reader
(ROTT = Apogee's 1994 Rise Of The Triad game) 

This is a utility written in .NET, for reading and exporting data from the ROTT WAD Files.
This utility is a graphical UI (WinForms) to represent textures (art), ASCII (text) and audio fragments from the game.
It can also export the textures to bitmap, the audio to original midi or wav files (wav files are converted from the original voc).

The behind the scenes is done with 3 libraries (dll's) that can read the wad files, a fast GDI+ drawing lib and a media lib.

ROTT has several versions (basically, 2 main releases):
- Shareware ROTT -> contains huntbgns.wad
- Registed ROTT  -> contains darkwar.wad

For the conversion to C#, or better the reverse engineering process, i did a lot of research and reading through:
- the original Apogee's DOS ROTT source code ( http://legacy.3drealms.com/rott/ )
- Birger's ROTT WinROTT port                 ( https://www.riseofthetriad.dk/ )
- Doom Source code                           ( https://github.com/id-Software/DOOM )
- Wolf3D Source code                         ( https://github.com/id-Software/wolf3d )
- Even some of the alpha Doom Source code found on the Internet.
- Doom Builder Source Code                   ( http://www.doombuilder.com/ )
- "rotthacker.txt"                           ( https://github.com/suglasp/ROTTWadReader/blob/master/RottWadReader/reading/hacker.txt )

You can find more about my reversing adventure in the code comments.

Books of interest:
- "Handbook of Data Compression" by David Salomon and Giovanni Motta ( Springer, ISBN 978-1848829022 )
- "Compressed Image File Formats: JPEG, PNG, GIF, XBM, BMP" by John Miano ( ACM Press, ISBN 978-0201604436 )
- "Masters Of Doom: How two guys created an empire and transformed pop culture" by David Kushner ( ISBN 978-0749924898 )


The idea of this tool was in the first place to create a 'ROTT 2D' game.
But after investing time in this utility, i was already statisfied to be able to reverse engineer most of the WAD file data lumps and convert the algorthims to C# code.

