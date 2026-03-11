.ORG $8000

reset:
  LDX #$FF
  TXS
  LDX #$00
  JSR load_sprites
  LDX #$00
  JSR load_background
  JMP main_loop

load_sprites:
  LDA sprites,X
  STA $2100,X 
  INX
  CPX #20 ;20 sprite bytes
  BNE load_sprites
  RTS

load_background:
  LDA background,X
  STA $2000,X 
  INX
  CPX #$00 ;256 background bytes
  BNE load_background
  RTS

main_loop:
  JMP main_loop

nmi:
  RTI

irq:
  RTI

sprites:
  .BYTE $72, $48 ;H
  .BYTE $73, $65 ;e
  .BYTE $74, $6C ;l
  .BYTE $75, $6C ;l
  .BYTE $76, $6F ;o
  .BYTE $79, $57 ;W
  .BYTE $7A, $6F ;o
  .BYTE $7B, $72 ;r
  .BYTE $7C, $6C ;l
  .BYTE $7D, $64 ;d

background:
  .BYTE $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $A4
  .BYTE $A4, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $00, $B7, $A4
  .BYTE $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4, $A4

.ORG $FFFA
.WORD nmi
.WORD reset
.WORD irq