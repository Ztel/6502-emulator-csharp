namespace Emulator6502
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Emulator emulator = new Emulator();
            CPU cpu = new CPU();
            byte[] rom = [0xEA, 0xA9];
            ushort index = 0;

            Console.WriteLine(emulator.GetInstruction(rom, index, cpu.Opcodes)[0]);
        }
    }
}
