using System;
using System.ComponentModel;
using System.Text;

namespace Emulator6502
{
    public class Display : INotifyPropertyChanged
    {
        readonly ushort backgroundBufferAddress = 0x2000;
        readonly ushort spriteBufferAddress = 0x2100;

        byte[] displayBuffer = new byte[256];
        byte[] spriteBuffer = new byte[256];

        public event PropertyChangedEventHandler PropertyChanged;

        private string renderedDisplay;

        public string RenderedDisplay
        {
            get { return renderedDisplay; }
            set
            {
                if (renderedDisplay != value)
                {
                    renderedDisplay = value;
                    OnPropertyChanged("RenderedDisplay");
                }
            }
        }


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

        //Converts the display buffer into a 16x16 grid string to be printed to the console.
        public void RenderDisplay()
        {
            StringBuilder displayBuilder = new StringBuilder();

            for (int i = 0; i < 16; i++)
            {
                for (int a = 0; a < 16; a++)
                {
                    //Replace ASCII control characters with spaces.
                    char character = (displayBuffer[(i * 16) + a] > 31) ? (char)displayBuffer[(i * 16) + a] : (char)0x20;

                    displayBuilder.Append(character + ((a < 15) ? " " : ""));
                }

                if (i < 15) 
                {
                    displayBuilder.Append("\n");
                }
            }

            RenderedDisplay = displayBuilder.ToString();
        }

        public void ClearDisplay()
        {
            RenderedDisplay = "";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
