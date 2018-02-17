( Programming language: GNU Forth/gforth in WSL Ubuntu or Raspbian, but probably works in other vt100/xterm and OS. )
( ABC80 & ABC800 & TRS-80 need font: teletext2 or teletext4 from https://github.com/peterkvt80/Muttlee/tree/master/public/assets . )
( ZX80 & ZX81 works with many fonts e.g.: DejaVu Sans Mono or Monospace. In X if you load teletext2 after Monospace you have both.)
( Paste this in running gforth console. )

decimal

: page ( -- ) \ Needs to redefine because of current version of Microsoft Windows [Version 10.0.17083.1000].
  0 0 at-xy ESC[ ." J" ;

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

: gemit ( gch -- )
  codepoints @ xemit ;

: gemit3x2 ( gch -- )
  60960 + dup 60992 >
  IF     32 +
  THEN
  xemit ;

: semigraphics ( -- )
  9618 xemit 
  16 0 
  DO     i gemit 9618 xemit 
  LOOP ;
  
: semigraphics3x2 ( -- )
  61103 xemit  
  64 0
  DO     i gemit3x2 61103 xemit
  LOOP
  ;

: cur ( r c -- )
  swap at-xy ;

: at ( r c -- )
  cur ;
  
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

: grefresh3x2 ( -- )
  _rows @ _cols @ * 0
  DO     i screen c@ ?dup
         IF     i _cols @ /mod at-xy gemit3x2
         THEN
  LOOP
  0 0 at-xy ;

: dotcommon ( y x -- dx dy r c )
  2 /mod rot 2 /mod ( dx c dy r )
  rot ( dx dy r c ) ;
  
: dotcommon3x2 ( y x -- dx dy r c )
  2 /mod rot 3 /mod ( dx c dy r )
  rot ( dx dy r c ) ;
  
: dotcommon2 ( dx dy r c -- r c gch )
  2swap ( r c dx dy )
  2* + 1 swap lshift ( r c gch ) ;

: dotindex ( r c -- index )
  swap _cols @ * + ( index ) ;

: dotcommon4 ( gch pos -- pos gch old )
  tuck ( pos gch pos )
  screen c@ ( pos gch old ) ;

: dotemit ( pos new -- )
  tuck gemit ( new pos )
  screen c! ( ) ;
  
: dotemit3x2 ( pos new -- )
  tuck gemit3x2 ( new pos )
  screen c! ( ) ;
  
( ABC80 if 2x2 semigraphics )
: setdot2x2 ( y x -- )
  dotcommon
  2dup cur
  dotcommon2
  -rot ( gch r c )
  dotindex
  dotcommon4
  or ( pos new )
  dotemit ;

: clrdot2x2 ( y x -- )
  dotcommon
  2dup cur
  dotcommon2
  invert ( r c igch )
  -rot ( igch r c )
  dotindex
  dotcommon4
  and ( pos new )
  dotemit ;

: dot2x2 ( y x -- f )
  dotcommon
  dotcommon2
  -rot ( gch r c )
  dotindex
  screen c@ ( gch old )
  and 0> ( f ) ;
  
( ZX81 )
: plot ( x y -- )
  _rows @ 2 - 2* swap - 1- swap
  setdot2x2 ;

: unplot ( x y -- )
  _rows @ 2 - 2* swap - 1- swap
  clrdot2x2 ;

: plotted ( x y -- f )
  _rows @ 2 - 2* swap - 1- swap
  dot2x2 ;

( ABC80 )
: setdot ( y x -- )
  dotcommon3x2
  2dup cur
  dotcommon2
  -rot ( gch r c )
  dotindex
  dotcommon4
  or ( pos new )
  dotemit3x2 ;

: clrdot ( y x -- )
  dotcommon3x2
  2dup cur
  dotcommon2
  invert ( r c igch )
  -rot ( igch r c )
  dotindex
  dotcommon4
  and ( pos new )
  dotemit3x2 ;

: dot ( y x -- f )
  dotcommon3x2
  dotcommon2
  -rot ( gch r c )
  dotindex
  screen c@ ( gch old )
  and 0> ( f ) ;
  
( ABC800, 24 rows terminal )
: txpoint ( x y c -- )
  -rot 3 _rows @ * swap - swap 2 + rot
  IF setdot
  ELSE clrdot
  THEN ;
  
: txpointdot ( x y -- f )
  3 _rows @ * swap - swap 2 +
  dot ;

( TRS-80 16x64 terminal )
: set
  swap setdot ;
  
: reset
  swap clrdot ;
  
: point
  swap dot ;
  
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
: EL .\" \e#6" ;
: DBLET .\" \e#3" ;
: DBLEB .\" \e#4" ;
: NRML .\" \e#5" ;
\ GCON GSEP
\ Not in BASIC II
: DFLT ESC[ ." 0m" ;
: FLSH-CUR ESC[ ." ?12h" ;
: STDY-CUR ESC[ ." ?12l" ;
: SHOW-CUR ESC[ ." ?25h" ;
: HIDE-CUR ESC[ ." ?25l" ;
: CUU ESC[ [char] A emit ;
: CUD ESC[ [char] B emit ;
: SAVE-CUR .\" \e7" ;
: RSTR-CUR .\" \e8" ;

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

variable iR
variable iK
: exgramod ( -- )
  DFLT
  58 5
  DO     41 i 2 / - iR ! 
         36 i 2 / - iK ! 
         i 1+ 1
         DO     iR @ i + iK @ setdot 
                iR @ j + iK @ i + setdot 
                iR @ j + i - iK @ j + setdot 
                iR @ iK @ j + i - setdot
         LOOP
         4
  +LOOP
  ;
\ Original exgramod from ABC 80 BASIC "Bruksanvisning ABC80" p. 30.
\ 10 I = 1 TO 24:;:;CHR¤(151);:NEXT I
\ 20 FOR J = 5 TO 57 STEP 4
\ 30 R = 41-J/2
\ 40 K = 36-J/2
\ 50 FOR I = 1 TO J
\ 60 SETDOT R+I,K
\ 70 SETDOT R+J,K+I
\ 80 SETDOT R+J-I,K+J
\ 90 SETDOT R,K+J-I
\ 100 NEXT I
\ 110 NEXT J

fvariable x
: sinus ( -- , f-- )
  DFLT gpage page 65 0 
  DO     i s>f 10e f/ x f!
         35 30e x f@ fsin f* f>s -
         2 10e x f@ f* f>s +
         setdot
  LOOP
  BEGIN
  AGAIN ;
\ Original sinus from "ABC om BASIC" p. 120.
\ 10 PRINT CHR¤(12)
\ 20 FOR I=0 TO 23 
\ 30 PRINT CUR(I,0);CHR¤(151);
\ 40 NEXT I
\ 50 FOR X=0 TO 6.4 STEP .1
\ 60 R=35-30*SIN(X)
\ 70 K=2+10*X
\ 80 SETDOT R,K
\ 90 NEXT X
\ 100 GOTO 100

: sinusII ( -- , f-- )
  DFLT gpage page GRN
  78 0 
  DO     i 
         32 i s>f 5e f/ fsin 30e f* f>s +
         1
         txpoint
  LOOP
  0 7 cur RED FLSH DBLET SAVE-CUR ." SINUS" CR DBLEB RSTR-CUR CUD ." SINUS" CUD DFLT ;
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
         DO     j i dot2x2
                IF     j i clrdot2x2
                ELSE   j i setdot2x2
                THEN
         LOOP
  LOOP
  0 0 cur ;

: invscreen3x2
  3 _rows @ * 0
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
15 4 setdot2x2 16 5 setdot2x2 17 6 setdot2x2 18 7 setdot2x2
waitforkey
16 5 clrdot2x2 18 7 clrdot2x2
waitforkey
page
waitforkey
page grefresh
15 4 dot2x2 . 16 5 dot2x2 . 17 6 dot2x2 . 18 7 dot2x2 .
0 16 cur _rows ? _cols ? ;

: test3x2
screensize _cols ! _rows !
gpage page exgramod invscreen3x2 invscreen3x2
waitforkey
gpage page
15 4 setdot 16 5 setdot 17 6 setdot 18 7 setdot
waitforkey
16 5 clrdot 18 7 clrdot
waitforkey
page
waitforkey
page grefresh3x2
15 4 dot . 16 5 dot . 17 6 dot . 18 7 dot .
0 30 cur _rows ? _cols ? ;

\ test or test3x2
