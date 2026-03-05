using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Emulator6502
{
    public class StackEntry : INotifyPropertyChanged
    {
        public int Address { get; }

        private byte _value;
        public byte Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                }
            }
        }

        public StackEntry(int address, byte value)
        {
            Address = address;
            _value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public partial class DebugWindow : Window
    {
        readonly MainWindow mainWindow;

        ObservableCollection<StackEntry> stackItems = [];

        private void InitializeStackList()
        {
            lbStack.ItemsSource = stackItems;

            if (stackItems.Count == 0)
            {
                for (int i = 0; i < 256; i++)
                {
                    stackItems.Add(new StackEntry(0x0100 + i, 0));
                }
            }
        }

        public DebugWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            lbDisassembly.ItemsSource = Disassembler.disassembly;

            InitializeStackList();
            UpdateUIPauseStatus();
        }

        public void UpdateUIPauseStatus()
        {
            bool isPaused = mainWindow.emulator.programPaused;

            btnPause.Content = isPaused ? "Resume" : "Pause";
            tbProgramCounter.IsEnabled = isPaused;
            tbAccumulator.IsEnabled = isPaused;
            tbXRegister.IsEnabled = isPaused;
            tbYRegister.IsEnabled = isPaused;
            tbStatusRegister.IsEnabled = isPaused;
            tbStackPointer.IsEnabled = isPaused;
            cbBreak.IsEnabled = isPaused;
            cbCarry.IsEnabled = isPaused;
            cbDecimal.IsEnabled = isPaused;
            cbInterrupt.IsEnabled = isPaused;
            cbNegative.IsEnabled = isPaused;
            cbOverflow.IsEnabled = isPaused;
            cbZero.IsEnabled = isPaused;
        }

        public void UpdateDebugWindow()
        {
            CPU cpu = mainWindow.emulator.Cpu;

            if (mainWindow.emulator.programPaused && Disassembler.disassembly != null)
            {
                if (Disassembler.disassembly.ContainsKey(cpu.ProgramCounter) /*&& lbDisassembly.Items.Contains(new KeyValuePair<ushort, Tuple<string, string>>(cpu.ProgramCounter, Disassembler.disassembly[cpu.ProgramCounter]))*/)
                {
                    lbDisassembly.SelectedItem = new KeyValuePair<ushort, Tuple<string, string>>(cpu.ProgramCounter, Disassembler.disassembly[cpu.ProgramCounter]);
                    lbDisassembly.ScrollIntoView(lbDisassembly.SelectedItem);
                }
                else
                {
                    lbDisassembly.SelectedItem = null;
                }
            }

            tbProgramCounter.Text = string.Format("{0:X4}", cpu.ProgramCounter);
            tbAccumulator.Text = string.Format("{0:X2}", cpu.Accumulator);
            tbXRegister.Text = string.Format("{0:X2}", cpu.XRegister);
            tbYRegister.Text = string.Format("{0:X2}", cpu.YRegister);
            tbStatusRegister.Text = string.Format("{0:X2}", cpu.StatusRegister);
            tbStackPointer.Text = string.Format("{0:X2}", cpu.StackPointer);

            cbBreak.IsChecked = cpu.GetStatusRegisterFlag('B') == 1;
            cbCarry.IsChecked = cpu.GetStatusRegisterFlag('C') == 1;
            cbDecimal.IsChecked = cpu.GetStatusRegisterFlag('D') == 1;
            cbInterrupt.IsChecked = cpu.GetStatusRegisterFlag('I') == 1;
            cbNegative.IsChecked = cpu.GetStatusRegisterFlag('N') == 1;
            cbOverflow.IsChecked = cpu.GetStatusRegisterFlag('V') == 1;
            cbZero.IsChecked = cpu.GetStatusRegisterFlag('Z') == 1;

            for (int i = 0; i < 256; i++)
            {
                stackItems[i].Value = cpu.Memory[0x0100 + i];
            }

            lbStack.SelectedIndex = cpu.StackPointer;

            if (mainWindow.emulator.programPaused)
            {
                lbStack.ScrollIntoView(lbStack.SelectedItem);
            }
        }

        public void ToggleRuntimeButtons(bool enabled)
        {
            btnReset.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnStepFrame.IsEnabled = enabled;
            btnStepInstruction.IsEnabled = enabled;
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.BtnReset_Click(sender, e);
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.BtnPause_Click(sender, e);
        }

        private void BtnStepFrame_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.BtnStepFrame_Click(sender, e);
        }

        private void BtnStepInstruction_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.BtnStepInstruction_Click(sender, e);
        }
    }
}
