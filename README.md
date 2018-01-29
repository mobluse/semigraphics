# Semigraphics
Functions for semigraphics similar to ABC80 (setdot, clrdot, dot), ABC800 (txpoint), TRS-80 (set, reset, point), TeleText, 
Text-TV, TeleData, and Videotex using 3x2 blocks.

This 3x2 (sometimes called 2x3) graphics depends on a font in the terminal emulator: teletext2 or teletext4 from
https://github.com/peterkvt80/Muttlee/tree/master/public/assets . Unfortunately the 64 characters needed for TeleText graphics are not 
part of Unicode (UTF8) and these fonts use codepoints for some Chinese characters. Maybe we could lobby for standard Unicode positions
for these TeleText semigraphics characters since they were used by many systems, and are still used.

Video: https://youtu.be/wVY0dhnFev8

I should also add support for ZX80 and ZX81 (plot, unplot) using 2x2 blocks, but these are standard Unicode characters. 
Unfortunately the 16 characters are not in teletext2.ttf or teletext4.ttf. ZX81 also had characters with gray, but they can be emulated
with the others since one can set forground and background color for each position.
