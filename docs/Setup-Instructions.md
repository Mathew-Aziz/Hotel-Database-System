# Setup Instructions

## Prerequisites
- Windows
- Visual Studio (with WinForms workload)
- SQL Server + SSMS

## Database setup
1. Create a new database in SQL Server, e.g. `HotelDB`.
2. Open `../sql/HotelDB.sql` in SSMS.
3. Ensure the script is targeting the correct database:
   - either `USE HotelDB;` at the top, or select the database from the dropdown.
4. Execute the script.

## Application setup
1. Open the solution: `../src/HotelManagementSystem.sln`
2. Update the connection string in:
   - `src/HotelManagementSystem.WinForms/App.config`
3. Run the WinForms project.

## Notes
- If the app cannot connect to SQL Server, confirm:
  - SQL Server service is running
  - server name in connection string is correct (e.g. `localhost` or `DESKTOP-XXXX\\SQLEXPRESS`)
  - SQL authentication vs Windows authentication settings
