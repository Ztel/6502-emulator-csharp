using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace Emulator6502
{
    public partial class DebugWindow : Window
    {
        readonly MainWindow mainWindow;
        const int UIUpdatesPerSecond = 10;
        long lastUIUpdateTime;

        public ObservableCollection<Tuple<int, byte>> Stack { get; set; } = []; //Have to use the old Tuple<T,T> syntax to generate class properties to be bound to the UI.

        public DebugWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;

            InitializeStackList();
            lbStack.ItemsSource = Stack;

            UpdateDisassembly();
            UpdateUIPauseStatus();
        }

        public void UpdateDisassembly()
        {
            lbDisassembly.ItemsSource = Disassembler.disassembly;
            //lbDisassembly.UpdateLayout();

            InitializeStackList();
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

        public void UpdateDebugWindow(bool forceUpdate = false)
        {
            long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            CPU cpu = mainWindow.emulator.Cpu;

            if (mainWindow.emulator.programPaused && Disassembler.disassembly != null)
            {
                if(Disassembler.disassembly.ContainsKey(cpu.ProgramCounter) /*&& lbDisassembly.Items.Contains(new KeyValuePair<ushort, Tuple<string, string>>(cpu.ProgramCounter, Disassembler.disassembly[cpu.ProgramCounter]))*/)
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

            /*if (forceUpdate == false && currentTime - lastUIUpdateTime < (1000 / UIUpdatesPerSecond))
            {
                return; //Return early if it's not time to update the stack UI yet.
            }*/

            for (int i = 0; i < 256; i++)
            {
                Stack[i] = new Tuple<int, byte>(0x0100 + i, cpu.Memory[0x0100 + i]);
            }

            //lbStack.SelectedIndex = cpu.StackPointer; //Possible cause of an exception, below is potential fix
            lbStack.SelectedItem = lbStack.Items.Contains(Stack[cpu.StackPointer]) ? Stack[cpu.StackPointer] : null;

            if (mainWindow.emulator.programPaused)
            {
                lbStack.ScrollIntoView(lbStack.Items[lbStack.SelectedIndex]);
            }

            lastUIUpdateTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public void ToggleRuntimeButtons(bool enabled)
        {
            btnReset.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnStepFrame.IsEnabled = enabled;
            btnStepInstruction.IsEnabled = enabled;
        }

        private void InitializeStackList()
        {
            lbStack.ItemStringFormat = "X2";

            for (int i = 0; i < 256; i++)
            {
                if (Stack.Count != 256)
                {
                    Stack.Add(new Tuple<int, byte>(0x0100 + i, 0));
                }
                else
                {
                    Stack[i] = new Tuple<int, byte>(0x0100 + i, 0);
                }
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
    }
}
