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

    private void GuestsForm_Load(object sender, EventArgs e)
    {
        // Remove this or keep empty - we're loading in constructor
    }

    private void BtnLoad_Click(object sender, EventArgs e)
    {
        if (!DesignMode)
            LoadGuests();
        ClearTextBoxes();
    }

    private void LoadGuests()
    {
        if (DesignMode) return;

        // FIXED: Changed from 'Guests' to 'GUEST'
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
        // FIXED: Updated column names to match database
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

        // FIXED: Updated table name and column names
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

        // FIXED: Updated table name and column names
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

        // FIXED: Changed from 'Guests' to 'GUEST'
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
        this.btnBack = new Button();
        this.btnInsert = new Button();
        this.btnUpdate = new Button();
        this.btnDelete = new Button();
        this.btnLoad = new Button();
        this.dgvGuests = new DataGridView();
        this.firstNameLabel = new Label();
        this.secondNameLabel = new Label();
        this.phoneLabel = new Label();
        this.nationalIDLabel = new Label();
        this.panel1 = new Panel();
        this.txtFirstName = new TextBox();
        this.txtLastName = new TextBox();
        this.txtPhone = new TextBox();
        this.txtNationalId = new TextBox();

        ((System.ComponentModel.ISupportInitialize)this.dgvGuests).BeginInit();
        this.panel1.SuspendLayout();
        this.SuspendLayout();

        // btnBack
        this.btnBack.Location = new System.Drawing.Point(13, 12);
        this.btnBack.Name = "btnBack";
        this.btnBack.Size = new System.Drawing.Size(98, 30);
        this.btnBack.TabIndex = 0;
        this.btnBack.Text = "Back";
        this.btnBack.UseVisualStyleBackColor = true;
        this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);

        // btnInsert
        this.btnInsert.Location = new System.Drawing.Point(28, 422);
        this.btnInsert.Name = "btnInsert";
        this.btnInsert.Size = new System.Drawing.Size(98, 30);
        this.btnInsert.TabIndex = 1;
        this.btnInsert.Text = "Insert";
        this.btnInsert.UseVisualStyleBackColor = true;
        this.btnInsert.Click += new System.EventHandler(this.BtnAdd_Click);

        // btnUpdate
        this.btnUpdate.Location = new System.Drawing.Point(425, 422);
        this.btnUpdate.Name = "btnUpdate";
        this.btnUpdate.Size = new System.Drawing.Size(98, 30);
        this.btnUpdate.TabIndex = 2;
        this.btnUpdate.Text = "Update";
        this.btnUpdate.UseVisualStyleBackColor = true;
        this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);

        // btnDelete
        this.btnDelete.Location = new System.Drawing.Point(604, 422);
        this.btnDelete.Name = "btnDelete";
        this.btnDelete.Size = new System.Drawing.Size(98, 30);
        this.btnDelete.TabIndex = 3;
        this.btnDelete.Text = "Delete";
        this.btnDelete.UseVisualStyleBackColor = true;
        this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);

        // btnLoad
        this.btnLoad.Location = new System.Drawing.Point(234, 422);
        this.btnLoad.Name = "btnLoad";
        this.btnLoad.Size = new System.Drawing.Size(98, 30);
        this.btnLoad.TabIndex = 4;
        this.btnLoad.Text = "Load";
        this.btnLoad.UseVisualStyleBackColor = true;
        this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);

        // dgvGuests
        this.dgvGuests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvGuests.Location = new System.Drawing.Point(13, 171);
        this.dgvGuests.Name = "dgvGuests";
        this.dgvGuests.RowHeadersWidth = 53;
        this.dgvGuests.Size = new System.Drawing.Size(689, 171);
        this.dgvGuests.TabIndex = 5;
        this.dgvGuests.SelectionChanged += new System.EventHandler(this.DgvGuests_SelectionChanged);

        // Labels
        this.firstNameLabel.AutoSize = true;
        this.firstNameLabel.Location = new System.Drawing.Point(50, 368);
        this.firstNameLabel.Name = "firstNameLabel";
        this.firstNameLabel.Size = new System.Drawing.Size(52, 21);
        this.firstNameLabel.TabIndex = 6;
        this.firstNameLabel.Text = "First Name";

        this.secondNameLabel.AutoSize = true;
        this.secondNameLabel.Location = new System.Drawing.Point(253, 368);
        this.secondNameLabel.Name = "secondNameLabel";
        this.secondNameLabel.Size = new System.Drawing.Size(52, 21);
        this.secondNameLabel.TabIndex = 6;
        this.secondNameLabel.Text = "Last Name";

        this.phoneLabel.AutoSize = true;
        this.phoneLabel.Location = new System.Drawing.Point(443, 368);
        this.phoneLabel.Name = "phoneLabel";
        this.phoneLabel.Size = new System.Drawing.Size(52, 21);
        this.phoneLabel.TabIndex = 6;
        this.phoneLabel.Text = "Phone";

        this.nationalIDLabel.AutoSize = true;
        this.nationalIDLabel.Location = new System.Drawing.Point(623, 368);
        this.nationalIDLabel.Name = "nationalIDLabel";
        this.nationalIDLabel.Size = new System.Drawing.Size(52, 21);
        this.nationalIDLabel.TabIndex = 6;
        this.nationalIDLabel.Text = "National ID";

        // Panel with TextBoxes
        this.panel1.Controls.Add(this.txtLastName);
        this.panel1.Controls.Add(this.txtPhone);
        this.panel1.Controls.Add(this.txtNationalId);
        this.panel1.Controls.Add(this.txtFirstName);
        this.panel1.Location = new System.Drawing.Point(13, 61);
        this.panel1.Name = "panel1";
        this.panel1.Size = new System.Drawing.Size(689, 82);
        this.panel1.TabIndex = 8;

        // TextBoxes
        this.txtFirstName.Location = new System.Drawing.Point(20, 21);
        this.txtFirstName.Name = "txtFirstName";
        this.txtFirstName.Size = new System.Drawing.Size(130, 29);
        this.txtFirstName.TabIndex = 7;

        this.txtLastName.Location = new System.Drawing.Point(188, 21);
        this.txtLastName.Name = "txtLastName";
        this.txtLastName.Size = new System.Drawing.Size(130, 29);
        this.txtLastName.TabIndex = 7;

        this.txtPhone.Location = new System.Drawing.Point(368, 21);
        this.txtPhone.Name = "txtPhone";
        this.txtPhone.Size = new System.Drawing.Size(130, 29);
        this.txtPhone.TabIndex = 7;

        this.txtNationalId.Location = new System.Drawing.Point(537, 21);
        this.txtNationalId.Name = "txtNationalId";
        this.txtNationalId.Size = new System.Drawing.Size(130, 29);
        this.txtNationalId.TabIndex = 7;

        // Form
        this.ClientSize = new System.Drawing.Size(892, 593);
        this.Controls.Add(this.panel1);
        this.Controls.Add(this.nationalIDLabel);
        this.Controls.Add(this.phoneLabel);
        this.Controls.Add(this.secondNameLabel);
        this.Controls.Add(this.firstNameLabel);
        this.Controls.Add(this.dgvGuests);
        this.Controls.Add(this.btnLoad);
        this.Controls.Add(this.btnDelete);
        this.Controls.Add(this.btnUpdate);
        this.Controls.Add(this.btnInsert);
        this.Controls.Add(this.btnBack);
        this.Name = "GuestsForm";
        this.Text = "Guests Management";
        this.Load += new System.EventHandler(this.GuestsForm_Load);

        ((System.ComponentModel.ISupportInitialize)this.dgvGuests).EndInit();
        this.panel1.ResumeLayout(false);
        this.panel1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    // Control declarations
    private Button btnBack;
    private Button btnInsert;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnLoad;
    private DataGridView dgvGuests;
    private Label firstNameLabel;
    private Label secondNameLabel;
    private Label phoneLabel;
    private Label nationalIDLabel;
    private TextBox txtFirstName;
    private TextBox txtLastName;
    private TextBox txtPhone;
    private TextBox txtNationalId;
    private Panel panel1;
}