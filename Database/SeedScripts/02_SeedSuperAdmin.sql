-- Script 2: Seed SuperAdmin User
-- Password: Admin@123456
USE HotelBookingDb;
GO

DECLARE @AdminId NVARCHAR(450) = NEWID();

IF NOT EXISTS (SELECT 1 FROM AspNetUsers WHERE Email = 'superadmin@hotelbooking.com')
BEGIN
    INSERT INTO AspNetUsers (
        Id, UserName, NormalizedUserName, Email, NormalizedEmail,
        EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp,
        PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
        FirstName, LastName, [Role], HotelId, LoyaltyPoints, IsActive, CreatedAt
    )
    VALUES (
        @AdminId,
        'superadmin@hotelbooking.com', 'SUPERADMIN@HOTELBOOKING.COM',
        'superadmin@hotelbooking.com', 'SUPERADMIN@HOTELBOOKING.COM',
        1,
        'AQAAAAIAAYagAAAAEL8YuNpFqT3bGVMNGi1xMqC5sCq5NF9rT2TlhwJcDE5j/4t/gM+j0H6m9WtcJ1QhEQ==',
        NEWID(), NEWID(),
        0, 0, 1, 0,
        'Super', 'Admin', 0, NULL, 0, 1, GETUTCDATE()
    );

    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT @AdminId, Id FROM AspNetRoles WHERE NormalizedName = 'SUPERADMIN';
END
GO
