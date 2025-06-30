-- Add Patient
CREATE OR ALTER PROCEDURE PR_Pat_Patient_Insert
    @Name NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(100),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @IsActive BIT = 1,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    INSERT INTO Patient (
	[Name]
	,[DateOfBirth], 
	 [Gender],
	 [Email], 
	 [Phone], 
	 [Address],
	 [City],
	 [State], 
	 [IsActive], 
	 [Modified], 
	 [UserID])
    VALUES (@Name, @DateOfBirth, @Gender, @Email, @Phone, @Address, @City, @State, @IsActive, @Modified, @UserID)
END




-- Edit Patient
CREATE OR ALTER PROCEDURE PR_Pat_Patient_UpdateByPK
    @PatientID INT,
    @Name NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Gender NVARCHAR(10),
    @Email NVARCHAR(100),
    @Phone NVARCHAR(100),
    @Address NVARCHAR(250),
    @City NVARCHAR(100),
    @State NVARCHAR(100),
    @IsActive BIT,
    @Modified DATETIME,
    @UserID INT
AS
BEGIN
    UPDATE Patient
    SET Name = @Name,
	    DateOfBirth = @DateOfBirth,
        Gender = @Gender,
        Email = @Email,
        Phone = @Phone,
        Address = @Address,
        City = @City,
        State = @State,
        IsActive = @IsActive,
        Modified = @Modified,
        UserID = @UserID
    WHERE PatientID = @PatientID
END





--Get All Patients
CREATE OR ALTER PROCEDURE PR_Pat_Patient_SelectAll
AS
BEGIN
    SELECT P.PatientID, P.Name, P.Phone, P.Email, P.Address, P.DateOfBirth, P.Gender, P.City, P.State, P.IsActive, P.Created, P.Modified, U.UserName, U.Email, U.MobileNo
    FROM Patient AS P JOIN [User] AS U
        ON P.UserID = U.UserID;
end





-- Delete Patient
CREATE OR ALTER PROCEDURE PR_Pat_Patient_Delete
    @PatientID INT
AS
BEGIN
    DELETE FROM Patient
    WHERE Patient.PatientID = @PatientID
END




--Select Patient By Primary Key
CREATE OR ALTER PROCEDURE PR_Pat_Patient_SelectByPK
    @PatientID INT
AS
BEGIN
    SELECT P.PatientID, P.Name, P.Phone, P.Email, P.Address, P.DateOfBirth, P.Gender, P.City, P.State, P.IsActive, P.Created, P.Modified, U.UserName, U.Email, U.MobileNo
    FROM Patient AS P JOIN [User] AS U
        ON P.UserID = U.UserID
    WHERE P.PatientID = @PatientID
END

	
