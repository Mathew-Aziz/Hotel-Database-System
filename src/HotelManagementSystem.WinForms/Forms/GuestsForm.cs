using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace HotelManagementSystem.WinForms.Forms;

public class GuestsForm : Form
{
    public string connectionString = "Server=localhost;Database=HotelDb;Trusted_Connection=True;";
    public GuestsForm()
    {

        InitializeComponents();

        //if (!DesignMode)
        //    LoadGuests();
    }

    private void GuestsForm_Load(object sender, EventArgs e)
    {
        //if (!DesignMode)
        //    LoadGuests();
    }

    private void BtnLoad_Click(object sender, EventArgs e)
    {
        if(!DesignMode)
            LoadGuests();
        ClearTextBoxes();
    }


    private void LoadGuests()
    {
        string query = "SELECT * FROM Guests Order By guest_id";

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
        txtFirstName.Text = row.Cells["first_name"].Value?.ToString() ?? "";
        txtLastName.Text = row.Cells["last_name"].Value?.ToString() ?? "";
        txtNationalId.Text = row.Cells["national_id"].Value?.ToString() ?? "";
        txtPhone.Text = row.Cells["phone"].Value?.ToString() ?? "";

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

        string query = @"INSERT INTO Guests (first_name, last_name, phone, national_id) VALUES (@firstName, @lastName, @phone, @nationalId)";

        SqlParameter[] parameters =
        {
            new SqlParameter("@first_name", txtFirstName.Text.Trim()),
            new SqlParameter("@last_name", txtLastName.Text.Trim()),
            new SqlParameter("@phone", txtPhone.Text.Trim()),
            new SqlParameter("@national_id", txtNationalId.Text.Trim())
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


        string query = @"Update Guest 
                            SET first_name = @first_name, 
                            last_name = @last_name, 
                            phone = @phone, 
                            national_id = @national_id 
                            WHERE guest_id = @guest_id";


        SqlParameter[] parameters =
        {
            new SqlParameter("@first_name", txtFirstName.Text.Trim()),
            new SqlParameter("@last_name", txtLastName.Text.Trim()),
            new SqlParameter("@phone", txtPhone.Text.Trim()),
            new SqlParameter("@national_id", txtNationalId.Text.Trim())
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
                    LoadGuests();  // Refresh the grid
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
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding guest: {ex.Message}", "Database Error",
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
        if (!ValidateInputs())
            return;

        // Confirm Deletion
        DialogResult result = MessageBox.Show("Are you sure you want to delete this guest?\n\n" +
               "Note: If this guest has existing bookings, deletion may fail due to foreign key constraints.",
               "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result != DialogResult.Yes)
            return;

        string query = "DELETE FROM Guest WHERE guest_id = @guest_id";
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
            bool foreignKeyViolation = ex.Number == 547;
            if (foreignKeyViolation) // Foreign key violation (guest has bookings)
            {
                MessageBox.Show("Cannot delete this guest because they have existing bookings.\n\n" +
                    "Delete the guest's bookings first or contact your database administrator.",
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

    private void InitializeComponents()
    {
        btnBack = new Button();
        btnInsert = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnLoad = new Button();

        dgvGuests = new DataGridView();

        firstNameLabel = new Label();
        secondNameLabel = new Label();
        phoneLabel = new Label();
        nationalIDLabel = new Label();

        panel1 = new Panel();
        txtFirstName = new TextBox();
        txtLastName = new TextBox();
        txtPhone = new TextBox();
        txtNationalId = new TextBox();

        ((System.ComponentModel.ISupportInitialize)dgvGuests).BeginInit();
        panel1.SuspendLayout();
        SuspendLayout();
        // 
        // button1
        // 
        btnBack.Location = new Point(13, 12);
        btnBack.Name = "Back";
        btnBack.Size = new Size(98, 30);
        btnBack.TabIndex = 0;
        btnBack.Text = "Back";
        btnBack.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        btnInsert.Location = new Point(28, 422);
        btnInsert.Name = "button2";
        btnInsert.Size = new Size(98, 30);
        btnInsert.TabIndex = 1;
        btnInsert.Text = "Insert";
        btnInsert.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        btnUpdate.Location = new Point(425, 422);
        btnUpdate.Name = "button3";
        btnUpdate.Size = new Size(98, 30);
        btnUpdate.TabIndex = 2;
        btnUpdate.Text = "Update";
        btnUpdate.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        btnDelete.Location = new Point(604, 422);
        btnDelete.Name = "button4";
        btnDelete.Size = new Size(98, 30);
        btnDelete.TabIndex = 3;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        btnLoad.Location = new Point(234, 422);
        btnLoad.Name = "button5";
        btnLoad.Size = new Size(98, 30);
        btnLoad.TabIndex = 4;
        btnLoad.Text = "Load";
        btnLoad.UseVisualStyleBackColor = true;

        this.btnLoad.Click += new System.EventHandler(this.BtnLoad_Click);
        //this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
        this.btnUpdate.Click += new System.EventHandler(this.BtnUpdate_Click);
        this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
        this.btnBack.Click += new System.EventHandler(this.BtnBack_Click);
        this.dgvGuests.SelectionChanged += new System.EventHandler(this.DgvGuests_SelectionChanged);


        // 
        // dataGridView1
        // 
        dgvGuests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvGuests.Location = new Point(13, 171);
        dgvGuests.Name = "dataGridView1";
        dgvGuests.RowHeadersWidth = 53;
        dgvGuests.Size = new Size(689, 171);
        dgvGuests.TabIndex = 5;
        // 
        // label1
        // 
        firstNameLabel.AutoSize = true;
        firstNameLabel.Location = new Point(50, 368);
        firstNameLabel.Name = "label1";
        firstNameLabel.Size = new Size(52, 21);
        firstNameLabel.TabIndex = 6;
        firstNameLabel.Text = "label1";
        // 
        // label2
        // 
        secondNameLabel.AutoSize = true;
        secondNameLabel.Location = new Point(253, 368);
        secondNameLabel.Name = "label2";
        secondNameLabel.Size = new Size(52, 21);
        secondNameLabel.TabIndex = 6;
        secondNameLabel.Text = "label1";
        // 
        // label3
        // 
        phoneLabel.AutoSize = true;
        phoneLabel.Location = new Point(443, 368);
        phoneLabel.Name = "label3";
        phoneLabel.Size = new Size(52, 21);
        phoneLabel.TabIndex = 6;
        phoneLabel.Text = "label1";
        // 
        // label4
        // 
        nationalIDLabel.AutoSize = true;
        nationalIDLabel.Location = new Point(623, 368);
        nationalIDLabel.Name = "label4";
        nationalIDLabel.Size = new Size(52, 21);
        nationalIDLabel.TabIndex = 6;
        nationalIDLabel.Text = "label1";
        // 
        // textBox1
        // 
        txtFirstName.Location = new Point(188, 21);
        txtFirstName.Name = "textBox1";
        txtFirstName.Size = new Size(130, 29);
        txtFirstName.TabIndex = 7;
        // 
        // textBox2
        // 
        txtLastName.Location = new Point(20, 21);
        txtLastName.Name = "textBox2";
        txtLastName.Size = new Size(130, 29);
        txtLastName.TabIndex = 7;
        // 
        // textBox3
        // 
        txtPhone.Location = new Point(368, 21);
        txtPhone.Name = "textBox3";
        txtPhone.Size = new Size(130, 29);
        txtPhone.TabIndex = 7;
        // 
        // textBox4
        // 
        txtNationalId.Location = new Point(537, 21);
        txtNationalId.Name = "textBox4";
        txtNationalId.Size = new Size(130, 29);
        txtNationalId.TabIndex = 7;
        // 
        // panel1
        // 
        panel1.Controls.Add(txtLastName);
        panel1.Controls.Add(txtPhone);
        panel1.Controls.Add(txtNationalId);
        panel1.Controls.Add(txtFirstName);
        panel1.Location = new Point(13, 61);
        panel1.Name = "panel1";
        panel1.Size = new Size(689, 82);
        panel1.TabIndex = 8;
        // 
        // GuestsForm
        // 
        ClientSize = new Size(892, 593);
        Controls.Add(panel1);
        Controls.Add(nationalIDLabel);
        Controls.Add(phoneLabel);
        Controls.Add(secondNameLabel);
        Controls.Add(firstNameLabel);
        Controls.Add(dgvGuests);
        Controls.Add(btnLoad);
        Controls.Add(btnDelete);
        Controls.Add(btnUpdate);
        Controls.Add(btnInsert);
        Controls.Add(btnBack);
        Name = "GuestsForm";
        ((System.ComponentModel.ISupportInitialize)dgvGuests).EndInit();
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();

    }

    private void DgvGuests_SelectionChanged(object sender, EventArgs e)
    {
        PopulateTextBoxesFromSelectedRow();
    }

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

    private void InitializeComponent()
    {
        SuspendLayout();
        // 
        // GuestsForm
        // 
        ClientSize = new Size(282, 252);
        Name = "GuestsForm";
        ResumeLayout(false);

    }
}
