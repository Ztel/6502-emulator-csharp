using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Emulator6502
{
    internal class Disassembler
    {
        public static Dictionary<ushort, Tuple<string, string>> disassembly; //Have to use the old Tuple<T,T> syntax to generate class properties to be bound to the UI.

        private static CPU cpu;


        public static void DisassembleRom(CPU cpu)
        {
            Disassembler.cpu = cpu;
            disassembly = new Dictionary<ushort, Tuple<string, string>>();
            int i = 0x8000;

            while (i <= 0xFFFF)
            {
                byte opcode = cpu.Memory[i];
                (string operation, string addressingMode) instruction = GetInstruction(opcode);
                int instructionLength = GetInstructionLength(instruction.addressingMode);

                string assemblyInstruction = "";
                string hexInstruction = "";

                assemblyInstruction = DisassembleInstruction(instruction.operation, (ushort)i, instruction.addressingMode);

                for (int a = 0; a < instructionLength; a++)
                {
                    hexInstruction += cpu.Memory[i + a].ToString("X2") + " ";
                }

                disassembly.Add((ushort)i, new Tuple<string, string>(hexInstruction, assemblyInstruction));

                i += (ushort)instructionLength;
            }
        }

        private static string DisassembleInstruction(string instruction, ushort romAddress, string addressingMode)
        {
            return instruction + " " + GetOperand(romAddress, addressingMode);
        }

        private static int GetInstructionLength(string addressingMode) => addressingMode switch
        {
            "Immediate" or "ZeroPage" or "ZeroPageX" or "ZeroPageY" or "Relative" or "IndexedIndirect" or "IndirectIndexed" => 2,
            "Absolute" or "AbsoluteX" or "AbsoluteY" or "Indirect" => 3,
            _ => 1
        };

        private static string GetOperand(ushort pc, string addressingMode) => addressingMode switch
        {
            "Immediate" =>          String.Format("#${0}", (cpu.Memory[pc + 1]).ToString("X2")),
            "ZeroPage" =>           String.Format("${0}", cpu.Memory[pc + 1].ToString("X2")),
            "ZeroPageX" =>          String.Format("${0},X", cpu.Memory[pc + 1].ToString("X2")),
            "ZeroPageY" =>          String.Format("${0},Y", cpu.Memory[pc + 1].ToString("X2")),
            "Absolute" =>           String.Format("${0}", (cpu.Memory[pc + 1] | (cpu.Memory[pc + 2] << 8)).ToString("X4")),
            "AbsoluteX" =>          String.Format("${0},X", (cpu.Memory[pc + 1] | (cpu.Memory[pc + 2] << 8)).ToString("X4")),
            "AbsoluteY" =>          String.Format("${0},Y", (cpu.Memory[pc + 1] | (cpu.Memory[pc + 2] << 8)).ToString("X4")),
            "Indirect" =>           String.Format("(${0})", (cpu.Memory[pc + 1] | (cpu.Memory[pc + 2] << 8)).ToString("X4")),
            "IndexedIndirect" =>    String.Format("(${0},X)", cpu.Memory[pc + 1].ToString("X2")),
            "IndirectIndexed" =>    String.Format("(${0}),Y", cpu.Memory[pc + 1].ToString("X2")),
            "WithAccumulator" =>    "A",
            "Relative" =>           String.Format("${0}", ((sbyte)cpu.Memory[pc + 1] + pc + 2).ToString("X4")),
            _ => ""
        };

        private static (string, string) GetInstruction(ushort opcode) => opcode switch
        {
            0xA9 => ("LDA", "Immediate"),
            0xA5 => ("LDA", "ZeroPage"),
            0xB5 => ("LDA", "ZeroPageX"),
            0xAD => ("LDA", "Absolute"),
            0xBD => ("LDA", "AbsoluteX"),
            0xB9 => ("LDA", "AbsoluteY"),
            0xA1 => ("LDA", "IndexedIndirect"),
            0xB1 => ("LDA", "IndirectIndexed"),

            0x85 => ("STA", "ZeroPage"),
            0x95 => ("STA", "ZeroPageX"),
            0x8D => ("STA", "Absolute"),
            0x9D => ("STA", "AbsoluteX"),
            0x99 => ("STA", "AbsoluteY"),
            0x81 => ("STA", "IndexedIndirect"),
            0x91 => ("STA", "IndirectIndexed"),

            0xA2 => ("LDX", "Immediate"),
            0xA6 => ("LDX", "ZeroPage"),
            0xB6 => ("LDX", "ZeroPageY"),
            0xAE => ("LDX", "Absolute"),
            0xBE => ("LDX", "AbsoluteY"),

            0x86 => ("STX", "ZeroPage"),
            0x96 => ("STX", "ZeroPageY"),
            0x8E => ("STX", "Absolute"),

            0xA0 => ("LDY", "Immediate"),
            0xA4 => ("LDY", "ZeroPage"),
            0xB4 => ("LDY", "ZeroPageX"),
            0xAC => ("LDY", "Absolute"),
            0xBC => ("LDY", "AbsoluteX"),

            0x84 => ("STY", "ZeroPage"),
            0x94 => ("STY", "ZeroPageX"),
            0x8C => ("STY", "Absolute"),

            0xAA => ("TAX", "Implicit"),

            0x8A => ("TXA", "Implicit"),

            0xA8 => ("TAY", "Implicit"),

            0x98 => ("TYA", "Implicit"),

            0x69 => ("ADC", "Immediate"),
            0x65 => ("ADC", "ZeroPage"),
            0x75 => ("ADC", "ZeroPageX"),
            0x6D => ("ADC", "Absolute"),
            0x7D => ("ADC", "AbsoluteX"),
            0x79 => ("ADC", "AbsoluteY"),
            0x61 => ("ADC", "IndexedIndirect"),
            0x71 => ("ADC", "IndirectIndexed"),

            0xE9 => ("SBC", "Immediate"),
            0xE5 => ("SBC", "ZeroPage"),
            0xF5 => ("SBC", "ZeroPageX"),
            0xED => ("SBC", "Absolute"),
            0xFD => ("SBC", "AbsoluteX"),
            0xF9 => ("SBC", "AbsoluteY"),
            0xE1 => ("SBC", "IndexedIndirect"),
            0xF1 => ("SBC", "IndirectIndexed"),

            0xE6 => ("INC", "ZeroPage"),
            0xF6 => ("INC", "ZeroPageX"),
            0xEE => ("INC", "Absolute"),
            0xFE => ("INC", "AbsoluteX"),

            0xC6 => ("DEC", "ZeroPage"),
            0xD6 => ("DEC", "ZeroPageX"),
            0xCE => ("DEC", "Absolute"),
            0xDE => ("DEC", "AbsoluteX"),

            0xE8 => ("INX", "Implicit"),

            0xCA => ("DEX", "Implicit"),

            0xC8 => ("INY", "Implicit"),

            0x88 => ("DEY", "Implicit"),

            0x0A => ("ASL", "WithAccumulator"),
            0x06 => ("ASL", "ZeroPage"),
            0x16 => ("ASL", "ZeroPageX"),
            0x0E => ("ASL", "Absolute"),
            0x1E => ("ASL", "AbsoluteX"),

            0x4A => ("LSR", "WithAccumulator"),
            0x46 => ("LSR", "ZeroPage"),
            0x56 => ("LSR", "ZeroPageX"),
            0x4E => ("LSR", "Absolute"),
            0x5E => ("LSR", "AbsoluteX"),

            0x2A => ("ROL", "WithAccumulator"),
            0x26 => ("ROL", "ZeroPage"),
            0x36 => ("ROL", "ZeroPageX"),
            0x2E => ("ROL", "Absolute"),
            0x3E => ("ROL", "AbsoluteX"),

            0x6A => ("ROR", "WithAccumulator"),
            0x66 => ("ROR", "ZeroPage"),
            0x76 => ("ROR", "ZeroPageX"),
            0x6E => ("ROR", "Absolute"),
            0x7E => ("ROR", "AbsoluteX"),

            0x29 => ("AND", "Immediate"),
            0x25 => ("AND", "ZeroPage"),
            0x35 => ("AND", "ZeroPageX"),
            0x2D => ("AND", "Absolute"),
            0x3D => ("AND", "AbsoluteX"),
            0x39 => ("AND", "AbsoluteY"),
            0x21 => ("AND", "IndexedIndirect"),
            0x31 => ("AND", "IndirectIndexed"),

            0x09 => ("ORA", "Immediate"),
            0x05 => ("ORA", "ZeroPage"),
            0x15 => ("ORA", "ZeroPageX"),
            0x0D => ("ORA", "Absolute"),
            0x1D => ("ORA", "AbsoluteX"),
            0x19 => ("ORA", "AbsoluteY"),
            0x01 => ("ORA", "IndexedIndirect"),
            0x11 => ("ORA", "IndirectIndexed"),

            0x49 => ("EOR", "Immediate"),
            0x45 => ("EOR", "ZeroPage"),
            0x55 => ("EOR", "ZeroPageX"),
            0x4D => ("EOR", "Absolute"),
            0x5D => ("EOR", "AbsoluteX"),
            0x59 => ("EOR", "AbsoluteY"),
            0x41 => ("EOR", "IndexedIndirect"),
            0x51 => ("EOR", "IndirectIndexed"),

            0x24 => ("BIT", "ZeroPage"),
            0x2C => ("BIT", "Absolute"),

            0xC9 => ("CMP", "Immediate"),
            0xC5 => ("CMP", "ZeroPage"),
            0xD5 => ("CMP", "ZeroPageX"),
            0xCD => ("CMP", "Absolute"),
            0xDD => ("CMP", "AbsoluteX"),
            0xD9 => ("CMP", "AbsoluteY"),
            0xC1 => ("CMP", "IndexedIndirect"),
            0xD1 => ("CMP", "IndirectIndexed"),

            0xE0 => ("CPX", "Immediate"),
            0xE4 => ("CPX", "ZeroPage"),
            0xEC => ("CPX", "Absolute"),

            0xC0 => ("CPY", "Immediate"),
            0xC4 => ("CPY", "ZeroPage"),
            0xCC => ("CPY", "Absolute"),

            0x90 => ("BCC", "Relative"),

            0xB0 => ("BCS", "Relative"),

            0xF0 => ("BEQ", "Relative"),

            0xD0 => ("BNE", "Relative"),

            0x10 => ("BPL", "Relative"),

            0x30 => ("BMI", "Relative"),

            0x50 => ("BVC", "Relative"),

            0x70 => ("BVS", "Relative"),

            0x4C => ("JMP", "Absolute"),
            0x6C => ("JMP", "Indirect"),

            0x20 => ("JSR", "Absolute"),

            0x60 => ("RTS", "Implicit"),

            0x00 => ("BRK", "Implicit"),

            0x40 => ("RTI", "Implicit"),

            0x48 => ("PHA", "Implicit"),

            0x68 => ("PLA", "Implicit"),

            0x08 => ("PHP", "Implicit"),

            0x28 => ("PLP", "Implicit"),

            0x9A => ("TXS", "Implicit"),

            0xBA => ("TSX", "Implicit"),

            0x18 => ("CLC", "Implicit"),

            0x38 => ("SEC", "Implicit"),

            0x58 => ("CLI", "Implicit"),

            0x78 => ("SEI", "Implicit"),

            0xD8 => ("CLD", "Implicit"),

            0xF8 => ("SED", "Implicit"),

            0xB8 => ("CLV", "Implicit"),

            0xEA => ("NOP", "Implicit"),

            _    => ("???", "Implicit")
        };
    }
}
