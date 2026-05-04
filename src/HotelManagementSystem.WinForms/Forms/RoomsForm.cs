using System;
using System.Windows.Forms;

namespace HotelManagementSystem.WinForms.Forms;

public class RoomsForm : Form
{
    public RoomsForm()
    {
        Text = "Rooms";
        Width = 900;
        Height = 600;

        var lbl = new Label { Text = "TODO: Implement Rooms CRUD (SELECT/INSERT/UPDATE/DELETE)", AutoSize = true, Left = 20, Top = 20 };
        Controls.Add(lbl);
    }
}
