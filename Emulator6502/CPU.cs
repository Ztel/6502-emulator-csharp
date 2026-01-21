namespace Emulator6502
{
    public class CPU
    {
        ////////////////////////////////////////
        ///           CPU REGISTERS          ///
        ////////////////////////////////////////  

        public ushort ProgramCounter { get; set; }          //Program Counter   (16-bit)
                                                            //Holds the memory address of the current instruction.

        public byte Accumulator { get; set; }               //Accumulator       (8-bit)
                                                            //Primary register for use with arithmetic and logic instructions.

        public byte XRegister { get; set; }                 //X Register        (8-bit)
                                                            //Indexing register.

        public byte YRegister { get; set; }                 //Y Register        (8-bit)
                                                            //Indexing register.

        public byte StackPointer { get; set; }              //Stack Pointer     (8-bit)
                                                            //Used as the low byte to access the 'top' of the stack, between $0100 and $01FF. 

        public byte StatusRegister { get; private set; }    //Status Register   (8-bit)
                                                            //Holds 7 flags indicating CPU status and results of previous instructions.
                                                            //From bit 7 to bit 0 they are:
                                                            //  N - Negative
                                                            //  V - Overflow
                                                            //    - [Not Used]
                                                            //  B - Break
                                                            //  D - Decimal
                                                            //  I - Interrupt
                                                            //  Z - Zero
                                                            //  C - Carry


        ////////////////////////////////////////
        ///          CPU INSTRUCTIONS        ///
        ////////////////////////////////////////  

        public Dictionary<byte, int> Opcodes { get; } = new Dictionary<byte, int>() //Table of all valid 6502 opcodes and their operand bytes.
        {                                                                           //Operand byte counts range from 0 - 2 depending on the instruction. 
            //ACCESS INSTRUCTIONS

            { 0xA9, 1 },        //LDA - Load A (#v    - Immediate)
            { 0xA5, 1 },        //LDA - Load A (d     - Zero Page)
            { 0xB5, 1 },        //LDA - Load A (d,x   - Zero Page, X)
            { 0xAD, 2 },        //LDA - Load A (a     - Absolute)
            { 0xBD, 2 },        //LDA - Load A (a,x   - Absolute, X)
            { 0xB9, 2 },        //LDA - Load A (a,y   - Absolute, Y)
            { 0xA1, 1 },        //LDA - Load A ((d,x) - Indexed Indirect)
            { 0xB1, 1 },        //LDA - Load A ((d),y - Indirect Indexed)

            { 0x85, 1 },        //STA - Store A (d     - Zero Page)
            { 0x95, 1 },        //STA - Store A (d,x   - Zero Page, X)
            { 0x8D, 2 },        //STA - Store A (a     - Absolute)
            { 0x9D, 2 },        //STA - Store A (a,x   - Absolute, X)
            { 0x99, 2 },        //STA - Store A (a,y   - Absolute, Y)
            { 0x81, 1 },        //STA - Store A ((d,x) - Indexed Indirect)
            { 0x91, 1 },        //STA - Store A ((d),y - Indirect Indexed)

            { 0xA2, 1 },        //LDX - Load X (#v    - Immediate)
            { 0xA6, 1 },        //LDX - Load X (d     - Zero Page)
            { 0xB6, 1 },        //LDX - Load X (d,y   - Zero Page, Y)
            { 0xAE, 2 },        //LDX - Load X (a     - Absolute)
            { 0xBE, 2 },        //LDX - Load X (a,y   - Absolute, Y)

            { 0x86, 1 },        //STX - Store X (d     - Zero Page)
            { 0x96, 1 },        //STX - Store X (d,y   - Zero Page, Y)
            { 0x8E, 2 },        //STX - Store X (a     - Absolute)

            { 0xA0, 1 },        //LDY - Load Y (#v    - Immediate)
            { 0xA4, 1 },        //LDY - Load Y (d     - Zero Page)
            { 0xB4, 1 },        //LDY - Load Y (d,x   - Zero Page, X)
            { 0xAC, 2 },        //LDY - Load Y (a     - Absolute)
            { 0xBC, 2 },        //LDY - Load Y (a,x   - Absolute, X)

            { 0x84, 1 },        //STY - Store Y (d     - Zero Page)
            { 0x94, 1 },        //STY - Store Y (d,x   - Zero Page, X)
            { 0x8C, 2 },        //STY - Store Y (a     - Absolute)

            //TRANSFER INSTRUCTIONS

            { 0xAA, 0 },        //TAX - Transfer A to X (Implicit)

            { 0x8A, 0 },        //TXA - Transfer X to A (Implicit)

            { 0xA8, 0 },        //TAY - Transfer A to Y (Implicit)

            { 0x98, 0 },        //TYA - Transfer Y to A (Implicit)

            //ARITHMETIC INSTRUCTIONS

            { 0x69, 1 },        //ADC - Add with Carry (#v    - Immediate)
            { 0x65, 1 },        //ADC - Add with Carry (d     - Zero Page)
            { 0x75, 1 },        //ADC - Add with Carry (d,x   - Zero Page, X)
            { 0x6D, 2 },        //ADC - Add with Carry (a     - Absolute)
            { 0x7D, 2 },        //ADC - Add with Carry (a,x   - Absolute, X)
            { 0x79, 2 },        //ADC - Add with Carry (a,y   - Absolute, Y)
            { 0x61, 1 },        //ADC - Add with Carry ((d,x) - Indexed Indirect)
            { 0x71, 1 },        //ADC - Add with Carry ((d),y - Indirect Indexed)

            { 0xE9, 1 },        //SBC - Subtract with Carry (#v    - Immediate)
            { 0xE5, 1 },        //SBC - Subtract with Carry (d     - Zero Page)
            { 0xF5, 1 },        //SBC - Subtract with Carry (d,x   - Zero Page, X)
            { 0xED, 2 },        //SBC - Subtract with Carry (a     - Absolute)
            { 0xFD, 2 },        //SBC - Subtract with Carry (a,x   - Absolute, X)
            { 0xF9, 2 },        //SBC - Subtract with Carry (a,y   - Absolute, Y)
            { 0xE1, 1 },        //SBC - Subtract with Carry ((d,x) - Indexed Indirect)
            { 0xF1, 1 },        //SBC - Subtract with Carry ((d),y - Indirect Indexed)

            { 0xE6, 1 },        //INC - Increment Memory (d     - Zero Page)
            { 0xF6, 1 },        //INC - Increment Memory (d,x   - Zero Page, X)
            { 0xEE, 2 },        //INC - Increment Memory (a     - Absolute)
            { 0xFE, 2 },        //INC - Increment Memory (a,x   - Absolute, X)

            { 0xC6, 1 },        //DEC - Decrement Memory (d     - Zero Page)
            { 0xD6, 1 },        //DEC - Decrement Memory (d,x   - Zero Page, X)
            { 0xCE, 2 },        //DEC - Decrement Memory (a     - Absolute)
            { 0xDE, 2 },        //DEC - Decrement Memory (a,x   - Absolute, X)

            { 0xE8, 0 },        //INX - Increment X (Implicit)

            { 0xCA, 0 },        //DEX - Decrement X (Implicit)

            { 0xC8, 0 },        //INY - Increment Y (Implicit)

            { 0x88, 0 },        //DEY - Decrement Y (Implicit)

            //SHIFT INSTRUCTIONS

            { 0x0A, 0 },        //ASL - Arithmetic Shift Left (A     - Accumulator)
            { 0x06, 1 },        //ASL - Arithmetic Shift Left (d     - Zero Page)
            { 0x16, 1 },        //ASL - Arithmetic Shift Left (d,x   - Zero Page, X)
            { 0x0E, 2 },        //ASL - Arithmetic Shift Left (a     - Absolute)
            { 0x1E, 2 },        //ASL - Arithmetic Shift Left (a,x   - Absolute, X)

            { 0x4A, 0 },        //LSR - Logical Shift Right (A     - Accumulator)
            { 0x46, 1 },        //LSR - Logical Shift Right (d     - Zero Page)
            { 0x56, 1 },        //LSR - Logical Shift Right (d,x   - Zero Page, X)
            { 0x4E, 2 },        //LSR - Logical Shift Right (a     - Absolute)
            { 0x5E, 2 },        //LSR - Logical Shift Right (a,x   - Absolute, X)

            { 0x2A, 0 },        //ROL - Rotate Left (A     - Accumulator)
            { 0x26, 1 },        //ROL - Rotate Left (d     - Zero Page)
            { 0x36, 1 },        //ROL - Rotate Left (d,x   - Zero Page, X)
            { 0x2E, 2 },        //ROL - Rotate Left (a     - Absolute)
            { 0x3E, 2 },        //ROL - Rotate Left (a,x   - Absolute, X)

            { 0x6A, 0 },        //ROR - Rotate Right (A     - Accumulator)
            { 0x66, 1 },        //ROR - Rotate Right (d     - Zero Page)
            { 0x76, 1 },        //ROR - Rotate Right (d,x   - Zero Page, X)
            { 0x6E, 2 },        //ROR - Rotate Right (a     - Absolute)
            { 0x7E, 2 },        //ROR - Rotate Right (a,x   - Absolute, X)

            //BITWISE INSTRUCTIONS

            { 0x29, 1 },        //AND - Bitwise AND (#v    - Immediate)
            { 0x25, 1 },        //AND - Bitwise AND (d     - Zero Page)
            { 0x35, 1 },        //AND - Bitwise AND (d,x   - Zero Page, X)
            { 0x2D, 2 },        //AND - Bitwise AND (a     - Absolute)
            { 0x3D, 2 },        //AND - Bitwise AND (a,x   - Absolute, X)
            { 0x39, 2 },        //AND - Bitwise AND (a,y   - Absolute, Y)
            { 0x21, 1 },        //AND - Bitwise AND ((d,x) - Indexed Indirect)
            { 0x31, 1 },        //AND - Bitwise AND ((d),y - Indirect Indexed)

            { 0x09, 1 },        //ORA - Bitwise OR (#v    - Immediate)
            { 0x05, 1 },        //ORA - Bitwise OR (d     - Zero Page)
            { 0x15, 1 },        //ORA - Bitwise OR (d,x   - Zero Page, X)
            { 0x0D, 2 },        //ORA - Bitwise OR (a     - Absolute)
            { 0x1D, 2 },        //ORA - Bitwise OR (a,x   - Absolute, X)
            { 0x19, 2 },        //ORA - Bitwise OR (a,y   - Absolute, Y)
            { 0x01, 1 },        //ORA - Bitwise OR ((d,x) - Indexed Indirect)
            { 0x11, 1 },        //ORA - Bitwise OR ((d),y - Indirect Indexed)

            { 0x49, 1 },        //EOR - Bitwise Exclusive OR (#v    - Immediate)
            { 0x45, 1 },        //EOR - Bitwise Exclusive OR (d     - Zero Page)
            { 0x55, 1 },        //EOR - Bitwise Exclusive OR (d,x   - Zero Page, X)
            { 0x4D, 2 },        //EOR - Bitwise Exclusive OR (a     - Absolute)
            { 0x5D, 2 },        //EOR - Bitwise Exclusive OR (a,x   - Absolute, X)
            { 0x59, 2 },        //EOR - Bitwise Exclusive OR (a,y   - Absolute, Y)
            { 0x41, 1 },        //EOR - Bitwise Exclusive OR ((d,x) - Indexed Indirect)
            { 0x51, 1 },        //EOR - Bitwise Exclusive OR ((d),y - Indirect Indexed)

            { 0x24, 1 },        //BIT - Bit Test (d     - Zero Page)
            { 0x2C, 2 },        //BIT - Bit Test (a     - Absolute)

            //COMPARE INSTRUCTIONS

            { 0xC9, 1 },        //CMP - Compare A (#v    - Immediate)
            { 0xC5, 1 },        //CMP - Compare A (d     - Zero Page)
            { 0xD5, 1 },        //CMP - Compare A (d,x   - Zero Page, X)
            { 0xCD, 2 },        //CMP - Compare A (a     - Absolute)
            { 0xDD, 2 },        //CMP - Compare A (a,x   - Absolute, X)
            { 0xD9, 2 },        //CMP - Compare A (a,y   - Absolute, Y)
            { 0xC1, 1 },        //CMP - Compare A ((d,x) - Indexed Indirect)
            { 0xD1, 1 },        //CMP - Compare A ((d),y - Indirect Indexed)

            { 0xE0, 1 },        //CPX - Compare X (#v    - Immediate)
            { 0xE4, 1 },        //CPX - Compare X (d     - Zero Page)
            { 0xEC, 2 },        //CPX - Compare X (a     - Absolute)

            { 0xC0, 1 },        //CPY - Compare Y (#v    - Immediate)
            { 0xC4, 1 },        //CPY - Compare Y (d     - Zero Page)
            { 0xCC, 2 },        //CPY - Compare Y (a     - Absolute)

            //BRANCH INSTRUCTIONS

            { 0x90, 1 },        //BCC - Branch if Carry Clear (label - Relative)

            { 0xB0, 1 },        //BCS - Branch if Carry Set (label - Relative)
            
            { 0xF0, 1 },        //BEQ - Branch if Equal (label - Relative)

            { 0xD0, 1 },        //BNE - Branch if Not Equal (label - Relative)

            { 0x10, 1 },        //BPL - Branch if Plus (label - Relative)

            { 0x30, 1 },        //BMI - Branch if Minus (label - Relative)

            { 0x50, 1 },        //BVC - Branch if Overflow Clear (label - Relative)

            { 0x70, 1 },        //BVS - Branch if Overflow Set (label - Relative)

            //JUMP INSTRUCTIONS

            { 0x4C, 2 },        //JMP - Jump (a     - Absolute)
            { 0x6C, 2 },        //JMP - Jump ((a)   - Indirect)

            { 0x20, 2 },        //JSR - Jump to Subroutine (a     - Absolute)

            { 0x60, 0 },        //RTS - Return from Subroutine (Implicit)

            { 0x00, 0 },        //BRK - Break (Implicit)

            { 0x40, 0 },        //RTI - Return from Interrupt (Implicit)

            //STACK INSTRUCTIONS

            { 0x48, 0 },        //PHA - Push A (Implicit)

            { 0x68, 0 },        //PLA - Pull A (Implicit)

            { 0x08, 0 },        //PHP - Push Processor Status (Implicit)

            { 0x28, 0 },        //PLP - Pull Processor Status (Implicit)

            { 0x9A, 0 },        //TXS - Transfer X to Stack Pointer (Implicit)

            { 0xBA, 0 },        //TSX - Transfer Stack Pointer to X (Implicit)

            //FLAG INSTRUCTIONS

            { 0x18, 0 },        //CLC - Clear Carry (Implicit)

            { 0x38, 0 },        //SEC - Set Carry (Implicit)

            { 0x58, 0 },        //CLI - Clear Interrupt Disable (Implicit)

            { 0x78, 0 },        //SEI - Set Interrupt Disable (Implicit)

            { 0xD8, 0 },        //CLD - Clear Decimal (Implicit)

            { 0xF8, 0 },        //SED - Set Decimal (Implicit)

            { 0xB8, 0 },        //CLV - Clear Overflow (Implicit)

            //OTHER INSTRUCTIONS

            { 0xEA, 0 }        //NOP - No Operation (Implicit)
        };



        //Returns whether or not a specific flag bit within the status register is set.
        //Valid inputs are chars N, V, B, D, I, Z, C
        public bool GetStatusRegisterFlag(char flag)
        {
            byte flagMask = GetStatusRegisterFlagMask(flag);

            return (StatusRegister & flagMask) == flagMask;
        }


        //Sets a specified status register to true or false (1 or 0).
        //Valid inputs are chars N, V, B, D, I, Z, C, along with a boolean value.
        public void SetStatusRegisterFlag(char flag, bool desiredValue)
        {
            byte flagMask = GetStatusRegisterFlagMask(flag);

            if (desiredValue == true)
            {
                StatusRegister = (byte)(StatusRegister | flagMask);
            }
            else
            {
                StatusRegister = (byte)(StatusRegister & (0b11111111 ^ flagMask));
            }
        }

        //Returns the binary mask associated with a given status register flag. 
        //Valid inputs are chars N, V, B, D, I, Z, C
        private byte GetStatusRegisterFlagMask(char flag) => flag switch
        {
            'N' => 0b10000000, //Negative Flag
            'V' => 0b01000000, //Overflow Flag
            'B' => 0b00010000, //Break Flag
            'D' => 0b00001000, //Decimal Flag
            'I' => 0b00000100, //Interrupt Flag
            'Z' => 0b00000010, //Zero Flag
            'C' => 0b00000001, //Carry Flag
            _ => throw new ArgumentException(String.Format("{0} is not a valid status register flag.", flag)),
        };
    }
}
