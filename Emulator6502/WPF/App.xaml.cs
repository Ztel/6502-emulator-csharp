using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;

namespace Emulator6502
{
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = false;

            Exception exception = e.Exception;

            //Globally handles KeyNotFoundExceptions, as a band-aid fix from them being thrown internally by a WPF dll randomly due to a bug, causing crashes.
            //Issue described at: https://github.com/dotnet/wpf/issues/7542
            //There are no ill effects when the exception is handled, so just ignore it and continue running the program.
            if (exception is KeyNotFoundException)
            {
                e.Handled = true;
            }
        }
    }
}
