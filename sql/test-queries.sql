-- Optional helper queries for development/testing

-- Guests
SELECT * FROM dbo.Guest;

-- Rooms
SELECT * FROM dbo.Room;

-- Bookings with guest name
SELECT b.booking_id, g.guest_first_name, g.guest_last_name, b.check_in_date, b.check_out_date, b.booking_status, b.payment_status
FROM dbo.Booking b
JOIN dbo.Guest g ON g.guest_id = b.guest_id;

-- Rooms in each booking
SELECT o.booking_id, o.room_id, r.room_type, r.status
FROM dbo.Occupies o
JOIN dbo.Room r ON r.room_id = o.room_id;

-- Services used per booking
SELECT u.booking_id, s.service_type, u.use_date, u.quantity, s.service_price
FROM dbo.Uses u
JOIN dbo.Service s ON s.service_id = u.service_id;
