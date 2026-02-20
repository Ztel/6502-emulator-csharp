using System;

namespace Emulator6502
{
    internal class Disassembler
    {
        public static string currentInstruction = "";

        public static void GenerateAssembly(CPU cpu, string instruction, string addressingMode, ushort data)
        {
            string operand = "";

            switch (addressingMode)
            {
                case "Immediate":
                    operand = String.Format("#${0}", ((byte)data).ToString("X2"));
                    break;

                case "ZeroPage":
                case "ZeroPageX":
                case "ZeroPageY":
                    operand = String.Format("${0}", ((byte)data).ToString("X2"));
                    break;

                case "Relative":
                case "Absolute":
                case "AbsoluteX":
                case "AbsoluteY":
                case "Indirect":
                case "IndexedIndirect":
                case "IndirectIndexed":
                    operand = String.Format("${0}", data.ToString("X4"));
                    break;

                default:
                    break;
            }

            currentInstruction = instruction + " " + operand;
        }
    }
}
