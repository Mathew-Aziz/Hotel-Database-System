using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HotelManagementSystem.WinForms.Data;
using HotelManagementSystem.WinForms.Utils;

namespace HotelManagementSystem.WinForms.Forms;

public class BookingsForm : Form
{
    private DataGridView dgvBookings, dgvOccupies;
    private ComboBox cmbGuest, cmbRoomToAdd;
    private DateTimePicker dtpCheckIn, dtpCheckOut;
    private Label lblTotal;
    private Button btnLoad, btnAdd, btnUpdate, btnDelete, btnAddRoom, btnRemoveRoom, btnBack;
    private int? selectedBookingId = null;

    public BookingsForm()
    {
        Text = "Manage Bookings";
        Width = 950;
        Height = 650;
        StartPosition = FormStartPosition.CenterScreen;
        BuildUI();
        LoadGuestCombo();
        LoadRoomCombo();
        LoadBookings();
    }

    private void BuildUI()
    {
        dgvBookings = new DataGridView
        {
            Location = new System.Drawing.Point(20, 20),
            Size = new System.Drawing.Size(580, 210),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvBookings.SelectionChanged += DgvBookings_SelectionChanged;
        Controls.Add(dgvBookings);

        int lx = 620, y = 20;

        Controls.Add(new Label { Text = "Guest:",          Left = lx, Top = y,      Width = 140 });
        cmbGuest = new ComboBox { Left = lx, Top = y + 20, Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
        Controls.Add(cmbGuest);

        y += 60;
        Controls.Add(new Label { Text = "Check-In:",       Left = lx, Top = y,      Width = 140 });
        dtpCheckIn = new DateTimePicker { Left = lx, Top = y + 20, Width = 290, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
        Controls.Add(dtpCheckIn);

        y += 60;
        Controls.Add(new Label { Text = "Check-Out:",      Left = lx, Top = y,      Width = 140 });
        dtpCheckOut = new DateTimePicker { Left = lx, Top = y + 20, Width = 290, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(1) };
        Controls.Add(dtpCheckOut);

        btnLoad   = new Button { Text = "Load",   Left = 20,  Top = 245, Width = 80 };
        btnAdd    = new Button { Text = "Add",    Left = 110, Top = 245, Width = 80 };
        btnUpdate = new Button { Text = "Update", Left = 200, Top = 245, Width = 80 };
        btnDelete = new Button { Text = "Delete", Left = 290, Top = 245, Width = 80 };

        btnLoad.Click   += (_, _) => LoadBookings();
        btnAdd.Click    += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;

        Controls.Add(btnLoad);
        Controls.Add(btnAdd);
        Controls.Add(btnUpdate);
        Controls.Add(btnDelete);

        lblTotal = new Label { Text = "Total Price: —", Left = 620, Top = 300, Width = 290, AutoSize = false };
        Controls.Add(lblTotal);

        Controls.Add(new Label { Text = "Rooms in Booking:", Left = 20, Top = 295, AutoSize = true });

        dgvOccupies = new DataGridView
        {
            Location = new System.Drawing.Point(20, 320),
            Size = new System.Drawing.Size(580, 200),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        Controls.Add(dgvOccupies);

        Controls.Add(new Label { Text = "Add Room:", Left = 620, Top = 320, Width = 140 });
        cmbRoomToAdd = new ComboBox { Left = 620, Top = 340, Width = 290, DropDownStyle = ComboBoxStyle.DropDownList };
        Controls.Add(cmbRoomToAdd);

        btnAddRoom    = new Button { Text = "Add Room",    Left = 620, Top = 375, Width = 140 };
        btnRemoveRoom = new Button { Text = "Remove Room", Left = 770, Top = 375, Width = 140 };

        btnAddRoom.Click    += BtnAddRoom_Click;
        btnRemoveRoom.Click += BtnRemoveRoom_Click;

        Controls.Add(btnAddRoom);
        Controls.Add(btnRemoveRoom);

        btnBack = new Button { Text = "Back to Menu", Left = 20, Top = 540, Width = 130 };
        btnBack.Click += (_, _) => this.Close();
        Controls.Add(btnBack);
    }

    private void LoadBookings()
    {
        try
        {
            string sql = @"
                SELECT b.booking_id,
                       g.guest_first_name + ' ' + g.guest_last_name AS guest_name,
                       b.check_in_date, b.check_out_date
                FROM Booking b
                JOIN Guest g ON b.guest_id = g.guest_id
                ORDER BY b.booking_id DESC";

            dgvBookings.DataSource = Db.ExecuteSelect(sql);
            ClearInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading bookings: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadGuestCombo()
    {
        try
        {
            var dt = Db.ExecuteSelect("SELECT guest_id, guest_first_name + ' ' + guest_last_name AS full_name FROM Guest ORDER BY guest_first_name");
            cmbGuest.DisplayMember = "full_name";
            cmbGuest.ValueMember   = "guest_id";
            cmbGuest.DataSource    = dt;
            cmbGuest.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading guests: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRoomCombo()
    {
        try
        {
            var dt = Db.ExecuteSelect("SELECT room_id, room_type + ' - $' + CAST(room_price AS VARCHAR) AS display FROM dbo.ROOM ORDER BY room_type");
            cmbRoomToAdd.DisplayMember = "display";
            cmbRoomToAdd.ValueMember   = "room_id";
            cmbRoomToAdd.DataSource    = dt;
            cmbRoomToAdd.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rooms: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadOccupies(int bookingId)
    {
        try
        {
            // FIX: Changed table name to Occupy
            string sql = @"
                SELECT o.room_id, r.room_type, r.room_price
                FROM Occupy o
                JOIN dbo.ROOM r ON o.room_id = r.room_id
                WHERE o.booking_id = @booking_id";

            dgvOccupies.DataSource = Db.ExecuteSelect(sql, new SqlParameter("@booking_id", bookingId));
            UpdateTotalLabel(bookingId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading occupies: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateTotalLabel(int bookingId)
    {
        try
        {
            // FIX: Changed table name to Occupy
            string sql = @"
                SELECT SUM(r.room_price * DATEDIFF(day, b.check_in_date, b.check_out_date))
                FROM Occupy o
                JOIN dbo.ROOM r ON o.room_id = r.room_id
                JOIN Booking b ON o.booking_id = b.booking_id
                WHERE o.booking_id = @booking_id";

            var dt = Db.ExecuteSelect(sql, new SqlParameter("@booking_id", bookingId));
            lblTotal.Text = (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                ? $"Total Price: ${dt.Rows[0][0]:F2}"
                : "Total Price: $0.00";
        }
        catch { lblTotal.Text = "Total Price: —"; }
    }

    //Grid selection
    private void DgvBookings_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvBookings.SelectedRows.Count == 0) return;

        var row = dgvBookings.SelectedRows[0];
        selectedBookingId = Convert.ToInt32(row.Cells["booking_id"].Value);

        if (cmbGuest.DataSource is DataTable guestTable)
            foreach (DataRow dr in guestTable.Rows)
                if (dr["full_name"].ToString() == row.Cells["guest_name"].Value?.ToString())
                { cmbGuest.SelectedValue = dr["guest_id"]; break; }

        dtpCheckIn.Value  = Convert.ToDateTime(row.Cells["check_in_date"].Value);
        dtpCheckOut.Value = Convert.ToDateTime(row.Cells["check_out_date"].Value);

        LoadOccupies(selectedBookingId.Value);
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        try
        {
            // FIX: Reflected schema Booking columns
            string sql = @"INSERT INTO Booking (guest_id, check_in_date, check_out_date)
                           VALUES (@guest_id, @check_in, @check_out)";
            SqlParameter[] p = {
                new SqlParameter("@guest_id", cmbGuest.SelectedValue),
                new SqlParameter("@check_in", dtpCheckIn.Value.Date),
                new SqlParameter("@check_out", dtpCheckOut.Value.Date)
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Booking added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInputs()) return;

        try
        {
            string sql = @"UPDATE Booking
                           SET guest_id = @guest_id, check_in_date = @check_in, check_out_date = @check_out
                           WHERE booking_id = @booking_id";
            SqlParameter[] p = {
                new SqlParameter("@guest_id", cmbGuest.SelectedValue),
                new SqlParameter("@check_in", dtpCheckIn.Value.Date),
                new SqlParameter("@check_out", dtpCheckOut.Value.Date),
                new SqlParameter("@booking_id", selectedBookingId.Value)
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Booking updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Delete this booking? It cannot be undone.", "Confirm Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        try
        {
            Db.ExecuteNonQuery("DELETE FROM Booking WHERE booking_id = @id",
                new SqlParameter("@id", selectedBookingId.Value));
            MessageBox.Show("Booking deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            selectedBookingId = null;
            dgvOccupies.DataSource = null;
            lblTotal.Text = "Total Price: —";
            LoadBookings();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            MessageBox.Show("Cannot delete: remove linked rooms first.", "Foreign Key", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnAddRoom_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking first.", "No Booking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbRoomToAdd.SelectedValue == null)
        {
            MessageBox.Show("Select a room to add.", "No Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Db.ExecuteNonQuery("INSERT INTO Occupy (room_id, booking_id) VALUES (@room_id, @booking_id)",
                new SqlParameter("@booking_id", selectedBookingId.Value),
                new SqlParameter("@room_id",    cmbRoomToAdd.SelectedValue));
            LoadOccupies(selectedBookingId.Value);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            MessageBox.Show("That room is already in this booking.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRemoveRoom_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking first.", "No Booking", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (dgvOccupies.SelectedRows.Count == 0)
        {
            MessageBox.Show("Select a room row to remove.", "No Room", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Remove this room from the booking?", "Confirm",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int roomId = Convert.ToInt32(dgvOccupies.SelectedRows[0].Cells["room_id"].Value);

        try
        {
            // FIX: Changed table name to Occupy
            Db.ExecuteNonQuery("DELETE FROM Occupy WHERE booking_id = @bid AND room_id = @rid",
                new SqlParameter("@bid", selectedBookingId.Value),
                new SqlParameter("@rid", roomId));
            LoadOccupies(selectedBookingId.Value);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateInputs()
    {
        if (cmbGuest.SelectedValue == null)
        {
            MessageBox.Show("Select a guest.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
        {
            MessageBox.Show("Check-out must be after check-in.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void ClearInputs()
    {
        selectedBookingId = null;
        cmbGuest.SelectedIndex = -1;
        dtpCheckIn.Value  = DateTime.Today;
        dtpCheckOut.Value = DateTime.Today.AddDays(1);
        lblTotal.Text = "Total Price: —";
        dgvOccupies.DataSource = null;
    }
}
