# Medical Appointment System 🏥  

A full-stack web application built with **.NET Core Web API**, **SQL Server**, and **Angular 19** (with **Bootstrap 5**) to manage patient appointments, doctor prescriptions, and related healthcare workflows.  

---

## 🚀 Features  

- **Patient Management** – Add, edit, delete patient details.  
- **Doctor Management** – Register and manage doctors.  
- **Appointments** – Patients can book, edit, and cancel appointments.  
- **Prescription Management** – Doctors can create, update, and download prescriptions.  
- **Inline Editing** – Quickly edit prescription details without leaving the page.  
- **Email Support** – Send prescription details directly to patients via email.  
- **Responsive UI** – Modern and mobile-friendly UI with Bootstrap 5.  
- **Pagination & Filtering** – Efficient data handling for large datasets.  

---

## 🛠️ Tech Stack  

**Frontend**  
- Angular 19  
- Bootstrap 5  
- TypeScript  

**Backend**  
- ASP.NET Core Web API  
- Entity Framework Core  
- LINQ  

**Database**  
- Microsoft SQL Server  

---

## 📂 Project Structure  

"# medical-appointment-system" 
Project Running Instruction:
migraton :  add-migration migrationName
dotnet run.
clinet side: ng serve -0
add valid email and apppassword in  appsetting.json
Script for procedure :
USE [MAS_DB]
GO

/****** Object:  StoredProcedure [dbo].[GetPrescriptionsWithSearch]    Script Date: 8/27/2025 9:00:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetPrescriptionsWithSearch]
    @SearchInput NVARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 OR @PageSize > 100 SET @PageSize = 10; -- Limit max page size
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredPrescriptions AS
    (
        SELECT 
            pr.AppointmentId,
			pr.PrescriptionDetailId,
			pr.CreatedAt,
			pr.CreatedBy,
			pr.DeletedAt,
			pr.Dosage,
			pr.StartDate,
			pr.EndDate,
			pr.Notes,
            p.PatientName, 
			p.PatientId,
            d.DoctorName,
			d.DoctorId,
            a.AppointDate, 
            a.VisitType, 
            a.Diagnosis,
			m.MedicineName,
			m.MedicineId
 
        FROM PrescriptionDetails pr
		INNER JOIN Appointments a ON pr.AppointmentId = a.AppointmentId
		INNER JOIN Patients p ON a.PatientId = p.PatientId
		INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
		INNER JOIN Medicines m ON pr.MedicineId = m.MedicineId

        WHERE 
            (@SearchInput IS NULL OR @SearchInput = '' OR 
             (d.DoctorName LIKE '%' + @SearchInput + '%' OR 
              a.VisitType LIKE '%' + @SearchInput + '%'))
    ),
    PaginatedResults AS
    (
        SELECT *,
               COUNT(*) OVER() AS TotalCount,
               ROW_NUMBER() OVER (ORDER BY AppointDate DESC) AS RowNum
        FROM FilteredPrescriptions
    )
    SELECT 
        AppointmentId,
		PrescriptionDetailId,
		MedicineId,
		DoctorId,
		PatientId,
        PatientName,
        DoctorName,
        AppointDate,
        VisitType,
        Diagnosis,
		MedicineName,
		Dosage,
		StartDate,
		EndDate,
		Notes,
        TotalCount,
        @PageNumber AS CurrentPage,
        @PageSize AS PageSize,
        CEILING(CAST(TotalCount AS FLOAT) / @PageSize) AS TotalPages
    FROM PaginatedResults
    WHERE RowNum BETWEEN (@Offset + 1) AND (@Offset + @PageSize)
    ORDER BY PrescriptionDetailId DESC;
END;
GO


USE [MAS_DB]
GO

/****** Object:  StoredProcedure [dbo].[GetAppointmentsWithSearch]    Script Date: 8/27/2025 8:59:54 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[GetAppointmentsWithSearch]
    @SearchInput NVARCHAR(200) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    

    IF @PageNumber < 1 SET @PageNumber = 1;
    IF @PageSize < 1 OR @PageSize > 100 SET @PageSize = 10; -- Limit max page size
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredAppointments AS
    (
        SELECT 
            a.AppointmentId,
            p.PatientName, 
            d.DoctorName, 
            a.AppointDate, 
            a.VisitType, 
            a.Diagnosis,
			p.Email
   
        FROM Appointments a
        INNER JOIN Patients p ON a.PatientId = p.PatientId
        INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
        WHERE 
            (@SearchInput IS NULL OR @SearchInput = '' OR 
             (d.DoctorName LIKE '%' + @SearchInput + '%' OR 
              a.VisitType LIKE '%' + @SearchInput + '%'))
    ),
    PaginatedResults AS
    (
        SELECT *,
               COUNT(*) OVER() AS TotalCount,
               ROW_NUMBER() OVER (ORDER BY AppointDate DESC) AS RowNum
        FROM FilteredAppointments
    )
    SELECT 
        AppointmentId,
        PatientName,
        DoctorName,
        AppointDate,
        VisitType,
        Diagnosis,
        TotalCount,
		Email,
        @PageNumber AS CurrentPage,
        @PageSize AS PageSize,
        CEILING(CAST(TotalCount AS FLOAT) / @PageSize) AS TotalPages
    FROM PaginatedResults
    WHERE RowNum BETWEEN (@Offset + 1) AND (@Offset + @PageSize)
    ORDER BY AppointDate DESC;
END;
GO






### 1️⃣ Clone Repository  
```bash
git clone https://github.com/your-username/medical-appointment-system.git
cd medical-appointment-system