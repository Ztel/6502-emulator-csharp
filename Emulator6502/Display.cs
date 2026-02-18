using System.Text;

namespace Emulator6502
{
    public class Display
    {
        readonly ushort backgroundBufferAddress = 0x2000;
        readonly ushort spriteBufferAddress = 0x2100;

        byte[] displayBuffer = new byte[256];
        byte[] spriteBuffer = new byte[256];


        public void ReadDisplayBuffers(byte[] memory)
        {
            for(int i  = 0; i < displayBuffer.Length; i++)
            {
                displayBuffer[i] = memory[backgroundBufferAddress + i];
            }

            for (int i = 0; i < spriteBuffer.Length; i++)
            {
                spriteBuffer[i] = memory[spriteBufferAddress + i];
            }

            //Overwrite background characters with sprite data. 
            for (int i = 0; i < spriteBuffer.Length; i += 2)
            {
                //Each sprite is two adjacent bytes.
                //First byte (i) is the position data, and the second byte is the ascii code
                //Sprites only override the background layer if their code is NOT 0 (null) 
                if (memory[spriteBufferAddress + i] != 0)
                {
                    displayBuffer[memory[spriteBufferAddress + i]] = memory[spriteBufferAddress + i + 1];
                }
            }
        }

        //Converts the display buffer into a 16x16 grid string (plus a border) to be printed to the console.
        public void RenderDisplay()
        {
            StringBuilder displayBuilder = new StringBuilder();

            displayBuilder.AppendLine("╔═════════════════════════════════╗");

            for (int i = 0; i < 16; i++)
            {
                displayBuilder.Append("║ ");

                for (int a = 0; a < 16; a++)
                {
                    //Replace ASCII control characters with spaces.
                    char character = (displayBuffer[(i * 16) + a] > 31) ? (char)displayBuffer[(i * 16) + a] : (char)0x20;
                    
                    displayBuilder.Append(character + " ");
                }

                displayBuilder.AppendLine("║");
            }

            displayBuilder.AppendLine("╚═════════════════════════════════╝");

            Console.CursorVisible = false;
            Console.Write(displayBuilder.ToString());
        }

        public void RenderUI(CPU cpu)
        {
            StringBuilder uiBuilder = new StringBuilder();

            uiBuilder.AppendLine(String.Format("                                           Program Counter:  ${0}        Accumulator:    ${1}\n", 
                cpu.ProgramCounter.ToString("X4"), 
                cpu.Accumulator.ToString("X2")));
            uiBuilder.AppendLine(String.Format("                                           X Register:       ${0}          Stack Pointer:  ${1}\n",
                cpu.XRegister.ToString("X2"), 
                cpu.StackPointer.ToString("X2")));
            uiBuilder.AppendLine(String.Format("                                           Y Register:       ${0}\n",
                cpu.YRegister.ToString("X2")));
            uiBuilder.AppendLine("                                           Status Register:");
            uiBuilder.AppendLine("                                           ┌───┬───┬───┬───┬───┬───┬───┬───┐");
            uiBuilder.AppendLine(String.Format("                                           │ {0} │ {1} │ 0 │ {2} │ {3} │ {4} │ {5} │ {6} │", 
                cpu.GetStatusRegisterFlag('N'),
                cpu.GetStatusRegisterFlag('V'),
                cpu.GetStatusRegisterFlag('B'),
                cpu.GetStatusRegisterFlag('D'),
                cpu.GetStatusRegisterFlag('I'),
                cpu.GetStatusRegisterFlag('Z'),
                cpu.GetStatusRegisterFlag('C')));
            uiBuilder.AppendLine("                                           └───┴───┴───┴───┴───┴───┴───┴───┘");
            uiBuilder.AppendLine("                                             │   │   │   │   │   │   │   └─ C - Carry");
            uiBuilder.AppendLine("                                             │   │   │   │   │   │   └───── Z - Zero");
            uiBuilder.AppendLine("                                             │   │   │   │   │   └───────── I - Interrupt Disable");
            uiBuilder.AppendLine("                                             │   │   │   │   └───────────── D - Decimal Mode");
            uiBuilder.AppendLine("                                             │   │   │   └───────────────── B - Break");
            uiBuilder.AppendLine("                                             │   │   └───────────────────── [ Not Used ]");
            uiBuilder.AppendLine("                                             │   └───────────────────────── V - Overflow");
            uiBuilder.AppendLine("                                             └───────────────────────────── N - Negative");

            Console.SetCursorPosition(0, 1);
            Console.WriteLine(uiBuilder.ToString());
        }
    }
}
