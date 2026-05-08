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

        int btnWidth = 200;
        int btnHeight = 32;
        int startLeft = 50;
        int startTop = 50;
        int spacing = 60;

        var btnGuests = new Button 
        { 
            Text = "Guests", 
            Left = startLeft, 
            Top = startTop, 
            Width = btnWidth, 
            Height = btnHeight 
        };
        
        var btnRooms = new Button 
        { 
            Text = "Rooms", 
            Left = startLeft, 
            Top = startTop + spacing, 
            Width = btnWidth, 
            Height = btnHeight 
        };
        
        var btnBookings = new Button 
        { 
            Text = "Bookings", 
            Left = startLeft, 
            Top = startTop + spacing * 2, 
            Width = btnWidth, 
            Height = btnHeight 
        };
        
        var btnServicesStaff = new Button 
        { 
            Text = "Services & Staff", 
            Left = startLeft, 
            Top = startTop + spacing * 3, 
            Width = btnWidth, 
            Height = btnHeight 
        };

        // CHANGED: Hide main menu, open child as dialog, then show main menu again
        btnGuests.Click += (_, _) => { 
            this.Hide();
            new GuestsForm().ShowDialog();
            this.Show();
        };
        btnRooms.Click += (_, _) => { 
            this.Hide();
            new RoomsForm().ShowDialog();
            this.Show();
        };
        btnBookings.Click += (_, _) => { 
            this.Hide();
            new BookingsForm().ShowDialog();
            this.Show();
        };
        btnServicesStaff.Click += (_, _) => { 
            this.Hide();
            new ServicesStaffForm().ShowDialog();
            this.Show();
        };

        Controls.Add(btnGuests);
        Controls.Add(btnRooms);
        Controls.Add(btnBookings);
        Controls.Add(btnServicesStaff);
    }
}