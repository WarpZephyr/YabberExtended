
--| YabberExtended 1.0.3
--| By WarpZephyr
--| https://github.com/WarpZephyr/YabberExtended
--| 
--| Forked from TKGP's Yabber:
--| https://github.com/JKAnderson/Yabber

An unpacker/repacker for various common FromSoftware game formats. Supports .bnd, .bhd/.bdt, .dcx, .fltparam, .fmg, .gparam, .luagnl, .luainfo, .tpf, and .mqb.
In order to decompress Sekiro files you must copy oo2core_6_win64.dll from Sekiro into YabberExtended's lib folder.
Does not support dvdbnds (the very large bhd/bdt pairs in the main game directory); use UDSFM or UXM to unpack those first.
https://www.nexusmods.com/darksouls/mods/1304
https://www.nexusmods.com/sekiro/mods/26
Also does not support encrypted files (enc_regulation.bnd.dcx in DS2, Data0.bdt in DS3); you can edit these with Yapped or unpack them with BinderTool.
https://www.nexusmods.com/darksouls3/mods/306
https://github.com/Atvaark/BinderTool
Requires .NET 4.7.2 - Windows 10 users should already have this.
https://www.microsoft.com/net/download/thank-you/net472


--| YabberExtended.exe

This program is for unpacking and repacking supported formats. Drag and drop a file (bnd, bhd, fmg, gparam, luagnl, luainfo, or tpf) onto the exe to unpack it; drag and drop an unpacked folder to repack it. Multiple files or folders can be selected and dropped at a time.
DCX versions of supported formats can be dropped directly onto YabberExtended.exe without decompressing them separately; they will automatically be recompressed when repacking.
Edit the .xml file in the unpacked folder to add, remove or rename files before repacking.
Non-container files such as FMG or GPARAM are simply extracted to an xml file with the same name. Drop the .xml back onto YabberExtended to repack it.


--| YabberExtended.DCX.exe

This program is for decompressing and recompressing any DCX file. Drag and drop a DCX file onto the exe to decompress it; drag and drop the decompressed file to recompress it. Multiple files can be selected and dropped at a time.
You don't need to use this to decompress container formats before dropping them on YabberExtended.exe; this is only for compressed formats that aren't otherwise supported by YabberExtended.


--| YabberExtended.Context.exe

This program registers the other two so that they can be run by right-clicking on a file or folder. Run it to choose whether to register or unregister them.
The other two programs are assumed to be in the same folder. If you move them, just run it again from the new location.


--| Formats

BND2
Extensions: .bnd, .BND
A generic file container used in many later FromSoftware games in the PS2 and PSP era.

BND3
Extension: .*bnd
A generic file container used before DS2. DS1 is fully supported; DeS is mostly supported.

BND4
Extension: .*bnd
A generic file container used since DS2.

BXF3
Extensions: .*bhd, .*bdt
A generic file container split into a header and data file, used before DS2. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

BXF4
Extensions: .*bhd, .*bdt
A generic file container split into a header and data file, used since DS2. Only drag-and-drop the .bhd to unpack it; the .bdt is assumed to be in the same directory.

DCX
Extension: .dcx
A single compressed file, used in all games.

FMG
Extension: .fmg
A collection of text strings with an associated ID number, used in all games. %null% is a special keyword indicating an ID that is present but has no text.

GPARAM
Extension: .fltparam, .gparam
A graphical configuration format used since DS2.

LUAGNL/LUAINFO
Extension: .luagnl/.luainfo
Lua scripting support files used in all games except DS2.

TPF
Extension: .tpf
A DDS texture container, used in all games. Console versions are somewhat supported.

MQB
A cutscene format which name stands for "MenuSeQuencerBinary".
MQB dates back to at least Armored Core V, however Armored Core V and Armored Core Verdict Day MQB are not yet supported.

--| Contributors

katalash - GPARAM support on Yabber
TKGP - Original Yabber
WarpZephyr - MQB Support and various misc Armored Core Formats.


--| Changelog

1.0.0
	YabberExtended is released.

1.0.1
	Stopped BND3 writing FileHeadersEnd

1.0.2
	New changes adding many BND formats

1.0.3
	Improved FMG support