namespace Emulator6502
{
    internal class Program
    {
        static Emulator emulator = new Emulator();

        static void Main(string[] args)
        {
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

            while (emulator.programActive)
            {
                HandleInput();

                if(emulator.programPaused)
                {

                }
                else
                {
                    emulator.StepFrame();
                }   
            }
        }

        static void HandleInput()
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
