-- Table for Landlords
CREATE TABLE Landlord (
    landlord_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(20),
    bank_account VARCHAR(20)  -- Optional
);

-- Table for Houses
CREATE TABLE House (
    house_id INT PRIMARY KEY,
    address VARCHAR(255) NOT NULL,
    description TEXT,
    num_rooms INT NOT NULL,
    max_occupancy INT NOT NULL,
    rental_price DECIMAL(10, 2) NOT NULL,
    landlord_id INT,
    FOREIGN KEY (landlord_id) REFERENCES Landlord(landlord_id)
);

-- Table for Tenants
CREATE TABLE Tenant (
    tenant_id INT PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    phone VARCHAR(20)
);

-- Table for Bookings
CREATE TABLE Booking (
    booking_id INT PRIMARY KEY,
    house_id INT,
    tenant_id INT,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    FOREIGN KEY (house_id) REFERENCES House(house_id),
    FOREIGN KEY (tenant_id) REFERENCES Tenant(tenant_id),
    CONSTRAINT check_booking_dates CHECK (start_date < end_date),
    CONSTRAINT no_overlapping_bookings UNIQUE (house_id, start_date, end_date)  -- No overlapping bookings for the same house
);