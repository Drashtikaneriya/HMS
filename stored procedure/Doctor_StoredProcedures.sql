-- Add Doctor
CREATE OR ALTER PROCEDURE PR_Doc_Doctor_Insert
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Qualification NVARCHAR(100),
    @Specialization NVARCHAR(100),
    @IsActive BIT = 1,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    INSERT INTO Doctor (Name, Phone, Email, Qualification, Specialization, IsActive, Modified, UserID)
    VALUES (@Name, @Phone, @Email, @Qualification, @Specialization, @IsActive, @Modified, @UserID)
END




-- Edit Doctor
CREATE OR ALTER PROCEDURE PR_Doc_Doctor_UpdateByPK
    @DoctorID INT,
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Qualification NVARCHAR(100),
    @Specialization NVARCHAR(100),
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE Doctor
    SET Name = @Name,
        Phone = @Phone,
        Email = @Email,
        Qualification = @Qualification,
        Specialization = @Specialization,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE DoctorID = @DoctorID
END




-- Get All Doctors
CREATE OR ALTER PROCEDURE PR_Doc_Doctor_SelectAll
AS
BEGIN
     SELECT D.Name, D.Phone, D.Email, D.Qualification, D.Specialization, D.IsActive, D.Created, D.Modified, U.UserName, U.Email, U.MobileNo
    FROM Doctor AS D JOIN [User] AS U
        ON D.UserID = U.UserID;
END




--Delete Doctor
CREATE OR ALTER PROCEDURE PR_Doc_Doctor_Delete
    @DoctorID INT
AS
BEGIN
    DELETE FROM Doctor
    WHERE DoctorID = @DoctorID
END




-- Select Doctor By Primary Key
CREATE OR ALTER PROCEDURE PR_Doc_Doctor_SelectByPK
    @DoctorID INT
AS
BEGIN
     SELECT D.Name, D.Phone, D.Email, D.Qualification, D.Specialization, D.IsActive, D.Created, D.Modified, U.UserName, U.Email, U.MobileNo
    FROM Doctor AS D JOIN [User] AS U
        ON D.UserID = U.UserID
    WHERE DoctorID = @DoctorID;
END

