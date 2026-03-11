using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;

namespace Emulator6502
{
    public class Emulator()
    {
        public CPU Cpu { get; set; } = new CPU();
        public Display Screen { get; set; } = new Display();

        public string ProgramName { get; set; }
        public string ProgramPath { get; set; }

        //Prevent unreasonable framerates by clamping between 1 and 999.
        public int Fps { get; set => field = Math.Clamp(value, 1, 999); } = 30;
        private const int stepsPerFrame = 10000;
        private long lastFrameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        public bool programActive = false; 
        public bool programPaused = false;

        private const ushort inputAddress = 0x4000;

        private long fpsTrackerStartTime;
        private int frameCount = 0;
        private long realFPS;

        public void LoadRom(string romFilePath)
        {
            Cpu = new CPU();
            byte[] rom = File.ReadAllBytes(romFilePath);
            ProgramName = Path.GetFileName(romFilePath).ToLower();
            ProgramPath = romFilePath;
            string programExtension = Path.GetExtension(romFilePath);

            if (programExtension != ".bin")
            {
                throw new ArgumentException(String.Format("{0} files are not supported. Please select a .bin file.", programExtension));
            }

            if (rom.Length != 8192 && rom.Length != 16384 && rom.Length != 32768)
            {
                throw new ArgumentException(string.Format("'{0}' is an unsupported size ({1} bytes).\nSupported file sizes are 8KB (8192 bytes), 16KB (16284 bytes), and 32KB (32768 bytes).", romFilePath, rom.Length));
            }

            //Mirror copies of ROM if it is 8KB or 16KB to place the vectors at the correct location in memory.
            for (int i = 0; i < 32768; i++)
            {
                Cpu.Memory[i + 0x8000] = rom[i % rom.Length];
            }

            Disassembler.DisassembleRom(Cpu);
        }

        public void StartProgram(bool startPaused)
        {
            programActive = true;
            programPaused = true;

            Thread.Sleep(100);
            fpsTrackerStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            Cpu.Reset();
            Thread.Sleep(100);
            programPaused = startPaused;
            UpdateScreen(triggerNMI: false);
        }

        public void ExitProgram()
        {
            programActive = false;
            Thread.Sleep(1000 / Fps + 10); //Give one frame for the CPU thread to halt before clearing the screen.
            Screen.RenderedDisplay = "";
        }

        public void PauseProgram()
        {
            programPaused = true;

            UpdateScreen(triggerNMI: false);
        }

        public void StepFrame()
        {
            int stepsThisFrame = 0;

            while (stepsThisFrame < stepsPerFrame)
            {
                Cpu.Step();
                stepsThisFrame++;
            }

            //If there is spare time left in the frame, idle.
            while ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastFrameTime) < 1000.0 / Fps) { }

            UpdateScreen(triggerNMI: true);

            lastFrameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            //Track actual FPS (in 2-second periods) for benchmarking performance.
            frameCount++;
            if ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - fpsTrackerStartTime) / 1000 > 2)
            {
                realFPS = frameCount / ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - fpsTrackerStartTime) / 1000);
                Debug.WriteLine("FPS: " + realFPS);

                fpsTrackerStartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                frameCount = 0;
            }
        }

        public void StepInstruction()
        {
            Cpu.Step();
            UpdateScreen(triggerNMI: false);
        }

        private void UpdateScreen(bool triggerNMI)
        {
            Screen.ReadDisplayBuffers(Cpu.Memory);
            Screen.RenderDisplay();

            if (triggerNMI)
            {
                Cpu.NMI();
            }
        }

        public void KeyChanged(Key key, bool keyIsPressed)
        {
            byte keyValue = (byte)(keyIsPressed ? 1 : 0);

            _ = key switch
            {
                Key.Up => Cpu.Memory[inputAddress] = keyValue,
                Key.Down => Cpu.Memory[inputAddress + 1] = keyValue,
                Key.Left => Cpu.Memory[inputAddress + 2] = keyValue,
                Key.Right => Cpu.Memory[inputAddress + 3] = keyValue,
                _ => Cpu.Memory[inputAddress + (byte)key] = keyValue
            };
        }

        public void ClearKeyboardInput()
        {
            for(int i = 0; i < 256; i++)
            {
                Cpu.Memory[inputAddress + i] = 0;
            }
        }
    }
}
