-- Script 6: Seed Rooms
USE HotelBookingDb;
GO

DECLARE @MumStd INT = (SELECT Id FROM RoomCategories WHERE HotelId = (SELECT Id FROM Hotels WHERE Name='The Grand Mumbai') AND [Name]=0);
DECLARE @MumDlx INT = (SELECT Id FROM RoomCategories WHERE HotelId = (SELECT Id FROM Hotels WHERE Name='The Grand Mumbai') AND [Name]=1);
DECLARE @MumSte INT = (SELECT Id FROM RoomCategories WHERE HotelId = (SELECT Id FROM Hotels WHERE Name='The Grand Mumbai') AND [Name]=2);

IF NOT EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber='101' AND RoomCategoryId=@MumStd)
    INSERT INTO Rooms (RoomCategoryId, RoomNumber, FloorNumber, IsActive)
    VALUES
    (@MumStd,'101',1,1), (@MumStd,'102',1,1), (@MumStd,'103',1,1),
    (@MumDlx,'201',2,1), (@MumDlx,'202',2,1), (@MumDlx,'203',2,1),
    (@MumSte,'501',5,1), (@MumSte,'502',5,1);

DECLARE @DelStd INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Taj Palace Delhi') AND [Name]=0);
DECLARE @DelDlx INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Taj Palace Delhi') AND [Name]=1);
DECLARE @DelPre INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Taj Palace Delhi') AND [Name]=3);

IF NOT EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber='101' AND RoomCategoryId=@DelStd)
    INSERT INTO Rooms (RoomCategoryId, RoomNumber, FloorNumber, IsActive)
    VALUES
    (@DelStd,'101',1,1), (@DelStd,'102',1,1), (@DelStd,'103',1,1),
    (@DelDlx,'301',3,1), (@DelDlx,'302',3,1),
    (@DelPre,'701',7,1);

DECLARE @GoaStd INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Ocean Breeze Goa') AND [Name]=0);
DECLARE @GoaDlx INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Ocean Breeze Goa') AND [Name]=1);
DECLARE @GoaSte INT = (SELECT Id FROM RoomCategories WHERE HotelId=(SELECT Id FROM Hotels WHERE Name='Ocean Breeze Goa') AND [Name]=2);

IF NOT EXISTS (SELECT 1 FROM Rooms WHERE RoomNumber='G01' AND RoomCategoryId=@GoaStd)
    INSERT INTO Rooms (RoomCategoryId, RoomNumber, FloorNumber, IsActive)
    VALUES
    (@GoaStd,'G01',0,1), (@GoaStd,'G02',0,1), (@GoaStd,'G03',0,1), (@GoaStd,'G04',0,1),
    (@GoaDlx,'G11',1,1), (@GoaDlx,'G12',1,1),
    (@GoaSte,'G21',2,1), (@GoaSte,'G22',2,1);
GO
