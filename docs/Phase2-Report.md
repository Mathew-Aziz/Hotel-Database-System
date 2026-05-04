# Phase 2 Report (Template)

## Project Theme
Hotel Database System

## Database Overview
This system manages hotel operations using core entities such as Guests, Rooms, Bookings, Services, and Staff, along with associative tables to represent many-to-many relationships.

## Forms (5) and Functionality
1. **Main Menu Form**
   - Navigation hub to open all other forms.

2. **Guests Form**
   - Select: list guests
   - Insert: add new guest
   - Update: edit guest
   - Delete: remove guest

3. **Rooms Form**
   - Select: list rooms
   - Insert/Update/Delete: manage rooms

4. **Bookings Form**
   - Select: list bookings
   - Insert: create a booking for a guest
   - Update: modify booking dates/status
   - Delete: cancel booking
   - Manage booked rooms via Occupies (Booking-Room link)

5. **Services & Staff Form**
   - Manage booking services via Uses (Booking-Service link)
   - Manage staff assignments via Provides (Staff-Service link)

## Navigation Diagram
Add a simple diagram showing:
- MainMenu -> Guests
- MainMenu -> Rooms
- MainMenu -> Bookings
- MainMenu -> ServicesStaff
- Each form has a button back to MainMenu

(Attach `Navigation-Diagram.png` or replace with a figure.)
