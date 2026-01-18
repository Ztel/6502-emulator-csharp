namespace Emulator6502
{
    public class Emulator
    {
        //Given an index in the ROM, returns an array containing the bytes for the instruction at that location.
        //Uses the opcode dictionary to look up the length of the given instruction in bytes.
        //Valid indices are indices that fall within the ROM and point to a valid opcode.
        public byte[] GetInstruction(byte[] rom, ushort index, Dictionary<byte, int> opcodes)
        {
            if (index < 0 || index >= rom.Length)
            {
                throw new ArgumentOutOfRangeException(String.Format("Attempted to retrieve instruction at index {0}, which is out of bounds of the ROM (0 - {1}).", index, rom.Length));
            }

            //Look up the opcode to check if it is valid and to see how many operand bytes it requires
            byte opcode = rom[index];

            if (opcodes.ContainsKey(opcode))
            {
                int operands = opcodes[opcode];
                byte[] instruction = new byte[operands + 1];

                //Append the operand bytes to the opcode to build the instruction array
                for(int i = 0; i < instruction.Length; i++)
                {
                    instruction[i] = rom[index + i];
                }

                return instruction;
            }
            else
            {
                throw new ArgumentException(String.Format("Attempted to parse invalid opcode {0} at index {1}.", rom[index], index));
            }
        }
    }
}
