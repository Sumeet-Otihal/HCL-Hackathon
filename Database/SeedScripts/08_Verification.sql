-- Script 8: Verification Query
USE HotelBookingDb;
GO

SELECT 'Roles'          AS [Table], COUNT(*) AS [Count] FROM AspNetRoles
UNION ALL SELECT 'Users',           COUNT(*) FROM AspNetUsers
UNION ALL SELECT 'Hotels',          COUNT(*) FROM Hotels
UNION ALL SELECT 'RoomCategories',  COUNT(*) FROM RoomCategories
UNION ALL SELECT 'Rooms',           COUNT(*) FROM Rooms
UNION ALL SELECT 'Promotions',      COUNT(*) FROM Promotions;

SELECT h.Name AS Hotel, h.City, h.StarRating,
       rc.[Name] AS CategoryEnum, rc.PricePerNight,
       COUNT(r.Id) AS RoomCount
FROM Hotels h
JOIN RoomCategories rc ON rc.HotelId = h.Id
JOIN Rooms r ON r.RoomCategoryId = rc.Id
GROUP BY h.Name, h.City, h.StarRating, rc.[Name], rc.PricePerNight
ORDER BY h.Name, rc.PricePerNight;
