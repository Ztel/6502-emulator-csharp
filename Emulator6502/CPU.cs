using System;

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
        ///          ADDRESSING MODES        ///
        ////////////////////////////////////////

        private delegate (byte, int) TranslateOperands(ref byte[] memory);  //Addressing Mode Methods
                                                                            //Returns the data byte and number of operand bytes processed.                                                                       

        private (byte, int) Immediate(ref byte[] memory)
        {
            return (memory[ProgramCounter + 1], 1);
        }

        private (byte, int) ZeroPage(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) ZeroPageX(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) ZeroPageY(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) Absolute(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) AbsoluteX(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) AbsoluteY(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) Indirect(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) IndexedIndirect(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) IndirectIndexed(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) WithAccumulator(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) Relative(ref byte[] memory)
        {
            return (0, 0);
        }

        private (byte, int) Implicit(ref byte[] memory)
        {
            return (0, 0);
        }



        ////////////////////////////////////////
        ///          CPU INSTRUCTIONS        ///
        ////////////////////////////////////////

        private delegate void PerformOperation(ref byte[] memory, ref byte data, int operands);   //Instruction Methods

        private (PerformOperation, TranslateOperands) TranslateOpcode(byte opcode) => opcode switch
        {
            0xA9 => (LDA, Immediate),
            //TODO: Add the rest of the opcodes
            _ => throw new ArgumentException(String.Format("Attempted to parse invalid opcode {0} at index {1}.", opcode, ProgramCounter))
        };

        //Load A - Loads a value into the accumulator.
        private void LDA(ref byte[] memory, ref byte data, int operands)
        {
            Accumulator = data;
            ProgramCounter += (ushort)(operands + 1);
        }



        ////////////////////////////////////////
        ///           CPU FUNCTIONS          ///
        ////////////////////////////////////////
        
        private void ExecuteInstruction(ref byte[] memory, PerformOperation operation, TranslateOperands operands)
        {
            (byte, int) data = operands(ref memory); //Item1 is the data byte, Item2 is the number of operand bytes processed.

            operation(ref memory, ref data.Item1, data.Item2);
        }

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
