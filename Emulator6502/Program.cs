namespace Emulator6502
{
    internal class Program
    {
        static Emulator emulator = new Emulator();

        static void Main(string[] args)
        {
            while (true)
            {
                if (emulator.programActive)
                {
                    try
                    {
                        HandleEmulatorInput();

                        if (!emulator.programPaused)
                        {
                            emulator.StepFrame();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine(ex.Message.ToString());
                        Console.SetCursorPosition(0, 0);
                    }
                }
                //Display a command line interface when the emulator is not active.
                else
                {
                    Console.CursorVisible = true;
                    Console.WriteLine("Emulator6502");
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine("   USAGE:\n      run <FILE> [FLAGS] [OPTIONS]");
                    Console.WriteLine("--------------------------------------------------------------------------");
                    Console.WriteLine("   ARGS:\n      <FILE>   relative or absolute path to rom binary file\n");
                    Console.WriteLine("   FLAGS:\n      -n, --no-debug   hide CPU debug UI\n      -p, --pause      start program paused\n");
                    Console.WriteLine("   OPTIONS:\n      -f, --fps <FRAMERATE>   target framerate (default 30)");
                    Console.WriteLine("---------------------------------------------------------------------------\n\n");

                    try
                    {
                        HandleCommandLineInput();
                    }
                    catch (Exception ex)
                    {
                        Console.SetCursorPosition(0, 15);
                        Console.WriteLine(ex.Message.ToString());
                        Console.SetCursorPosition(0, 0);
                    }
                }
            }
        }

        static void HandleCommandLineInput()
        {
            string input = Console.ReadLine().ToLower();

            string[] command = input.Split(' ');

            Console.Clear();

            if (command[0] != "run")
            {
                throw new ArgumentException(string.Format("\"{0}\" is not a valid command.", command[0]));
            }

            //Optional -fps parameter. Defaults to 30.
            int targetFPS = 30;

            emulator.LoadRom(command[1]);
            emulator.StartProgram(targetFPS);
        }

        static void HandleEmulatorInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch(key.Key)
                {
                    case ConsoleKey.Escape:
                        emulator.ExitProgram();
                        break;

                    case ConsoleKey.Spacebar:
                        if (emulator.programPaused)
                        {
                            emulator.programPaused = false;
                        }
                        else
                        {
                            emulator.PauseProgram();
                        }
                        break;

                    case ConsoleKey.Enter:
                        emulator.PauseProgram();
                        emulator.StepFrame();
                        break;

                    case ConsoleKey.Backspace:
                        emulator.PauseProgram();
                        emulator.StepInstruction();
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
