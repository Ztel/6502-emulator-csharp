using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Windows;

namespace Emulator6502
{
    public partial class MemoryViewerWindow : Window
    {
        readonly MainWindow mainWindow;
        const int UIUpdatesPerSecond = 10;
        long lastUIUpdateTime;

        public ObservableCollection<Tuple<int, string>> MemoryGrid { get; set; } = []; //Have to use the old Tuple<T,T> syntax to generate class properties to be bound to the UI.


        public MemoryViewerWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            InitializeMemoryGrid();
            lbMemory.ItemsSource = MemoryGrid;
        }

        private void InitializeMemoryGrid()
        {
            byte[] memory = mainWindow.emulator.Cpu.Memory;

            for (int i = 0; i < 4096; i++)
            {
                if (MemoryGrid.Count < 4096)
                {
                    MemoryGrid.Add(new Tuple<int, string>(i * 16, ""));
                }
                else
                {
                    MemoryGrid[i] = new Tuple<int, string>(i * 16, "");
                }
            }
        }

        public void UpdateMemoryViewer(bool forceUpdate = false)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (forceUpdate == false && currentTime - lastUIUpdateTime < (1000 / UIUpdatesPerSecond))
            {
                return; //Return early if it's not time to update the UI yet.
            }

            byte[] memory = mainWindow.emulator.Cpu.Memory;

            StringBuilder rowBuilder = new();

            for (int i = 0; i < 4096; i++)
            {
                rowBuilder.Clear();

                rowBuilder.Append("    ");

                for (int a = 0; a < 16; a++)
                {
                    rowBuilder.Append(memory[i * 16 + a].ToString("X2"));
                    rowBuilder.Append(' ');
                }

                rowBuilder.Append("   ");

                //Display the ASCII characters for the memory values, replacing non-printable characters with a dot.
                for (int a = 0; a < 16; a++)
                {
                    char memoryChar = (char)memory[i * 16 + a];

                    if (char.IsControl(memoryChar) || memoryChar == 173) //ASCII 173 does not seem to work properly in .NET WPF for some reason, so replace it with a dot as well.
                    {
                        rowBuilder.Append('.');
                    }
                    else
                    {
                        rowBuilder.Append((char)memory[i * 16 + a]);
                    }

                    rowBuilder.Append(' ');
                }

                MemoryGrid[i] = new Tuple<int, string>(i * 16, rowBuilder.ToString());
            }

            lastUIUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
