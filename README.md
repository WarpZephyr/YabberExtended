# YabberExtended
An unpacker/repacker for common Demon's Souls, Dark Souls 1-3, Bloodborne, and Sekiro file formats. Supports .bnd, .bhd/.bdt, .dcx, .fltparam, .fmg, .gparam, .luagnl, .luainfo, and .tpf.  
In order to decompress Sekiro files you must copy oo2core_6_win64.dll from Sekiro into Yabber's lib folder.  
Does not support dvdbnds (the very large bhd/bdt pairs in the main game directory); use [UDSFM](https://www.nexusmods.com/darksouls/mods/1304) or [UXM](https://www.nexusmods.com/sekiro/mods/26) to unpack those first.  
Also does not support encrypted files (enc_regulation.bnd.dcx in DS2, Data0.bdt in DS3); you can edit these with [Yapped](https://www.nexusmods.com/darksouls3/mods/306) or unpack them with [BinderTool](https://github.com/Atvaark/BinderTool).  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) - Windows 10 users should already have this.  
[NexusMods Page](https://www.nexusmods.com/sekiro/mods/42)  

Please see the included readme for detailed instructions.  
Forked by WarpZephyr from original project by TKGP:  
https://github.com/JKAnderson/Yabber

# Contributors
*katalash* - GPARAM support on Yabber
*TKGP* - Original Yabber
*WarpZephyr* - MQB Support and various misc Armored Core Formats.

# Changelog
### 1.0.0
* YabberExtended is released.