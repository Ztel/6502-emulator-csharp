using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace Emulator6502
{
    public class MemoryRow : INotifyPropertyChanged
    {
        public int Address { get; }

        private string content;
        public string Content
        {
            get => content;
            set
            {
                if (this.content != value)
                {
                    this.content = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Content)));
                }
            }
        }

        public MemoryRow(int address, string content)
        {
            Address = address;
            this.content = content;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class MemoryViewerWindow : Window
    {
        readonly MainWindow mainWindow;
        private JumpToAddressWindow jumpToAddressWindow;

        ObservableCollection<MemoryRow> memoryRows = [];

        public MemoryViewerWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            jumpToAddressWindow = new JumpToAddressWindow(this);

            InitializeMemoryGrid();
            UpdateMemoryViewer();
        }

        private void InitializeMemoryGrid()
        {
            lbMemory.ItemsSource = memoryRows;

            if (memoryRows.Count == 0)
            {
                for (int i = 0; i < 4096; i++)
                {
                    memoryRows.Add(new MemoryRow(i * 16, ""));
                }
            }
        }

        public void UpdateMemoryViewer()
        {
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

                memoryRows[i].Content = rowBuilder.ToString();
            }
        }

        public void JumpToAddress(int address)
        {
            int rowIndex = address / 16;

            if (rowIndex >= 0 && rowIndex < memoryRows.Count)
            {
                lbMemory.SelectedIndex = rowIndex;
                lbMemory.ScrollIntoView(lbMemory.SelectedItem);
                jumpToAddressWindow.Close();
            }
            else
            {
                MessageBox.Show(String.Format("Address {0} not within range (0000-FFFF).", address.ToString("X2")), "Address Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnJumpToAddress_Click(object sender, RoutedEventArgs e)
        {
            jumpToAddressWindow.Close();
            jumpToAddressWindow = new JumpToAddressWindow(this);
            jumpToAddressWindow.Owner = this;
            jumpToAddressWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            jumpToAddressWindow.Show();
        }
    }
}
