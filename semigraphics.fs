( gforth in Ubuntu on WSL in Windows 10, but probably works in other vt100/xterm and OS. )
( It depends on font in terminal: teletext2 or teletext4 from https://github.com/peterkvt80/Muttlee/tree/master/public/assets . )
( Paste this in running gforth console. )

(  DECXCPR will change stack-out. )
: DECXCPR ( -- )
  BEGIN  stdin key?-file
  WHILE  stdin key-file drop
  REPEAT
  ESC[ ." 6n"
  BEGIN  stdin key-file dup [char] R <>
  WHILE  dup 27 =
  IF  drop
         ELSE   emit
         THEN
  REPEAT
  drop ;

: gemit ( gch -- )
  60960 + dup 60992 >
  IF     32 +
  THEN
  xemit ; ok

: block64 ( -- )
  64 0
  DO     i gemit 61103 xemit
  LOOP
  ; ok

: cur ( r c -- )
  swap at-xy ;

: string
  Create allot does> + ;

variable cols
80 cols !
variable rows
24 rows !

rows @ cols @ * string screen

: gpage ( -- )
  rows @ cols @ * 0
  DO     0 i screen c!
  LOOP
  ;

: grefresh ( -- )
  rows @ cols @ * 0
  DO     i screen c@ ?dup
         IF     i cols @ /mod at-xy gemit
         THEN
  LOOP
  0 0 at-xy ;

: dotcommon1 ( y x -- dx dy r c )
  2 /mod rot 3 /mod ( dx c dy r )
  rot ( dx dy r c ) ;

: dotcommon2 ( dx dy r c -- r c gch )
  2swap ( r c dx dy )
  2* + 1 swap lshift ( r c gch ) ;

: dotindex ( r c -- index )
  swap cols @ * + ( index ) ;

: dotcommon4 ( gch pos -- pos gch old )
  tuck ( pos gch pos )
  screen c@ ( pos gch old ) ;

: dotcommon5 ( pos new -- )
  tuck gemit ( new pos )
  screen c! ( ) ;

: setdot ( y x -- )
  dotcommon1
  2dup cur
  dotcommon2
  -rot ( gch r c )
  dotindex
  dotcommon4
  or ( pos new )
  dotcommon5 ;

: clrdot ( y x -- )
  dotcommon1
  2dup cur
  dotcommon2
  invert ( r c igch )
  -rot ( igch r c )
  dotindex
  dotcommon4
  and ( pos new )
  dotcommon5 ;

: dot ( y x -- f )
  dotcommon1
  dotcommon2
  -rot ( gch r c )
  dotindex
  screen c@ ( gch old )
  and 0> ( f ) ;

( Examples )
gpage page
15 4 setdot 16 5 setdot 17 6 setdot 18 7 setdot
16 5 clrdot 18 7 clrdot
page grefresh
15 4 dot . 16 5 dot . 17 6 dot . 18 7 dot .
999 999 cur decxcpr
