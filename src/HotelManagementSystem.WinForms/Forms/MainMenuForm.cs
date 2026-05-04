using System;
using System.Windows.Forms;

namespace HotelManagementSystem.WinForms.Forms;

public class MainMenuForm : Form
{
    public MainMenuForm()
    {
        Text = "Hotel Database System - Main Menu";
        Width = 800;
        Height = 500;

        var btnGuests = new Button { Text = "Guests", Left = 50, Top = 50, Width = 200 };
        var btnRooms = new Button { Text = "Rooms", Left = 50, Top = 100, Width = 200 };
        var btnBookings = new Button { Text = "Bookings", Left = 50, Top = 150, Width = 200 };
        var btnServicesStaff = new Button { Text = "Services & Staff", Left = 50, Top = 200, Width = 200 };

        btnGuests.Click += (_, _) => { new GuestsForm().Show(); };
        btnRooms.Click += (_, _) => { new RoomsForm().Show(); };
        btnBookings.Click += (_, _) => { new BookingsForm().Show(); };
        btnServicesStaff.Click += (_, _) => { new ServicesStaffForm().Show(); };

        Controls.Add(btnGuests);
        Controls.Add(btnRooms);
        Controls.Add(btnBookings);
        Controls.Add(btnServicesStaff);
    }
}
