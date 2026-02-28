using System;
using System.ComponentModel;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Emulator6502
{
    public class Emulator
    {
        public CPU Cpu { get; set; } = new CPU();
        public Display Screen { get; set; } = new Display();

        private MainWindow mainWindow;

        public string ProgramName { get; set; }
        public string ProgramPath { get; set; }

        private int Fps { get; set => field = (value > 0) ? value : 0; }
        private int stepsPerFrame = 10000; 

        public bool programActive = false; 
        public bool programPaused = true;

        private ushort inputAddress = 0x4000;

        public Emulator(MainWindow window) 
        {
            mainWindow = window;
        }


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
        }

        public void StartProgram(int framerate, bool startPaused)
        {
            //Console.OutputEncoding = Encoding.UTF8;
            //Console.SetWindowSize(102, 25);
            //Console.Clear();

            programActive = true;
            programPaused = startPaused;
            Fps = framerate;

            Cpu.Reset();
            UpdateScreen(false);
        }

        public void ExitProgram()
        {
            programActive = false;
            programPaused = true;
            //Console.CursorVisible = true;
            //Console.Clear();
        }

        public void PauseProgram()
        {
            programPaused = true;

            UpdateScreen(false);
        }

        public void StepFrame()
        {
            long lastFrameTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            int stepsThisFrame = 0;

            while (stepsThisFrame < stepsPerFrame)
            {
                Cpu.Step();
                stepsThisFrame++;
            }

            //If there is spare time left in the frame, idle.
            while ((DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastFrameTime) < 1000.0 / Fps) { }

            UpdateScreen(true);
        }

        public void StepInstruction()
        {
            Cpu.Step();
            UpdateScreen(false);
        }

        private void UpdateScreen(bool triggerNMI)
        {
            //if (drawDebug)
            //{
            //    Screen.RenderUI(Cpu);
            //}

            //Console.SetCursorPosition(0, 0);
            //string statusHeader = programPaused ? programName + ": ▌▌ paused" : programName + ": ► running";
            //Console.WriteLine(statusHeader);

            Screen.ReadDisplayBuffers(Cpu.Memory);
            Screen.RenderDisplay();

            //Console.Write(new String(' ', Console.WindowWidth));
            //Console.WriteLine("\r" + Disassembler.currentInstruction);

            //Console.WriteLine(DateTime.Now.ToLongTimeString() + "\n\n\nESC: return to command line   SPACE: pause/play program   ENTER: step frame   BKSP: step instruction");

            //Execute UI update on the main thread.
            Application.Current.Dispatcher.Invoke(() => { mainWindow.UpdateDisplay(Screen.renderedDisplay); });

            if (triggerNMI)
            {
                Cpu.NMI();
            }
        }

        public void HandleEmulatorInput()
        {
            //Initialize memory-mapped inputs to 0
            for (int i = 0; i < 0xFF; i++)
            {
                Cpu.Memory[inputAddress + i] = 0;
            }

            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Escape:
                        ExitProgram();
                        break;

                    case ConsoleKey.Spacebar:
                        if (programPaused)
                        {
                            programPaused = false;
                        }
                        else
                        {
                            PauseProgram();
                        }
                        break;

                    case ConsoleKey.Enter:
                        PauseProgram();
                        StepFrame();
                        break;

                    case ConsoleKey.Backspace:
                        PauseProgram();
                        StepInstruction();
                        break;

                    //Memory-mapped program input
                    case ConsoleKey.UpArrow:
                        Cpu.Memory[inputAddress] |= 0b00001000;
                        break;

                    case ConsoleKey.DownArrow:
                        Cpu.Memory[inputAddress] |= 0b00000100;
                        break;

                    case ConsoleKey.LeftArrow:
                        Cpu.Memory[inputAddress] |= 0b00000010;
                        break;

                    case ConsoleKey.RightArrow:
                        Cpu.Memory[inputAddress] |= 0b00000001;
                        break;

                    default:
                        //Store keyboard inputs at the input start address + an offset of the ASCII code of the key pressed
                        if ((int)key.Key > 0 && (int)key.Key <= 0xFF)
                        {
                            Cpu.Memory[inputAddress + (byte)key.Key] = 1;
                        }                        
                        break;
                }
            }
        }
    }
}
