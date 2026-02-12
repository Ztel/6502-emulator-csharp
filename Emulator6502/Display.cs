using System.Text;

namespace Emulator6502
{
    public class Display
    {
        ushort backgroundBufferAddress = 0x2000;
        ushort spriteBufferAddress = 0x2100;

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
        public string RenderDisplay()
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

            displayBuilder.AppendLine(DateTime.Now.ToLongTimeString());

            return displayBuilder.ToString();
        }
    }
}
