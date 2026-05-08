using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using HotelManagementSystem.WinForms.Data;

namespace HotelManagementSystem.WinForms.Forms
{
    public class ServicesStaffForm : Form
    {
        // Controls 
        private DataGridView dgvServices, dgvStaff, dgvAssignments;
        private ComboBox cmbServiceToAssign, cmbStaffToAssign;
        private TextBox txtServiceType, txtServicePrice, txtStaffFirst, txtStaffLast;
        private TextBox txtStaffPhone, txtStaffNatId, txtStaffSalary;
        private ComboBox cmbShift, cmbStatus, cmbStaffRole;
        private Button btnLoadServices, btnAddService, btnDeleteService;
        private Button btnLoadStaff, btnAddStaff, btnDeleteStaff;
        private Button btnLoadAssignments, btnAddAssignment, btnDeleteAssignment;
        private Button btnBack;
        private int? selectedServiceId = null, selectedStaffId = null;
        private int? selectedAssignmentStaffId = null;
        private int? selectedAssignmentServiceId = null;
        private DateTime? selectedAssignmentDate = null;

        public ServicesStaffForm()
        {
            Text = "Manage Services & Staff";
            Width = 1700;
            Height = 1000;
            StartPosition = FormStartPosition.CenterScreen;
            BuildUI();
            LoadServiceCombo();
            LoadAllData();
            LoadStaffCombo();
        }

        private void BuildUI()
        {
            int lx = 50, y = 20;

            // ========================================
            // SECTION 1: SERVICES (Top Left) - 
            // ========================================
            Controls.Add(new Label { Text = "Services", Left = lx, Top = y, Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold), AutoSize = true });
            y += 25;

            dgvServices = new DataGridView
            {
                Location = new System.Drawing.Point(lx, y),
                Size = new System.Drawing.Size(540, 200),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvServices.SelectionChanged += DgvServices_SelectionChanged;
            Controls.Add(dgvServices);
            y += 210;

            // Service input fields -
            int inputY = y;
            Controls.Add(new Label { Text = "Service Type:", Left = lx, Top = inputY, Width = 100 });
            txtServiceType = new TextBox { Left = lx + 110, Top = inputY, Width = 200 };
            Controls.Add(txtServiceType);

            Controls.Add(new Label { Text = "Price:", Left = lx + 330, Top = inputY, Width = 50 });
            txtServicePrice = new TextBox { Left = lx + 400, Top = inputY, Width = 120 };
            Controls.Add(txtServicePrice);
            inputY += 35;

            // Service buttons -
            btnLoadServices = new Button { Text = "Refresh", Left = lx, Top = inputY, Width = 80, Height = 32 };
            btnAddService = new Button { Text = "Add Service", Left = lx + 90, Top = inputY, Width = 100, Height = 32 };
            btnDeleteService = new Button { Text = "Delete Service", Left = lx + 200, Top = inputY, Width = 120, Height = 32 };

            btnLoadServices.Click += (_, _) => LoadServices();
            btnAddService.Click += BtnAddService_Click;
            btnDeleteService.Click += BtnDeleteService_Click;

            Controls.Add(btnLoadServices);
            Controls.Add(btnAddService);
            Controls.Add(btnDeleteService);

            // ========================================
            // SECTION 2: STAFF (Top Right)
            // ========================================
            y = 20;
            lx = 680;
            Controls.Add(new Label { Text = "Staff", Left = lx, Top = y, Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold), AutoSize = true });
            y += 25;

            dgvStaff = new DataGridView
            {
                Location = new System.Drawing.Point(lx, y),
                Size = new System.Drawing.Size(900, 200),
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvStaff.SelectionChanged += DgvStaff_SelectionChanged;
            Controls.Add(dgvStaff);
            y += 210;

            // Staff input fields
            int staffInputY = y;
            Controls.Add(new Label { Text = "First:", Left = lx, Top = staffInputY, Width = 50 });
            txtStaffFirst = new TextBox { Left = lx + 55, Top = staffInputY, Width = 100 };
            Controls.Add(txtStaffFirst);

            Controls.Add(new Label { Text = "Last:", Left = lx + 165, Top = staffInputY, Width = 40 });
            txtStaffLast = new TextBox { Left = lx + 210, Top = staffInputY, Width = 100 };
            Controls.Add(txtStaffLast);

            Controls.Add(new Label { Text = "Role:", Left = lx + 320, Top = staffInputY, Width = 40 });
            cmbStaffRole = new ComboBox
            {
                Left = lx + 365,
                Top = staffInputY,
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            LoadRoleCombo();
            Controls.Add(cmbStaffRole);

            Controls.Add(new Label { Text = "Phone:", Left = lx + 475, Top = staffInputY, Width = 50 });
            txtStaffPhone = new TextBox { Left = lx + 530, Top = staffInputY, Width = 100 };
            Controls.Add(txtStaffPhone);

            Controls.Add(new Label { Text = "ID:", Left = lx + 640, Top = staffInputY, Width = 30 });
            txtStaffNatId = new TextBox { Left = lx + 675, Top = staffInputY, Width = 120 };
            Controls.Add(txtStaffNatId);

            Controls.Add(new Label { Text = "Salary:", Left = lx + 805, Top = staffInputY, Width = 50 });
            txtStaffSalary = new TextBox { Left = lx + 860, Top = staffInputY, Width = 80 };
            Controls.Add(txtStaffSalary);

            btnAddStaff = new Button { Text = "Add Staff", Left = lx, Top = staffInputY + 40, Width = 90, Height = 32 };
            btnLoadStaff = new Button { Text = "Refresh", Left = lx + 100, Top = staffInputY + 40, Width = 90, Height = 32 };
            btnDeleteStaff = new Button { Text = "Delete", Left = lx + 200, Top = staffInputY + 40, Width = 90, Height = 32 };

            btnLoadStaff.Click += (_, _) => LoadStaff();
            btnAddStaff.Click += BtnAddStaff_Click;
            btnDeleteStaff.Click += BtnDeleteStaff_Click;

            Controls.Add(btnLoadStaff);
            Controls.Add(btnAddStaff);
            Controls.Add(btnDeleteStaff);

            // ========================================
            // SECTION 3: ASSIGNMENTS (Bottom)
            // ========================================
            y = 500;
            lx = 50;
            Controls.Add(new Label { Text = "Staff-Service Assignments", Left = lx, Top = y, Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold), AutoSize = true });
            y += 35;

            dgvAssignments = new DataGridView
            {
                Location = new System.Drawing.Point(lx, y),
                Size = new System.Drawing.Size(1450, 200),
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvAssignments.SelectionChanged += DgvAssignments_SelectionChanged;
            Controls.Add(dgvAssignments);

            y += 215;

            // Assignment controls - SIMPLIFIED (NO ROLE FILTERING)
            Controls.Add(new Label { Text = "Staff:", Left = lx, Top = y, Width = 50 });
            cmbStaffToAssign = new ComboBox { Left = lx + 55, Top = y + 2, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbStaffToAssign);

            Controls.Add(new Label { Text = "Service:", Left = lx + 265, Top = y, Width = 60 });
            cmbServiceToAssign = new ComboBox { Left = lx + 330, Top = y + 2, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbServiceToAssign);

            Controls.Add(new Label { Text = "Shift:", Left = lx + 545, Top = y, Width = 50 });
            cmbShift = new ComboBox { Left = lx + 600, Top = y + 2, Width = 100 };
            cmbShift.Items.AddRange(new[] { "Morning", "Evening" });
            Controls.Add(cmbShift);

            Controls.Add(new Label { Text = "Status:", Left = lx + 715, Top = y, Width = 60 });
            cmbStatus = new ComboBox { Left = lx + 780, Top = y + 2, Width = 100 };
            cmbStatus.Items.AddRange(new[] { "Active", "Inactive" });
            Controls.Add(cmbStatus);

            y += 40;

            btnLoadAssignments = new Button { Text = "Refresh", Left = lx, Top = y, Width = 90, Height = 35 };
            btnAddAssignment = new Button { Text = "Add Assignment", Left = lx + 95, Top = y, Width = 130, Height = 35 };
            btnDeleteAssignment = new Button { Text = "Delete Assignment", Left = lx + 235, Top = y, Width = 140, Height = 35 };
            btnBack = new Button { Text = "Back", Left = lx + 385, Top = y, Width = 80, Height = 35 };

            btnLoadAssignments.Click += (_, _) => LoadAssignments();
            btnAddAssignment.Click += BtnAddAssignment_Click;
            btnDeleteAssignment.Click += BtnDeleteAssignment_Click;
            btnBack.Click += (_, _) => Close();

            Controls.Add(btnLoadAssignments);
            Controls.Add(btnAddAssignment);
            Controls.Add(btnDeleteAssignment);
            Controls.Add(btnBack);
        }

        // ========================================
        // LOAD DATA
        // ========================================
        private void LoadServices()
        {
            string sql = """
                SELECT service_id, service_type, service_price 
                FROM SERVICE ORDER BY service_id
                """;
            dgvServices.DataSource = Db.ExecuteSelect(sql);
        }

        private void LoadStaff()
        {
            string sql = """
                SELECT staff_id, staff_first_name, staff_last_name, role, 
                       staff_phone, staff_national_id, salary
                FROM Staff ORDER BY staff_id
                """;
            dgvStaff.DataSource = Db.ExecuteSelect(sql);
        }

        private void LoadAssignments()
        {
            string sql = """
                SELECT p.staff_id, 
                       (st.staff_first_name + ' ' + st.staff_last_name) AS staff_name,
                       p.service_id, sv.service_type, 
                       p.assigned_date, p.shift, p.status
                FROM Provides p
                JOIN Staff st ON st.staff_id = p.staff_id
                JOIN SERVICE sv ON sv.service_id = p.service_id
                ORDER BY p.assigned_date DESC
                """;
            dgvAssignments.DataSource = Db.ExecuteSelect(sql);
        }

        private void LoadServiceCombo()
        {
            var dt = Db.ExecuteSelect("SELECT service_id, service_type FROM SERVICE ORDER BY service_type");
            cmbServiceToAssign.DisplayMember = "service_type";
            cmbServiceToAssign.ValueMember = "service_id";
            cmbServiceToAssign.DataSource = dt;
        }

        private void LoadStaffCombo()
        {
            // ✅ REMOVED ROLE FILTER - ANY ROLE CAN BE ASSIGNED
            var dt = Db.ExecuteSelect("""
                SELECT staff_id, staff_first_name + ' ' + staff_last_name AS name 
                FROM Staff ORDER BY staff_first_name
                """);
            cmbStaffToAssign.DisplayMember = "name";
            cmbStaffToAssign.ValueMember = "staff_id";
            cmbStaffToAssign.DataSource = dt;
        }

        private void LoadRoleCombo()
        {
            try
            {
                string sql = """
                    SELECT DISTINCT role 
                    FROM Staff 
                    WHERE role IS NOT NULL 
                    AND RTRIM(LTRIM(role)) != '' 
                    ORDER BY role
                    """;
                var dt = Db.ExecuteSelect(sql);
                cmbStaffRole.DataSource = null;
                cmbStaffRole.DataSource = dt;
                cmbStaffRole.DisplayMember = "role";
                cmbStaffRole.ValueMember = "role";
                cmbStaffRole.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ LoadRoleCombo Error: {ex.Message}");
            }
        }

        private void LoadAllData()
        {
            LoadServices();
            LoadStaff();
            LoadAssignments();
            LoadRoleCombo();
        }

        // ========================================
        // EVENT HANDLERS
        // ========================================
        private void DgvServices_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count > 0)
            {
                var row = dgvServices.SelectedRows[0];
                selectedServiceId = Convert.ToInt32(row.Cells["service_id"].Value);
                txtServiceType.Text = row.Cells["service_type"].Value?.ToString() ?? "";
                txtServicePrice.Text = row.Cells["service_price"].Value?.ToString() ?? "";
            }
        }

        private void DgvStaff_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count > 0)
            {
                var row = dgvStaff.SelectedRows[0];
                selectedStaffId = Convert.ToInt32(row.Cells["staff_id"].Value);
            }
        }

      


        /*private void DgvAssignments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAssignments.SelectedRows.Count > 0)
            {
                var row = dgvAssignments.SelectedRows[0];
                selectedAssignmentStaffId = Convert.ToInt32(row.Cells["staff_id"].Value);
                selectedAssignmentServiceId = Convert.ToInt32(row.Cells["service_id"].Value);
                selectedAssignmentDate = DateTime.Parse(row.Cells["assigned_date"].Value.ToString());
            }
        }*/

        private void DgvAssignments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAssignments.SelectedRows.Count > 0)
            {
                var row = dgvAssignments.SelectedRows[0];

                // Extract the keys from the hidden columns in the DataGridView
                if (row.Cells["staff_id"].Value != DBNull.Value &&
                    row.Cells["service_id"].Value != DBNull.Value &&
                    row.Cells["assigned_date"].Value != DBNull.Value)
                {
                    selectedAssignmentStaffId = Convert.ToInt32(row.Cells["staff_id"].Value);
                    selectedAssignmentServiceId = Convert.ToInt32(row.Cells["service_id"].Value);
                    selectedAssignmentDate = Convert.ToDateTime(row.Cells["assigned_date"].Value);
                }
            }
        }

        // ========================================
        // SERVICE CRUD - FROM OLD CODE
        // ========================================
        private void BtnAddService_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServiceType.Text) || string.IsNullOrWhiteSpace(txtServicePrice.Text))
            {
                MessageBox.Show("Fill Service Type and Price");
                return;
            }

            try
            {
                string sql = """
                    INSERT INTO SERVICE (service_type, service_price)
                    VALUES (@type, @price)
                    """;
                var p = new SqlParameter[]
                {
                    new("@type", txtServiceType.Text),
                    new("@price", decimal.Parse(txtServicePrice.Text))
                };
                Db.ExecuteNonQuery(sql, p);
                MessageBox.Show("✅ Service Added!");
                LoadServices();
                LoadServiceCombo();
                txtServiceType.Clear();
                txtServicePrice.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}");
            }
        }

        private void BtnDeleteService_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == null)
            {
                MessageBox.Show("Select a service");
                return;
            }
            if (MessageBox.Show("Delete this service?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                Db.ExecuteNonQuery("DELETE FROM SERVICE WHERE service_id = @id",
                    new SqlParameter("@id", selectedServiceId));
                MessageBox.Show("✅ Service Deleted!");
                LoadServices();
                LoadServiceCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Cannot delete (used in assignments): {ex.Message}");
            }
        }

        // ========================================
        // STAFF CRUD
        // ========================================
        private void BtnAddStaff_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffInputs()) return;

            try
            {
                string sql = """
                    INSERT INTO Staff (staff_first_name, staff_last_name, role, 
                                       staff_phone, staff_national_id, salary)
                    VALUES (@first, @last, @role, @phone, @natid, @salary)
                    """;
                var p = new SqlParameter[]
                {
                    new("@first", txtStaffFirst.Text),
                    new("@last", txtStaffLast.Text),
                    new("@role", cmbStaffRole.SelectedValue?.ToString() ?? ""),
                    new("@phone", txtStaffPhone.Text),
                    new("@natid", txtStaffNatId.Text),
                    new("@salary", decimal.Parse(txtStaffSalary.Text))
                };
                Db.ExecuteNonQuery(sql, p);
                MessageBox.Show("✅ Staff Added!");
                LoadStaff();
                LoadStaffCombo();
                ClearStaffInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}");
            }
        }

        private void BtnDeleteStaff_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == null)
            {
                MessageBox.Show("Select staff");
                return;
            }
            if (MessageBox.Show("Delete this staff?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                Db.ExecuteNonQuery("DELETE FROM Staff WHERE staff_id = @id",
                    new SqlParameter("@id", selectedStaffId));
                MessageBox.Show("✅ Staff Deleted!");
                LoadStaff();
                LoadStaffCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Cannot delete (assigned to services): {ex.Message}");
            }
        }

        /*private void BtnDeleteAssignment_Click(object sender, EventArgs e)
        {
            if (selectedAssignmentStaffId == null || selectedAssignmentServiceId == null || selectedAssignmentDate == null)
            {
                MessageBox.Show("Select an assignment from grid");
                return;
            }

            if (MessageBox.Show("Delete this assignment?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                string sql = """
                    DELETE FROM Provides 
                    WHERE staff_id = @staff AND service_id = @service AND assigned_date = @date
                    """;
                var p = new SqlParameter[]
                {
                    new("@staff", selectedAssignmentStaffId),
                    new("@service", selectedAssignmentServiceId),
                    new("@date", selectedAssignmentDate)
                };

                int deleted = Db.ExecuteNonQuery(sql, p);
                if (deleted > 0)
                {
                    MessageBox.Show("✅ Assignment deleted!");
                    LoadAssignments();
                    selectedAssignmentStaffId = null;
                    selectedAssignmentServiceId = null;
                    selectedAssignmentDate = null;
                }
                else
                {
                    MessageBox.Show("❌ No assignment found");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}");
            }
        }*/
        private void BtnDeleteAssignment_Click(object sender, EventArgs e)
        {
            // 1. Validation: Check if a row was actually selected
            if (selectedAssignmentStaffId == null || selectedAssignmentServiceId == null || selectedAssignmentDate == null)
            {
                MessageBox.Show("Please select an assignment from the table first.", "Selection Required");
                return;
            }

            // 2. Confirmation: Always ask before deleting
            if (MessageBox.Show("Are you sure you want to delete this specific assignment?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                // 3. Precise SQL: Use all three parts of the composite key
                string sql = """
            DELETE FROM Provides 
            WHERE staff_id = @staff 
              AND service_id = @service 
              AND assigned_date = @date
            """;

                SqlParameter[] p = {
            new SqlParameter("@staff", selectedAssignmentStaffId),
            new SqlParameter("@service", selectedAssignmentServiceId),
            new SqlParameter("@date", selectedAssignmentDate)
        };

                int rowsAffected = Db.ExecuteNonQuery(sql, p);

                if (rowsAffected > 0)
                {
                    MessageBox.Show("✅ Assignment deleted successfully!");
                    LoadAssignments(); // Refresh the table to show it's gone

                    // 4. Reset memory so we don't accidentally delete the same thing twice
                    selectedAssignmentStaffId = null;
                    selectedAssignmentServiceId = null;
                    selectedAssignmentDate = null;
                }
                else
                {
                    MessageBox.Show("❌ Could not find the assignment. It may have been deleted already.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error deleting assignment: {ex.Message}");
            }
        }

        private void BtnAddAssignment_Click(object sender, EventArgs e)
        {
            if (cmbStaffToAssign.SelectedValue == null || cmbServiceToAssign.SelectedValue == null)
            {
                MessageBox.Show("Select Staff and Service");
                return;
            }

            try
            {
                string sql = """
                    INSERT INTO Provides (staff_id, service_id, assigned_date, shift, status)
                    VALUES (@staff, @service, @date, @shift, @status)
                    """;
                var p = new SqlParameter[]
                {
                    new("@staff", cmbStaffToAssign.SelectedValue),
                    new("@service", cmbServiceToAssign.SelectedValue),
                    new("@date", DateTime.Today),
                    new("@shift", cmbShift.SelectedItem ?? "Morning"),
                    new("@status", cmbStatus.SelectedItem ?? "Active")
                };
                Db.ExecuteNonQuery(sql, p);
                MessageBox.Show("✅ Assignment Added!");
                LoadAssignments();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {ex.Message}");
            }
        }

        // Helpers
        private bool ValidateStaffInputs()
        {
            if (string.IsNullOrWhiteSpace(txtStaffFirst.Text) || string.IsNullOrWhiteSpace(txtStaffLast.Text))
                return false;
            if (!decimal.TryParse(txtStaffSalary.Text, out _))
                return false;
            return true;
        }

        private void ClearStaffInputs()
        {
            txtStaffFirst.Clear();
            txtStaffLast.Clear();
            txtStaffPhone.Clear();
            txtStaffNatId.Clear();
            txtStaffSalary.Clear();
            cmbStaffRole.SelectedIndex = -1;
            selectedStaffId = null;
        }
    }
}