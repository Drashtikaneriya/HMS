--Add Appointment
CREATE OR ALTER PROCEDURE PR_App_Appointment_insert
    @DoctorID INT,
    @PatientID INT,
    @AppointmentDate DATETIME,
    @AppointmentStatus NVARCHAR(20),
    @Description NVARCHAR(250),
    @SpecialRemarks NVARCHAR(100),
    @Modified DATETIME,
    @UserID INT,
    @TotalConsultedAmount DECIMAL(5,2) = NULL
AS
BEGIN
    INSERT INTO Appointment (
        Appointment.DoctorID, Appointment.Appoimtment.PatientID, Appoimtment.[AppointmentDate], Appoimtment.[AppointmentStatus], Appoimtment.[Description],
        SpecialRemarks, Modified, UserID, TotalConsultedAmount
    )
    VALUES (
        @DoctorID, @PatientID, @AppointmentDate, @AppointmentStatus, @Description,
        @SpecialRemarks, @Modified, @UserID, @TotalConsultedAmount
    )
END




-- Edit Appointment
CREATE OR ALTER PROCEDURE PR_App_Appointment_UpdateByPK
    @AppointmentID INT,
    @DoctorID INT,
    @PatientID INT,
    @AppointmentDate DATETIME,
    @AppointmentStatus NVARCHAR(20),
    @Description NVARCHAR(250),
    @SpecialRemarks NVARCHAR(100),
    @Modified DATETIME,
    @UserID INT,
    @TotalConsultedAmount DECIMAL(5,2) = NULL
AS
BEGIN
    UPDATE Appointment
    SET DoctorID = @DoctorID,
        PatientID = @PatientID,
        AppointmentDate = @AppointmentDate,
        AppointmentStatus = @AppointmentStatus,
        [Description] = @Description,
        SpecialRemarks = @SpecialRemarks,
        Modified = @Modified,
        UserID = @UserID,
        TotalConsultedAmount = @TotalConsultedAmount
    WHERE AppointmentID = @AppointmentID
END





-- Get All Appointments
CREATE OR ALTER PROCEDURE PR_App_Appointment_SelectAll
AS
BEGIN
    SELECT A.AppointmentID, A.DoctorID, A.PatientID, A.AppointmentDate, A.AppointmentStatus, A.Description, A.SpecialRemarks, A.Created, A.Modified, A.UserID, A.TotalConsultedAmount,
        D.Name AS DoctorName, P.Name AS PatientName, U.UserName AS UserName
    FROM Appointment AS A JOIN Doctor AS D
        ON A.DoctorID = D.DoctorID JOIN Patient AS P
        ON A.PatientID = P.PatientID JOIN [User] AS U
        ON A.UserID = U.UserID;
END





--Delete Appointment
CREATE OR ALTER PROCEDURE PR_App_Appointment_Delete
    @AppointmentID INT
AS
BEGIN
    DELETE FROM Appointment
    WHERE AppointmentID = @AppointmentID
END




--Select Appointment By Primary Key
CREATE OR ALTER PROCEDURE PR_App_Appointment_SelectByPK
    @AppointmentID INT
AS
BEGIN
    SELECT A.AppointmentID, A.DoctorID, A.PatientID, A.AppointmentDate, A.AppointmentStatus, A.Description, A.SpecialRemarks, A.Created, A.Modified, A.UserID, A.TotalConsultedAmount,
        D.Name AS DoctorName, P.Name AS PatientName, U.UserName AS UserName
    FROM Appointment AS A JOIN Doctor AS D
        ON A.DoctorID = D.DoctorID JOIN Patient AS P
        ON A.PatientID = P.PatientID JOIN [User] AS U
        ON A.UserID = U.UserID
    WHERE AppointmentID = @AppointmentID;
end