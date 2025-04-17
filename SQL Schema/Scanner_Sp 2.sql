

CREATE OR ALTER PROCEDURE sp_FreshInstall
AS
BEGIN
    SET NOCOUNT ON;

    PRINT 'Dropping foreign key constraints...';

    -- Drop foreign keys from UserMaster
    IF OBJECT_ID('FK_UserMaster_CreatedBy','F') IS NOT NULL
        ALTER TABLE UserMaster DROP CONSTRAINT FK_UserMaster_CreatedBy;
    IF OBJECT_ID('FK_UserMaster_ModifiedBy','F') IS NOT NULL
        ALTER TABLE UserMaster DROP CONSTRAINT FK_UserMaster_ModifiedBy;
    IF OBJECT_ID('FK_UserMaster_Role','F') IS NOT NULL
        ALTER TABLE UserMaster DROP CONSTRAINT FK_UserMaster_Role;


    -- Drop foreign keys from RoleMasters
    IF OBJECT_ID('FK_RoleMasters_CreatedBy','F') IS NOT NULL
        ALTER TABLE RoleMasters DROP CONSTRAINT FK_RoleMasters_CreatedBy;
    IF OBJECT_ID('FK_RoleMasters_ModifiedBy','F') IS NOT NULL
        ALTER TABLE RoleMasters DROP CONSTRAINT FK_RoleMasters_ModifiedBy;


    -- Drop foreign keys from PageMaster
    IF OBJECT_ID('FK_PageMaster_CreatedBy','F') IS NOT NULL
        ALTER TABLE PageMaster DROP CONSTRAINT FK_PageMaster_CreatedBy;
    IF OBJECT_ID('FK_PageMaster_ModifiedBy','F') IS NOT NULL
        ALTER TABLE PageMaster DROP CONSTRAINT FK_PageMaster_ModifiedBy;


    -- Drop foreign keys from RolePageAccess
    IF OBJECT_ID('FK_RolePageAccess_Role','F') IS NOT NULL
        ALTER TABLE RolePageAccess DROP CONSTRAINT FK_RolePageAccess_Role;
    IF OBJECT_ID('FK_RolePageAccess_Page','F') IS NOT NULL
        ALTER TABLE RolePageAccess DROP CONSTRAINT FK_RolePageAccess_Page;
    IF OBJECT_ID('FK_RolePageAccess_CreatedBy','F') IS NOT NULL
        ALTER TABLE RolePageAccess DROP CONSTRAINT FK_RolePageAccess_CreatedBy;
    IF OBJECT_ID('FK_RolePageAccess_ModifiedBy','F') IS NOT NULL
        ALTER TABLE RolePageAccess DROP CONSTRAINT FK_RolePageAccess_ModifiedBy;

    -- Drop foreign keys from ScanDetails
    IF OBJECT_ID('FK_ScanDetails_Employee','F') IS NOT NULL
        ALTER TABLE ScanDetails DROP CONSTRAINT FK_ScanDetails_Employee;
    IF OBJECT_ID('FK_ScanDetails_Asset','F') IS NOT NULL
        ALTER TABLE ScanDetails DROP CONSTRAINT FK_ScanDetails_Asset;

    -- Drop foreign keys from EmployeeAssetMapping
    IF OBJECT_ID('FK_EmployeeAssetMapping_Employee','F') IS NOT NULL
        ALTER TABLE EmployeeAssetMapping DROP CONSTRAINT FK_EmployeeAssetMapping_Employee;
    IF OBJECT_ID('FK_EmployeeAssetMapping_Asset','F') IS NOT NULL
        ALTER TABLE EmployeeAssetMapping DROP CONSTRAINT FK_EmployeeAssetMapping_Asset;

    -- Drop foreign keys from AssetMaster
    IF OBJECT_ID('FK_AssetMaster_Category','F') IS NOT NULL
        ALTER TABLE AssetMaster DROP CONSTRAINT FK_AssetMaster_Category;
    IF OBJECT_ID('FK_AssetMaster_Manufacturer','F') IS NOT NULL
        ALTER TABLE AssetMaster DROP CONSTRAINT FK_AssetMaster_Manufacturer;
    IF OBJECT_ID('FK_AssetMaster_Supplier','F') IS NOT NULL
        ALTER TABLE AssetMaster DROP CONSTRAINT FK_AssetMaster_Supplier;
    IF OBJECT_ID('FK_AssetMaster_Location','F') IS NOT NULL
        ALTER TABLE AssetMaster DROP CONSTRAINT FK_AssetMaster_Location;

    PRINT 'Dropping old tables...';

    -- Drop tables in an order that minimizes FK conflicts
    IF OBJECT_ID('dbo.ScanDetails', 'U') IS NOT NULL DROP TABLE ScanDetails;
    IF OBJECT_ID('dbo.EmployeeAssetMapping', 'U') IS NOT NULL DROP TABLE EmployeeAssetMapping;
    IF OBJECT_ID('dbo.RolePageAccess', 'U') IS NOT NULL DROP TABLE RolePageAccess;
    IF OBJECT_ID('dbo.PageMaster', 'U') IS NOT NULL DROP TABLE PageMaster;
    IF OBJECT_ID('dbo.UserMaster', 'U') IS NOT NULL DROP TABLE UserMaster;
    IF OBJECT_ID('dbo.RoleMasters', 'U') IS NOT NULL DROP TABLE RoleMasters;
    IF OBJECT_ID('dbo.AssetMaster', 'U') IS NOT NULL DROP TABLE AssetMaster;
    IF OBJECT_ID('dbo.Employee', 'U') IS NOT NULL DROP TABLE Employee;
    IF OBJECT_ID('dbo.LocationMaster', 'U') IS NOT NULL DROP TABLE LocationMaster;
    IF OBJECT_ID('dbo.VendorMaster', 'U') IS NOT NULL DROP TABLE VendorMaster;
    IF OBJECT_ID('dbo.CategoryMaster', 'U') IS NOT NULL DROP TABLE CategoryMaster;

    PRINT 'Creating tables...';

    /******************************************************************
     *  1. RoleMasters
     ******************************************************************/
    CREATE TABLE RoleMasters (
        RoleID UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        RoleName NVARCHAR(100) UNIQUE NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy UNIQUEIDENTIFIER NULL,
        ModifiedOn DATETIME NULL
    );

    /******************************************************************
     *  2. PageMaster
     ******************************************************************/
    CREATE TABLE PageMaster (
        PageID UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        PageName NVARCHAR(255) NOT NULL,
        PageURL NVARCHAR(255) UNIQUE NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy UNIQUEIDENTIFIER NULL,
        ModifiedOn DATETIME NULL
    );

    /******************************************************************
     *  3. UserMaster (Updated)
     ******************************************************************/
    CREATE TABLE UserMaster (
        UserID UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        EmailID NVARCHAR(255) UNIQUE NOT NULL,
        PasswordHash NVARCHAR(255) NOT NULL,
        EmployeeName NVARCHAR(255) NOT NULL,
        Username NVARCHAR(255) NULL,
        EmployeeID NVARCHAR(50) NULL,
        Location NVARCHAR(255) NULL,
        RoleID UNIQUEIDENTIFIER NOT NULL,
        IsActive BIT DEFAULT 1,
        CreatedBy UNIQUEIDENTIFIER NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy UNIQUEIDENTIFIER NULL,
        ModifiedOn DATETIME NULL,
        CONSTRAINT FK_UserMaster_Role FOREIGN KEY (RoleID) REFERENCES RoleMasters(RoleID)
    );

    -- Self-referencing constraints for audit fields in UserMaster
    ALTER TABLE UserMaster
        ADD CONSTRAINT FK_UserMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES UserMaster(UserID);
    ALTER TABLE UserMaster
        ADD CONSTRAINT FK_UserMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES UserMaster(UserID);

    -- Foreign keys for RoleMasters referencing UserMaster for CreatedBy/ModifiedBy
    ALTER TABLE RoleMasters
        ADD CONSTRAINT FK_RoleMasters_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES UserMaster(UserID);
    ALTER TABLE RoleMasters
        ADD CONSTRAINT FK_RoleMasters_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES UserMaster(UserID);

    -- Foreign keys for PageMaster referencing UserMaster for CreatedBy/ModifiedBy
    ALTER TABLE PageMaster
        ADD CONSTRAINT FK_PageMaster_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES UserMaster(UserID);
    ALTER TABLE PageMaster
        ADD CONSTRAINT FK_PageMaster_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES UserMaster(UserID);

    /******************************************************************
     *  4. RolePageAccess
     ******************************************************************/
    CREATE TABLE RolePageAccess (
        RolePageAccessID UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() PRIMARY KEY,
        RoleID UNIQUEIDENTIFIER NOT NULL,
        PageID UNIQUEIDENTIFIER NOT NULL,
        CreatedBy UNIQUEIDENTIFIER NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy UNIQUEIDENTIFIER NULL,
        ModifiedOn DATETIME NULL,
        CONSTRAINT FK_RolePageAccess_Role FOREIGN KEY (RoleID) REFERENCES RoleMasters(RoleID),
        CONSTRAINT FK_RolePageAccess_Page FOREIGN KEY (PageID) REFERENCES PageMaster(PageID),
        CONSTRAINT FK_RolePageAccess_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES UserMaster(UserID),
        CONSTRAINT FK_RolePageAccess_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES UserMaster(UserID),
        UNIQUE (RoleID, PageID)
    );

    /******************************************************************
     *  5. CategoryMaster
     ******************************************************************/
    CREATE TABLE CategoryMaster (
        CategoryID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        CategoryName NVARCHAR(100) UNIQUE NOT NULL DEFAULT(''),
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT('')
    );

    /******************************************************************
     *  6. VendorMaster
     ******************************************************************/
    CREATE TABLE VendorMaster (
        VendorID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Name NVARCHAR(255) NOT NULL DEFAULT(''),
        VendorType NVARCHAR(200) NOT NULL DEFAULT(''),
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT(''),
		CONSTRAINT UQ_VendorMaster_NameVendorType UNIQUE (Name, VendorType)
    );

    /******************************************************************
     *  7. LocationMaster
     ******************************************************************/
    CREATE TABLE LocationMaster (
        LocationID UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
        Country NVARCHAR(100) NULL DEFAULT(''),
        State NVARCHAR(100) NULL DEFAULT(''),
        City NVARCHAR(100) NOT NULL DEFAULT(''),
        Address NVARCHAR(255) NULL DEFAULT(''),
        ZipCode NVARCHAR(20) NULL DEFAULT(''),
        LocationType INT NULL DEFAULT(0),
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT('')
    );

    /******************************************************************
     *  8. Employee
     ******************************************************************/
    CREATE TABLE Employee (
        EmpId INT IDENTITY(1,1) ,
		EmployeeId INT PRIMARY KEY,
        EmployeeName NVARCHAR(100)  NOT NULL DEFAULT(''),
        Manager NVARCHAR(100) NOT NULL DEFAULT(''),
        Department NVARCHAR(255) NOT NULL DEFAULT(''),
        Title NVARCHAR(255) NOT NULL DEFAULT(''),
        Phone NVARCHAR(50) NOT NULL DEFAULT(''),
        Address NVARCHAR(255) NOT NULL DEFAULT(''),
        City NVARCHAR(100) NOT NULL DEFAULT(''),
        State NVARCHAR(100) NOT NULL DEFAULT(''),
        Country NVARCHAR(100) NOT NULL DEFAULT(''),
        ZipCode NVARCHAR(20) NOT NULL DEFAULT(''),
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT('')
    );

    /******************************************************************
     * Create Sequence for AssetMaster.AssetID Generation
     ******************************************************************/
    IF OBJECT_ID('dbo.Seq_AssetID', 'SO') IS NOT NULL
        DROP SEQUENCE dbo.Seq_AssetID;
    CREATE SEQUENCE dbo.Seq_AssetID
        AS INT
        START WITH 1
        INCREMENT BY 1;

    /******************************************************************
     *  9. AssetMaster (Updated AssetID Format)
     ******************************************************************/
    CREATE TABLE AssetMaster (
        AssetID NVARCHAR(10) PRIMARY KEY 
            DEFAULT ('AST-' + RIGHT('000' + CAST(NEXT VALUE FOR dbo.Seq_AssetID AS VARCHAR(3)), 3)),
        CompanyName NVARCHAR(255) NOT NULL DEFAULT(''),
        AssetName NVARCHAR(255) NOT NULL DEFAULT(''),
        AssetTag NVARCHAR(50) NOT NULL DEFAULT('') UNIQUE,
        Model NVARCHAR(255) NOT NULL DEFAULT(''),
        ModelNo NVARCHAR(50) NOT NULL DEFAULT(''),
        CategoryID UNIQUEIDENTIFIER,
        ManufacturerID UNIQUEIDENTIFIER,
        SerialNumber NVARCHAR(100) NOT NULL DEFAULT('') UNIQUE,
        PurchasedDate DATE,
        Cost DECIMAL(18,2),
        EOL DATE,
        OrderNumber NVARCHAR(50) NOT NULL DEFAULT(''),
        SupplierID UNIQUEIDENTIFIER,
        DefaultLocationID UNIQUEIDENTIFIER,
        Status NVARCHAR(100) NOT NULL DEFAULT(''),
        WarrantyPeriod INT,
        WarrantyExpires DATE,
        CurrentValue DECIMAL(18,2),
        FullyDepreciated BIT DEFAULT 0,
        LastAudit DATE NULL,
        NextAuditDate DATE NULL,
        Notes NVARCHAR(MAX) NOT NULL DEFAULT(''),
		AssetEntryProcess NVARCHAR(255),
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        Deleted BIT DEFAULT 0,
        CONSTRAINT FK_AssetMaster_Category FOREIGN KEY (CategoryID) REFERENCES CategoryMaster(CategoryID),
        CONSTRAINT FK_AssetMaster_Manufacturer FOREIGN KEY (ManufacturerID) REFERENCES VendorMaster(VendorID),
        CONSTRAINT FK_AssetMaster_Supplier FOREIGN KEY (SupplierID) REFERENCES VendorMaster(VendorID),
        CONSTRAINT FK_AssetMaster_Location FOREIGN KEY (DefaultLocationID) REFERENCES LocationMaster(LocationID)
    );

    /******************************************************************
     *  10. EmployeeAssetMapping
     ******************************************************************/
    CREATE TABLE EmployeeAssetMapping (
        MappedId INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeId INT NOT NULL,
        AssetId NVARCHAR(10) NOT NULL,
        CreatedOn DATETIME DEFAULT GETDATE(),
        CreatedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        ModifiedOn DATETIME DEFAULT GETDATE(),
        ModifiedBy NVARCHAR(255) NOT NULL DEFAULT(''),
        UnmappedID INT,
        CONSTRAINT FK_EmployeeAssetMapping_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
        CONSTRAINT FK_EmployeeAssetMapping_Asset FOREIGN KEY (AssetId) REFERENCES AssetMaster(AssetID)
    );

    /******************************************************************
     *  11. ScanDetails
     ******************************************************************/
    CREATE TABLE ScanDetails (
        ScanId INT IDENTITY(1,1) PRIMARY KEY,
        EmployeeId INT NOT NULL,
        AssetId NVARCHAR(10) NOT NULL,
        EmployeeName NVARCHAR(100) NOT NULL DEFAULT(''),
        TransactionTime DATETIME,
        TransactionType NVARCHAR(50) NOT NULL,
        MismatchedEmployeeId NVARCHAR(255),
        CONSTRAINT FK_ScanDetails_Employee FOREIGN KEY (EmployeeId) REFERENCES Employee(EmployeeId),
        CONSTRAINT FK_ScanDetails_Asset FOREIGN KEY (AssetId) REFERENCES AssetMaster(AssetID)
    );
	
	ALTER TABLE ScanDetails  
ADD AssetName NVARCHAR(255) NULL,
    Location NVARCHAR(255) NULL;


    PRINT 'Tables created successfully. Inserting sample data...';

    /******************************************************************
     * Insert Sample Roles
     ******************************************************************/
    INSERT INTO RoleMasters (RoleID, RoleName, CreatedOn)
    VALUES 
        (NEWID(), 'Super Admin', GETDATE()),
        (NEWID(), 'Transaction Manager', GETDATE()),
        (NEWID(), 'Asset Administrator', GETDATE());

    /******************************************************************
     * Insert Sample Pages
     ******************************************************************/
    INSERT INTO PageMaster (PageID, PageName, PageURL, CreatedOn)
    VALUES
        (NEWID(), 'Dashboard', '/Dashboard', GETDATE()),
        (NEWID(), 'QR Code Generator', '/QRGenerator', GETDATE()),
        (NEWID(), 'AssetLogs', '/AssetLogReport', GETDATE()),
        (NEWID(), 'Add New Asset', '/AddAsset', GETDATE()),
        (NEWID(), 'Map Asset', '/MapAsset', GETDATE()),
        (NEWID(), 'Scanner', '/Scanner', GETDATE()),
        (NEWID(), 'Manage Users', '/ManageUsers', GETDATE());

    /******************************************************************
     * Retrieve RoleIDs for sample roles
     ******************************************************************/
    DECLARE @SuperAdminID UNIQUEIDENTIFIER = (
        SELECT RoleID FROM RoleMasters WHERE RoleName = 'Super Admin'
    );
    DECLARE @TransactionManagerID UNIQUEIDENTIFIER = (
        SELECT RoleID FROM RoleMasters WHERE RoleName = 'Transaction Manager'
    );
    DECLARE @AssetAdminID UNIQUEIDENTIFIER = (
        SELECT RoleID FROM RoleMasters WHERE RoleName = 'Asset Administrator'
    );

    /******************************************************************
     * Insert Sample Users
     ******************************************************************/
    INSERT INTO UserMaster (UserID, EmailID, PasswordHash, EmployeeName, Username, RoleID, CreatedOn)
VALUES
    (NEWID(), 'adheek.ahmed@bs.nttdata.com', 'password', 'Super Admin User', 'adheek.ahmed', @SuperAdminID, GETDATE()),
    (NEWID(), 'preethi.vijayasanjay@bs.nttdata.com', 'password', 'Transaction Manager', 'preethi.vijayasanjay', @TransactionManagerID, GETDATE()),
    (NEWID(), 'balaganesh.gopalakrishnan@bs.nttdata.com', 'password', 'Asset Admin', 'balaganesh.gopalakrishnan', @AssetAdminID, GETDATE());

    /******************************************************************
     * Retrieve PageIDs for sample pages
     ******************************************************************/
    DECLARE @DashboardPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'Dashboard'
    );
    DECLARE @QRGeneratorPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'QR Code Generator'
    );
    DECLARE @AssetLogReportPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'AssetLogs'
    );
    DECLARE @AddAssetPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'Add New Asset'
    );
    DECLARE @MapAssetPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'Map Asset'
    );
    DECLARE @ScannerPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'Scanner'
    );
    DECLARE @ManageUsersPageID UNIQUEIDENTIFIER = (
        SELECT PageID FROM PageMaster WHERE PageName = 'Manage Users'
    );

    /******************************************************************
     * Insert Sample RolePageAccess
     ******************************************************************/
    INSERT INTO RolePageAccess (RolePageAccessID, RoleID, PageID, CreatedOn)
    VALUES
        -- Super Admin
        (NEWID(), @SuperAdminID, @DashboardPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @QRGeneratorPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @AssetLogReportPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @AddAssetPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @MapAssetPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @ScannerPageID, GETDATE()),
        (NEWID(), @SuperAdminID, @ManageUsersPageID, GETDATE()),

        -- Transaction Manager
        (NEWID(), @TransactionManagerID, @DashboardPageID, GETDATE()),
        (NEWID(), @TransactionManagerID, @ScannerPageID, GETDATE()),
        (NEWID(), @TransactionManagerID, @AssetLogReportPageID, GETDATE()),

        -- Asset Administrator
        (NEWID(), @AssetAdminID, @DashboardPageID, GETDATE()),
        (NEWID(), @AssetAdminID, @QRGeneratorPageID, GETDATE()),
        (NEWID(), @AssetAdminID, @AssetLogReportPageID, GETDATE()),
        (NEWID(), @AssetAdminID, @MapAssetPageID, GETDATE()),
        (NEWID(), @AssetAdminID, @ScannerPageID, GETDATE()),
        (NEWID(), @AssetAdminID, @AddAssetPageID, GETDATE());

    PRINT 'All tables created and sample data inserted successfully.';
END;
GO
