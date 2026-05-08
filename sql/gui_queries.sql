SELECT
  guest_id,               -- Primary Key (used for Update/Delete)
  guest_national_id,      -- Unique national ID (user enters this)
  guest_first_name,
  guest_last_name,
  guest_phone
FROM dbo.GUEST
ORDER BY guest_id;

INSERT INTO dbo.GUEST (guest_national_id, guest_first_name, guest_last_name, guest_phone)
VALUES (@national_id, @first_name, @last_name, @phone);

UPDATE dbo.GUEST
SET
  guest_national_id = @national_id,
  guest_first_name  = @first_name,
  guest_last_name   = @last_name,
  guest_phone       = @phone
WHERE guest_id = @guest_id;

DELETE FROM dbo.GUEST
WHERE guest_id = @guest_id;


SELECT
  room_id,     -- PK used for Update/Delete and for Occupies link
  room_type,   -- e.g., SINGLE, DOUBLE, etc.
   room_price AS room_price_per_night -- price per unit (you can interpret as per night in GUI)
FROM dbo.ROOM
ORDER BY room_id;

INSERT INTO dbo.ROOM (room_type, room_price)
VALUES (@room_type, @room_price);

UPDATE dbo.ROOM
SET
  room_type  = @room_type,
  room_price = @room_price
WHERE room_id = @room_id;

DELETE FROM dbo.ROOM
WHERE room_id = @room_id;


SELECT room_id, features
FROM dbo.ROOMFEATURES
WHERE room_id = @room_id
ORDER BY features;

INSERT INTO dbo.ROOMFEATURES (room_id, features)
VALUES (@room_id, @feature);

DELETE FROM dbo.ROOMFEATURES
WHERE room_id = @room_id AND features = @feature;


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

SELECT
  booking_id, guest_id, check_in_date, check_out_date, booking_status, payment_status
FROM dbo.BOOKING
WHERE guest_id = @guest_id
ORDER BY booking_id;

INSERT INTO dbo.BOOKING (guest_id, check_in_date, check_out_date, booking_status, payment_status)
VALUES (@guest_id, @check_in, @check_out, @booking_status, @payment_status);

UPDATE dbo.BOOKING
SET
  guest_id        = @guest_id,
  check_in_date   = @check_in,
  check_out_date  = @check_out,
  booking_status  = @booking_status,
  payment_status  = @payment_status
WHERE booking_id = @booking_id;

DELETE FROM dbo.BOOKING
WHERE booking_id = @booking_id;


SELECT
  o.booking_id,
  o.room_id,
  r.room_type,
  r.room_price
FROM dbo.Occupies o
JOIN dbo.ROOM r ON r.room_id = o.room_id
WHERE o.booking_id = @booking_id
ORDER BY o.room_id;


INSERT INTO dbo.Occupies (booking_id, room_id)
VALUES (@booking_id, @room_id);

DELETE FROM dbo.Occupies
WHERE booking_id = @booking_id AND room_id = @room_id;


SELECT
  service_id,
  service_type,
  service_price
FROM dbo.SERVICE
ORDER BY service_id;

INSERT INTO dbo.SERVICE (service_type, service_price)
VALUES (@service_type, @service_price);

UPDATE dbo.SERVICE
SET
  service_type  = @service_type,
  service_price = @service_price
WHERE service_id = @service_id;

DELETE FROM dbo.SERVICE
WHERE service_id = @service_id;


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

INSERT INTO dbo.Uses (booking_id, service_id, use_date, quantity)
VALUES (@booking_id, @service_id, @use_date, @quantity);

UPDATE dbo.Uses
SET quantity = @quantity
WHERE booking_id = @booking_id AND service_id = @service_id AND use_date = @use_date;

WHERE booking_id = @booking_id AND service_id = @service_id AND use_date = @use_date;

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

INSERT INTO dbo.Staff (staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary)
VALUES (@first_name, @last_name, @role, @phone, @national_id, @salary);

UPDATE dbo.Staff
SET
  staff_first_name = @first_name,
  staff_last_name  = @last_name,
  role             = @role,
  staff_phone      = @phone,
  staff_national_id = @national_id,
  salary           = @salary
WHERE staff_id = @staff_id;

DELETE FROM dbo.Staff
WHERE staff_id = @staff_id;

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

SELECT
  p.staff_id,
  p.service_id,
  p.assigned_date,
  p.shift,
  p.status
FROM dbo.Provides p
WHERE p.service_id = @service_id
ORDER BY p.assigned_date DESC;

INSERT INTO dbo.Provides (staff_id, service_id, assigned_date, shift, status)
VALUES (@staff_id, @service_id, @assigned_date, @shift, @status);

UPDATE dbo.Provides
SET
  shift = @shift,
  status = @status
WHERE staff_id = @staff_id AND service_id = @service_id AND assigned_date = @assigned_date;

DELETE FROM dbo.Provides
WHERE staff_id = @staff_id AND service_id = @service_id AND assigned_date = @assigned_date;

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