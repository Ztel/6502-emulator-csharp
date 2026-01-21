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

            //BITWISE INSTRUCTIONS

            //COMPARE INSTRUCTIONS

            //BRANCH INSTRUCTIONS

            //JUMP INSTRUCTIONS

            //STACK INSTRUCTIONS

            //FLAG INSTRUCTIONS

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
