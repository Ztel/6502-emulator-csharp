namespace Emulator6502
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] memory = new byte[65536];
            memory[0] = 0xA9;
            memory[1] = 0xFF;
            memory[2] = 0x8D;
            memory[3] = 0x01;
            memory[4] = 0xFF;
            CPU cpu = new CPU();
            cpu.ProgramCounter = 0;
            Console.WriteLine(cpu.Accumulator);
            cpu.Step(ref memory);
            Console.WriteLine(cpu.Accumulator);
            cpu.Step(ref memory);
            Console.WriteLine(cpu.ProgramCounter);
            Console.WriteLine(memory[0xFF01]);
        }
    }
}
