using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using HotelManagementSystem.WinForms.Data;
using HotelManagementSystem.WinForms.Utils;

namespace HotelManagementSystem.WinForms.Forms;

public class RoomsForm : Form
{
    private DataGridView dgvRooms;
    private TextBox txtRoomPrice;
    private ComboBox cmbRoomType;
    private Button btnLoad, btnAdd, btnUpdate, btnDelete, btnBack;
    private int currentRoomId = -1;

    public RoomsForm()
    {
        Text = "Manage Rooms";
        Width = 800;
        Height = 450;
        StartPosition = FormStartPosition.CenterScreen;
        BuildUI();
        LoadRooms();
    }

    private void BuildUI()
    {
        // DataGridView
        dgvRooms = new DataGridView
        {
            Location = new System.Drawing.Point(20, 20),
            Size = new System.Drawing.Size(450, 300),
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };
        dgvRooms.SelectionChanged += DgvRooms_SelectionChanged;
        Controls.Add(dgvRooms);

        // Labels and inputs
        var lblType = new Label { Text = "Room Type:", Left = 500, Top = 30, Width = 100 };
        var lblPrice = new Label { Text = "Price / Night:", Left = 500, Top = 70, Width = 100 };

        cmbRoomType = new ComboBox { Left = 610, Top = 30, Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        cmbRoomType.Items.AddRange(new[] { "SINGLE", "DOUBLE", "TWIN", "SUITE", "DELUXE" });
        cmbRoomType.SelectedIndex = 0;

        txtRoomPrice = new TextBox { Left = 610, Top = 70, Width = 150 };

        // Buttons - uniform height = 32
        int btnHeight = 32;
        btnLoad = new Button { Text = "Load", Left = 500, Top = 120, Width = 80, Height = btnHeight };
        btnAdd = new Button { Text = "Add", Left = 590, Top = 120, Width = 80, Height = btnHeight };
        btnUpdate = new Button { Text = "Update", Left = 500, Top = 160, Width = 80, Height = btnHeight };
        btnDelete = new Button { Text = "Delete", Left = 590, Top = 160, Width = 80, Height = btnHeight };
        btnBack = new Button { Text = "Back to Menu", Left = 500, Top = 210, Width = 170, Height = btnHeight };

        btnLoad.Click += (_, _) => LoadRooms();
        btnAdd.Click += BtnAdd_Click;
        btnUpdate.Click += BtnUpdate_Click;
        btnDelete.Click += BtnDelete_Click;
        btnBack.Click += (_, _) => this.Close();

        Controls.Add(lblType);
        Controls.Add(lblPrice);
        Controls.Add(cmbRoomType);
        Controls.Add(txtRoomPrice);
        Controls.Add(btnLoad);
        Controls.Add(btnAdd);
        Controls.Add(btnUpdate);
        Controls.Add(btnDelete);
        Controls.Add(btnBack);
    }

    private void LoadRooms()
    {
        try
        {
            // Query from gui_queries.sql
            string sql = "SELECT room_id, room_type, room_price FROM dbo.ROOM ORDER BY room_id";
            DataTable dt = Db.ExecuteSelect(sql);
            dgvRooms.DataSource = dt;
            ClearFields();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading rooms: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DgvRooms_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvRooms.SelectedRows.Count > 0)
        {
            DataGridViewRow row = dgvRooms.SelectedRows[0];
            currentRoomId = Convert.ToInt32(row.Cells["room_id"].Value);
            cmbRoomType.SelectedItem = row.Cells["room_type"].Value.ToString();
            txtRoomPrice.Text = row.Cells["room_price"].Value.ToString();
        }
    }

    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInputs(out decimal price)) return;

        try
        {
            string sql = "INSERT INTO dbo.ROOM (room_type, room_price) VALUES (@room_type, @room_price)";
            SqlParameter[] p = {
                new SqlParameter("@room_type", cmbRoomType.SelectedItem.ToString()),
                new SqlParameter("@room_price", price)
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Room added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRooms();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        if (currentRoomId == -1)
        {
            MessageBox.Show("Select a room to update.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ValidateInputs(out decimal price)) return;

        try
        {
            string sql = "UPDATE dbo.ROOM SET room_type = @room_type, room_price = @room_price WHERE room_id = @room_id";
            SqlParameter[] p = {
                new SqlParameter("@room_type", cmbRoomType.SelectedItem.ToString()),
                new SqlParameter("@room_price", price),
                new SqlParameter("@room_id", currentRoomId)
            };
            Db.ExecuteNonQuery(sql, p);
            MessageBox.Show("Room updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRooms();
        }
        catch (SqlException ex)
        {
            MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnDelete_Click(object sender, EventArgs e)
    {
        if (currentRoomId == -1)
        {
            MessageBox.Show("Select a room to delete.", "No selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show("Delete this room? It cannot be undone.", "Confirm Delete",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            string sql = "DELETE FROM dbo.ROOM WHERE room_id = @room_id";
            Db.ExecuteNonQuery(sql, new SqlParameter("@room_id", currentRoomId));
            MessageBox.Show("Room deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadRooms();
        }
        catch (SqlException)
        {
            MessageBox.Show("Cannot delete – this room is assigned to one or more bookings (Occupies).",
                "Foreign Key Constraint", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private bool ValidateInputs(out decimal price)
    {
        price = 0;
        if (!Validation.IsPositiveDecimal(txtRoomPrice.Text, out price))
        {
            MessageBox.Show("Price must be a positive number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (cmbRoomType.SelectedItem == null)
        {
            MessageBox.Show("Select a room type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        return true;
    }

    private void ClearFields()
    {
        currentRoomId = -1;
        cmbRoomType.SelectedIndex = 0;
        txtRoomPrice.Clear();
    }
}