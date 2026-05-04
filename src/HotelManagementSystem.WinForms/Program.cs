using System;
using System.Windows.Forms;

namespace HotelManagementSystem.WinForms;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new Forms.MainMenuForm());
    }
}
