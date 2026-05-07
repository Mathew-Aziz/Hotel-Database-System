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

        // Use consistent button dimensions (Width = 200, Height = 32)
        int btnWidth = 200;
        int btnHeight = 32;
        int startLeft = 50;
        int startTop = 50;
        int spacing = 60; // vertical spacing between buttons

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