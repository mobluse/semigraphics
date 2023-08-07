![ZX81](https://pbs.twimg.com/media/DVAPoNsVwAAfFRv.jpg:large)
# Semigraphics
Functions for semigraphics similar to ABC80 (setdot, clrdot, dot), ABC800 (txpoint), TRS-80 (set, reset, point), TeleText, 
Text-TV, TeleData, and Videotex using 3x2 blocks, and ZX81 (plot, unplot) using 2x2 blocks.

A game that uses this semigraphics library is [Convoy Bomb](https://github.com/mobluse/convoy-bomb).

This 3x2 (AKA 2x3) graphics depends on a font in the terminal emulator: teletext2 or teletext4 from
https://github.com/peterkvt80/Muttlee/tree/master/public/assets or Bedstead from http://bjh21.me.uk/bedstead/. Unfortunately 
the 64 characters needed for TeleText graphics are on non standard codepoints in Bedstead, and in Unicode these are used for some Chinese 
characters. Now there are [Symbols for Legacy Computing](https://en.wikipedia.org/wiki/Symbols_for_Legacy_Computing) in the new Unicode standard
and these also exist in the new versions of DejaVu Sans Mono and Monospace Regular and these have the 64 characters for TeleText graphics.

Video: https://youtu.be/wVY0dhnFev8

Support for ZX80 and ZX81 (plot, unplot) using 2x2 blocks, but these have standard Unicode codepoints and exists in e.g. DejaVu Sans Mono,
MS Gothic, NNimSun (almost), and SimSun-ExtB (almost). Unfortunately the 16 characters are not in teletext2.ttf or teletext4.ttf. 
I have tested various ZX81 and ZX Spectrum fonts, but none I found worked in the console since they were not monospaced. 
ZX81 also had characters with gray, but they can be emulated with the others since one can set forground and background color for 
each position, but they are also supported in Symbols for Legacy Computing.

![ABC800](https://pbs.twimg.com/media/DU0-hfFX0AIe_LM.jpg:large)
