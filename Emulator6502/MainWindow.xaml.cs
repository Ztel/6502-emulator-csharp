using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace Emulator6502
{
    public partial class MainWindow : Window
    {
        public Emulator emulator;

        private Thread emulatorThread;
        private Thread uiLogicThread;
        private int targetFPS = 60;
        private bool startPaused = false;

        private DebugWindow debugWindow;
        private MemoryViewerWindow memoryViewerWindow;


        public MainWindow()
        {
            InitializeComponent();

            ToggleRuntimeButtons(false);

            Thread.CurrentThread.Name = "Main UI Thread";

            emulator = new Emulator(this);
            emulatorThread = new Thread(EmulatorLoop);
            emulatorThread.Name = "Emulation Thread";
            emulatorThread.IsBackground = true;
            emulatorThread.Priority = ThreadPriority.AboveNormal;
            emulatorThread.Start();

            uiLogicThread = new Thread(UILogicUpdate);
            uiLogicThread.Name = "UI Logic Thread";
            uiLogicThread.IsBackground = true;
            uiLogicThread.Priority = ThreadPriority.BelowNormal;
            uiLogicThread.Start();

            debugWindow = new DebugWindow(this);
            memoryViewerWindow = new MemoryViewerWindow(this);

            tbDisplayGrid.DataContext = emulator.Screen;
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

        private void Step(bool stepWholeFrame, bool forceUIUpdate = false)
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
                    Application.Current?.Dispatcher.Invoke(() => { TogglePause(true); }); //Invoke the TogglePause method on the main UI thread.

                    MessageBox.Show(ex.Message, "Emulator Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UILogicUpdate()
        {
            while(true)
            {
                debugWindow?.UpdateDebugWindow();
                memoryViewerWindow?.UpdateMemoryViewer();

                Thread.Sleep(50); //Prevent hogging the CPU with unnecessary UI updates.
            }
        }

        private bool LoadRom(string path)
        {
            try
            {
                emulator.LoadRom(path);
            }
            catch (Exception ex)
            {
                emulator.ExitProgram();
                tbRomStatus.Text = " No ROM loaded.";
                ToggleRuntimeButtons(false);
                MessageBox.Show(ex.Message, "Error loading ROM", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void StartProgram()
        {
            bool wasPaused = emulator.programPaused;

            ToggleRuntimeButtons(true);
            emulator.StartProgram(targetFPS, startPaused || wasPaused);
            TogglePause(startPaused || wasPaused);
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

            debugWindow?.ToggleRuntimeButtons(enabled);
        }

        private void TogglePause(bool shouldPause)
        {
            if (shouldPause)
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

            debugWindow.UpdateUIPauseStatus();
        }

        private void BtnLoadRom_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new();

            openFileDialog.Filter = "Binary ROM files (*.bin)|*.bin|All files (*.*)|*.*";
            openFileDialog.Title = "Please pick a ROM file...";

            if (openFileDialog.ShowDialog() == true)
            {
                if (LoadRom(openFileDialog.FileName))
                {
                    StartProgram();
                }
            }
        }

        private void BtnEjectRom_Click(object sender, RoutedEventArgs e)
        {
            emulator.ExitProgram();
            tbRomStatus.Text = " No ROM loaded.";
            ToggleRuntimeButtons(false);
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (emulator.programActive)
            {
                emulator.ExitProgram();
                LoadRom(emulator.ProgramPath);
                StartProgram();
            }
        }

        public void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            TogglePause(!emulator.programPaused);
        }

        public void BtnStepFrame_Click(object sender, RoutedEventArgs e)
        {
            TogglePause(true);
            Thread.Sleep(20); //Give CPU thread time to halt before stepping.
            Step(stepWholeFrame: true, forceUIUpdate: true);
            debugWindow.ScrollSelectionsIntoView();
        }

        public void BtnStepInstruction_Click(object sender, RoutedEventArgs e)
        {
            TogglePause(true);
            Thread.Sleep(20); //Give CPU thread time to halt before stepping.
            Step(stepWholeFrame: false, forceUIUpdate: true);
            debugWindow.ScrollSelectionsIntoView();
        }

        private void BtnSaveSlot_Click(object sender, RoutedEventArgs e)
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

        private void BtnLoadSlot_Click(object sender, RoutedEventArgs e)
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
        }

        private void BtnOpenDebugWindow_Click(object sender, RoutedEventArgs e)
        {
            debugWindow.Close();
            debugWindow = new DebugWindow(this);
            debugWindow.Owner = this;
            debugWindow?.ToggleRuntimeButtons(emulator.programActive);
            debugWindow.Show();
        }

        private void BtnOpenMemoryViewer_Click(object sender, RoutedEventArgs e)
        {
            memoryViewerWindow.Close();
            memoryViewerWindow = new MemoryViewerWindow(this);
            memoryViewerWindow.Owner = this;
            memoryViewerWindow.Show();
        }
    }
}