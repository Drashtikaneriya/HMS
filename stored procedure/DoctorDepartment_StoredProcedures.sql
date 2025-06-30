-- Add DoctorDeparmtent
CREATE OR ALTER PROCEDURE PR_DocDept_DoctorDepartment_Insert
    @DoctorID INT,
    @DepartmentID INT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    INSERT INTO DoctorDepartment (DoctorID, DepartmentID, Modified, UserID)
    VALUES (@DoctorID, @DepartmentID, @Modified, @UserID)
END




-- Edit DoctorDepartment
CREATE OR ALTER PROCEDURE PR_DocDept_DoctorDepartment_Update 
      @DoctorID INT,
    @Name NVARCHAR(100),
    @Phone NVARCHAR(20),
    @Email NVARCHAR(100),
    @Qualification NVARCHAR(100),
    @Specialization NVARCHAR(100),
    @IsActive BIT,
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
        Modified = GETDATE(),
        UserID = @UserID
    WHERE DoctorID = @DoctorID;
END


--Get All DoctorDepartments
CREATE OR ALTER PROCEDURE PR_DocDept_DoctorDepartment_SelectAll
AS
BEGIN
    SELECT D.Name, D.Phone, D.Email, D.Qualification, D.Specialization, D.IsActive, D.Created, D.Modified, U.UserName, U.Email, U.MobileNo
    FROM Doctor AS D JOIN [User] AS U
        ON D.UserID = U.UserID;
END



--Delete DoctorDepartment
CREATE OR ALTER PROCEDURE PR_DocDept_DoctorDepartment_Delete
    @DoctorDepartmentID INT
AS
BEGIN
    DELETE FROM DoctorDepartment
    WHERE DoctorDepartmentID = @DoctorDepartmentID
END




--Select DoctorDepartment By Primary Key
CREATE OR ALTER PROCEDURE PR_DocDept_DoctorDepartment_SelectByPK
    @DoctorDepartmentID INT
AS
BEGIN
     SELECT D.Name, D.Phone, D.Email, D.Qualification, D.Specialization, D.IsActive, D.Created, D.Modified, U.UserName, U.Email, U.MobileNo
    FROM Doctor AS D JOIN [User] AS U
        ON D.UserID = U.UserID
    WHERE DoctorID = @DoctorDepartmentID;
END
