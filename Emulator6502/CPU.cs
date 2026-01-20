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

            //ARITHMETIC INSTRUCTIONS
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
