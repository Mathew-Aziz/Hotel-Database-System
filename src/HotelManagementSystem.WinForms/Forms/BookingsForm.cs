using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HotelManagementSystem.WinForms.Data;

namespace HotelManagementSystem.WinForms.Forms;

public class BookingsForm : Form
{
    private DataGridView dgvBookings, dgvOccupies;
    private ComboBox cmbGuest, cmbRoomToAdd;
    private ComboBox cmbBookingStatus, cmbPaymentStatus;
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
    // Increase form height to fit all controls
    this.Height = 700;

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
    int labelWidth = 140, controlWidth = 290;
    int rowHeight = 50;  // space for label + combo

    // Guest
    Controls.Add(new Label { Text = "Guest:", Left = lx, Top = y, Width = labelWidth });
    cmbGuest = new ComboBox { Left = lx, Top = y + 20, Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
    Controls.Add(cmbGuest);
    y += rowHeight;

    // Check-In
    Controls.Add(new Label { Text = "Check-In:", Left = lx, Top = y, Width = labelWidth });
    dtpCheckIn = new DateTimePicker { Left = lx, Top = y + 20, Width = controlWidth, Format = DateTimePickerFormat.Short, Value = DateTime.Today };
    Controls.Add(dtpCheckIn);
    y += rowHeight;

    // Check-Out
    Controls.Add(new Label { Text = "Check-Out:", Left = lx, Top = y, Width = labelWidth });
    dtpCheckOut = new DateTimePicker { Left = lx, Top = y + 20, Width = controlWidth, Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddDays(1) };
    Controls.Add(dtpCheckOut);
    y += rowHeight;

    // Booking Status
    Controls.Add(new Label { Text = "Booking Status:", Left = lx, Top = y, Width = labelWidth });
    cmbBookingStatus = new ComboBox { Left = lx, Top = y + 20, Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
    cmbBookingStatus.Items.AddRange(new[] { "Pending", "Confirmed", "Cancelled", "CheckedOut" });
    Controls.Add(cmbBookingStatus);
    y += rowHeight;

    // Payment Status
    Controls.Add(new Label { Text = "Payment Status:", Left = lx, Top = y, Width = labelWidth });
    cmbPaymentStatus = new ComboBox { Left = lx, Top = y + 20, Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
    cmbPaymentStatus.Items.AddRange(new[] { "Unpaid", "Paid", "Refunded" });
    Controls.Add(cmbPaymentStatus);
    y += rowHeight;

    // Total Price label – placed right after payment status, before buttons
    lblTotal = new Label { Text = "Total Price: —", Left = lx, Top = y + 5, Width = controlWidth, Height = 30, AutoSize = false, Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold) };
    Controls.Add(lblTotal);
    y += rowHeight;

    // Buttons (Load, Add, Update, Delete) – keep at same Y as before, but ensure they are below bookings grid
    int btnY = 245; // unchanged
    int btnHeight = 32;
    btnLoad = new Button { Text = "Load", Left = 20, Top = btnY, Width = 80, Height = btnHeight };
    btnAdd = new Button { Text = "Add", Left = 110, Top = btnY, Width = 80, Height = btnHeight };
    btnUpdate = new Button { Text = "Update", Left = 200, Top = btnY, Width = 80, Height = btnHeight };
    btnDelete = new Button { Text = "Delete", Left = 290, Top = btnY, Width = 80, Height = btnHeight };

    btnLoad.Click += (_, _) => LoadBookings();
    btnAdd.Click += BtnAdd_Click;
    btnUpdate.Click += BtnUpdate_Click;
    btnDelete.Click += BtnDelete_Click;

    Controls.Add(btnLoad);
    Controls.Add(btnAdd);
    Controls.Add(btnUpdate);
    Controls.Add(btnDelete);

    // Rooms in Booking section
    Controls.Add(new Label { Text = "Rooms in Booking:", Left = 20, Top = 295, AutoSize = true });

    dgvOccupies = new DataGridView
    {
        Location = new System.Drawing.Point(20, 320),
        Size = new System.Drawing.Size(580, 180),
        ReadOnly = true,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AllowUserToAddRows = false,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    };
    Controls.Add(dgvOccupies);

    // Add Room section (right side, below total price)
    int addRoomY = 400; // well below the total price
    Controls.Add(new Label { Text = "Add Room:", Left = lx, Top = addRoomY, Width = labelWidth });
    cmbRoomToAdd = new ComboBox { Left = lx, Top = addRoomY + 20, Width = controlWidth, DropDownStyle = ComboBoxStyle.DropDownList };
    Controls.Add(cmbRoomToAdd);

    btnAddRoom = new Button { Text = "Add Room", Left = lx, Top = addRoomY + 55, Width = 140, Height = btnHeight };
    btnRemoveRoom = new Button { Text = "Remove Room", Left = lx + 150, Top = addRoomY + 55, Width = 140, Height = btnHeight };

    btnAddRoom.Click += BtnAddRoom_Click;
    btnRemoveRoom.Click += BtnRemoveRoom_Click;

    Controls.Add(btnAddRoom);
    Controls.Add(btnRemoveRoom);

    // Back button
    btnBack = new Button { Text = "Back to Menu", Left = 20, Top = 540, Width = 130, Height = btnHeight };
    btnBack.Click += (_, _) => this.Close();
    Controls.Add(btnBack);
}

    // ---------------------------------------------------------------
    // LOAD methods
    // ---------------------------------------------------------------

    private void LoadBookings()
    {
        try
        {
            // FIX 1: Include guest_id and b.guest_id in SELECT so we can set
            //         cmbGuest.SelectedValue directly without fragile name matching.
            // FIX 2: Use dbo. schema prefix on all tables (matches HotelDB.sql).
            string sql = @"
                SELECT b.booking_id,
                       b.guest_id,
                       g.guest_first_name + ' ' + g.guest_last_name AS guest_name,
                       b.check_in_date,
                       b.check_out_date,
                       b.booking_status,
                       b.payment_status
                FROM dbo.BOOKING b
                JOIN dbo.GUEST   g ON g.guest_id = b.guest_id
                ORDER BY b.booking_id DESC";

            dgvBookings.DataSource = Db.ExecuteSelect(sql);
            ClearInputs();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading bookings: {ex.Message}", "DB Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadGuestCombo()
    {
        try
        {
            var dt = Db.ExecuteSelect(
                "SELECT guest_id, guest_first_name + ' ' + guest_last_name AS full_name " +
                "FROM dbo.GUEST ORDER BY guest_first_name");
            cmbGuest.DisplayMember = "full_name";
            cmbGuest.ValueMember   = "guest_id";
            cmbGuest.DataSource    = dt;
            cmbGuest.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading guests: {ex.Message}", "DB Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadRoomCombo()
    {
        try
        {
            var dt = Db.ExecuteSelect(
                "SELECT room_id, room_type + ' - $' + CAST(room_price AS VARCHAR) AS display " +
                "FROM dbo.ROOM ORDER BY room_type");
            cmbRoomToAdd.DisplayMember = "display";
            cmbRoomToAdd.ValueMember   = "room_id";
            cmbRoomToAdd.DataSource    = dt;
            cmbRoomToAdd.SelectedIndex = -1;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rooms: {ex.Message}", "DB Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadOccupies(int bookingId)
    {
        try
        {
            string sql = @"
                SELECT o.room_id, r.room_type, r.room_price
                FROM dbo.OCCUPIES o
                JOIN dbo.ROOM     r ON r.room_id = o.room_id
                WHERE o.booking_id = @booking_id";

            dgvOccupies.DataSource = Db.ExecuteSelect(sql,
                new SqlParameter("@booking_id", bookingId));
            UpdateTotalLabel(bookingId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rooms in booking: {ex.Message}", "DB Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void UpdateTotalLabel(int bookingId)
    {
        try
        {
            string sql = @"
                SELECT SUM(r.room_price * DATEDIFF(day, b.check_in_date, b.check_out_date))
                FROM dbo.OCCUPIES o
                JOIN dbo.ROOM     r ON r.room_id    = o.room_id
                JOIN dbo.BOOKING  b ON b.booking_id = o.booking_id
                WHERE o.booking_id = @booking_id";

            var dt = Db.ExecuteSelect(sql, new SqlParameter("@booking_id", bookingId));
            lblTotal.Text = (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                ? $"Total Price: ${dt.Rows[0][0]:F2}"
                : "Total Price: $0.00";
        }
        catch
        {
            lblTotal.Text = "Total Price: —";
        }
    }

    // ---------------------------------------------------------------
    // Selection changed
    // ---------------------------------------------------------------

    private void DgvBookings_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvBookings.SelectedRows.Count == 0) return;

        var row = dgvBookings.SelectedRows[0];
        selectedBookingId = Convert.ToInt32(row.Cells["booking_id"].Value);

        // FIX 3: Use guest_id (int) to set ComboBox value directly — no fragile name matching.
        cmbGuest.SelectedValue = Convert.ToInt32(row.Cells["guest_id"].Value);

        dtpCheckIn.Value  = Convert.ToDateTime(row.Cells["check_in_date"].Value);
        dtpCheckOut.Value = Convert.ToDateTime(row.Cells["check_out_date"].Value);

        cmbBookingStatus.SelectedItem = row.Cells["booking_status"].Value?.ToString();
        cmbPaymentStatus.SelectedItem = row.Cells["payment_status"].Value?.ToString();

        LoadOccupies(selectedBookingId.Value);
    }

    // ---------------------------------------------------------------
    // CRUD buttons
    // ---------------------------------------------------------------

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        try
        {
            string sql = @"
                INSERT INTO dbo.BOOKING
                    (guest_id, check_in_date, check_out_date, booking_status, payment_status)
                VALUES
                    (@guest_id, @check_in, @check_out, @booking_status, @payment_status)";

            // FIX 4: Always pass params as an array — matches ExecuteNonQuery(string, params SqlParameter[])
            SqlParameter[] p = {
                new SqlParameter("@guest_id",        cmbGuest.SelectedValue),
                new SqlParameter("@check_in",        dtpCheckIn.Value.Date),
                new SqlParameter("@check_out",       dtpCheckOut.Value.Date),
                new SqlParameter("@booking_status",  cmbBookingStatus.SelectedItem.ToString()),
                new SqlParameter("@payment_status",  cmbPaymentStatus.SelectedItem.ToString())
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Booking added successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking to update.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInputs()) return;

        try
        {
            string sql = @"
                UPDATE dbo.BOOKING
                SET guest_id       = @guest_id,
                    check_in_date  = @check_in,
                    check_out_date = @check_out,
                    booking_status = @booking_status,
                    payment_status = @payment_status
                WHERE booking_id   = @booking_id";

            SqlParameter[] p = {
                new SqlParameter("@guest_id",        cmbGuest.SelectedValue),
                new SqlParameter("@check_in",        dtpCheckIn.Value.Date),
                new SqlParameter("@check_out",       dtpCheckOut.Value.Date),
                new SqlParameter("@booking_status",  cmbBookingStatus.SelectedItem.ToString()),
                new SqlParameter("@payment_status",  cmbPaymentStatus.SelectedItem.ToString()),
                new SqlParameter("@booking_id",      selectedBookingId.Value)
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Booking updated successfully.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadBookings();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Delete this booking? Linked rooms and services will be removed too.",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            // CASCADE is set on OCCUPIES and USES, so child rows are deleted automatically.
            Db.ExecuteNonQuery("DELETE FROM dbo.BOOKING WHERE booking_id = @id",
                new SqlParameter("@id", selectedBookingId.Value));
            MessageBox.Show("Booking deleted.", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            selectedBookingId    = null;
            dgvOccupies.DataSource = null;
            lblTotal.Text        = "Total Price: —";
            LoadBookings();
        }
        catch (SqlException ex) when (ex.Number == 547)
        {
            // 547 = FK violation (e.g. if cascade was not set for some reason)
            MessageBox.Show("Cannot delete: remove linked records first.", "Foreign Key Constraint",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ---------------------------------------------------------------
    // Occupies (room-in-booking) buttons
    // ---------------------------------------------------------------

    private void BtnAddRoom_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking first.", "No Booking",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cmbRoomToAdd.SelectedValue == null)
        {
            MessageBox.Show("Select a room to add.", "No Room",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            Db.ExecuteNonQuery(
                "INSERT INTO dbo.OCCUPIES (booking_id, room_id) VALUES (@booking_id, @room_id)",
                new SqlParameter("@booking_id", selectedBookingId.Value),
                new SqlParameter("@room_id",    cmbRoomToAdd.SelectedValue));
            LoadOccupies(selectedBookingId.Value);
        }
        catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
        {
            MessageBox.Show("That room is already in this booking.", "Duplicate",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRemoveRoom_Click(object sender, EventArgs e)
    {
        if (selectedBookingId == null)
        {
            MessageBox.Show("Select a booking first.", "No Booking",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (dgvOccupies.SelectedRows.Count == 0)
        {
            MessageBox.Show("Select a room row to remove.", "No Room",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (dgvOccupies.Rows.Count == 1)
        {
            MessageBox.Show("A booking must have at least one room. Cannot remove the last room.",
                "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show("Remove this room from the booking?", "Confirm",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

        int roomId = Convert.ToInt32(dgvOccupies.SelectedRows[0].Cells["room_id"].Value);

        try
        {
            Db.ExecuteNonQuery(
                "DELETE FROM dbo.OCCUPIES WHERE booking_id = @bid AND room_id = @rid",
                new SqlParameter("@bid", selectedBookingId.Value),
                new SqlParameter("@rid", roomId));
            LoadOccupies(selectedBookingId.Value);
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ---------------------------------------------------------------
    // Validation + helpers
    // ---------------------------------------------------------------

    private bool ValidateInputs()
    {
        if (cmbGuest.SelectedValue == null)
        {
            MessageBox.Show("Select a guest.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (dtpCheckOut.Value.Date <= dtpCheckIn.Value.Date)
        {
            MessageBox.Show("Check-out date must be after check-in date.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (cmbBookingStatus.SelectedItem == null)
        {
            MessageBox.Show("Select a booking status.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (cmbPaymentStatus.SelectedItem == null)
        {
            MessageBox.Show("Select a payment status.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void ClearInputs()
    {
        selectedBookingId      = null;
        cmbGuest.SelectedIndex = -1;
        dtpCheckIn.Value       = DateTime.Today;
        dtpCheckOut.Value      = DateTime.Today.AddDays(1);
        cmbBookingStatus.SelectedIndex = -1;
        cmbPaymentStatus.SelectedIndex = -1;
        lblTotal.Text          = "Total Price: —";
        dgvOccupies.DataSource = null;
    }
}