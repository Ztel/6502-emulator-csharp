namespace Emulator6502
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Emulator emulator = new Emulator();

            string romFilePath = @"C:\\users\admin\source\repos\emulator6502\exampleprograms\test.bin";

            try
            {
                emulator.LoadRom(romFilePath);
                emulator.StartProgram();
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message.ToString());
            }
        }
    }
}
