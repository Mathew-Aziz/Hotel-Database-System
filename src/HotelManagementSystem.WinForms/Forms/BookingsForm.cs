using System;
using System.Windows.Forms;

namespace HotelManagementSystem.WinForms.Forms;

public class BookingsForm : Form
{
    public BookingsForm()
    {
        Text = "Bookings";
        Width = 1000;
        Height = 650;

        var lbl = new Label { Text = "TODO: Implement Bookings CRUD + Occupies management", AutoSize = true, Left = 20, Top = 20 };
        Controls.Add(lbl);
    }
}
