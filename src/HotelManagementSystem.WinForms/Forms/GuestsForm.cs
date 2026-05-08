using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace HotelManagementSystem.WinForms.Forms;

public class GuestsForm : Form
{
    public string connectionString = "Server=localhost;Database=HotelDB;Trusted_Connection=True;TrustServerCertificate=True;";
    public GuestsForm()
    {
        InitializeComponent(); // This should call the auto-generated method

        // Only load data if not in designer mode
        if (!DesignMode)
            LoadGuests();
    }
    private void BtnLoad_Click(object sender, EventArgs e)
    {
        if (!DesignMode)
            LoadGuests();
        ClearTextBoxes();
    }

    private void LoadGuests()
    {
        if (DesignMode) 
            return;

        string query = "SELECT * FROM GUEST ORDER BY guest_id";
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand command = new SqlCommand(query, conn))
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                dgvGuests.DataSource = table;

                // Hide the guest_id column
                if (dgvGuests.Columns["guest_id"] != null)
                    dgvGuests.Columns["guest_id"].Visible = false;
            }
        }
        catch (Exception ex)
        {
            if (!DesignMode)
                MessageBox.Show($"Error loading guests: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void ClearTextBoxes()
    {
        txtFirstName.Clear();
        txtLastName.Clear();
        txtPhone.Clear();
        txtNationalId.Clear();
    }

    private void PopulateTextBoxesFromSelectedRow()
    {
        if (dgvGuests.CurrentRow == null)
            return;

        DataGridViewRow row = dgvGuests.CurrentRow;
        txtFirstName.Text = row.Cells["guest_first_name"].Value?.ToString() ?? "";
        txtLastName.Text = row.Cells["guest_last_name"].Value?.ToString() ?? "";
        txtNationalId.Text = row.Cells["guest_national_id"].Value?.ToString() ?? "";
        txtPhone.Text = row.Cells["guest_phone"].Value?.ToString() ?? "";
    }

    private int GetSelectedGuestId()
    {
        DataGridViewRow currentRow = dgvGuests.CurrentRow;
        if (currentRow == null || currentRow.Cells["guest_id"] == null)
            return -1;

        int guestId = Convert.ToInt32(currentRow.Cells["guest_id"].Value);
        return guestId;
    }

    // Validation
    private static bool ValidateField(TextBox field, string fieldname)
    {
        var isEmpty = string.IsNullOrWhiteSpace(field.Text);
        if (isEmpty)
        {
            MessageBox.Show("Please write " + fieldname, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            field.Focus();
            return false;
        }
        return true;
    }

    private bool ValidateInputs()
    {
        var inputsValid = ValidateField(txtFirstName, "First Name") &&
                            ValidateField(txtLastName, "Last Name") &&
                            ValidateField(txtPhone, "Phone Number") &&
                            ValidateField(txtNationalId, "National ID");
        return inputsValid;
    }

    // === CRUD ===
    private void BtnAdd_Click(object sender, EventArgs e)
    {
        if (!ValidateInputs())
            return;

        string query = @"INSERT INTO GUEST (guest_first_name, guest_last_name, guest_phone, guest_national_id) 
                     VALUES (@firstName, @lastName, @phone, @nationalId)";

        SqlParameter[] parameters =
        {
        new SqlParameter("@firstName", txtFirstName.Text.Trim()),
        new SqlParameter("@lastName", txtLastName.Text.Trim()),
        new SqlParameter("@phone", txtPhone.Text.Trim()),
        new SqlParameter("@nationalId", txtNationalId.Text.Trim())
    };

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                var rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Guest added successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGuests();
                    ClearTextBoxes();
                }
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627)
                MessageBox.Show("A guest with this National ID already exists.", "Duplicate Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding guest: {ex.Message}", "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void BtnUpdate_Click(object sender, EventArgs e)
    {
        int guestId = GetSelectedGuestId();
        if (guestId == -1)
        {
            MessageBox.Show("Please select a guest to update.", "No Selection",
                   MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!ValidateInputs())
            return;

        string query = @"UPDATE GUEST 
                     SET guest_first_name = @first_name, 
                         guest_last_name = @last_name, 
                         guest_phone = @phone, 
                         guest_national_id = @national_id 
                     WHERE guest_id = @guest_id";

        SqlParameter[] parameters =
        {
        new SqlParameter("@first_name", txtFirstName.Text.Trim()),
        new SqlParameter("@last_name", txtLastName.Text.Trim()),
        new SqlParameter("@phone", txtPhone.Text.Trim()),
        new SqlParameter("@national_id", txtNationalId.Text.Trim()),
        new SqlParameter("@guest_id", guestId)
    };

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddRange(parameters);
                conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Guest updated successfully!", "Success",
                           MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGuests();
                    ClearTextBoxes();
                }
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 2627)
            {
                MessageBox.Show("A guest with this National ID already exists.", "Duplicate Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"SQL Error: {ex.Message}", "Database Error");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error updating guest: {ex.Message}", "Database Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private void BtnDelete_Click(object sender, EventArgs e)
    {
        int guestId = GetSelectedGuestId();

        if (guestId == -1)
        {
            MessageBox.Show("Please select a guest to delete.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Confirm Deletion
        DialogResult result = MessageBox.Show("Are you sure you want to delete this guest?\n\n" +
               "Note: If this guest has existing bookings, deletion may fail due to foreign key constraints.",
               "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
            return;

        string query = "DELETE FROM GUEST WHERE guest_id = @guest_id";
        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@guest_id", guestId);
                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Guest Deleted Successfully", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadGuests();
                    ClearTextBoxes();
                }
            }
        }
        catch (SqlException ex)
        {
            if (ex.Number == 547) // Foreign key violation
            {
                MessageBox.Show("Cannot delete this guest because they have existing bookings.\n\n" +
                    "Delete the guest's bookings first.",
                    "Constraint Violation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Error deleting guest: {ex.Message}", "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    private void BtnBack_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    private void DgvGuests_SelectionChanged(object sender, EventArgs e)
    {
        PopulateTextBoxesFromSelectedRow();
    }

    // DESIGNER AUTO-GENERATED CODE - DO NOT MODIFY MANUALLY
    private void InitializeComponent()
    {
        btnBack = new Button();
        btnInsert = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnLoad = new Button();
        dgvGuests = new DataGridView();
        panel1 = new Panel();
        label4 = new Label();
        label3 = new Label();
        label2 = new Label();
        label1 = new Label();
        txtLastName = new TextBox();
        txtPhone = new TextBox();
        txtNationalId = new TextBox();
        txtFirstName = new TextBox();
        ((System.ComponentModel.ISupportInitialize)dgvGuests).BeginInit();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // btnBack
        // 
        btnBack.Location = new Point(14, 12);
        btnBack.Name = "btnBack";
        btnBack.Size = new Size(98, 30);
        btnBack.TabIndex = 0;
        btnBack.Text = "Back";
        btnBack.UseVisualStyleBackColor = true;
        btnBack.Click += BtnBack_Click;
        // 
        // btnInsert
        // 
        btnInsert.Location = new Point(14, 125);
        btnInsert.Name = "btnInsert";
        btnInsert.Size = new Size(149, 30);
        btnInsert.TabIndex = 1;
        btnInsert.Text = "Add New Guest";
        btnInsert.UseVisualStyleBackColor = true;
        btnInsert.Click += BtnAdd_Click;
        // 
        // btnUpdate
        // 
        btnUpdate.Location = new Point(401, 125);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(169, 30);
        btnUpdate.TabIndex = 2;
        btnUpdate.Text = "Update Information";
        btnUpdate.UseVisualStyleBackColor = true;
        btnUpdate.Click += BtnUpdate_Click;
        // 
        // btnDelete
        // 
        btnDelete.Location = new Point(421, 432);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(149, 30);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete Guest";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += BtnDelete_Click;
        // 
        // btnLoad
        // 
        btnLoad.Location = new Point(14, 432);
        btnLoad.Name = "btnLoad";
        btnLoad.Size = new Size(149, 30);
        btnLoad.TabIndex = 4;
        btnLoad.Text = "Load Records";
        btnLoad.UseVisualStyleBackColor = true;
        btnLoad.Click += BtnLoad_Click;
        // 
        // dgvGuests
        // 
        dgvGuests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvGuests.Location = new Point(14, 161);
        dgvGuests.Name = "dgvGuests";
        dgvGuests.RowHeadersWidth = 53;
        dgvGuests.Size = new Size(556, 265);
        dgvGuests.TabIndex = 5;
        dgvGuests.SelectionChanged += DgvGuests_SelectionChanged;
        // 
        // panel1
        // 
        panel1.Controls.Add(label4);
        panel1.Controls.Add(label3);
        panel1.Controls.Add(label2);
        panel1.Controls.Add(label1);
        panel1.Controls.Add(txtLastName);
        panel1.Controls.Add(txtPhone);
        panel1.Controls.Add(txtNationalId);
        panel1.Controls.Add(txtFirstName);
        panel1.Location = new Point(14, 48);
        panel1.Name = "panel1";
        panel1.Size = new Size(556, 71);
        panel1.TabIndex = 8;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(468, 10);
        label4.Name = "label4";
        label4.Size = new Size(88, 21);
        label4.TabIndex = 8;
        label4.Text = "National ID";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(308, 10);
        label3.Name = "label3";
        label3.Size = new Size(116, 21);
        label3.TabIndex = 8;
        label3.Text = "Phone Number";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(152, 10);
        label2.Name = "label2";
        label2.Size = new Size(107, 21);
        label2.TabIndex = 8;
        label2.Text = "Second Name";
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(19, 10);
        label1.Name = "label1";
        label1.Size = new Size(86, 21);
        label1.TabIndex = 8;
        label1.Text = "First Name";
        // 
        // txtLastName
        // 
        txtLastName.Location = new Point(135, 34);
        txtLastName.Name = "txtLastName";
        txtLastName.Size = new Size(133, 29);
        txtLastName.TabIndex = 7;
        // 
        // txtPhone
        // 
        txtPhone.Location = new Point(298, 34);
        txtPhone.Name = "txtPhone";
        txtPhone.Size = new Size(136, 29);
        txtPhone.TabIndex = 7;
        // 
        // txtNationalId
        // 
        txtNationalId.Location = new Point(468, 34);
        txtNationalId.Name = "txtNationalId";
        txtNationalId.Size = new Size(88, 29);
        txtNationalId.TabIndex = 7;
        // 
        // txtFirstName
        // 
        txtFirstName.Location = new Point(0, 34);
        txtFirstName.Name = "txtFirstName";
        txtFirstName.Size = new Size(109, 29);
        txtFirstName.TabIndex = 7;
        // 
        // GuestsForm
        // 
        ClientSize = new Size(593, 475);
        Controls.Add(panel1);
        Controls.Add(dgvGuests);
        Controls.Add(btnLoad);
        Controls.Add(btnDelete);
        Controls.Add(btnUpdate);
        Controls.Add(btnInsert);
        Controls.Add(btnBack);
        Name = "GuestsForm";
        Text = "Guests Management";
        //Load += GuestsForm_Load;
        ((System.ComponentModel.ISupportInitialize)dgvGuests).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
    }

    // Control declarations
    private Button btnBack;
    private Button btnInsert;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnLoad;
    private DataGridView dgvGuests;
    private TextBox txtFirstName;
    private TextBox txtLastName;
    private TextBox txtPhone;
    private TextBox txtNationalId;
    private Panel panel1;
    private Label label4;
    private Label label3;
    private Label label2;
    private Label label1;

}