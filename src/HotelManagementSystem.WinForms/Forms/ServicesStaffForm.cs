using System;
using System.Windows.Forms;

namespace HotelManagementSystem.WinForms.Forms;

public class ServicesStaffForm : Form
{
    public ServicesStaffForm()
    {
        Text = "Services & Staff";
        Width = 1000;
        Height = 650;

        var lbl = new Label { Text = "TODO: Implement Uses (Booking-Service) and Provides (Staff-Service) management", AutoSize = true, Left = 20, Top = 20 };
        Controls.Add(lbl);
    }
}
