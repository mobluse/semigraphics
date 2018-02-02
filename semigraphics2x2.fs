( Programming language: GNU Forth/gforth in Ubuntu on WSL for Windows 10, but probably works in other vt100/xterm and OS. )
( It depends on font in terminal: DejaVu Sans Mono. )
( Paste this in running gforth console. )

decimal

( To print answer: DECXCPR type )
: DECXCPR ( -- addr count )
  \ BEGIN  stdin key?-file
  \ WHILE  stdin key-file drop
  \ REPEAT
  ESC[ ." 6n"
  9 0 DO
  stdin key-file dup 
  pad i + c!
  [char] R =
  IF pad 2 + i 1- LEAVE THEN
  LOOP ;

: str2coords ( addr count -- r1 c1 )
  drop 0. rot 5 >number drop 1+ 0. rot 5 >number 2drop drop nip ;

( screensize is similar to form in gforth. )
: screensize ( -- r c )
  999 999 at-xy DECXCPR str2coords ;

: intarray create cells allot does> swap cells + ;

16 intarray codepoints

: codepoints! 16 0 do i codepoints ! loop ;

9608 9631 9625 9604 9628 9616 9626 9623 9627 9630 9612 9622 9600 9629 9624 8199 codepoints!

: gemit codepoints @ xemit ;

: semigraphics2x2 ( -- )
  9618 xemit 
  16 0 
  DO     i gemit 9618 xemit 
  LOOP ;

: cur ( r c -- )
  swap at-xy ;

: string
  Create allot does> + ;

( In future _cols and _rows will be set using DECXCPR. )
variable _cols
80 _cols !
variable _rows
25 _rows !
_rows @ _cols @ * string screen

: gpage ( -- )
  _rows @ _cols @ * 0
  DO     0 i screen c!
  LOOP
  ;

: grefresh ( -- )
  _rows @ _cols @ * 0
  DO     i screen c@ ?dup
         IF     i _cols @ /mod at-xy gemit
         THEN
  LOOP
  0 0 at-xy ;

: dotcommon1 ( y x -- dx dy r c )
  2 /mod rot 2 /mod ( dx c dy r )
  rot ( dx dy r c ) ;

: dotcommon2 ( dx dy r c -- r c gch )
  2swap ( r c dx dy )
  2* + 1 swap lshift ( r c gch ) ;

: dotindex ( r c -- index )
  swap _cols @ * + ( index ) ;

: dotcommon4 ( gch pos -- pos gch old )
  tuck ( pos gch pos )
  screen c@ ( pos gch old ) ;

: dotcommon5 ( pos new -- )
  tuck gemit ( new pos )
  screen c! ( ) ;

( ABC80 if 2x2 semigraphics )
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

( ZX81 )
: plot ( x y -- )
  _rows @ 2 - 2* swap - 1- swap
  setdot ;

: unplot ( x y -- )
  _rows @ 2 - 2* swap - 1- swap
  clrdot ;

: plotted ( x y -- f )
  _rows @ 2 - 2* swap - 1- swap
  dot ;

( Examples )

\ Described in http://www.abc80.net/archive/luxor/ABC80x/ABC806-dator-manual-BASIC-II.pdf .
: BLK ESC[ ." 22;30m" ;
: RED ESC[ ." 1;31m" ;
: GRN ESC[ ." 22;32m" ;
: YEL ESC[ ." 22;33m" ;
: BLU ESC[ ." 22;34m" ;
: MAG ESC[ ." 22;35m" ;
: CYA ESC[ ." 22;36m" ;
: WHT ESC[ ." 22;37m" ;
: BLK-NWBG ESC[ ." 40m" ;
: BLBG BLK-NWBG ;
: RED-NWBG ESC[ ." 101m" ;
: GRN-NWBG ESC[ ." 102m" ;
: YEL-NWBG ESC[ ." 103m" ;
: BLU-NWBG ESC[ ." 104m" ;
: MAG-NWBG ESC[ ." 105m" ;
: CYA-NWBG ESC[ ." 106m" ;
: WHT-NWBG ESC[ ." 107m" ;
: ULN ESC[ ." 4m" ;
: NULN ESC[ ." 24m" ;
: FLSH ESC[ ." 5m" ;
: STDY ESC[ ." 25m" ;
: DFLT ESC[ ." 0m" ;
: FLSH-CUR ESC[ ." ?12h" ;
: STDY-CUR ESC[ ." ?12l" ;
: SHOW-CUR ESC[ ." ?25h" ;
: HIDE-CUR ESC[ ." ?25l" ;

: sinusZX81 ( -- , f-- )
  HIDE-CUR gpage WHT-NWBG GRN page
  64 0 
  DO     i 
         22 i s>f 4e f/ fsin 17e f* f>s +
         plot
  LOOP
  0 12 cur RED ." SINUS" BLK 1 0 cur ." ZX81" 23 0 cur ." PRINT 2+2" BLBG WHT ." L" WHT-NWBG BLK 0 0 cur KEY DROP SHOW-CUR ;
\ Original sinus from http://www.abc80.net/archive/luxor/ABC80x/ABC800-manual-BASIC-II.pdf p. 82.
\ 10 PRINT CHR$(12)
\ 20 FOR I=0 TO 23
\ 30 PRINT CUR(I,0) GGRN;
\ 40 NEXT I
\ 50 FOR I=0 TO 77
\ 60 TXPOINT I,32+SIN(I/5)*30
\ 70 NEXT I
\ 80 PRINT CUR(0,15) RED FLSH DBLE "SINUS"
\ 90 END

: invscreen
  2 _rows @ * 0
  DO     2 _cols @ * 0
         DO     j i dot
                IF     j i clrdot
                ELSE   j i setdot
                THEN
         LOOP

  LOOP
  0 0 cur ;

: waitforkey 0 0 cur ." Press e.g. Enter." key drop ;

: test
screensize _cols ! _rows !
gpage page
15 4 setdot 16 5 setdot 17 6 setdot 18 7 setdot
waitforkey
16 5 clrdot 18 7 clrdot
waitforkey
page
waitforkey
page grefresh
15 4 dot . 16 5 dot . 17 6 dot . 18 7 dot .
0 16 cur _rows ? _cols ? ;
test
