using System.Xml.Serialization;

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

            ushort destinationAddress = (ushort)(memory[sourceLocation + 1] * 256 + memory[sourceLocation]);

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
            ushort destinationAddress = (ushort)(ProgramCounter + (sbyte)memory[ProgramCounter + 1] + 2);

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

            0x85 => (STA, ZeroPage),
            0x95 => (STA, ZeroPageX),
            0x8D => (STA, Absolute),
            0x9D => (STA, AbsoluteX),
            0x99 => (STA, AbsoluteY),
            0x81 => (STA, IndexedIndirect),
            0x91 => (STA, IndirectIndexed),

            0xA2 => (LDX, Immediate),
            0xA6 => (LDX, ZeroPage),
            0xB6 => (LDX, ZeroPageY),
            0xAE => (LDX, Absolute),
            0xBE => (LDX, AbsoluteY),

            0x86 => (STX, ZeroPage),
            0x96 => (STX, ZeroPageY),
            0x8E => (STX, Absolute),

            0xA0 => (LDY, Immediate),
            0xA4 => (LDY, ZeroPage),
            0xB4 => (LDY, ZeroPageX),
            0xAC => (LDY, Absolute),
            0xBC => (LDY, AbsoluteX),

            0x84 => (STY, ZeroPage),
            0x94 => (STY, ZeroPageX),
            0x8C => (STY, Absolute),

            0xAA => (TAX, Implicit),

            0x8A => (TXA, Implicit),

            0xA8 => (TAY, Implicit),

            0x98 => (TYA, Implicit),

            0x69 => (ADC, Immediate),
            0x65 => (ADC, ZeroPage),
            0x75 => (ADC, ZeroPageX),
            0x6D => (ADC, Absolute),
            0x7D => (ADC, AbsoluteX),
            0x79 => (ADC, AbsoluteY),
            0x61 => (ADC, IndexedIndirect),
            0x71 => (ADC, IndirectIndexed),

            0xE9 => (SBC, Immediate),
            0xE5 => (SBC, ZeroPage),
            0xF5 => (SBC, ZeroPageX),
            0xED => (SBC, Absolute),
            0xFD => (SBC, AbsoluteX),
            0xF9 => (SBC, AbsoluteY),
            0xE1 => (SBC, IndexedIndirect),
            0xF1 => (SBC, IndirectIndexed),

            0xE6 => (INC, ZeroPage),
            0xF6 => (INC, ZeroPageX),
            0xEE => (INC, Absolute),
            0xFE => (INC, AbsoluteX),

            0xC6 => (DEC, ZeroPage),
            0xD6 => (DEC, ZeroPageX),
            0xCE => (DEC, Absolute),
            0xDE => (DEC, AbsoluteX),

            0xE8 => (INX, Implicit),

            0xCA => (DEX, Implicit),

            0xC8 => (INY, Implicit),
                       
            0x88 => (DEY, Implicit),

            0x0A => (ASL, WithAccumulator),
            0x06 => (ASL, ZeroPage),
            0x16 => (ASL, ZeroPageX),
            0x0E => (ASL, Absolute),
            0x1E => (ASL, AbsoluteX),

            0x4A => (LSR, WithAccumulator),
            0x46 => (LSR, ZeroPage),
            0x56 => (LSR, ZeroPageX),
            0x4E => (LSR, Absolute),
            0x5E => (LSR, AbsoluteX),

            0x2A => (ROL, WithAccumulator),
            0x26 => (ROL, ZeroPage),
            0x36 => (ROL, ZeroPageX),
            0x2E => (ROL, Absolute),
            0x3E => (ROL, AbsoluteX),

            0x6A => (ROR, WithAccumulator),
            0x66 => (ROR, ZeroPage),
            0x76 => (ROR, ZeroPageX),
            0x6E => (ROR, Absolute),
            0x7E => (ROR, AbsoluteX),

            0x29 => (AND, Immediate),
            0x25 => (AND, ZeroPage),
            0x35 => (AND, ZeroPageX),
            0x2D => (AND, Absolute),
            0x3D => (AND, AbsoluteX),
            0x39 => (AND, AbsoluteY),
            0x21 => (AND, IndexedIndirect),
            0x31 => (AND, IndirectIndexed),

            0x09 => (ORA, Immediate),
            0x05 => (ORA, ZeroPage),
            0x15 => (ORA, ZeroPageX),
            0x0D => (ORA, Absolute),
            0x1D => (ORA, AbsoluteX),
            0x19 => (ORA, AbsoluteY),
            0x01 => (ORA, IndexedIndirect),
            0x11 => (ORA, IndirectIndexed),

            0x49 => (EOR, Immediate),
            0x45 => (EOR, ZeroPage),
            0x55 => (EOR, ZeroPageX),
            0x4D => (EOR, Absolute),
            0x5D => (EOR, AbsoluteX),
            0x59 => (EOR, AbsoluteY),
            0x41 => (EOR, IndexedIndirect),
            0x51 => (EOR, IndirectIndexed),

            0x24 => (BIT, ZeroPage),
            0x2C => (BIT, Absolute),

            0xC9 => (CMP, Immediate),
            0xC5 => (CMP, ZeroPage),
            0xD5 => (CMP, ZeroPageX),
            0xCD => (CMP, Absolute),
            0xDD => (CMP, AbsoluteX),
            0xD9 => (CMP, AbsoluteY),
            0xC1 => (CMP, IndexedIndirect),
            0xD1 => (CMP, IndirectIndexed),

            0xE0 => (CPX, Immediate),
            0xE4 => (CPX, ZeroPage),
            0xEC => (CPX, Absolute),

            0xC0 => (CPY, Immediate),
            0xC4 => (CPY, ZeroPage),
            0xCC => (CPY, Absolute),

            0x90 => (BCC, Relative),

            0xB0 => (BCS, Relative),

            0xF0 => (BEQ, Relative),

            0xD0 => (BNE, Relative),

            0x10 => (BPL, Relative),

            0x30 => (BMI, Relative),

            0x50 => (BVC, Relative),

            0x70 => (BVS, Relative),

            0x4C => (JMP, Absolute),
            0x6C => (JMP, Indirect),

            0x20 => (JSR, Absolute),

            0x60 => (RTS, Implicit),

            0x00 => (BRK, Implicit),

            0x40 => (RTI, Implicit),

            0x48 => (PHA, Implicit),

            0x68 => (PLA, Implicit),

            0x08 => (PHP, Implicit),

            0x28 => (PLP, Implicit),

            0x9A => (TXS, Implicit),

            0xBA => (TSX, Implicit),

            0x18 => (CLC, Implicit),

            0x38 => (SEC, Implicit),

            0x58 => (CLI, Implicit),

            0x78 => (SEI, Implicit),

            0xD8 => (CLD, Implicit),

            0xF8 => (SED, Implicit),

            0xB8 => (CLV, Implicit),

            0xEA => (NOP, Implicit),

            _ => throw new ArgumentException(String.Format("Attempted to parse invalid opcode {0} at index {1}.", opcode, ProgramCounter))
        };

        //Load A - Loads a value into the accumulator.
        private void LDA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Store A - Stores the value of the accumulator into memory.
        private void STA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Write(ref memory, data, Accumulator);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Load X - Loads a value into the X register.
        private void LDX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            XRegister = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('Z', XRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(XRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Store X - Stores the value of the X register into memory.
        private void STX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Write(ref memory, data, XRegister);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Load Y - Loads a value into the Y register.
        private void LDY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            YRegister = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('Z', YRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(YRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Store Y - Stores the value of the Y register into memory.
        private void STY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Write(ref memory, data, YRegister);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer A to X - Copy the value of the accumulator into the X register. 
        private void TAX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            XRegister = Accumulator;

            SetStatusRegisterFlag('Z', XRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(XRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer X to A - Copy the value of the X register into the accumulator.
        private void TXA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = XRegister;

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer A to Y - Copy the value of the accumulator into the Y register.
        private void TAY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            YRegister = Accumulator;

            SetStatusRegisterFlag('Z', YRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(YRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer Y to A - Copy the value of the Y register into the accumulator.
        private void TYA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = YRegister;

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Add with Carry - Add the carry flag and a memory value to the accumulator.
        private void ADC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = (isDirectValue) ? (byte)data : memory[data];
            int carry = GetStatusRegisterFlag('C');
            int result = Accumulator + value + carry;

            SetStatusRegisterFlag('C', result > 255);
            SetStatusRegisterFlag('V', (((byte)result ^ Accumulator) & ((byte)result ^ value) & 0b10000000) == 0b10000000);

            Accumulator = (byte)result;
            
            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Subtract with Carry - Subtract a memory value and the NOT of the carry flag from the accumulator.
        private void SBC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = (isDirectValue) ? (byte)data : memory[data];
            int carry = GetStatusRegisterFlag('C');
            int result = Accumulator + ~value + carry;

            SetStatusRegisterFlag('C', !(result < 0));
            SetStatusRegisterFlag('V', (((byte)result ^ Accumulator) & ((byte)result ^ ~value) & 0b10000000) == 0b10000000);

            Accumulator = (byte)result;

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Increment Memory - Add 1 to a value in memory.
        private void INC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Write(ref memory, data, (byte)(memory[data] + 1));

            SetStatusRegisterFlag('Z', memory[data] == 0);
            SetStatusRegisterFlag('N', IsNegative(memory[data]));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Decrement Memory - Subtract 1 from a value in memory.
        private void DEC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Write(ref memory, data, (byte)(memory[data] - 1));

            SetStatusRegisterFlag('Z', memory[data] == 0);
            SetStatusRegisterFlag('N', IsNegative(memory[data]));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Increment X - Add 1 to the X register.
        private void INX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            XRegister++;

            SetStatusRegisterFlag('Z', XRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(XRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Decrement X - Subtract 1 from the X register.
        private void DEX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            XRegister--;

            SetStatusRegisterFlag('Z', XRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(XRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Increment Y - Add 1 to the Y register.
        private void INY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            YRegister++;

            SetStatusRegisterFlag('Z', YRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(YRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Decrement Y - Subtract 1 from the Y register.
        private void DEY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            YRegister--;

            SetStatusRegisterFlag('Z', YRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(YRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Arithmetic Shift Left - Shift all bits of a value one position to the left and fill the open bit with 0.
        private void ASL(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('C', value >> 7 == 1);

            value = (byte)(value << 1);

            if (isDirectValue)
            {
                Accumulator = value;
            }
            else
            {
                Write(ref memory, data, value);
            }

            SetStatusRegisterFlag('Z', value == 0);
            SetStatusRegisterFlag('N', IsNegative(value));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Logical Shift Right - Shift all bits of a value one position to the right and fill the open bit with 0.
        private void LSR(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('C', (value & 0b00000001) == 1);

            value = (byte)(value >> 1);

            if (isDirectValue)
            {
                Accumulator = value;
            }
            else
            {
                Write(ref memory, data, value);
            }

            SetStatusRegisterFlag('Z', value == 0);
            SetStatusRegisterFlag('N', IsNegative(value));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Rotate Left - Shift all bits of a value one position to the left and fill the open bit with the carry flag.
        private void ROL(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            bool carry = value >> 7 == 1;

            value = (byte)(value << 1);
            value = (byte)((value & 0b11111110) | GetStatusRegisterFlag('C'));

            SetStatusRegisterFlag('C', carry);

            if (isDirectValue)
            {
                Accumulator = value;
            }
            else
            {
                Write(ref memory, data, value);
            }

            SetStatusRegisterFlag('Z', value == 0);
            SetStatusRegisterFlag('N', IsNegative(value));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Rotate Right - Shift all bits of a value one position to the right and fill the open bit with the carry flag.
        private void ROR(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            bool carry = (value & 0b00000001) == 1;

            value = (byte)(value >> 1);
            value = (byte)((value & 0b01111111) | (GetStatusRegisterFlag('C') << 7));

            SetStatusRegisterFlag('C', carry);

            if (isDirectValue)
            {
                Accumulator = value;
            }
            else
            {
                Write(ref memory, data, value);
            }

            SetStatusRegisterFlag('Z', value == 0);
            SetStatusRegisterFlag('N', IsNegative(value));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Bitwise AND - Perform an AND between a value and the accumulator.
        private void AND(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = isDirectValue ? (byte)(Accumulator & data) : (byte)(Accumulator & memory[data]);

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Bitwise OR - Perform an inclusive OR between a value and the accumulator.
        private void ORA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = isDirectValue ? (byte)(Accumulator | data) : (byte)(Accumulator | memory[data]);

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Bitwise Exclusive OR - Perform an exclusive OR between a value and the accumulator.
        private void EOR(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = isDirectValue ? (byte)(Accumulator ^ data) : (byte)(Accumulator ^ memory[data]);

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Bit Test - Perform an AND without modifying the accumulator, only setting status register flags.
        private void BIT(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = memory[data];

            SetStatusRegisterFlag('Z', (Accumulator & value) == 0);
            SetStatusRegisterFlag('V', (value & 0b01000000) >> 6 == 1);
            SetStatusRegisterFlag('N', IsNegative(value));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Compare A - Compare the accumulator to a value.
        private void CMP(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('C', Accumulator >= value);
            SetStatusRegisterFlag('Z', Accumulator == value);
            SetStatusRegisterFlag('N', IsNegative((byte)(Accumulator - value)));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Compare X - Compare the X register to a value.
        private void CPX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('C', XRegister >= value);
            SetStatusRegisterFlag('Z', XRegister == value);
            SetStatusRegisterFlag('N', IsNegative((byte)(XRegister - value)));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Compare Y - Compare the Y register to a value.
        private void CPY(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte value = isDirectValue ? (byte)data : memory[data];

            SetStatusRegisterFlag('C', YRegister >= value);
            SetStatusRegisterFlag('Z', YRegister == value);
            SetStatusRegisterFlag('N', IsNegative((byte)(YRegister - value)));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Branch if Carry Clear - Branch to an offset location if the carry flag is clear.
        private void BCC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('C') == 0 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Carry Set - Branch to an offset location if the carry flag is set.
        private void BCS(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('C') == 1 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Equal - Branch to an offset location if the zero flag is set.
        private void BEQ(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('Z') == 1 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Not Equal - Branch to an offset location if the zero flag is clear.
        private void BNE(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('Z') == 0 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Plus - Branch to an offset location if the negative flag is clear.
        private void BPL(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('N') == 0 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Minus - Branch to an offset location if the negative flag is set.
        private void BMI(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('N') == 1 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Overflow Clear - Branch to an offset location if the overflow flag is clear.
        private void BVC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('V') == 0 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Branch if Overflow Set - Branch to an offset location if the overflow flag is set.
        private void BVS(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = GetStatusRegisterFlag('V') == 1 ? data : (ushort)(ProgramCounter + operands + 1);
        }

        //Jump - Execute code from a new location.
        private void JMP(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            ProgramCounter = data;
        }

        //Jump to Subroutine - Push the program counter to the stack, then execute code from a new location.
        private void JSR(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StackPush(ref memory, (byte)((ProgramCounter + 2) >> 8));
            StackPush(ref memory, (byte)((ProgramCounter + 2) & 0x00FF));

            ProgramCounter = data;
        }

        //Return from Subroutine - Pull an address from the stack, then jump to that location plus 1.
        private void RTS(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            byte destinationLower = StackPull(ref memory);
            byte destinationUpper = StackPull(ref memory);

            ushort destinationAddress = (ushort)(destinationUpper * 256 + destinationLower + 1);

            ProgramCounter = destinationAddress;
        }

        //Break - Trigger an IRQ (Interrupt Request).
        private void BRK(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StackPush(ref memory, (byte)((ProgramCounter + 2) >> 8));
            StackPush(ref memory, (byte)((ProgramCounter + 2) & 0x00FF));
            StackPush(ref memory, (byte)(StatusRegister | 0b00110000));

            SetStatusRegisterFlag('I', true);

            byte destinationLower = memory[0xFFFE];
            byte destinationUpper = memory[0xFFFF];

            ushort destinationAddress = (ushort)(destinationUpper * 256 + destinationLower);

            ProgramCounter = destinationAddress;
        }

        //Return from Interrupt - Pull CPU state from the stack, then resume execution using those values.
        private void RTI(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StatusRegister = (byte)(StackPull(ref memory) & 0b11001111);
            byte destinationLower = StackPull(ref memory);
            byte destinationUpper = StackPull(ref memory);

            ushort destinationAddress = (ushort)(destinationUpper * 256 + destinationLower);

            ProgramCounter = destinationAddress;
        }

        //Push A - Push the value of the accumulator to the stack.
        private void PHA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StackPush(ref memory, Accumulator);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Pull A - Pull from the stack and set the accumulator to that value.
        private void PLA(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            Accumulator = StackPull(ref memory);

            SetStatusRegisterFlag('Z', Accumulator == 0);
            SetStatusRegisterFlag('N', IsNegative(Accumulator));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Push Processor Status - Push the value of the status register to the stack.
        private void PHP(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StackPush(ref memory, (byte)(StatusRegister | 0b00110000));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Pull Processor Status - Pull from the stack and set the status register to that value.
        private void PLP(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StatusRegister = (byte)((StackPull(ref memory) & 0b11001111) | (StatusRegister & 0b00110000));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer X to Stack Pointer - Copy the value of the X register to the stack pointer.
        private void TXS(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            StackPointer = XRegister;

            ProgramCounter += (ushort)(operands + 1);
        }

        //Transfer Stack Pointer to X - Copy the value of the stack pointer to the X register.
        private void TSX(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            XRegister = StackPointer;

            SetStatusRegisterFlag('Z', XRegister == 0);
            SetStatusRegisterFlag('N', IsNegative(XRegister));

            ProgramCounter += (ushort)(operands + 1);
        }

        //Clear Carry - Set the carry flag bit to 0.
        private void CLC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('C', false);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Set Carry - Set the carry flag bit to 1.
        private void SEC(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('C', true);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Clear Interrupt Disable - Set the interrupt disable flag bit to 0.
        private void CLI(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('I', false);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Set Interrupt Disable - Set the interrupt disable flag bit to 1.
        private void SEI(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('I', true);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Clear Decimal - Set the decimal mode flag bit to 0.
        private void CLD(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('D', false);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Set Decimal - Set the decimal mode flag bit to 1.
        private void SED(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('D', true);

            ProgramCounter += (ushort)(operands + 1);
        }

        //Clear Overflow - Set the overflow flag bit to 0.
        private void CLV(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
            SetStatusRegisterFlag('V', false);

            ProgramCounter += (ushort)(operands + 1);
        }

        //No Operation - Do nothing.
        private void NOP(ref byte[] memory, ref ushort data, int operands, bool isDirectValue)
        {
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

        private void Write(ref byte[] memory, int index, byte value)
        {
            if(index < 0x8000 || index > 0xFFFF)
            {
                memory[index] = value;
            }
        }

        //Returns the current value of a specific flag bit within the status register.
        //Valid inputs are chars N, V, B, D, I, Z, C
        public int GetStatusRegisterFlag(char flag)
        {
            byte flagMask = GetStatusRegisterFlagMask(flag);

            return ((StatusRegister & flagMask) == flagMask) ? 1 : 0;
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

        private void StackPush(ref byte[] memory, byte value)
        {
            ushort sourceAddress = (ushort)(0x0100 + StackPointer);

            memory[sourceAddress] = value;

            StackPointer--;
        }

        private byte StackPull(ref byte[] memory)
        {
            StackPointer++;

            ushort sourceAddress = (ushort)(0x0100 + StackPointer);

            return memory[sourceAddress];
        }

        //Checks if an unsigned byte is negative (the 7th bit is set).
        bool IsNegative(byte value)
        {
            return (value & 0b10000000) == 0b10000000 ? true : false;
        }
    }
}
