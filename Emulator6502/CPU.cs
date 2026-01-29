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

        private delegate (ushort, int, bool) TranslateOperands(ref byte[] memory);  //Addressing Mode Methods
                                                                                    //Returns the data byte/s, the number of
                                                                                    //operand bytes processed, and whether
                                                                                    //or not the returned data is a direct
                                                                                    //value (as opposed to a memory address).

        private (ushort, int, bool) Immediate(ref byte[] memory)
        {
            ushort value = memory[ProgramCounter + 1];

            return (value, 1, true);
        }

        private (ushort, int, bool) ZeroPage(ref byte[] memory)
        {
            ushort destinationAddress = memory[ProgramCounter + 1];

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) ZeroPageX(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)((memory[ProgramCounter + 1] + XRegister) % 256);

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) ZeroPageY(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)((memory[ProgramCounter + 1] + YRegister) % 256);

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) Absolute(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)(memory[ProgramCounter + 2] * 256 + memory[ProgramCounter + 1]);

            return (destinationAddress, 2, false);
        }

        private (ushort, int, bool) AbsoluteX(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)(memory[ProgramCounter + 2] * 256 + memory[ProgramCounter + 1] + XRegister);

            return (destinationAddress, 2, false);
        }

        private (ushort, int, bool) AbsoluteY(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)(memory[ProgramCounter + 2] * 256 + memory[ProgramCounter + 1] + YRegister);

            return (destinationAddress, 2, false);
        }

        private (ushort, int, bool) Indirect(ref byte[] memory)
        {
            ushort sourceLocation = (ushort)(memory[ProgramCounter + 2] * 256 + memory[ProgramCounter + 1]);

            ushort destinationAddress = (ushort)(memory[sourceLocation + 1] * 256 + memory[sourceLocation + 1]);

            return (destinationAddress, 2, false);
        }

        private (ushort, int, bool) IndexedIndirect(ref byte[] memory)
        {
            ushort sourceLocation = (ushort)((memory[ProgramCounter + 1] + XRegister) % 256);

            byte destinationLower = memory[sourceLocation];
            byte destinationUpper = memory[sourceLocation + 1];

            ushort destinationAddress = (ushort)(destinationUpper * 256 + destinationLower);

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) IndirectIndexed(ref byte[] memory)
        {
            ushort sourceLocation = memory[ProgramCounter + 1];

            byte destinationLower = memory[sourceLocation];
            byte destinationUpper = memory[sourceLocation + 1];

            ushort destinationAddress = (ushort)(destinationUpper * 256 + destinationLower + YRegister);

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) WithAccumulator(ref byte[] memory)
        {
            return (Accumulator, 0, true);
        }

        private (ushort, int, bool) Relative(ref byte[] memory)
        {
            ushort destinationAddress = (ushort)(ProgramCounter + (sbyte)memory[ProgramCounter] + 2);

            return (destinationAddress, 1, false);
        }

        private (ushort, int, bool) Implicit(ref byte[] memory)
        {
            return (0, 0, false);
        }



        ////////////////////////////////////////
        ///          CPU INSTRUCTIONS        ///
        ////////////////////////////////////////

        private delegate void PerformOperation(ref byte[] memory, ref ushort data, int operands, bool isDirectValue);   //Instruction Methods

        private (PerformOperation, TranslateOperands) TranslateOpcode(byte opcode) => opcode switch
        {
            0xA9 => (LDA, Immediate),
            0xA5 => (LDA, ZeroPage),
            0xB5 => (LDA, ZeroPageX),
            0xAD => (LDA, Absolute),
            0xBD => (LDA, AbsoluteX),
            0xB9 => (LDA, AbsoluteY),
            0xA1 => (LDA, IndexedIndirect),
            0xB1 => (LDA, IndirectIndexed),

            0x8D => (STA, Absolute),
            //TODO: Add the rest of the opcodes
            _ => throw new ArgumentException(String.Format("Attempted to parse invalid opcode {0} at index {1}.", opcode, ProgramCounter))
        };

        //Load A - Loads a value into the accumulator.
        private void LDA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = (isDirectValue) ? (byte)data : memory[data];

            ProgramCounter += (ushort)(operands + 1);
        }

        //Store A - Stores the value of the accumulator into memory.
        private void STA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            memory[data] = Accumulator;

            ProgramCounter += (ushort)(operands + 1);
        }



        ////////////////////////////////////////
        ///           CPU FUNCTIONS          ///
        ////////////////////////////////////////

        public void Step(ref byte[] memory)
        {
            (PerformOperation, TranslateOperands) instruction = TranslateOpcode(memory[ProgramCounter]);

            ExecuteInstruction(ref memory, instruction.Item1, instruction.Item2);
        }

        private void ExecuteInstruction(ref byte[] memory, PerformOperation operation, TranslateOperands operands)
        {
            (ushort, int, bool) data = operands(ref memory); //Item1 is the data byte, Item2 is the number of operand bytes processed.

            operation(ref memory, ref data.Item1, data.Item2, data.Item3);
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
