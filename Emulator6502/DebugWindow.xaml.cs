using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public DebugWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            lbDisassembly.ItemsSource = Disassembler.disassembly;

            InitializeStackList();
            UpdateUIPauseStatus();

            if (mainWindow.emulator.programPaused)
            {
                ScrollSelectionsIntoView();
            }
        }

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

            lbStack.ScrollIntoView(lbStack.Items[lbStack.Items.Count - 1]);
        }

        public void RefreshDisassemblyList()
        {
            lbDisassembly.ItemsSource = null;
            lbDisassembly.ItemsSource = Disassembler.disassembly;
        }

        public void UpdateListBoxSelections()
        {
            CPU cpu = mainWindow.emulator.Cpu;

            try
            {
                lbStack.SelectedItem = (cpu.StackPointer < lbStack.Items.Count) ? lbStack.Items[cpu.StackPointer] : null;

                if (Disassembler.disassembly != null && Disassembler.disassembly.ContainsKey(cpu.ProgramCounter))
                {
                    lbDisassembly.SelectedItem = new KeyValuePair<ushort, Tuple<string, string>>(cpu.ProgramCounter, Disassembler.disassembly[cpu.ProgramCounter]);
                }
                else
                {
                    lbDisassembly.SelectedItem = null;
                }
            }
            catch (KeyNotFoundException)
            {
                //Updating selections can rarely throw a KeyNotFound exception due to other threads causing a momentary desync between the source dictionary and the UI items.
                //This exception can be safely swallowed, so just ignore it and continue.
            }
        }

        public void ScrollSelectionsIntoView()
        {
            UpdateListBoxSelections();

            //If possible, scroll the stack list so that the items above and below the selected item are also visible.
            lbStack.ScrollIntoView((lbStack.SelectedIndex + 1 < lbStack.Items.Count) ? lbStack.Items[lbStack.SelectedIndex + 1] : lbStack.Items[lbStack.SelectedIndex]);
            lbStack.ScrollIntoView((lbStack.SelectedIndex - 1 >= 0) ? lbStack.Items[lbStack.SelectedIndex - 1] : lbStack.Items[lbStack.SelectedIndex]);

            lbDisassembly.ScrollIntoView(lbDisassembly.SelectedItem);
        }

        public void UpdateDebugWindow()
        {
            CPU cpu = mainWindow.emulator.Cpu;

            for (int i = 0; i < 256; i++)
            {
                stackItems[i].Value = cpu.Memory[0x0100 + i];
            }

            Application.Current?.Dispatcher.Invoke(UpdateDebugUIControls); //Invoke the UpdateDebugUIControls method on the UI thread.
        }

        public void ToggleRuntimeButtons(bool enabled)
        {
            btnReset.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnStepFrame.IsEnabled = enabled;
            btnStepInstruction.IsEnabled = enabled;
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

        private void UpdateDebugUIControls()
        {
            CPU cpu = mainWindow.emulator.Cpu;

            if (!tbProgramCounter.IsFocused) { tbProgramCounter.Text = string.Format("{0:X4}", cpu.ProgramCounter); }
            if (!tbAccumulator.IsFocused) { tbAccumulator.Text = string.Format("{0:X2}", cpu.Accumulator); }
            if (!tbXRegister.IsFocused) { tbXRegister.Text = string.Format("{0:X2}", cpu.XRegister); }
            if (!tbYRegister.IsFocused) { tbYRegister.Text = string.Format("{0:X2}", cpu.YRegister); }
            if (!tbStatusRegister.IsFocused) { tbStatusRegister.Text = string.Format("{0:X2}", cpu.StatusRegister); }
            if (!tbStackPointer.IsFocused) { tbStackPointer.Text = string.Format("{0:X2}", cpu.StackPointer); }

            cbBreak.IsChecked = cpu.GetStatusRegisterFlag('B') == 1;
            cbCarry.IsChecked = cpu.GetStatusRegisterFlag('C') == 1;
            cbDecimal.IsChecked = cpu.GetStatusRegisterFlag('D') == 1;
            cbInterrupt.IsChecked = cpu.GetStatusRegisterFlag('I') == 1;
            cbNegative.IsChecked = cpu.GetStatusRegisterFlag('N') == 1;
            cbOverflow.IsChecked = cpu.GetStatusRegisterFlag('V') == 1;
            cbZero.IsChecked = cpu.GetStatusRegisterFlag('Z') == 1;           

            if(!mainWindow.emulator.programPaused)
            {
                lbDisassembly.SelectedItem = null;
                lbStack.SelectedItem = null;
            }
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

        private void CbFlag_Click(object sender, RoutedEventArgs e)
        {
            CheckBox element = (CheckBox)sender;

            char flag = element.Name switch
            {
                "cbCarry" => 'C',
                "cbZero" => 'Z',
                "cbInterrupt" => 'I',
                "cbDecimal" => 'D',
                "cbBreak" => 'B',
                "cbOverflow" => 'V',
                "cbNegative" => 'N',
                _ => throw new ArgumentException(String.Format("Invalid checkbox clicked.", element.Name))
            };

            mainWindow.emulator.Cpu.SetStatusRegisterFlag(flag, element.IsChecked == true);
        }

        private void Tb_Submit(object sender, RoutedEventArgs e)
        {
            CPU cpu = mainWindow.emulator.Cpu;
            TextBox textBox = (TextBox)sender;
            string input = textBox.Text.Trim();

            if (int.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int newValue))
            {
                _ = textBox.Name switch
                {
                    "tbProgramCounter" => cpu.ProgramCounter = (ushort)newValue,
                    "tbAccumulator" => cpu.Accumulator = (byte)newValue,
                    "tbXRegister" => cpu.XRegister = (byte)newValue,
                    "tbYRegister" => cpu.YRegister = (byte)newValue,
                    "tbStatusRegister" => cpu.StatusRegister = (byte)newValue,
                    "tbStackPointer" => cpu.StackPointer = (byte)newValue,
                    _ => throw new ArgumentException(String.Format("Invalid textbox {0} submitted.", textBox.Name))
                };
            }

            UpdateListBoxSelections();
            ScrollSelectionsIntoView();
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                gWindowGrid.Focus();
            }
        }

        private void GWindowGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Due to WPF not removing focus from elements when they are clicked off of, it must be done manually.
            //Focus is assigned to the grid.
            gWindowGrid.Focus();
        }
    }
}
