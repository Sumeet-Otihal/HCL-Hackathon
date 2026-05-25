-- Script 4: Seed HotelAdmin Users (one per hotel)
-- Password for all: Admin@123456
USE HotelBookingDb;
GO

-- Mumbai HotelAdmin
DECLARE @MumbaiId INT = (SELECT Id FROM Hotels WHERE Name = 'The Grand Mumbai');
DECLARE @MumbaiAdminId NVARCHAR(450) = NEWID();

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin.mumbai@hotelbooking.com')
BEGIN
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail,
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, [Role], HotelId, LoyaltyPoints, IsActive, CreatedAt
    )
    VALUES (
        @MumbaiAdminId,
        'admin.mumbai@hotelbooking.com', 'ADMIN.MUMBAI@HOTELBOOKING.COM',
        'admin.mumbai@hotelbooking.com', 'ADMIN.MUMBAI@HOTELBOOKING.COM',
        1,
        'AQAAAAIAAYagAAAAEL8YuNpFqT3bGVMNGi1xMqC5sCq5NF9rT2TlhwJcDE5j/4t/gM+j0H6m9WtcJ1QhEQ==',
        NEWID(), NEWID(),
        0, 0, 1, 0,
        'Mumbai', 'Admin', 1, @MumbaiId, 0, 1, GETUTCDATE()
    );

    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @MumbaiAdminId, Id FROM AspNetRoles WHERE NormalizedName = 'HOTELADMIN';
END
GO

-- Delhi HotelAdmin
DECLARE @DelhiId INT = (SELECT Id FROM Hotels WHERE Name = 'Taj Palace Delhi');
DECLARE @DelhiAdminId NVARCHAR(450) = NEWID();

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin.delhi@hotelbooking.com')
BEGIN
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail,
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, [Role], HotelId, LoyaltyPoints, IsActive, CreatedAt
    )
    VALUES (
        @DelhiAdminId,
        'admin.delhi@hotelbooking.com', 'ADMIN.DELHI@HOTELBOOKING.COM',
        'admin.delhi@hotelbooking.com', 'ADMIN.DELHI@HOTELBOOKING.COM',
        1,
        'AQAAAAIAAYagAAAAEL8YuNpFqT3bGVMNGi1xMqC5sCq5NF9rT2TlhwJcDE5j/4t/gM+j0H6m9WtcJ1QhEQ==',
        NEWID(), NEWID(),
        0, 0, 1, 0,
        'Delhi', 'Admin', 1, @DelhiId, 0, 1, GETUTCDATE()
    );

    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @DelhiAdminId, Id FROM AspNetRoles WHERE NormalizedName = 'HOTELADMIN';
END
GO

-- Goa HotelAdmin
DECLARE @GoaId INT = (SELECT Id FROM Hotels WHERE Name = 'Ocean Breeze Goa');
DECLARE @GoaAdminId NVARCHAR(450) = NEWID();

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'admin.goa@hotelbooking.com')
BEGIN
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail,
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, [Role], HotelId, LoyaltyPoints, IsActive, CreatedAt
    )
    VALUES (
        @GoaAdminId,
        'admin.goa@hotelbooking.com', 'ADMIN.GOA@HOTELBOOKING.COM',
        'admin.goa@hotelbooking.com', 'ADMIN.GOA@HOTELBOOKING.COM',
        1,
        'AQAAAAIAAYagAAAAEL8YuNpFqT3bGVMNGi1xMqC5sCq5NF9rT2TlhwJcDE5j/4t/gM+j0H6m9WtcJ1QhEQ==',
        NEWID(), NEWID(),
        0, 0, 1, 0,
        'Goa', 'Admin', 1, @GoaId, 0, 1, GETUTCDATE()
    );

    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @GoaAdminId, Id FROM AspNetRoles WHERE NormalizedName = 'HOTELADMIN';
END
GO
