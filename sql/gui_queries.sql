/* ============================================================
   GUI Queries (ADO.NET) — HotelDB
   PURPOSE:
   - This file is meant for the GUI team to COPY/PASTE queries into C# (ADO.NET).
   - These queries use parameters like @guest_id, @room_id, etc.
     Those parameters are provided from C# using SqlCommand.Parameters.
   - If you run this whole file directly in SSMS, you will get:
       "Must declare the scalar variable @..."
     That is NORMAL because SSMS doesn't know the parameter values unless you DECLARE them.

   HOW TO USE IN WINFORMS:
   - For SELECT queries:
       DataTable dt = Db.ExecuteSelect(sql, parameters...);
       dgv.DataSource = dt;
   - For INSERT/UPDATE/DELETE:
       int rows = Db.ExecuteNonQuery(sql, parameters...);
       then re-run the SELECT to refresh the DataGridView.

   FORMS MAPPING (recommended 5 forms):
   1) GuestsForm            -> GUEST queries
   2) RoomsForm             -> ROOM + ROOMFEATURES queries (optional section/tab)
   3) BookingsForm          -> BOOKING + OCCUPIES queries
   4) ServicesStaffForm     -> USES + PROVIDES queries (+ optional SERVICE/STAFF CRUD)
   5) MainMenuForm          -> no SQL, just navigation
   ============================================================ */

---------------------------------------------------------------
-- GUEST (CRUD)  --> GuestsForm
---------------------------------------------------------------

-- [GuestsForm] SELECT: Load/Refresh guests list in DataGridView.
-- Use this on:
--   - Form_Load
--   - "Refresh" button click
-- Shows every guest record with its PK (guest_id).
SELECT
  guest_id,               -- Primary Key (used for Update/Delete)
  guest_national_id,      -- Unique national ID (user enters this)
  guest_first_name,
  guest_last_name,
  guest_phone
FROM dbo.GUEST
ORDER BY guest_id;

-- [GuestsForm] INSERT: Add new guest.
-- Use this on "Add" button after validating inputs.
-- Params expected from TextBoxes:
--   @national_id, @first_name, @last_name, @phone
-- Notes:
--   - guest_national_id is UNIQUE + NOT NULL (insert will fail if duplicate/blank).
INSERT INTO dbo.GUEST (guest_national_id, guest_first_name, guest_last_name, guest_phone)
VALUES (@national_id, @first_name, @last_name, @phone);

-- [GuestsForm] UPDATE: Update selected guest.
-- Use this on "Update" button.
-- Typical UI pattern:
--   - user selects a row in DataGridView
--   - populate TextBoxes
--   - user edits fields
--   - click Update
-- Params:
--   @guest_id must come from the selected row (PK).
UPDATE dbo.GUEST
SET
  guest_national_id = @national_id,
  guest_first_name  = @first_name,
  guest_last_name   = @last_name,
  guest_phone       = @phone
WHERE guest_id = @guest_id;

-- [GuestsForm] DELETE: Delete selected guest.
-- Use this on "Delete" button for the selected row.
-- WARNING:
--   If the guest has bookings, deletion may fail unless you allow cascading.
--   Handle SqlException in GUI and show a friendly message.
DELETE FROM dbo.GUEST
WHERE guest_id = @guest_id;


---------------------------------------------------------------
-- ROOM (CRUD)  --> RoomsForm
---------------------------------------------------------------

-- [RoomsForm] SELECT: Load/Refresh rooms list in DataGridView.
-- Shows all rooms and their prices.
-- (In your current schema, room status is NOT stored; only type + price.)
SELECT
  room_id,     -- PK used for Update/Delete and for Occupies link
  room_type,   -- e.g., SINGLE, DOUBLE, etc.
   room_price AS room_price_per_night -- price per unit (you can interpret as per night in GUI)
FROM dbo.ROOM
ORDER BY room_id;

-- [RoomsForm] INSERT: Add a new room.
-- Params:
--   @room_type from ComboBox/TextBox
--   @room_price from numeric input (validate > 0)
INSERT INTO dbo.ROOM (room_type, room_price)
VALUES (@room_type, @room_price);

-- [RoomsForm] UPDATE: Update selected room.
-- Params:
--   @room_id from selected DataGridView row
UPDATE dbo.ROOM
SET
  room_type  = @room_type,
  room_price = @room_price
WHERE room_id = @room_id;

-- [RoomsForm] DELETE: Delete selected room.
-- WARNING:
--   If this room is referenced in Occupies (booked), deletion will fail.
--   GUI should catch the error and instruct user to remove the room from bookings first.
DELETE FROM dbo.ROOM
WHERE room_id = @room_id;


---------------------------------------------------------------
-- ROOMFEATURES (CRUD-ish)  --> RoomsForm (optional section/tab)
---------------------------------------------------------------

-- [RoomsForm - Features] SELECT: Load features for ONE selected room.
-- Params:
--   @room_id = selected room's PK
-- Use case:
--   - user selects a room in Rooms grid
--   - GUI loads its features into another grid/list
SELECT room_id, features
FROM dbo.ROOMFEATURES
WHERE room_id = @room_id
ORDER BY features;

-- [RoomsForm - Features] INSERT: Add one feature to the room.
-- Params:
--   @room_id, @feature (string like 'AC', 'Kitchen', 'Coffee Machine')
-- NOTE:
--   PK is (room_id, features) so duplicates will fail automatically.
INSERT INTO dbo.ROOMFEATURES (room_id, features)
VALUES (@room_id, @feature);

-- [RoomsForm - Features] DELETE: Remove one feature from room.
-- Params:
--   @room_id, @feature
DELETE FROM dbo.ROOMFEATURES
WHERE room_id = @room_id AND features = @feature;


---------------------------------------------------------------
-- BOOKING (CRUD + display)  --> BookingsForm
---------------------------------------------------------------

-- [BookingsForm] SELECT: Load bookings list for DataGridView (includes guest name).
-- This is a JOIN so the GUI can show guest_full_name instead of only guest_id.
-- Good for "Bookings grid" main display.
SELECT
  b.booking_id,  -- PK used for Update/Delete and linking to Occupies/Uses
  b.guest_id,    -- FK; still included so GUI can store the ID
  (g.guest_first_name + ' ' + g.guest_last_name) AS guest_full_name,
  b.check_in_date,
  b.check_out_date,
  b.booking_status,
  b.payment_status
FROM dbo.BOOKING b
JOIN dbo.GUEST g ON g.guest_id = b.guest_id
ORDER BY b.booking_id;

-- [BookingsForm] SELECT: Load bookings for ONE guest (optional filter).
-- Params:
--   @guest_id selected from guest ComboBox (or a guest row)
-- Use case:
--   - show only bookings for a guest to reduce clutter
SELECT
  booking_id, guest_id, check_in_date, check_out_date, booking_status, payment_status
FROM dbo.BOOKING
WHERE guest_id = @guest_id
ORDER BY booking_id;

-- [BookingsForm] INSERT: Create a new booking.
-- Params:
--   @guest_id from guest ComboBox
--   @check_in, @check_out from DateTimePickers
--   @booking_status, @payment_status from ComboBoxes
-- IMPORTANT:
--   Your DB has CHECK(check_out_date > check_in_date) so validate in GUI too.
INSERT INTO dbo.BOOKING (guest_id, check_in_date, check_out_date, booking_status, payment_status)
VALUES (@guest_id, @check_in, @check_out, @booking_status, @payment_status);

-- [BookingsForm] UPDATE: Edit existing booking.
-- Use case:
--   - change dates/status/payment status
-- Params:
--   @booking_id from selected booking row (PK)
UPDATE dbo.BOOKING
SET
  guest_id        = @guest_id,
  check_in_date   = @check_in,
  check_out_date  = @check_out,
  booking_status  = @booking_status,
  payment_status  = @payment_status
WHERE booking_id = @booking_id;

-- [BookingsForm] DELETE: Delete booking.
-- Because you used ON DELETE CASCADE, related rows in:
--   - dbo.Occupies (rooms in booking)
--   - dbo.Uses (services used in booking)
-- will be deleted automatically.
-- GUI should confirm with user (MessageBox Yes/No) before deleting.
DELETE FROM dbo.BOOKING
WHERE booking_id = @booking_id;


---------------------------------------------------------------
-- OCCUPIES (Booking-Room)  --> BookingsForm (rooms-in-booking section)
---------------------------------------------------------------

-- [BookingsForm - Rooms in Booking] SELECT: Show all rooms assigned to ONE booking.
-- Params:
--   @booking_id from selected booking row
-- Use case:
--   - when user clicks a booking, load its rooms in a second grid
SELECT
  o.booking_id,
  o.room_id,
  r.room_type,
  r.room_price
FROM dbo.Occupies o
JOIN dbo.ROOM r ON r.room_id = o.room_id
WHERE o.booking_id = @booking_id
ORDER BY o.room_id;


-- [BookingsForm - Rooms in Booking] INSERT: Add a room to a booking.
-- Params:
--   @booking_id (selected booking)
--   @room_id (selected room)
-- Common UI:
--   - ComboBox listing available room_ids/types
-- NOTE:
--   PK is (booking_id, room_id) so you cannot add the same room twice to same booking.
INSERT INTO dbo.Occupies (booking_id, room_id)
VALUES (@booking_id, @room_id);

-- [BookingsForm - Rooms in Booking] DELETE: Remove a room from a booking.
-- Params:
--   @booking_id, @room_id
-- NOTE:
--   This does NOT delete the room itself; it only removes the relationship.
DELETE FROM dbo.Occupies
WHERE booking_id = @booking_id AND room_id = @room_id;


---------------------------------------------------------------
-- SERVICE (CRUD)  --> ServicesStaffForm (optional tab/section)
---------------------------------------------------------------

-- [ServicesStaffForm - Services] SELECT: Load all services.
-- Useful to populate:
--   - a services DataGridView
--   - or a ComboBox used when adding Uses rows (service usage)
SELECT
  service_id,
  service_type,
  service_price
FROM dbo.SERVICE
ORDER BY service_id;

-- [ServicesStaffForm - Services] INSERT: Add new service type.
-- Params: @service_type, @service_price
INSERT INTO dbo.SERVICE (service_type, service_price)
VALUES (@service_type, @service_price);

-- [ServicesStaffForm - Services] UPDATE: Edit service.
-- Params: @service_id (PK), @service_type, @service_price
UPDATE dbo.SERVICE
SET
  service_type  = @service_type,
  service_price = @service_price
WHERE service_id = @service_id;

-- [ServicesStaffForm - Services] DELETE: Remove a service.
-- WARNING:
--   Will fail if referenced by Uses or Provides.
--   You can either:
--     (a) prevent delete in GUI, or
--     (b) catch error and tell user to remove references first.
DELETE FROM dbo.SERVICE
WHERE service_id = @service_id;


---------------------------------------------------------------
-- USES (Booking-Service usage)  --> ServicesStaffForm (booking services tab)
---------------------------------------------------------------

-- [ServicesStaffForm - Booking Services] SELECT: Show all services used in ONE booking.
-- Params: @booking_id
-- Output includes:
--   - service_type for display
--   - line_total (quantity * price) for billing-like display
SELECT
  u.booking_id,
  u.service_id,
  s.service_type,
  u.use_date,
  u.quantity,
  s.service_price,
  (u.quantity * s.service_price) AS line_total
FROM dbo.Uses u
JOIN dbo.SERVICE s ON s.service_id = u.service_id
WHERE u.booking_id = @booking_id
ORDER BY u.use_date, u.service_id;

-- [ServicesStaffForm - Booking Services] INSERT: Add service usage to booking.
-- Params:
--   @booking_id (selected booking)
--   @service_id (selected service)
--   @use_date (date picker)
--   @quantity (numeric input > 0)
-- NOTE:
--   PK is (booking_id, service_id, use_date)
--   so same service on same day cannot be duplicated; update it instead.
INSERT INTO dbo.Uses (booking_id, service_id, use_date, quantity)
VALUES (@booking_id, @service_id, @use_date, @quantity);

-- [ServicesStaffForm - Booking Services] UPDATE: Update quantity for an existing Uses row.
-- Params: @booking_id, @service_id, @use_date, @quantity
UPDATE dbo.Uses
SET quantity = @quantity
WHERE booking_id = @booking_id AND service_id = @service_id AND use_date = @use_date;

-- [ServicesStaffForm - Booking Services] DELETE: Remove a service usage record.
-- Params: @booking_id, @service_id, @use_date
DELETE FROM dbo.Uses
WHERE booking_id = @booking_id AND service_id = @service_id AND use_date = @use_date;


---------------------------------------------------------------
-- STAFF (CRUD)  --> ServicesStaffForm (optional tab/section)
---------------------------------------------------------------

-- [ServicesStaffForm - Staff] SELECT: Load all staff.
-- Useful for:
--   - staff management grid
--   - staff ComboBox when assigning Provides
SELECT
  staff_id,
  staff_first_name,
  staff_last_name,
  role,
  staff_phone,
  staff_national_id,
  salary
FROM dbo.Staff
ORDER BY staff_id;

-- [ServicesStaffForm - Staff] INSERT: Add staff member.
-- Params: @first_name, @last_name, @role, @phone, @national_id, @salary
INSERT INTO dbo.Staff (staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary)
VALUES (@first_name, @last_name, @role, @phone, @national_id, @salary);

-- [ServicesStaffForm - Staff] UPDATE: Edit staff member.
-- Params: @staff_id (PK), @first_name, @last_name, @role, @phone, @national_id, @salary
UPDATE dbo.Staff
SET
  staff_first_name = @first_name,
  staff_last_name  = @last_name,
  role             = @role,
  staff_phone      = @phone,
  staff_national_id = @national_id,
  salary           = @salary
WHERE staff_id = @staff_id;

-- [ServicesStaffForm - Staff] DELETE: Remove staff member.
-- WARNING:
--   Will fail if referenced in Provides unless you remove assignments first.
DELETE FROM dbo.Staff
WHERE staff_id = @staff_id;


---------------------------------------------------------------
-- PROVIDES (Staff-Service assignment)  --> ServicesStaffForm (assignments tab)
---------------------------------------------------------------

-- [ServicesStaffForm - Assignments] SELECT: Show staff-service assignments (Provides).
-- JOINs are used so GUI can display human-readable names:
--   - staff_full_name instead of staff_id only
--   - service_type instead of service_id only
SELECT
  p.staff_id,
  (st.staff_first_name + ' ' + st.staff_last_name) AS staff_full_name,
  p.service_id,
  sv.service_type,
  p.assigned_date,
  p.shift,
  p.status
FROM dbo.Provides p
JOIN dbo.Staff st ON st.staff_id = p.staff_id
JOIN dbo.SERVICE sv ON sv.service_id = p.service_id
ORDER BY p.assigned_date DESC, p.staff_id, p.service_id;

-- [ServicesStaffForm - Assignments] SELECT: Filter assignments for one service (optional).
-- Params: @service_id
SELECT
  p.staff_id,
  p.service_id,
  p.assigned_date,
  p.shift,
  p.status
FROM dbo.Provides p
WHERE p.service_id = @service_id
ORDER BY p.assigned_date DESC;

-- [ServicesStaffForm - Assignments] INSERT: Assign a staff member to provide a service.
-- Params:
--   @staff_id (selected staff)
--   @service_id (selected service)
--   @assigned_date (date picker)
--   @shift (e.g., Morning/Evening)
--   @status (e.g., Active/Inactive)
-- NOTE:
--   PK is (staff_id, service_id, assigned_date) so duplicates on same date are prevented.
INSERT INTO dbo.Provides (staff_id, service_id, assigned_date, shift, status)
VALUES (@staff_id, @service_id, @assigned_date, @shift, @status);

-- [ServicesStaffForm - Assignments] UPDATE: Edit assignment details.
-- Use case:
--   - change shift
--   - change status (Active/Inactive)
UPDATE dbo.Provides
SET
  shift = @shift,
  status = @status
WHERE staff_id = @staff_id AND service_id = @service_id AND assigned_date = @assigned_date;

-- [ServicesStaffForm - Assignments] DELETE: Remove assignment row.
DELETE FROM dbo.Provides
WHERE staff_id = @staff_id AND service_id = @service_id AND assigned_date = @assigned_date;


---------------------------------------------------------------
-- OPTIONAL: Booking total price calculation (derived) --> BookingsForm or ServicesStaffForm
---------------------------------------------------------------

-- [BookingsForm/ServicesStaffForm] SELECT: Calculate total price for ONE booking.
-- Params: @booking_id
-- This query DOES NOT store total in DB. It computes it dynamically:
--   nights = DATEDIFF(DAY, check_in, check_out)
--   rooms_total = SUM(room_price for rooms in booking) * nights
--   services_total = SUM(quantity * service_price) for services used in booking
--   booking_total = rooms_total + services_total
-- Use case in GUI:
--   - user selects a booking
--   - GUI calls this query and shows totals in labels
SELECT
  b.booking_id,
  DATEDIFF(DAY, b.check_in_date, b.check_out_date) AS nights,

  (
    SELECT ISNULL(SUM(r.room_price), 0)
    FROM dbo.Occupies o
    JOIN dbo.ROOM r ON r.room_id = o.room_id
    WHERE o.booking_id = b.booking_id
  ) * DATEDIFF(DAY, b.check_in_date, b.check_out_date) AS rooms_total,

  (
    SELECT ISNULL(SUM(u.quantity * s.service_price), 0)
    FROM dbo.Uses u
    JOIN dbo.SERVICE s ON s.service_id = u.service_id
    WHERE u.booking_id = b.booking_id
  ) AS services_total,

  (
    (
      SELECT ISNULL(SUM(r.room_price), 0)
      FROM dbo.Occupies o
      JOIN dbo.ROOM r ON r.room_id = o.room_id
      WHERE o.booking_id = b.booking_id
    ) * DATEDIFF(DAY, b.check_in_date, b.check_out_date)
  ) +
  (
    SELECT ISNULL(SUM(u.quantity * s.service_price), 0)
    FROM dbo.Uses u
    JOIN dbo.SERVICE s ON s.service_id = u.service_id
    WHERE u.booking_id = b.booking_id
  ) AS booking_total
FROM dbo.BOOKING b
WHERE b.booking_id = @booking_id;