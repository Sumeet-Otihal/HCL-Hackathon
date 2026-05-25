-- Script 1: Seed Identity Roles
USE HotelBookingDb;
GO

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE [Name] = 'SuperAdmin')
    INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'SuperAdmin', 'SUPERADMIN', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE [Name] = 'HotelAdmin')
    INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'HotelAdmin', 'HOTELADMIN', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE [Name] = 'User')
    INSERT INTO AspNetRoles (Id, [Name], NormalizedName, ConcurrencyStamp)
    VALUES (NEWID(), 'User', 'USER', NEWID());
GO
