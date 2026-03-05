using System;
using System.Windows;
using System.Windows.Input;

namespace Emulator6502
{
    public partial class JumpToAddressWindow : Window
    {
        MemoryViewerWindow memoryViewerWindow;

        public JumpToAddressWindow(MemoryViewerWindow memoryViewerWindow)
        {
            InitializeComponent();

            this.memoryViewerWindow = memoryViewerWindow;
            tbAddress.Focus();
        }

        private void BtnFind_Click(object sender, RoutedEventArgs e)
        {
            string input = tbAddress.Text.Trim();
            int targetAddress;

            try
            {
                targetAddress = int.Parse(input, System.Globalization.NumberStyles.HexNumber);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid address format.", "Address Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            memoryViewerWindow.JumpToAddress(targetAddress);
        }

        private void TbAddress_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnFind_Click(sender, e);
            }
        }
    }
}
