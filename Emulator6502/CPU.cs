namespace Emulator6502
{
    public class CPU
    {
        ////////////////////////////////////////
        ///           CPU REGISTERS          ///
        ////////////////////////////////////////  

        public ushort PC { get; set; }          //Program Counter   (16-bit)
                                                //Holds the memory address of the current instruction.

        public byte AC { get; set; }            //Accumulator       (8-bit)
                                                //Primary register for use with arithmetic and logic instructions.

        public byte X { get; set; }             //X Register        (8-bit)
                                                //Index register.

        public byte Y { get; set; }             //Y Register        (8-bit)
                                                //Index register.

        public byte SP { get; set; }            //Stack Pointer     (8-bit)
                                                //Used as the low byte to access the 'top' of the stack, between $0100 and $01FF. 

        public byte SR { get; private set; }    //Status Register   (8-bit)
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
        public bool GetStatusRegisterFlag(char flag) => flag switch
        {
            'N' => (SR & 0b10000000) == 0b10000000, //Negative Flag
            'V' => (SR & 0b01000000) == 0b01000000, //Overflow Flag
            'B' => (SR & 0b00010000) == 0b00010000, //Break Flag
            'D' => (SR & 0b00001000) == 0b00001000, //Decimal Flag
            'I' => (SR & 0b00000100) == 0b00000100, //Interrupt Flag
            'Z' => (SR & 0b00000010) == 0b00000010, //Zero Flag
            'C' => (SR & 0b00000001) == 0b00000001, //Carry Flag
            _ => throw new ArgumentException(String.Format("{0} is not a valid status register flag.", flag)),
        };


        //Sets a specified status register to true or false (1 or 0).
        //Valid inputs are chars N, V, B, D, I, Z, C, along with a boolean value.
        public void SetStatusRegisterFlag(char flag, bool desiredValue)
        {
            //Get a mask to pick which flag bit to manipulate
            var flagMask = flag switch
            {
                'N' => (byte)0b10000000,//Negative Flag
                'V' => (byte)0b01000000,//Overflow Flag
                'B' => (byte)0b00010000,//Break Flag
                'D' => (byte)0b00001000,//Decimal Flag
                'I' => (byte)0b00000100,//Interrupt Flag
                'Z' => (byte)0b00000010,//Zero Flag
                'C' => (byte)0b00000001,//Carry Flag
                _ => throw new ArgumentException(String.Format("{0} is not a valid status register flag.", flag)),
            };

            //Set the flag bit to the desired value
            if (desiredValue == true)
            {
                SR = (byte)(SR | flagMask); //Use a bitwise OR to set the correct bit to 1
            }
            else
            {
                SR = (byte)(SR & (0b11111111 ^ flagMask)); //Use a bitwise AND with an inverted mask to set the correct bit to 0
            }
        }
    }
}
