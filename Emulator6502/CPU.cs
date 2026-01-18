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

        public Dictionary<byte, int> Opcodes { get; } = new Dictionary<byte, int>() //Table of all valid 6502 opcodes and their corresponding number of operand bytes.
        {                                                                           //Operand byte counts range from 0 - 2 depending on the instruction. 
            { 0xEA, 0 },    //NOP - No Operation
                            //TODO: Add the rest of the opcodes
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
