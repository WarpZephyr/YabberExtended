# YabberExtended
An unpacker/repacker for common Demon's Souls, Dark Souls 1-3, Bloodborne, and Sekiro file formats. Supports .bnd, .bhd/.bdt, .dcx, .fltparam, .fmg, .gparam, .luagnl, .luainfo, and .tpf.  
In order to decompress Sekiro files you must copy oo2core_6_win64.dll from Sekiro into Yabber's lib folder.  
Does not support dvdbnds (the very large bhd/bdt pairs in the main game directory); use [UDSFM](https://www.nexusmods.com/darksouls/mods/1304) or [UXM](https://www.nexusmods.com/sekiro/mods/26) to unpack those first.  
Also does not support encrypted files (enc_regulation.bnd.dcx in DS2, Data0.bdt in DS3); you can edit these with [Yapped](https://www.nexusmods.com/darksouls3/mods/306) or unpack them with [BinderTool](https://github.com/Atvaark/BinderTool).  
Requires [.NET 4.7.2](https://www.microsoft.com/net/download/thank-you/net472) for versions older than 1.1.0 - Windows 10 users should already have this.  
Requires [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) for versions 1.1.0 and up - .NET Runtime is required at a minimum.

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

### 1.0.1
* Stopped BND3 writing FileHeadersEnd

### 1.0.2
* New changes adding many BND formats

### 1.0.3
* Improved FMG support

### 1.0.4
* Fixed BND3 writing extra field by mistake in SoulsFormatsExtended
* Fixed BND3 using BigEndian and not BitBigEndian field for flags when writing format
	
### 1.0.5
* Added LDMU support
* Added basic FSDATA support

### 1.0.6
* Added FSDATA repack support

### 1.0.7
* Fixed TPF padding issues, mainly aimed at Armored Core Verdict Day for now.

### 1.0.8
* Improved BND2 support
* Added AC4 and ACFA AcParts unpacking support

### 1.1.0
* Upgraded to .NET 8.0
* Added many text and XML class extensions for easy numeric data parsing
* Majorly reorganized some files
* Made minor changes to some BND classes
* Fixed many AcParts4 unpack issues
* Added AcParts4 repacking support

### 1.1.1
* Added write file headers end option to BND3
* Fixed BND KUON repack error, fixed BND KUON not writing correct version number during repack

### 1.1.2
* Made a minor change to BND KUON padding on repack.

### 1.1.3
* Updated Zero3 unpacking.
* Added DDL unpacking.

### 1.1.4
* Added MLB unpack and repack support for AC4 and ACFA US PS3.

### 1.1.5
* Added MLB unpack and repack support for ACV and ACVD US PS3.

### 1.1.6
* Fixed an issue for MQB repack where colors were not parsed correctly.

### 1.1.7
* Fixed an issue in SoulsFormatsExtended where BND2 repack was not using AlignmentSize.

### 1.1.8
* Added reuse offsets option to FMG repacking.

### 1.1.9
* Improved Zero3 unpack support.
* Added Zero3 repack support.

### 1.2.0
* Fixed minor oversight in Zero3 repack.

### 1.2.1
* Fixed an issue in SoulsFormatsExtended for DCX EDGE compression.