-- HotelDB.sql
-- Phase 2 Deliverable: CREATE TABLE + INSERT seed data (>= 3 rows per table)
-- Target: SQL Server

/*
  TODO (team):
  - Replace placeholder schema with your final schema.
  - Ensure Booking is a STRONG entity with booking_id as PK.
  - Ensure all FKs and constraints match your ERD.
  - Insert at least 3 rows per table.
*/

-- Create / select database (optional)
-- CREATE DATABASE HotelDB;
-- GO
-- USE HotelDB;
-- GO

SET NOCOUNT ON;

/* Drop order (safe for re-running during development) */
IF OBJECT_ID('dbo.Provides', 'U') IS NOT NULL DROP TABLE dbo.Provides;
IF OBJECT_ID('dbo.Uses', 'U') IS NOT NULL DROP TABLE dbo.Uses;
IF OBJECT_ID('dbo.Occupies', 'U') IS NOT NULL DROP TABLE dbo.Occupies;
IF OBJECT_ID('dbo.Staff', 'U') IS NOT NULL DROP TABLE dbo.Staff;
IF OBJECT_ID('dbo.Service', 'U') IS NOT NULL DROP TABLE dbo.Service;
IF OBJECT_ID('dbo.Room', 'U') IS NOT NULL DROP TABLE dbo.Room;
IF OBJECT_ID('dbo.Booking', 'U') IS NOT NULL DROP TABLE dbo.Booking;
IF OBJECT_ID('dbo.Guest', 'U') IS NOT NULL DROP TABLE dbo.Guest;
GO

/* Core tables */
CREATE TABLE dbo.Guest (
  guest_id INT IDENTITY(1,1) PRIMARY KEY,
  guest_first_name NVARCHAR(50) NOT NULL,
  guest_last_name  NVARCHAR(50) NOT NULL,
  guest_phone      NVARCHAR(30) NULL,
  guest_national_id NVARCHAR(30) NULL
);

CREATE TABLE dbo.Booking (
  booking_id INT IDENTITY(1,1) PRIMARY KEY,
  guest_id INT NOT NULL,
  check_in_date DATE NOT NULL,
  check_out_date DATE NOT NULL,
  booking_status NVARCHAR(30) NOT NULL DEFAULT('Pending'),
  payment_status NVARCHAR(30) NOT NULL DEFAULT('Unpaid'),
  CONSTRAINT FK_Booking_Guest FOREIGN KEY (guest_id) REFERENCES dbo.Guest(guest_id),
  CONSTRAINT CK_Booking_Dates CHECK (check_out_date > check_in_date)
);

CREATE TABLE dbo.Room (
  room_id INT IDENTITY(1,1) PRIMARY KEY,
  room_type NVARCHAR(20) NOT NULL,
  room_price_per_night DECIMAL(10,2) NOT NULL,
  status NVARCHAR(20) NOT NULL
);

CREATE TABLE dbo.Service (
  service_id INT IDENTITY(1,1) PRIMARY KEY,
  service_type NVARCHAR(50) NOT NULL,
  service_price DECIMAL(10,2) NOT NULL
);

CREATE TABLE dbo.Staff (
  staff_id INT IDENTITY(1,1) PRIMARY KEY,
  staff_first_name NVARCHAR(50) NOT NULL,
  staff_last_name  NVARCHAR(50) NOT NULL,
  role NVARCHAR(50) NOT NULL,
  staff_phone NVARCHAR(30) NULL,
  staff_national_id NVARCHAR(30) NULL,
  salary DECIMAL(10,2) NOT NULL
);

/* Associative tables */
CREATE TABLE dbo.Occupies (
  booking_id INT NOT NULL,
  room_id INT NOT NULL,
  CONSTRAINT PK_Occupies PRIMARY KEY (booking_id, room_id),
  CONSTRAINT FK_Occupies_Booking FOREIGN KEY (booking_id) REFERENCES dbo.Booking(booking_id),
  CONSTRAINT FK_Occupies_Room FOREIGN KEY (room_id) REFERENCES dbo.Room(room_id)
);

CREATE TABLE dbo.Uses (
  booking_id INT NOT NULL,
  service_id INT NOT NULL,
  use_date DATE NOT NULL,
  quantity INT NOT NULL,
  CONSTRAINT PK_Uses PRIMARY KEY (booking_id, service_id, use_date),
  CONSTRAINT FK_Uses_Booking FOREIGN KEY (booking_id) REFERENCES dbo.Booking(booking_id),
  CONSTRAINT FK_Uses_Service FOREIGN KEY (service_id) REFERENCES dbo.Service(service_id),
  CONSTRAINT CK_Uses_Quantity CHECK (quantity > 0)
);

CREATE TABLE dbo.Provides (
  staff_id INT NOT NULL,
  service_id INT NOT NULL,
  assigned_date DATE NOT NULL,
  shift NVARCHAR(30) NOT NULL,
  status NVARCHAR(30) NOT NULL,
  CONSTRAINT PK_Provides PRIMARY KEY (staff_id, service_id, assigned_date),
  CONSTRAINT FK_Provides_Staff FOREIGN KEY (staff_id) REFERENCES dbo.Staff(staff_id),
  CONSTRAINT FK_Provides_Service FOREIGN KEY (service_id) REFERENCES dbo.Service(service_id)
);
GO

/* Seed data (>= 3 rows per table) */
INSERT INTO dbo.Guest (guest_first_name, guest_last_name, guest_phone, guest_national_id)
VALUES
('Ahmed', 'Hassan', '01000000001', 'NAT001'),
('Sara',  'Ali',    '01000000002', 'NAT002'),
('Omar',  'Yousef', '01000000003', 'NAT003');

INSERT INTO dbo.Room (room_type, room_price_per_night, status)
VALUES
('SINGLE',  800.00, 'available'),
('DOUBLE', 1200.00, 'available'),
('SUITE',  2500.00, 'not available');

INSERT INTO dbo.Service (service_type, service_price)
VALUES
('Breakfast', 150.00),
('Laundry',   100.00),
('Cleaning',  200.00);

INSERT INTO dbo.Staff (staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary)
VALUES
('Mona',  'Said',   'Receptionist', '01100000001', 'STAFF001', 7000.00),
('Khaled','Nabil',  'Cleaner',      '01100000002', 'STAFF002', 5000.00),
('Nour',  'Adel',   'Manager',      '01100000003', 'STAFF003', 12000.00);

INSERT INTO dbo.Booking (guest_id, check_in_date, check_out_date, booking_status, payment_status)
VALUES
(1, '2026-05-10', '2026-05-12', 'Confirmed', 'Unpaid'),
(2, '2026-05-15', '2026-05-18', 'Pending',   'Unpaid'),
(3, '2026-06-01', '2026-06-03', 'Confirmed', 'Paid');

INSERT INTO dbo.Occupies (booking_id, room_id)
VALUES
(1, 1),
(2, 2),
(3, 3);

INSERT INTO dbo.Uses (booking_id, service_id, use_date, quantity)
VALUES
(1, 1, '2026-05-10', 2),
(1, 2, '2026-05-11', 1),
(3, 3, '2026-06-02', 1);

INSERT INTO dbo.Provides (staff_id, service_id, assigned_date, shift, status)
VALUES
(1, 1, '2026-05-01', 'Morning', 'Active'),
(2, 3, '2026-05-01', 'Evening', 'Active'),
(3, 2, '2026-05-01', 'Morning', 'Active');
GO
