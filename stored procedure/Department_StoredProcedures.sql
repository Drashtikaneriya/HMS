-- Add Department
CREATE OR ALTER PROCEDURE PR_Dept_Department_Insert
    @DepartmentName NVARCHAR(100),
    @Description NVARCHAR(250) = NULL,
    @IsActive BIT = 1,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    INSERT INTO Department (DepartmentName, [Description], IsActive, Modified, UserID)
    VALUES (@DepartmentName, @Description, @IsActive, @Modified, @UserID)
END



-- Edit Department
CREATE OR ALTER PROCEDURE PR_Dept_Department_UpdateByPK
    @DepartmentID INT,
    @DepartmentName NVARCHAR(100),
    @Description NVARCHAR(250) = NULL,
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE Department
    SET DepartmentName = @DepartmentName,
        [Description] = @Description,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE DepartmentID = @DepartmentID
END



-- Get All Deparments
CREATE OR ALTER PROCEDURE PR_Dept_Department_SelectAll
AS
BEGIN
    SELECT 
	D.DepartmentID,
	D.DepartmentName,
	D.Description,
	D.IsActive,
	D.Modified,
	D.UserID,
	D.Created,
	U.UserName,
	U.Email,
	U.MobileNo
	FROM 
	 Department AS D JOIN [User] AS U
        ON D.UserID = U.UserID;
END



-- Delete Department
CREATE OR ALTER PROCEDURE PR_Dept_Department_Delete
    @DepartmentID INT
AS
BEGIN
    DELETE FROM Department
    WHERE DepartmentID = @DepartmentID
END


-- SelectByPrimaryKey
CREATE OR ALTER PROCEDURE PR_Dept_Department_SelectByPK
    @DepartmentID INT
AS
BEGIN
   SELECT D.DepartmentID, D.DepartmentName, D.Description, D.IsActive, D.Created, D.Modified, U.UserName, U.Email, U.MobileNo
    FROM Department AS D JOIN [User] AS U
        ON D.UserID = U.UserID
    WHERE DepartmentID = @DepartmentID;
End

