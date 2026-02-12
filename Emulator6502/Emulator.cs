namespace Emulator6502
{
    public class Emulator
    {
        public CPU Cpu { get; set; } = new CPU();
        public Display Screen { get; set; } = new Display();

        private string displayString = "";

        private int stepsPerFrame = 3200;
        private int stepsUntilNewFrame;

        public bool stepMode = false;
        private bool executeProgram = false;



        public void LoadRom(string romFilePath)
        {
            byte[] rom = File.ReadAllBytes(romFilePath);

            if (rom.Length != 8192 && rom.Length != 16384 && rom.Length != 32768)
            {
                throw new ArgumentException(string.Format("File '{0}' is an unsupported size ({1} bytes).\nSupported file sizes are 8KB (8192 bytes), 16KB (16284 bytes), and 32KB (32768 bytes).", romFilePath, rom.Length));
            }

            //Mirror copies of ROM if it is 8KB or 16KB to place the vectors at the correct location in memory.
            for (int i = 0; i < 32768; i++)
            {
                Cpu.Memory[i + 0x8000] = rom[i % rom.Length];
            }
        }

        public void StartProgram()
        {
            executeProgram = true;
            Console.CursorVisible = false;

            Cpu.Reset();
            RunProgram();
        }

        public void HaltProgram()
        {
            executeProgram = false;
            Console.CursorVisible = true;
        }

        private void RunProgram()
        {
            while (executeProgram)
            {
                Cpu.Step();
                stepsUntilNewFrame--;

                Console.SetCursorPosition(0, 0);
                Console.Write("PC: $" + Cpu.ProgramCounter.ToString("X4") + "       A: $" + Cpu.Accumulator.ToString("X2") + "   X: $" + Cpu.XRegister.ToString("X2") + "   Y: $" + Cpu.YRegister.ToString("X2"));

                if (stepsUntilNewFrame <= 0)
                {
                    UpdateScreen();

                    stepsUntilNewFrame = stepsPerFrame;
                }

                if(stepMode)
                {
                    executeProgram = false;
                }
            }
        }

        private void UpdateScreen()
        {
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("PC: $" + Cpu.ProgramCounter.ToString("X4") + "       A: $" + Cpu.Accumulator.ToString("X2") + "   X: $" + Cpu.XRegister.ToString("X2") + "   Y: $" + Cpu.YRegister.ToString("X2"));

            Screen.ReadDisplayBuffers(Cpu.Memory);
            displayString = Screen.RenderDisplay();

            Console.WriteLine(displayString);

            Cpu.NMI();
        }
    }
}
