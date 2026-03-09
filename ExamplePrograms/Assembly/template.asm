.ORG $8000

reset:
  ;arrange initial startup state here
  JMP main_loop

main_loop:
  ;logic that loops constantly
  JMP main_loop

nmi:
  ;logic that occurs once every frame
  RTI

irq:
  ;interrup handler
  RTI

.ORG $FFFA
.WORD nmi
.WORD reset
.WORD irq