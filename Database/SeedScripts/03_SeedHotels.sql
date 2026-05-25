-- Script 3: Seed Hotels
USE HotelBookingDb;
GO

IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Name = 'The Grand Mumbai')
    INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Amenities, ImageUrls, IsActive, CreatedAt)
    VALUES (
        'The Grand Mumbai',
        'A luxury 5-star hotel in the heart of Mumbai offering world-class amenities and breathtaking sea views.',
        '12 Marine Drive, Nariman Point', 'Mumbai', 'India', 5,
        '["Free WiFi","Swimming Pool","Spa","Gym","Restaurant","Room Service","Valet Parking","Conference Rooms"]',
        '["https://images.unsplash.com/photo-1566073771259-6a8506099945"]',
        1, GETUTCDATE()
    );

IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Name = 'Taj Palace Delhi')
    INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Amenities, ImageUrls, IsActive, CreatedAt)
    VALUES (
        'Taj Palace Delhi',
        'An iconic heritage hotel in New Delhi blending Mughal architecture with modern luxury.',
        '2 Sardar Patel Marg, Diplomatic Enclave', 'New Delhi', 'India', 5,
        '["Free WiFi","Multiple Restaurants","Outdoor Pool","Spa","Fitness Center","Business Center","Concierge","Airport Shuttle"]',
        '["https://images.unsplash.com/photo-1520250497591-112f2f40a3f4"]',
        1, GETUTCDATE()
    );

IF NOT EXISTS (SELECT 1 FROM Hotels WHERE Name = 'Ocean Breeze Goa')
    INSERT INTO Hotels (Name, Description, Address, City, Country, StarRating, Amenities, ImageUrls, IsActive, CreatedAt)
    VALUES (
        'Ocean Breeze Goa',
        'A beachfront 4-star resort in North Goa with stunning Arabian Sea views and a vibrant beach club.',
        '45 Calangute Beach Road, Calangute', 'Goa', 'India', 4,
        '["Beachfront","Free WiFi","Infinity Pool","Beach Bar","Water Sports","Yoga Classes","Restaurant","Parking"]',
        '["https://images.unsplash.com/photo-1540541338287-41700207dee6"]',
        1, GETUTCDATE()
    );
GO
