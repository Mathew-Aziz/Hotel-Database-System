using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using HotelManagementSystem.WinForms.Data;

namespace HotelManagementSystem.WinForms.Forms
{
    public class ServicesStaffForm : Form
    {
        private DataGridView dgvServices;
        private TextBox txtServiceType, txtServicePrice;
        private Button btnLoadServices, btnAddService, btnUpdateService, btnDeleteService;
        private int? selectedServiceId = null;

        private DataGridView dgvStaff;
        private TextBox txtStaffFirst, txtStaffLast, txtStaffPhone, txtStaffNatId, txtStaffSalary;
        private ComboBox cmbStaffRole;
        private Button btnLoadStaff, btnAddStaff, btnUpdateStaff, btnDeleteStaff;
        private int? selectedStaffId = null;

        private DataGridView dgvUses;
        private ComboBox cmbUsesBooking, cmbUsesService;
        private DateTimePicker dtpUseDate;
        private TextBox txtUseQuantity;
        private Button btnLoadUses, btnAddUse, btnUpdateUse, btnDeleteUse;
        private int? selectedUsesBookingId = null;
        private int? selectedUsesServiceId = null;
        private DateTime? selectedUseDate = null;


        private DataGridView dgvAssignments;
        private ComboBox cmbStaffToAssign, cmbServiceToAssign, cmbShift, cmbStatus;
        private Button btnLoadAssignments, btnAddAssignment, btnUpdateAssignment, btnDeleteAssignment;
        private int? selectedAssignmentStaffId = null;
        private int? selectedAssignmentServiceId = null;
        private DateTime? selectedAssignmentDate = null;


        private Button btnBack;

        public ServicesStaffForm()
        {
            Text = "Manage Services & Staff";
            Width = 1750;
            Height = 1080;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScroll = true;
            BuildUI();
            LoadAllCombos();
            LoadAllGrids();
        }

        private void BuildUI()
        {
            btnBack = new Button
            {
                Text = "◄ Back to Menu",
                Width = 130, Height = 32,
                Left = Width - 160, Top = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBack.Click += BtnBack_Click;
            Controls.Add(btnBack);

            int y = 10;

            // ── SECTION 1: SERVICES ───────────────────────────────
            y = AddSectionLabel("Services (CRUD)", 20, y);

            dgvServices = MakeGrid(20, y, 700, 180);
            dgvServices.SelectionChanged += DgvServices_SelectionChanged;
            Controls.Add(dgvServices);
            y += 190;

            int x = 20;
            Controls.Add(MakeLabel("Type:", x, y));
            txtServiceType = new TextBox { Left = x + 45, Top = y, Width = 160 };
            Controls.Add(txtServiceType);

            Controls.Add(MakeLabel("Price:", x + 215, y));
            txtServicePrice = new TextBox { Left = x + 260, Top = y, Width = 100 };
            Controls.Add(txtServicePrice);
            y += 32;


            btnLoadServices   = MakeButton("Refresh",        x,        y);
            btnAddService     = MakeButton("Add",            x + 95,   y);
            btnUpdateService  = MakeButton("Update",         x + 185,  y);  
            btnDeleteService  = MakeButton("Delete",         x + 280,  y);

            btnLoadServices.Click  += (_, _) => LoadServices();
            btnAddService.Click    += BtnAddService_Click;
            btnUpdateService.Click += BtnUpdateService_Click;  
            btnDeleteService.Click += BtnDeleteService_Click;

            Controls.AddRange(new Control[] { btnLoadServices, btnAddService, btnUpdateService, btnDeleteService });
            y += 50;

            // ── SECTION 2: STAFF ──────────────────────────────────
            y = AddSectionLabel("Staff (CRUD)", 20, y);

            dgvStaff = MakeGrid(20, y, 1300, 180);
            dgvStaff.SelectionChanged += DgvStaff_SelectionChanged;
            Controls.Add(dgvStaff);
            y += 190;

            x = 20;
            Controls.Add(MakeLabel("First:", x, y));
            txtStaffFirst = new TextBox { Left = x + 45, Top = y, Width = 120 };
            Controls.Add(txtStaffFirst);

            Controls.Add(MakeLabel("Last:", x + 175, y));
            txtStaffLast = new TextBox { Left = x + 215, Top = y, Width = 120 };
            Controls.Add(txtStaffLast);

            Controls.Add(MakeLabel("Role:", x + 345, y));
            cmbStaffRole = new ComboBox
            {
                Left = x + 385, Top = y, Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStaffRole.Items.AddRange(new[] { "Manager", "Receptionist", "Cleaner", "Chef", "Laundry", "Security", "Maintenance" });
            Controls.Add(cmbStaffRole);

            Controls.Add(MakeLabel("Phone:", x + 515, y));
            txtStaffPhone = new TextBox { Left = x + 560, Top = y, Width = 120 };
            Controls.Add(txtStaffPhone);

            Controls.Add(MakeLabel("Nat ID:", x + 690, y));
            txtStaffNatId = new TextBox { Left = x + 745, Top = y, Width = 120 };
            Controls.Add(txtStaffNatId);

            Controls.Add(MakeLabel("Salary:", x + 875, y));
            txtStaffSalary = new TextBox { Left = x + 920, Top = y, Width = 100 };
            Controls.Add(txtStaffSalary);
            y += 35;

            btnLoadStaff  = MakeButton("Refresh",  x,        y);
            btnAddStaff   = MakeButton("Add",      x + 95,   y);
            btnUpdateStaff = MakeButton("Update",  x + 185,  y);  
            btnDeleteStaff = MakeButton("Delete",  x + 280,  y);

            btnLoadStaff.Click   += (_, _) => LoadStaff();
            btnAddStaff.Click    += BtnAddStaff_Click;
            btnUpdateStaff.Click += BtnUpdateStaff_Click;           /
            btnDeleteStaff.Click += BtnDeleteStaff_Click;

            Controls.AddRange(new Control[] { btnLoadStaff, btnAddStaff, btnUpdateStaff, btnDeleteStaff });
            y += 50;

            // ── SECTION 3: USES (Booking ↔ Service) ──────────────
            y = AddSectionLabel("Service Usage per Booking — USES (CRUD)", 20, y);

            dgvUses = MakeGrid(20, y, 1300, 180);
            dgvUses.SelectionChanged += DgvUses_SelectionChanged;
            Controls.Add(dgvUses);
            y += 190;

            x = 20;
            Controls.Add(MakeLabel("Booking:", x, y));
            cmbUsesBooking = new ComboBox { Left = x + 65, Top = y, Width = 220, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbUsesBooking);

            Controls.Add(MakeLabel("Service:", x + 295, y));
            cmbUsesService = new ComboBox { Left = x + 355, Top = y, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbUsesService);

            Controls.Add(MakeLabel("Date:", x + 545, y));
            dtpUseDate = new DateTimePicker { Left = x + 585, Top = y, Width = 140, Format = DateTimePickerFormat.Short };
            Controls.Add(dtpUseDate);

            Controls.Add(MakeLabel("Qty:", x + 735, y));
            txtUseQuantity = new TextBox { Left = x + 765, Top = y, Width = 70 };
            Controls.Add(txtUseQuantity);
            y += 35;

            btnLoadUses   = MakeButton("Refresh", x,        y);
            btnAddUse     = MakeButton("Add",     x + 95,   y);
            btnUpdateUse  = MakeButton("Update",  x + 185,  y);
            btnDeleteUse  = MakeButton("Delete",  x + 280,  y);

            btnLoadUses.Click   += (_, _) => LoadUses();
            btnAddUse.Click     += BtnAddUse_Click;
            btnUpdateUse.Click  += BtnUpdateUse_Click;
            btnDeleteUse.Click  += BtnDeleteUse_Click;

            Controls.AddRange(new Control[] { btnLoadUses, btnAddUse, btnUpdateUse, btnDeleteUse });
            y += 50;

            // ── SECTION 4: PROVIDES (Staff ↔ Service) ────────────
            y = AddSectionLabel("Staff-Service Assignments — PROVIDES (CRUD)", 20, y);

            dgvAssignments = MakeGrid(20, y, 1300, 180);
            dgvAssignments.SelectionChanged += DgvAssignments_SelectionChanged;
            Controls.Add(dgvAssignments);
            y += 190;

            x = 20;
            Controls.Add(MakeLabel("Staff:", x, y));
            cmbStaffToAssign = new ComboBox { Left = x + 50, Top = y, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbStaffToAssign);

            Controls.Add(MakeLabel("Service:", x + 260, y));
            cmbServiceToAssign = new ComboBox { Left = x + 320, Top = y, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.Add(cmbServiceToAssign);

            Controls.Add(MakeLabel("Shift:", x + 510, y));
            cmbShift = new ComboBox { Left = x + 555, Top = y, Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbShift.Items.AddRange(new[] { "Morning", "Evening" });
            cmbShift.SelectedIndex = 0;
            Controls.Add(cmbShift);

            Controls.Add(MakeLabel("Status:", x + 675, y));
            cmbStatus = new ComboBox { Left = x + 730, Top = y, Width = 110, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbStatus.Items.AddRange(new[] { "Active", "Inactive" });
            cmbStatus.SelectedIndex = 0;
            Controls.Add(cmbStatus);
            y += 35;

            btnLoadAssignments   = MakeButton("Refresh", x,       y);
            btnAddAssignment     = MakeButton("Add",     x + 95,  y);
            btnUpdateAssignment  = MakeButton("Update",  x + 185, y);   
            btnDeleteAssignment  = MakeButton("Delete",  x + 280, y);

            btnLoadAssignments.Click  += (_, _) => LoadAssignments();
            btnAddAssignment.Click    += BtnAddAssignment_Click;
            btnUpdateAssignment.Click += BtnUpdateAssignment_Click;    
            btnDeleteAssignment.Click += BtnDeleteAssignment_Click;

            Controls.AddRange(new Control[] { btnLoadAssignments, btnAddAssignment, btnUpdateAssignment, btnDeleteAssignment });
        }

        private void LoadAllGrids()
        {
            LoadServices();
            LoadStaff();
            LoadUses();
            LoadAssignments();
        }

        private void LoadAllCombos()
        {
            LoadServiceCombos();
            LoadStaffCombo();
            LoadBookingCombo();
        }

        private void LoadServices()
        {
            dgvServices.DataSource = Db.ExecuteSelect(
                "SELECT service_id, service_type, service_price FROM dbo.SERVICE ORDER BY service_id");
        }

        private void LoadStaff()
        {
            dgvStaff.DataSource = Db.ExecuteSelect(
                "SELECT staff_id, staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary FROM dbo.Staff ORDER BY staff_id");
        }

        private void LoadUses()
        {
            string sql = """
                SELECT u.booking_id, u.service_id, s.service_type,
                       u.use_date, u.quantity, s.service_price,
                       (u.quantity * s.service_price) AS line_total
                FROM dbo.Uses u
                JOIN dbo.SERVICE s ON s.service_id = u.service_id
                ORDER BY u.use_date, u.booking_id
                """;
            dgvUses.DataSource = Db.ExecuteSelect(sql);
        }

        private void LoadAssignments()
        {
            string sql = """
                SELECT p.staff_id,
                       (st.staff_first_name + ' ' + st.staff_last_name) AS staff_name,
                       p.service_id, sv.service_type,
                       p.assigned_date, p.shift, p.status
                FROM dbo.Provides p
                JOIN dbo.Staff st ON st.staff_id = p.staff_id
                JOIN dbo.SERVICE sv ON sv.service_id = p.service_id
                ORDER BY p.assigned_date DESC
                """;
            dgvAssignments.DataSource = Db.ExecuteSelect(sql);
        }

        private void LoadServiceCombos()
        {
            var dt = Db.ExecuteSelect("SELECT service_id, service_type FROM dbo.SERVICE ORDER BY service_type");

            cmbServiceToAssign.DataSource    = null;
            cmbServiceToAssign.DataSource    = dt;
            cmbServiceToAssign.DisplayMember = "service_type";
            cmbServiceToAssign.ValueMember   = "service_id";

            var dt2 = Db.ExecuteSelect("SELECT service_id, service_type FROM dbo.SERVICE ORDER BY service_type");
            cmbUsesService.DataSource    = null;
            cmbUsesService.DataSource    = dt2;
            cmbUsesService.DisplayMember = "service_type";
            cmbUsesService.ValueMember   = "service_id";
        }

        private void LoadStaffCombo()
        {
            var dt = Db.ExecuteSelect(
                "SELECT staff_id, staff_first_name + ' ' + staff_last_name AS name FROM dbo.Staff ORDER BY staff_first_name");
            cmbStaffToAssign.DataSource    = null;
            cmbStaffToAssign.DataSource    = dt;
            cmbStaffToAssign.DisplayMember = "name";
            cmbStaffToAssign.ValueMember   = "staff_id";
        }

        private void LoadBookingCombo()
        {
            var dt = Db.ExecuteSelect("""
                SELECT b.booking_id,
                       CAST(b.booking_id AS NVARCHAR) + ' - ' +
                       g.guest_first_name + ' ' + g.guest_last_name AS display
                FROM dbo.BOOKING b
                JOIN dbo.GUEST g ON g.guest_id = b.guest_id
                ORDER BY b.booking_id
                """);
            cmbUsesBooking.DataSource    = null;
            cmbUsesBooking.DataSource    = dt;
            cmbUsesBooking.DisplayMember = "display";
            cmbUsesBooking.ValueMember   = "booking_id";
        }

        private void DgvServices_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvServices.SelectedRows.Count == 0) return;
            var row = dgvServices.SelectedRows[0];
            if (row.Cells["service_id"].Value == DBNull.Value) return;

            selectedServiceId     = Convert.ToInt32(row.Cells["service_id"].Value);
            txtServiceType.Text   = row.Cells["service_type"].Value?.ToString() ?? "";
            txtServicePrice.Text  = row.Cells["service_price"].Value?.ToString() ?? "";
        }

        private void DgvStaff_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvStaff.SelectedRows.Count == 0) return;
            var row = dgvStaff.SelectedRows[0];
            if (row.Cells["staff_id"].Value == DBNull.Value) return;

            selectedStaffId       = Convert.ToInt32(row.Cells["staff_id"].Value);
            txtStaffFirst.Text    = row.Cells["staff_first_name"].Value?.ToString() ?? "";
            txtStaffLast.Text     = row.Cells["staff_last_name"].Value?.ToString() ?? "";
            txtStaffPhone.Text    = row.Cells["staff_phone"].Value?.ToString() ?? "";
            txtStaffNatId.Text    = row.Cells["staff_national_id"].Value?.ToString() ?? "";
            txtStaffSalary.Text   = row.Cells["salary"].Value?.ToString() ?? "";

            string role = row.Cells["role"].Value?.ToString() ?? "";
            int idx = cmbStaffRole.FindStringExact(role);
            cmbStaffRole.SelectedIndex = idx >= 0 ? idx : -1;
        }

        private void DgvUses_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUses.SelectedRows.Count == 0) return;
            var row = dgvUses.SelectedRows[0];
            if (row.Cells["booking_id"].Value == DBNull.Value) return;

            selectedUsesBookingId = Convert.ToInt32(row.Cells["booking_id"].Value);
            selectedUsesServiceId = Convert.ToInt32(row.Cells["service_id"].Value);
            selectedUseDate       = Convert.ToDateTime(row.Cells["use_date"].Value);

            SetComboValue(cmbUsesBooking,  selectedUsesBookingId);
            SetComboValue(cmbUsesService,  selectedUsesServiceId);
            dtpUseDate.Value       = selectedUseDate.Value;
            txtUseQuantity.Text    = row.Cells["quantity"].Value?.ToString() ?? "";
        }

        private void DgvAssignments_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvAssignments.SelectedRows.Count == 0) return;
            var row = dgvAssignments.SelectedRows[0];
            if (row.Cells["staff_id"].Value == DBNull.Value) return;

            selectedAssignmentStaffId   = Convert.ToInt32(row.Cells["staff_id"].Value);
            selectedAssignmentServiceId = Convert.ToInt32(row.Cells["service_id"].Value);
            selectedAssignmentDate      = Convert.ToDateTime(row.Cells["assigned_date"].Value);

            SetComboValue(cmbStaffToAssign,   selectedAssignmentStaffId);
            SetComboValue(cmbServiceToAssign, selectedAssignmentServiceId);

            string shift  = row.Cells["shift"].Value?.ToString() ?? "";
            string status = row.Cells["status"].Value?.ToString() ?? "";
            int si = cmbShift.FindStringExact(shift);
            int st = cmbStatus.FindStringExact(status);
            if (si >= 0) cmbShift.SelectedIndex  = si;
            if (st >= 0) cmbStatus.SelectedIndex = st;
        }

        //  SERVICE CRUD
        private void BtnAddService_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServiceType.Text))
            { MessageBox.Show("Enter service type.", "Validation"); return; }

            if (!decimal.TryParse(txtServicePrice.Text, out decimal price) || price <= 0)
            { MessageBox.Show("Price must be a number greater than 0.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "INSERT INTO dbo.SERVICE (service_type, service_price) VALUES (@type, @price)",
                    new SqlParameter("@type",  txtServiceType.Text.Trim()),
                    new SqlParameter("@price", price));

                MessageBox.Show("✅ Service added!");
                ClearServiceInputs();
                LoadServices();
                LoadServiceCombos();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void BtnUpdateService_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == null)
            { MessageBox.Show("Select a service row first.", "Selection Required"); return; }

            if (string.IsNullOrWhiteSpace(txtServiceType.Text))
            { MessageBox.Show("Enter service type.", "Validation"); return; }

            if (!decimal.TryParse(txtServicePrice.Text, out decimal price) || price <= 0)
            { MessageBox.Show("Price must be a number greater than 0.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "UPDATE dbo.SERVICE SET service_type=@type, service_price=@price WHERE service_id=@id",
                    new SqlParameter("@type",  txtServiceType.Text.Trim()),
                    new SqlParameter("@price", price),
                    new SqlParameter("@id",    selectedServiceId));

                MessageBox.Show("✅ Service updated!");
                ClearServiceInputs();
                LoadServices();
                LoadServiceCombos();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void BtnDeleteService_Click(object sender, EventArgs e)
        {
            if (selectedServiceId == null)
            { MessageBox.Show("Select a service row first.", "Selection Required"); return; }

            if (MessageBox.Show("Delete this service?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                Db.ExecuteNonQuery(
                    "DELETE FROM dbo.SERVICE WHERE service_id=@id",
                    new SqlParameter("@id", selectedServiceId));

                MessageBox.Show("✅ Service deleted!");
                ClearServiceInputs();
                LoadServices();
                LoadServiceCombos();
            }
            catch (Exception ex) { ShowDbError(ex, "Cannot delete — service is used in assignments or bookings."); }
        }

        private void ClearServiceInputs()
        {
            txtServiceType.Clear();
            txtServicePrice.Clear();
            selectedServiceId = null;
        }

        //  STAFF CRUD
        private void BtnAddStaff_Click(object sender, EventArgs e)
        {
            if (!ValidateStaffInputs(out decimal salary)) return;

            try
            {
                Db.ExecuteNonQuery(
                    """
                    INSERT INTO dbo.Staff
                        (staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary)
                    VALUES (@first, @last, @role, @phone, @natid, @salary)
                    """,
                    new SqlParameter("@first",  txtStaffFirst.Text.Trim()),
                    new SqlParameter("@last",   txtStaffLast.Text.Trim()),
                    new SqlParameter("@role",   cmbStaffRole.SelectedItem?.ToString() ?? ""),
                    new SqlParameter("@phone",  txtStaffPhone.Text.Trim()),
                    new SqlParameter("@natid",  txtStaffNatId.Text.Trim()),
                    new SqlParameter("@salary", salary));

                MessageBox.Show("✅ Staff added!");
                ClearStaffInputs();
                LoadStaff();
                LoadStaffCombo();
            }
            catch (Exception ex) { ShowDbError(ex, "National ID may already exist."); }
        }

        private void BtnUpdateStaff_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == null)
            { MessageBox.Show("Select a staff row first.", "Selection Required"); return; }

            if (!ValidateStaffInputs(out decimal salary)) return;

            try
            {
                Db.ExecuteNonQuery(
                    """
                    UPDATE dbo.Staff SET
                        staff_first_name  = @first,
                        staff_last_name   = @last,
                        role              = @role,
                        staff_phone       = @phone,
                        staff_national_id = @natid,
                        salary            = @salary
                    WHERE staff_id = @id
                    """,
                    new SqlParameter("@first",  txtStaffFirst.Text.Trim()),
                    new SqlParameter("@last",   txtStaffLast.Text.Trim()),
                    new SqlParameter("@role",   cmbStaffRole.SelectedItem?.ToString() ?? ""),
                    new SqlParameter("@phone",  txtStaffPhone.Text.Trim()),
                    new SqlParameter("@natid",  txtStaffNatId.Text.Trim()),
                    new SqlParameter("@salary", salary),
                    new SqlParameter("@id",     selectedStaffId));

                MessageBox.Show("✅ Staff updated!");
                ClearStaffInputs();
                LoadStaff();
                LoadStaffCombo();
            }
            catch (Exception ex) { ShowDbError(ex, "National ID may already exist."); }
        }

        private void BtnDeleteStaff_Click(object sender, EventArgs e)
        {
            if (selectedStaffId == null)
            { MessageBox.Show("Select a staff row first.", "Selection Required"); return; }

            if (MessageBox.Show("Delete this staff member?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                Db.ExecuteNonQuery(
                    "DELETE FROM dbo.Staff WHERE staff_id=@id",
                    new SqlParameter("@id", selectedStaffId));

                MessageBox.Show("✅ Staff deleted!");
                ClearStaffInputs();
                LoadStaff();
                LoadStaffCombo();
            }
            catch (Exception ex) { ShowDbError(ex, "Cannot delete — staff member has service assignments."); }
        }

        private bool ValidateStaffInputs(out decimal salary)
        {
            salary = 0;
            if (string.IsNullOrWhiteSpace(txtStaffFirst.Text) || string.IsNullOrWhiteSpace(txtStaffLast.Text))
            { MessageBox.Show("First and last name are required.", "Validation"); return false; }

            if (string.IsNullOrWhiteSpace(txtStaffNatId.Text))
            { MessageBox.Show("National ID is required.", "Validation"); return false; }

            if (cmbStaffRole.SelectedIndex < 0)
            { MessageBox.Show("Select a role.", "Validation"); return false; }

            // FIX: salary validation (>= 0 required by DB constraint)
            if (!decimal.TryParse(txtStaffSalary.Text, out salary) || salary < 0)
            { MessageBox.Show("Salary must be a number 0 or greater.", "Validation"); return false; }

            return true;
        }

        private void ClearStaffInputs()
        {
            txtStaffFirst.Clear(); txtStaffLast.Clear();
            txtStaffPhone.Clear(); txtStaffNatId.Clear(); txtStaffSalary.Clear();
            cmbStaffRole.SelectedIndex = -1;
            selectedStaffId = null;
        }

        //  USES CRUD  
        private void BtnAddUse_Click(object sender, EventArgs e)
        {
            if (cmbUsesBooking.SelectedValue == null || cmbUsesService.SelectedValue == null)
            { MessageBox.Show("Select booking and service.", "Validation"); return; }

            if (!int.TryParse(txtUseQuantity.Text, out int qty) || qty <= 0)
            { MessageBox.Show("Quantity must be an integer greater than 0.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "INSERT INTO dbo.Uses (booking_id, service_id, use_date, quantity) VALUES (@bid, @sid, @date, @qty)",
                    new SqlParameter("@bid",  cmbUsesBooking.SelectedValue),
                    new SqlParameter("@sid",  cmbUsesService.SelectedValue),
                    new SqlParameter("@date", dtpUseDate.Value.Date),
                    new SqlParameter("@qty",  qty));

                MessageBox.Show("✅ Service usage added!");
                ClearUsesSelection();
                LoadUses();
            }
            catch (Exception ex) { ShowDbError(ex, "That service may already be recorded for this booking on that date."); }
        }

        private void BtnUpdateUse_Click(object sender, EventArgs e)
        {
            if (selectedUsesBookingId == null || selectedUsesServiceId == null || selectedUseDate == null)
            { MessageBox.Show("Select a uses row first.", "Selection Required"); return; }

            if (!int.TryParse(txtUseQuantity.Text, out int qty) || qty <= 0)
            { MessageBox.Show("Quantity must be an integer greater than 0.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "UPDATE dbo.Uses SET quantity=@qty WHERE booking_id=@bid AND service_id=@sid AND use_date=@date",
                    new SqlParameter("@qty",  qty),
                    new SqlParameter("@bid",  selectedUsesBookingId),
                    new SqlParameter("@sid",  selectedUsesServiceId),
                    new SqlParameter("@date", selectedUseDate.Value.Date));

                MessageBox.Show("✅ Usage updated!");
                ClearUsesSelection();
                LoadUses();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void BtnDeleteUse_Click(object sender, EventArgs e)
        {
            if (selectedUsesBookingId == null || selectedUsesServiceId == null || selectedUseDate == null)
            { MessageBox.Show("Select a uses row first.", "Selection Required"); return; }

            if (MessageBox.Show("Delete this usage record?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                int rows = Db.ExecuteNonQuery(
                    "DELETE FROM dbo.Uses WHERE booking_id=@bid AND service_id=@sid AND use_date=@date",
                    new SqlParameter("@bid",  selectedUsesBookingId),
                    new SqlParameter("@sid",  selectedUsesServiceId),
                    new SqlParameter("@date", selectedUseDate.Value.Date));

                MessageBox.Show(rows > 0 ? "✅ Usage deleted!" : "❌ Record not found.");
                ClearUsesSelection();
                LoadUses();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void ClearUsesSelection()
        {
            selectedUsesBookingId = null;
            selectedUsesServiceId = null;
            selectedUseDate       = null;
            txtUseQuantity.Clear();
        }

        //  PROVIDES CRUD
        private void BtnAddAssignment_Click(object sender, EventArgs e)
        {
            if (cmbStaffToAssign.SelectedValue == null)
            { MessageBox.Show("Select a staff member.", "Validation"); return; }
            if (cmbServiceToAssign.SelectedValue == null)
            { MessageBox.Show("Select a service.", "Validation"); return; }
            if (cmbShift.SelectedIndex < 0)
            { MessageBox.Show("Select a shift.", "Validation"); return; }
            if (cmbStatus.SelectedIndex < 0)
            { MessageBox.Show("Select a status.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "INSERT INTO dbo.Provides (staff_id, service_id, assigned_date, shift, status) VALUES (@staff, @service, @date, @shift, @status)",
                    new SqlParameter("@staff",   cmbStaffToAssign.SelectedValue),
                    new SqlParameter("@service", cmbServiceToAssign.SelectedValue),
                    new SqlParameter("@date",    DateTime.Today),
                    new SqlParameter("@shift",   cmbShift.SelectedItem.ToString()),
                    new SqlParameter("@status",  cmbStatus.SelectedItem.ToString()));

                MessageBox.Show("✅ Assignment added!");
                ClearAssignmentSelection();
                LoadAssignments();
            }
            catch (Exception ex) { ShowDbError(ex, "This staff-service combination may already be assigned today."); }
        }

        private void BtnUpdateAssignment_Click(object sender, EventArgs e)
        {
            if (selectedAssignmentStaffId == null || selectedAssignmentServiceId == null || selectedAssignmentDate == null)
            { MessageBox.Show("Select an assignment row first.", "Selection Required"); return; }

            if (cmbShift.SelectedIndex < 0 || cmbStatus.SelectedIndex < 0)
            { MessageBox.Show("Select shift and status.", "Validation"); return; }

            try
            {
                Db.ExecuteNonQuery(
                    "UPDATE dbo.Provides SET shift=@shift, status=@status WHERE staff_id=@staff AND service_id=@service AND assigned_date=@date",
                    new SqlParameter("@shift",   cmbShift.SelectedItem.ToString()),
                    new SqlParameter("@status",  cmbStatus.SelectedItem.ToString()),
                    new SqlParameter("@staff",   selectedAssignmentStaffId),
                    new SqlParameter("@service", selectedAssignmentServiceId),
                    new SqlParameter("@date",    selectedAssignmentDate.Value.Date));

                MessageBox.Show("✅ Assignment updated!");
                ClearAssignmentSelection();
                LoadAssignments();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void BtnDeleteAssignment_Click(object sender, EventArgs e)
        {
            if (selectedAssignmentStaffId == null || selectedAssignmentServiceId == null || selectedAssignmentDate == null)
            { MessageBox.Show("Select an assignment row first.", "Selection Required"); return; }

            if (MessageBox.Show("Delete this assignment?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            try
            {
                int rows = Db.ExecuteNonQuery(
                    "DELETE FROM dbo.Provides WHERE staff_id=@staff AND service_id=@service AND assigned_date=@date",
                    new SqlParameter("@staff",   selectedAssignmentStaffId),
                    new SqlParameter("@service", selectedAssignmentServiceId),
                    new SqlParameter("@date",    selectedAssignmentDate.Value.Date));

                MessageBox.Show(rows > 0 ? "✅ Assignment deleted!" : "❌ Assignment not found.");
                ClearAssignmentSelection();
                LoadAssignments();
            }
            catch (Exception ex) { ShowDbError(ex); }
        }

        private void ClearAssignmentSelection()
        {
            selectedAssignmentStaffId   = null;
            selectedAssignmentServiceId = null;
            selectedAssignmentDate      = null;
        }

        //  NAVIGATION — FIX: show main menu, don't just close
        private void BtnBack_Click(object sender, EventArgs e)
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f is MainMenuForm)
                {
                    f.Show();
                    break;
                }
            }
            Close();
        }


        //  HELPERS

        /// <summary>Shows a friendly error message for DB failures.</summary>
        private static void ShowDbError(Exception ex, string hint = "")
        {
            string msg = "❌ Database error.";
            if (!string.IsNullOrEmpty(hint)) msg += "\n" + hint;
            msg += "\n\nDetail: " + ex.Message;
            MessageBox.Show(msg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>Sets a ComboBox selection by its ValueMember value.</summary>
        private static void SetComboValue(ComboBox cmb, object? value)
        {
            if (value == null || cmb.DataSource is not DataTable dt) return;
            string target = value.ToString() ?? "";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][cmb.ValueMember].ToString() == target)
                {
                    cmb.SelectedIndex = i;
                    return;
                }
            }
        }

        private DataGridView MakeGrid(int left, int top, int width, int height) =>
            new DataGridView
            {
                Location = new System.Drawing.Point(left, top),
                Size     = new System.Drawing.Size(width, height),
                ReadOnly = true,
                SelectionMode        = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode  = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows   = false
            };

        private Button MakeButton(string text, int left, int top) =>
            new Button { Text = text, Left = left, Top = top, Width = 88, Height = 32 };

        private Label MakeLabel(string text, int left, int top) =>
            new Label { Text = text, Left = left, Top = top, AutoSize = true };

        private int AddSectionLabel(string text, int left, int top)
        {
            Controls.Add(new Label
            {
                Text     = text,
                Left     = left,
                Top      = top,
                AutoSize = true,
                Font     = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold)
            });
            return top + 28;
        }
    }
}