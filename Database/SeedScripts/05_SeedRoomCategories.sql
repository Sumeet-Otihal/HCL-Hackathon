-- Script 5: Seed Room Categories
USE HotelBookingDb;
GO

DECLARE @MumbaiHotelId INT = (SELECT Id FROM Hotels WHERE Name = 'The Grand Mumbai');
DECLARE @DelhiHotelId  INT = (SELECT Id FROM Hotels WHERE Name = 'Taj Palace Delhi');
DECLARE @GoaHotelId    INT = (SELECT Id FROM Hotels WHERE Name = 'Ocean Breeze Goa');

-- Mumbai categories
IF NOT EXISTS (SELECT 1 FROM RoomCategories WHERE HotelId = @MumbaiHotelId AND [Name] = 0)
    INSERT INTO RoomCategories (HotelId, [Name], Description, PricePerNight, MaxOccupancy, Amenities, ImageUrls, IsActive)
    VALUES
    (@MumbaiHotelId, 0, 'Comfortable standard room with city view.',
     4500.00, 2, '["King Bed","City View","Free WiFi","Air Conditioning","Mini Bar"]',
     '["https://images.unsplash.com/photo-1631049307264-da0ec9d70304"]', 1),
    (@MumbaiHotelId, 1, 'Spacious deluxe room with sea-facing balcony.',
     8500.00, 2, '["King Bed","Sea View Balcony","Free WiFi","Bathtub","Nespresso Machine"]',
     '["https://images.unsplash.com/photo-1611892440504-42a792e24d32"]', 1),
    (@MumbaiHotelId, 2, 'Luxurious suite with separate living area and panoramic ocean views.',
     18000.00, 4, '["King Bed","Ocean View","Living Room","Jacuzzi","Butler Service"]',
     '["https://images.unsplash.com/photo-1582719508461-905c673771fd"]', 1);

-- Delhi categories
IF NOT EXISTS (SELECT 1 FROM RoomCategories WHERE HotelId = @DelhiHotelId AND [Name] = 0)
    INSERT INTO RoomCategories (HotelId, [Name], Description, PricePerNight, MaxOccupancy, Amenities, ImageUrls, IsActive)
    VALUES
    (@DelhiHotelId, 0, 'Elegant standard room with heritage decor and garden view.',
     5500.00, 2, '["King Bed","Garden View","Free WiFi","Air Conditioning","Mini Bar"]',
     '["https://images.unsplash.com/photo-1631049307264-da0ec9d70304"]', 1),
    (@DelhiHotelId, 1, 'Spacious deluxe room with contemporary interiors and pool view.',
     10000.00, 2, '["King Bed","Pool View","Free WiFi","Rain Shower","Espresso Machine"]',
     '["https://images.unsplash.com/photo-1611892440504-42a792e24d32"]', 1),
    (@DelhiHotelId, 3, 'Presidential suite with private terrace and dedicated butler.',
     55000.00, 4, '["Presidential Bed","Private Terrace","Butler Service","Grand Piano","Jacuzzi"]',
     '["https://images.unsplash.com/photo-1582719508461-905c673771fd"]', 1);

-- Goa categories
IF NOT EXISTS (SELECT 1 FROM RoomCategories WHERE HotelId = @GoaHotelId AND [Name] = 0)
    INSERT INTO RoomCategories (HotelId, [Name], Description, PricePerNight, MaxOccupancy, Amenities, ImageUrls, IsActive)
    VALUES
    (@GoaHotelId, 0, 'Cozy standard room steps from the beach with tropical decor.',
     3200.00, 2, '["Double Bed","Garden View","Free WiFi","Air Conditioning","Private Balcony","Beach Towels"]',
     '["https://images.unsplash.com/photo-1631049307264-da0ec9d70304"]', 1),
    (@GoaHotelId, 1, 'Deluxe room with private plunge pool and direct beach access.',
     7800.00, 2, '["King Bed","Beach View","Private Plunge Pool","Free WiFi","Hammock"]',
     '["https://images.unsplash.com/photo-1611892440504-42a792e24d32"]', 1),
    (@GoaHotelId, 2, 'Overwater suite with unobstructed sea views and sunset-facing deck.',
     15000.00, 3, '["King Bed","Sea View Deck","Jacuzzi","Butler Service","Kitchenette"]',
     '["https://images.unsplash.com/photo-1582719508461-905c673771fd"]', 1);
GO
