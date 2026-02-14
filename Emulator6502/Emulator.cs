using System.Text;

namespace Emulator6502
{
    public class Emulator
    {
        public CPU Cpu { get; set; } = new CPU();
        public Display Screen { get; set; } = new Display();

        private string programName = "";

        private int fps = 30;

        public bool programActive = false;
        public bool programPaused = true;



        public void LoadRom(string romFilePath)
        {
            byte[] rom = File.ReadAllBytes(romFilePath);
            programName = Path.GetFileName(romFilePath).ToLower();

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

        public void StartProgram(int framerate)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.SetWindowSize(102, 25);
            Console.Clear();

            programActive = true;
            programPaused = false;

            fps = framerate;

            Cpu.Reset();
        }

        public void ExitProgram()
        {
            programActive = false;
            Console.CursorVisible = true;
            Console.Clear();
        }

        public void PauseProgram()
        {
            programPaused = true;

            UpdateScreen(false);
        }

        public void StepFrame()
        {
            long lastFrameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            //Update the screen at the set framerate.
            while ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastFrameTime) < 1000.0 / fps)
            {
                Cpu.Step();
            }

            UpdateScreen(true);
        }

        public void StepInstruction()
        {
            Cpu.Step();
            UpdateScreen(false);
        }

        private void UpdateScreen(bool triggerNMI)
        {
            Screen.RenderUI(Cpu);

            Console.SetCursorPosition(0, 0);
            string statusHeader = programPaused ? programName + ": ▌▌ paused" : programName + ": ► running";
            Console.WriteLine(statusHeader);

            Screen.ReadDisplayBuffers(Cpu.Memory);
            Screen.RenderDisplay();

            Console.WriteLine(DateTime.Now.ToLongTimeString() + "\n\n\nESC: return to command line   SPACE: pause/play program   ENTER: step frame   BKSP: step instruction");

            if(triggerNMI)
            {
                Cpu.NMI();
            }
        }
    }
}
