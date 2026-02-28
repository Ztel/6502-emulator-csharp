using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Emulator6502
{
    public partial class MainWindow : Window
    {
        private Emulator emulator;
        private Thread emulatorThread;
        private int targetFPS = 15;
        private bool startPaused = false;


        public MainWindow()
        {
            InitializeComponent();

            ToggleRuntimeButtons(false);

            emulator = new Emulator(this);
            emulatorThread = new Thread(EmulatorLoop);
            emulatorThread.IsBackground = true;
            emulatorThread.Start();
        }

        private void EmulatorLoop()
        {
            while (true)
            {
                while (emulator.programActive)
                {
                    if (!emulator.programPaused)
                    {
                        Step(stepWholeFrame: true);
                    }
                }
            }
        }

        private void Step(bool stepWholeFrame)
        {
            if (emulator.programActive)
            {
                try
                {
                    if(stepWholeFrame)
                    {
                        emulator.StepFrame();
                    }
                    else
                    {
                        emulator.StepInstruction();
                    }
                }
                catch (Exception ex)
                {
                    emulator.ExitProgram();
                    Application.Current.Dispatcher.Invoke(() => { TogglePause(true); }); //Invoke the TogglePause method on the correct thread.

                    MessageBox.Show(ex.Message, "Emulator Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public bool LoadRom(string path)
        {
            try
            {
                emulator.LoadRom(path);
            }
            catch (Exception ex)
            {
                emulator.ExitProgram();
                tbDisplayGrid.Text = "";
                tbRomStatus.Text = " No ROM loaded.";
                ToggleRuntimeButtons(false);
                MessageBox.Show(ex.Message, "Error loading ROM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void StartProgram()
        {
            ToggleRuntimeButtons(true);
            emulator.StartProgram(targetFPS, startPaused);
            TogglePause(startPaused);
        }

        public void UpdateDisplay(string displayString)
        {
            if(emulator.programActive)
            {
                tbDisplayGrid.Text = displayString;
            }
        }

        private void ToggleRuntimeButtons(bool enabled)
        {
            btnEjectRom.IsEnabled = enabled;
            btnCreateSavestate.IsEnabled = enabled;
            btnLoadSavestate.IsEnabled = enabled;
            btnReset.IsEnabled = enabled;
            btnPause.IsEnabled = enabled;
            btnStepInstruction.IsEnabled = enabled; 
            btnStepFrame.IsEnabled = enabled;
        }

        private void TogglePause(bool pause)
        {
            if (pause)
            {
                if (emulator.programActive)
                {
                    emulator.PauseProgram();
                }
                tbRomStatus.Text = string.Format(" {0}: ▌ ▌ paused", emulator.ProgramName);
                btnPause.IsChecked = true;
            }
            else
            {
                emulator.programPaused = false;
                tbRomStatus.Text = string.Format(" {0}: ▶︎ running", emulator.ProgramName);
                btnPause.IsChecked = false;
            }
        }

        public void btnLoadRom_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            openFileDialog.Filter = "Binary ROM files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog.Title = "Please pick a ROM source file...";

            if (openFileDialog.ShowDialog() == true)
            {
                if (LoadRom(openFileDialog.FileName))
                {
                    StartProgram();
                }
            }
        }

        public void btnEjectRom_Click(object sender, RoutedEventArgs e)
        {
            emulator.ExitProgram();
            tbDisplayGrid.Text = "";
            tbRomStatus.Text = " No ROM loaded.";
            ToggleRuntimeButtons(false);
        }

        public void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tbDisplayGrid.Text = "";
            emulator.ExitProgram();
            LoadRom(emulator.ProgramPath);
            StartProgram();
        }

        public void btnPause_Click(object sender, RoutedEventArgs e)
        {
            TogglePause(!emulator.programPaused);
        }

        private void btnStepFrame_Click(object sender, RoutedEventArgs e)
        {
            if(emulator.programPaused)
            {
                Step(stepWholeFrame: true);
            }
            else
            {
                TogglePause(true);
            }            
        }

        private void btnStepInstruction_Click(object sender, RoutedEventArgs e)
        {
            if (emulator.programPaused)
            {
                Step(stepWholeFrame: false);
            }
            else
            {
                TogglePause(true);
            }
        }

        private void btnSaveSlot_Click(object sender, RoutedEventArgs e)
        {
            bool wasPaused = emulator.programPaused;
            TogglePause(true);

            //16 header bytes, 7 cpu register bytes, 32 KB for the graphics buffers.
            byte[] savestate = new byte[16 + 7 + 32768];

            //Save the program name into the savestate header.
            for (int i = 0; i < 16; i++)
            {
                if (i < emulator.ProgramName.Length)
                {
                    savestate[i] = (byte)emulator.ProgramName[i];
                }
            }

            //Save the CPU registers.
            savestate[16 + 0] = (byte)(emulator.Cpu.ProgramCounter >>> 8); //PC high byte
            savestate[16 + 1] = (byte)(emulator.Cpu.ProgramCounter & 0x00FF); //PC low byte
            savestate[16 + 2] = emulator.Cpu.Accumulator;
            savestate[16 + 3] = emulator.Cpu.XRegister;
            savestate[16 + 4] = emulator.Cpu.YRegister;
            savestate[16 + 5] = emulator.Cpu.StatusRegister;
            savestate[16 + 6] = emulator.Cpu.StackPointer;

            //Save RAM.
            for (int i = 0; i < 32768; i++)
            {
                savestate[16 + 7 + i] = (byte)emulator.Cpu.Memory[i];
            }

            FrameworkElement element = (FrameworkElement)sender;
            string slotNumber = element.Name[element.Name.Length - 1].ToString();

            Directory.CreateDirectory("EmulatorData\\Savestates");
            string path = string.Format("EmulatorData\\Savestates\\save{0}.state", slotNumber);

            File.WriteAllBytes(path, savestate);

            Thread.Sleep(200);
            TogglePause(wasPaused);
            tbRomStatus.Text = string.Format(" {0}: Saved state to slot {1}.", emulator.ProgramName, slotNumber);
        }

        private void btnLoadSlot_Click(object sender, RoutedEventArgs e)
        {
            bool wasPaused = emulator.programPaused;
            TogglePause(true);

            FrameworkElement element = (FrameworkElement)sender;
            string slotNumber = element.Name[element.Name.Length - 1].ToString();
            string path = string.Format("EmulatorData\\Savestates\\save{0}.state", slotNumber);
            byte[] savestate;

            try
            {
                savestate = File.ReadAllBytes(path);
            }
            catch
            {
                MessageBox.Show(string.Format("The savestate in slot {0} could not be loaded.", slotNumber), "Invalid Savestate", MessageBoxButton.OK, MessageBoxImage.Error);
                TogglePause(wasPaused);
                return;
            }

            if (savestate.Length != 16 + 7 + 32768)
            {
                MessageBox.Show(string.Format("The savestate in slot {0} is not the correct size.", slotNumber), "Invalid Savestate", MessageBoxButton.OK, MessageBoxImage.Error);
                TogglePause(wasPaused);
                return;
            }

            //Check the program name in the savestate header against the currently loaded program to prevent loading an incompatible state.
            for (int i = 0; i < 16; i++)
            {
                if (i < emulator.ProgramName.Length)
                {
                    if ((byte)emulator.ProgramName[i] != savestate[i])
                    {
                        MessageBox.Show(string.Format("The savestate in slot {0} does not match the currently loaded program.", slotNumber), "Invalid Savestate", MessageBoxButton.OK, MessageBoxImage.Warning);
                        TogglePause(wasPaused);
                        return;
                    }
                }
            }         

            //Load the CPU registers.
            emulator.Cpu.ProgramCounter = (ushort)((savestate[16 + 0] << 8) | savestate[16 + 1]);
            emulator.Cpu.Accumulator = savestate[16 + 2];
            emulator.Cpu.XRegister = savestate[16 + 3];
            emulator.Cpu.YRegister = savestate[16 + 4];
            emulator.Cpu.StatusRegister = savestate[16 + 5];
            emulator.Cpu.StackPointer = savestate[16 + 6];

            //Load RAM.
            for (int i = 0; i < 32768; i++)
            {
                emulator.Cpu.Memory[i] = savestate[16 + 7 + i];
            }

            Thread.Sleep(200);
            TogglePause(wasPaused);
            tbRomStatus.Text = string.Format(" {0}: Loaded state from slot {1}.", emulator.ProgramName, slotNumber);
            emulator.Screen.RenderDisplay();
            UpdateDisplay(emulator.Screen.renderedDisplay);
        }
    }
}