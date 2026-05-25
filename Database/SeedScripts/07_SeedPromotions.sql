-- Script 7: Seed Promotions
USE HotelBookingDb;
GO

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code='WELCOME20')
    INSERT INTO Promotions (Code, DiscountType, DiscountValue, MinBookingAmount, ExpiryDate, IsActive, UsageLimit, UsageCount)
    VALUES ('WELCOME20', 0, 20.00, 3000.00, DATEADD(YEAR,1,GETUTCDATE()), 1, 500, 0);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code='FLAT500')
    INSERT INTO Promotions (Code, DiscountType, DiscountValue, MinBookingAmount, ExpiryDate, IsActive, UsageLimit, UsageCount)
    VALUES ('FLAT500', 1, 500.00, 5000.00, DATEADD(YEAR,1,GETUTCDATE()), 1, 200, 0);

IF NOT EXISTS (SELECT 1 FROM Promotions WHERE Code='SUMMER15')
    INSERT INTO Promotions (Code, DiscountType, DiscountValue, MinBookingAmount, ExpiryDate, IsActive, UsageLimit, UsageCount)
    VALUES ('SUMMER15', 0, 15.00, 2000.00, DATEADD(MONTH,6,GETUTCDATE()), 1, 300, 0);
GO
