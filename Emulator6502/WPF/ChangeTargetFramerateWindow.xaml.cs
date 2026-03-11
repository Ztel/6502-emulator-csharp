using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Emulator6502
{
    public partial class ChangeTargetFramerateWindow : Window
    {
        readonly MainWindow mainWindow;

        public ChangeTargetFramerateWindow(MainWindow mainWindow)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            tbCurrentTarget.Text = mainWindow.emulator.Fps.ToString();
            tbFramerate.Focus();
        }

        private void UpdateFramerateFile()
        {
            Directory.CreateDirectory("EmulatorData\\Settings");
            string path = "EmulatorData\\Settings\\framerate.ini";

            File.WriteAllText(path, mainWindow.emulator.Fps.ToString());
        }

        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(tbFramerate.Text, out int newTarget))
            {
                tbCurrentTarget.Text = newTarget.ToString();
                mainWindow.emulator.Fps = newTarget;
                mainWindow.tbRomStatus.Text = String.Format(" Set framerate to {0}.", newTarget);
                UpdateFramerateFile();

                Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid integer for the target framerate.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void TbFramerate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOK_Click(sender, e);
            }
        }
    }
}
