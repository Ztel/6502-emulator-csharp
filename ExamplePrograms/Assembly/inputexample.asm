player_position = $00
player_input = $01
movement_steps = $02
current_movement_delay = $03
walk_delay = $04
sprint_delay = $05

.ORG $8000

reset:
  LDX #$FF
  TXS
  LDX #$00
  JSR load_background
  JSR initialize_player
  JMP main_loop

load_background:
  LDA background,X
  STA $2080,X 
  INX
  CPX #$70 ;112 background bytes
  BNE load_background
  RTS

initialize_player:
  ;clear input
  LDA #$00
  STA player_input
  ;put the player near the center
  LDA #$57
  STA player_position
  ;adjust delay between player movement steps to change speed
  LDA #$04
  STA walk_delay
  LDA #$02
  STA sprint_delay
  LDA walk_delay
  STA current_movement_delay
  ;set the player sprite to an '@' symbol
  LDA #$40
  STA $2101
  RTS

main_loop:
  JMP main_loop

;convert arrow key input into a single movement vector byte
handle_input:
  ;up
  LDA $4000 
  CMP #$01
  BNE :+
  SEC
  LDA player_input
  SBC #$10
  STA player_input
  ;down
: LDA $4001 
  CMP #$01
  BNE :+
  CLC
  LDA player_input 
  ADC #$10 
  STA player_input
  ;left
: LDA $4002 
  CMP #$01
  BNE :+
  SEC
  LDA player_input
  SBC #$01
  STA player_input
  ;right
: LDA $4003 
  CMP #$01
  BNE :+
  CLC
  LDA player_input
  ADC #$01
  STA player_input
: RTS

handle_sprint:
  LDA walk_delay
  STA current_movement_delay
  LDA $4074 ;left shift key
  CMP #$01
  BNE :+
  LDA sprint_delay
  STA current_movement_delay
: RTS

apply_movement:
  LDA player_input
  CLC
  ADC player_position
  STA player_position
  STA $2100
  RTS

nmi:
  INC movement_steps
  JSR handle_input
  JSR handle_sprint
  ;if the number of elapsed movement steps is greater than or equal to the 
  ;current movement delay, then apply movement and reset the elapsed steps
  LDA movement_steps
  CMP current_movement_delay
  BCC :+
  JSR apply_movement
  LDA #$00
  STA movement_steps 
  ;clear input at the end of each frame
: LDA #$00
  STA player_input
  RTI

irq:
  RTI

background:
  .BYTE $00, $48, $6F, $6C, $64, $00, $61, $72, $72, $6F, $77, $00, $00, $00, $00, $00 ; Hold arrow
  .BYTE $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00 ;
  .BYTE $00, $6B, $65, $79, $73, $00, $74, $6F, $00, $6D, $6F, $76, $65, $2E, $00, $00 ; keys to move.
  .BYTE $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00 ;
  .BYTE $00, $48, $6F, $6C, $64, $00, $73, $68, $69, $66, $74, $00, $74, $6F, $00, $00 ; Hold shift to
  .BYTE $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00, $00 ;
  .BYTE $00, $73, $70, $72, $69, $6E, $74, $2E, $00, $00, $00, $00, $00, $00, $00, $00 ; sprint.

.ORG $FFFA
.WORD nmi
.WORD reset
.WORD irq