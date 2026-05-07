-- Hotel Database System - Phase 2
-- SQL Server Script: Schema + Seed Data

USE HotelDB;
GO

-- Uncomment the below commands in case you want to rerun the script
-- from scratch , it will delete all tables

IF OBJECT_ID('dbo.PROVIDES', 'U') IS NOT NULL
    DROP TABLE dbo.PROVIDES;

IF OBJECT_ID('dbo.USES', 'U') IS NOT NULL
    DROP TABLE dbo.USES;

IF OBJECT_ID('dbo.OCCUPIES', 'U') IS NOT NULL
    DROP TABLE dbo.OCCUPIES;

IF OBJECT_ID('dbo.STAFF', 'U') IS NOT NULL
    DROP TABLE dbo.STAFF;

IF OBJECT_ID('dbo.SERVICE', 'U') IS NOT NULL
    DROP TABLE dbo.SERVICE;

IF OBJECT_ID('dbo.ROOMFEATURES', 'U') IS NOT NULL
    DROP TABLE dbo.ROOMFEATURES;

IF OBJECT_ID('dbo.ROOM', 'U') IS NOT NULL
    DROP TABLE dbo.ROOM;

IF OBJECT_ID('dbo.BOOKING', 'U') IS NOT NULL
    DROP TABLE dbo.BOOKING;

IF OBJECT_ID('dbo.GUEST', 'U') IS NOT NULL
    DROP TABLE dbo.GUEST;

GO



-- Data definition (Tables Creation)

CREATE TABLE dbo.GUEST(
    guest_id INT IDENTITY(1,1) PRIMARY KEY,
    guest_national_id NVARCHAR(30) UNIQUE NOT NULL,
    guest_first_name  NVARCHAR(50) NOT NULL,
    guest_last_name   NVARCHAR(50) NOT NULL,
    guest_phone       NVARCHAR(30) NULL
);

CREATE TABLE dbo.BOOKING(
    booking_id INT IDENTITY(1,1) PRIMARY KEY,
    guest_id INT NOT NULL,

    check_in_date  DATE NOT NULL,
    check_out_date DATE NOT NULL,

    booking_status NVARCHAR(30) NOT NULL DEFAULT('Pending'),
    payment_status NVARCHAR(30) NOT NULL DEFAULT('Unpaid'),

    
    CONSTRAINT FK_BOOKING_GUEST
        FOREIGN KEY(guest_id) REFERENCES dbo.GUEST(guest_id),

    CONSTRAINT CK_BOOKING_DATES
        CHECK (check_out_date > check_in_date)
);

CREATE TABLE dbo.ROOM(
    room_id INT IDENTITY(1,1) PRIMARY KEY,
    room_type NVARCHAR(30) NOT NULL,
    room_price DECIMAL(10,2) NOT NULL,

    CONSTRAINT CK_Room_Price
        CHECK (room_price > 0)
);

CREATE TABLE dbo.ROOMFEATURES (
    room_id INT NOT NULL,
    features NVARCHAR(50) NOT NULL,

    PRIMARY KEY (room_id, features),

    FOREIGN KEY (room_id) REFERENCES dbo.ROOM(room_id)
);

CREATE TABLE dbo.OCCUPIES (
    booking_id INT NOT NULL,
    room_id INT NOT NULL,

    CONSTRAINT PK_Occupies
        PRIMARY KEY (booking_id, room_id),

    CONSTRAINT FK_Occupies_Booking
        FOREIGN KEY (booking_id) REFERENCES dbo.BOOKING(booking_id) ON DELETE CASCADE,

    CONSTRAINT FK_Occupies_Room
        FOREIGN KEY (room_id) REFERENCES dbo.ROOM(room_id)

);

CREATE TABLE dbo.SERVICE (
    service_id INT IDENTITY(1,1) PRIMARY KEY,
    service_type NVARCHAR(50) NOT NULL,
    service_price DECIMAL(10,2) NOT NULL,

    CONSTRAINT CK_Service_Price
        CHECK (service_price > 0)
);



CREATE TABLE dbo.USES (
    booking_id INT NOT NULL,
    service_id INT NOT NULL,
    use_date DATE NOT NULL,
    quantity INT NOT NULL,

    CONSTRAINT PK_Uses
        PRIMARY KEY (booking_id, service_id, use_date),

    CONSTRAINT FK_Uses_Booking
        FOREIGN KEY (booking_id) REFERENCES dbo.BOOKING(booking_id) ON DELETE CASCADE,

    CONSTRAINT FK_Uses_Service
        FOREIGN KEY (service_id) REFERENCES dbo.SERVICE(service_id),

    CONSTRAINT CK_Uses_Quantity
        CHECK (quantity > 0)

);


CREATE TABLE dbo.STAFF (
    staff_id INT IDENTITY(1,1) PRIMARY KEY,
    staff_first_name NVARCHAR(50) NOT NULL,
    staff_last_name  NVARCHAR(50) NOT NULL,
    role NVARCHAR(50) NOT NULL,
    staff_phone NVARCHAR(30) NULL,
    staff_national_id NVARCHAR(30) NOT NULL UNIQUE,
    salary DECIMAL(10,2) NOT NULL,

    CONSTRAINT CK_Staff_Salary
        CHECK (salary >= 0)
);

CREATE TABLE dbo.PROVIDES (
    staff_id INT NOT NULL,
    service_id INT NOT NULL,
    assigned_date DATE NOT NULL,
    shift NVARCHAR(30) NOT NULL,
    status NVARCHAR(30) NOT NULL,

    CONSTRAINT PK_Provides
        PRIMARY KEY (staff_id, service_id, assigned_date),

    CONSTRAINT FK_Provides_Staff
        FOREIGN KEY (staff_id) REFERENCES dbo.STAFF(staff_id),

    CONSTRAINT FK_Provides_Service
        FOREIGN KEY (service_id) REFERENCES dbo.SERVICE(service_id)
);

GO 


-- Entering 5 children into each table 

SET NOCOUNT ON;

-- 1) Guest (5)
INSERT INTO dbo.Guest (guest_first_name, guest_last_name, guest_phone, guest_national_id)
VALUES
('Ahmed', 'Hassan', '01000000001', 'NAT001'),
('Sara',  'Ali',    '01000000002', 'NAT002'),
('Omar',  'Yousef', '01000000003', 'NAT003'),
('Mariam','Fathy',  '01000000004', 'NAT004'),
('Youssef','Nabil', '01000000005', 'NAT005');

-- 2) Room (5)
INSERT INTO dbo.Room (room_type, room_price)
VALUES
('SINGLE',  800.00),
('DOUBLE', 1200.00),
('TWIN',   1100.00),
('SUITE',  2500.00),
('DELUXE', 1800.00);

-- 3) Service (5)
INSERT INTO dbo.Service (service_type, service_price)
VALUES
('Breakfast', 150.00),
('Lunch',     250.00),
('Cleaning',  200.00),
('Laundry',   100.00),
('Dinner',    300.00);

-- 4) Staff (5)
INSERT INTO dbo.Staff (staff_first_name, staff_last_name, role, staff_phone, staff_national_id, salary)
VALUES
('Mona',   'Said',   'Receptionist', '01100000001', 'STAFF001', 7000.00),
('Khaled', 'Nabil',  'Cleaner',      '01100000002', 'STAFF002', 5000.00),
('Nour',   'Adel',   'Manager',      '01100000003', 'STAFF003', 12000.00),
('Hany',   'Mahmoud','Laundry',      '01100000004', 'STAFF004', 5500.00),
('Salma',  'Ibrahim','Chef',         '01100000005', 'STAFF005', 8000.00);

-- 5) Booking (5) — references guest_id 1..5
INSERT INTO dbo.Booking (guest_id, check_in_date, check_out_date, booking_status, payment_status)
VALUES
(1, '2026-05-10', '2026-05-12', 'Confirmed', 'Unpaid'),
(2, '2026-05-15', '2026-05-18', 'Pending',   'Unpaid'),
(3, '2026-06-01', '2026-06-03', 'Confirmed', 'Paid'),
(4, '2026-06-10', '2026-06-15', 'Confirmed', 'Unpaid'),
(5, '2026-07-01', '2026-07-04', 'Pending',   'Unpaid');

-- 6) Occupies (5) — booking_id 1..5, room_id 1..5
INSERT INTO dbo.Occupies (booking_id, room_id)
VALUES
(1, 1),
(2, 2),
(3, 3),
(4, 4),
(5, 5);

-- 7) Uses (5) — booking_id/service_id must exist
INSERT INTO dbo.Uses (booking_id, service_id, use_date, quantity)
VALUES
(1, 1, '2026-05-10', 2),  -- Booking 1 uses Breakfast
(1, 4, '2026-05-11', 1),  -- Booking 1 uses Laundry
(2, 3, '2026-05-16', 1),  -- Booking 2 uses Cleaning
(4, 2, '2026-06-11', 3),  -- Booking 4 uses Lunch
(5, 5, '2026-07-02', 2);  -- Booking 5 uses Dinner

-- 8) Provides (5) — staff_id/service_id must exist
INSERT INTO dbo.Provides (staff_id, service_id, assigned_date, shift, status)
VALUES
(1, 1, '2026-05-01', 'Morning', 'Active'), -- Receptionist assigned to Breakfast service ops
(2, 3, '2026-05-01', 'Evening', 'Active'), -- Cleaner provides Cleaning
(3, 2, '2026-05-01', 'Morning', 'Active'), -- Manager assigned to Lunch (oversight)
(4, 4, '2026-05-01', 'Morning', 'Active'), -- Laundry staff provides Laundry
(5, 5, '2026-05-01', 'Evening', 'Active'); -- Chef provides Dinner


--Checking SELECTS

SELECT COUNT(*) FROM dbo.GUEST;
SELECT COUNT(*) FROM dbo.BOOKING;
SELECT COUNT(*) FROM dbo.Occupies;
SELECT COUNT(*) FROM dbo.Uses;