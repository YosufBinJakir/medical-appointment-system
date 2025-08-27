using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicalAppointMentSystem.Data;
using MedicalAppointMentSystem.Entities;
using Microsoft.Data.SqlClient;
using System.Data;
using MedicalAppointMentSystem.DTOs;
using MedicalAppointMentSystem.DTOs.EditAppointment;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System.Drawing;
using MedicalAppointMentSystem.EmailServices;
using Microsoft.Build.Logging;
using MedicalAppointMentSystem.DTOs.EmailDtos;

namespace MedicalAppointMentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(AppDbContext context, IConfiguration configuration, IEmailService emailService, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        // GET: api/Appointments
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(int page, int pageSize, string searchInput)
        //{

        //    return await _context.Appointments.ToListAsync();
        //}
        [HttpGet]
        public async Task<IActionResult> GetAppointments(
        [FromQuery] string? searchInput = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("connection");
                var appointments = new List<AppointmentGridDto>();
                var paginationInfo = new PaginationInfoDto();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetAppointmentsWithSearch", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SearchInput", (object?)searchInput ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageSize);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var appointment = new AppointmentGridDto
                                {
                                   AppointmentId = reader.GetInt32("AppointmentId"),
                                    PatientName = reader.GetString("PatientName"),
                                    DoctorName = reader.GetString("DoctorName"),
                                    AppointDate = reader.GetDateTime("AppointDate"),
                                    VisitType = reader.GetString("VisitType"),
                                    Diagnosis = reader.IsDBNull("Diagnosis") ? null : reader.GetString("Diagnosis"),
                                    Email = reader.GetString("Email"),
                                };

                                if (appointments.Count == 0)
                                {
                                    //paginationInfo.TotalCount = reader.GetInt32("TotalCount");
                                    //paginationInfo.CurrentPage = reader.GetInt32("CurrentPage");
                                    //paginationInfo.PageSize = reader.GetInt32("PageSize");
                                    //paginationInfo.TotalPages = reader.GetInt32("TotalPages");
                                    paginationInfo.TotalCount = Convert.ToInt32(reader["TotalCount"]);
                                    paginationInfo.CurrentPage = Convert.ToInt32(reader["CurrentPage"]);
                                    paginationInfo.PageSize = Convert.ToInt32(reader["PageSize"]);
                                    paginationInfo.TotalPages = Convert.ToInt32(reader["TotalPages"]);

                                }

                                appointments.Add(appointment);
                            }
                        }
                    }
                }

                var result = new AppointmentResultDto
                {
                    Data = appointments,
                    Pagination = paginationInfo,
                    Success = true,
                    Message = appointments.Any() ? "Appointments retrieved successfully" : "No appointments found"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving appointments",
                    Error = ex.Message
                });
            }
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            var appointmentDto = await (
                from a in _context.Appointments
                join d in _context.Doctors on a.DoctorId equals d.DoctorId
                join p in _context.Patients on a.PatientId equals p.PatientId
                where a.AppointmentId == id
                select new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorId = a.DoctorId,
                    DoctorName = d.DoctorName,
                    PatientId = a.PatientId,
                    PatientName = p.PatientName,
                    VisitType = a.VisitType,
                    Notes = a.Notes,
                    Diagnosis = a.Diagnosis,
                    AppointDate = a.AppointDate,

                    // join prescription + medicine
                    PrescriptionDetails = (
                        from pd in _context.PrescriptionDetails
                        join m in _context.Medicines on pd.MedicineId equals m.MedicineId into meds
                        from med in meds.DefaultIfEmpty() 
                        where pd.AppointmentId == a.AppointmentId
                        select new PrescriptionDetailDto
                        {
                            PrescriptionDetailId = pd.PrescriptionDetailId,
                            MedicineId = pd.MedicineId,
                            MedicineName = med != null ? med.MedicineName : null,
                            Dosage = pd.Dosage,
                            Notes = pd.Notes,
                            StartDate = pd.StartDate,
                            EndDate = pd.EndDate
                        }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();

            if (appointmentDto == null)
                return NotFound();

            return appointmentDto;
        }


        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentDto appointmentDto)
        {
            if (id != appointmentDto.AppointmentId)
                return BadRequest();

            // Get existing appointment with children
            var appointment = await _context.Appointments
                .Include(a => a.PrescriptionDetails)
                .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            // Update main appointment fields
            appointment.DoctorId = appointmentDto.DoctorId;
            appointment.PatientId = appointmentDto.PatientId;
            appointment.VisitType = appointmentDto.VisitType;
            appointment.Notes = appointmentDto.Notes;
            appointment.Diagnosis = appointmentDto.Diagnosis;
            appointment.AppointDate = appointmentDto.AppointDate;
            appointment.CreatedBy = appointment.CreatedBy;
            appointment.UpdatedBy = appointment.UpdatedBy;

            // --- Handle PrescriptionDetails ---
            // 1. Remove deleted items
            var incomingIds = appointmentDto.PrescriptionDetails
                .Where(p => p.PrescriptionDetailId != 0)
                .Select(p => p.PrescriptionDetailId)
                .ToList();

            var toRemove = appointment.PrescriptionDetails
                .Where(p => !incomingIds.Contains(p.PrescriptionDetailId))
                .ToList();

            _context.PrescriptionDetails.RemoveRange(toRemove);

            // 2. Update existing and add new items
            foreach (var pdDto in appointmentDto.PrescriptionDetails)
            {
                if (pdDto.PrescriptionDetailId == 0)
                {
                    // New prescription
                    var newPd = new PrescriptionDetail
                    {
                        MedicineId = pdDto.MedicineId,
                        Dosage = pdDto.Dosage,
                        Notes = pdDto.Notes,
                        StartDate = pdDto.StartDate,
                        EndDate = pdDto.EndDate,
                        AppointmentId = appointment.AppointmentId,
                        CreatedBy = "Default",
                        UpdatedBy = "Default"
                    };
                    appointment.PrescriptionDetails.Add(newPd);
                }
                else
                {
                    // Existing prescription
                    var existingPd = appointment.PrescriptionDetails
                        .FirstOrDefault(p => p.PrescriptionDetailId == pdDto.PrescriptionDetailId);
                    if (existingPd != null)
                    {
                        existingPd.MedicineId = pdDto.MedicineId;
                        existingPd.Dosage = pdDto.Dosage;
                        existingPd.Notes = pdDto.Notes;
                        existingPd.StartDate = pdDto.StartDate;
                        existingPd.EndDate = pdDto.EndDate;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }


        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostAppointment(AppointmentFormDto appointmentDto)
        {
            if (appointmentDto == null)
                return BadRequest("Appointment data is required.");

            // Map DTO to entity
            var appointment = new Appointment
            {
                PatientId = appointmentDto.PatientId!.Value,
                DoctorId = appointmentDto.DoctorId!.Value,
                AppointDate = appointmentDto.AppointmentDate!.Value,
                VisitType = appointmentDto.VisitType!,
                Notes = appointmentDto.Notes,
                Diagnosis = appointmentDto.Diagnosis,
                CreatedBy ="User1",
                UpdatedBy ="User1",
                PrescriptionDetails = appointmentDto.PrescriptionDetailFormDtos.Select(p => new PrescriptionDetail
                {
                    MedicineId = p.MedicineId!.Value,
                    Dosage = p.Dosage!,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Notes = p.Notes,
                    CreatedBy = "User1",
                    UpdatedBy = "User1"
                }).ToList()
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return Ok(appointment);
        }


        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments
                                            .Include(a => a.PrescriptionDetails)
                                            .FirstOrDefaultAsync(a => a.AppointmentId == id);

            if (appointment == null)
                return NotFound();

            // Delete related prescription details
            if (appointment.PrescriptionDetails != null)
            {
                _context.PrescriptionDetails.RemoveRange(appointment.PrescriptionDetails);
            }

            // Delete the appointment
            _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();

            return NoContent();
        }


        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> DownloadAppointmentPdf(int id)
        {
            var appointment = await (
                from a in _context.Appointments
                join d in _context.Doctors on a.DoctorId equals d.DoctorId
                join p in _context.Patients on a.PatientId equals p.PatientId
                where a.AppointmentId == id
                select new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorId = a.DoctorId,
                    DoctorName = d.DoctorName,
                    PatientId = a.PatientId,
                    PatientName = p.PatientName,
                    VisitType = a.VisitType,
                    Notes = a.Notes,
                    Diagnosis = a.Diagnosis,
                    AppointDate = a.AppointDate,

                    PrescriptionDetails = (
                        from pd in _context.PrescriptionDetails
                        join m in _context.Medicines on pd.MedicineId equals m.MedicineId into meds
                        from med in meds.DefaultIfEmpty()
                        where pd.AppointmentId == a.AppointmentId
                        select new PrescriptionDetailDto
                        {
                            PrescriptionDetailId = pd.PrescriptionDetailId,
                            MedicineId = pd.MedicineId,
                            MedicineName = med != null ? med.MedicineName : null,
                            Dosage = pd.Dosage,
                            Notes = pd.Notes,
                            StartDate = pd.StartDate,
                            EndDate = pd.EndDate
                        }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();

            if (appointment == null)
                return NotFound();

      
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.PageColor("#F8F7F7");
                    page.Size(PageSizes.A4);

                    page.Header().PaddingTop(30).PaddingLeft(25).AlignLeft()
                        .Text("Prescription Report")
                        .Bold().FontSize(20);

                    page.Content()
                        .PaddingVertical(10)
                        .PaddingTop(3)
                        .PaddingLeft(25)
                        .Column(column =>
                        {
                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Patient: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.PatientName}");
                            });


                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Doctor: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.DoctorName}");
                            });

                           

                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Date: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.AppointDate:dd-MMM-yyyy} "
                                   // +
                                   // $"Time: {appointment.AppointDate:hh:mm tt}"
                                   );
                            });

                            column.Item().Padding(2).PaddingBottom(10).Text(text =>
                            {
                                text.Span("Visit Type: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.VisitType}");
                            });



                            column.Item().Padding(2).PaddingBottom(5).PaddingTop(5).Text("Prescriptions:").Bold().FontSize(14);

                            if (appointment.PrescriptionDetails.Any())
                            {
                                column.Item().Padding(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Medicine").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Dosage").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Start Date").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("End Date").Bold();
                                    });

                                    foreach (var pd in appointment.PrescriptionDetails)
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.MedicineName ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.Dosage ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.StartDate?.ToString("dd-MMM-yyyy") ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.EndDate?.ToString("dd-MMM-yyyy") ?? "-");
                                    }
                                });
                            }
                            else
                            {
                                column.Item()
                                      .Border(1)
                                      .BorderColor(Colors.Black)
                                      .Padding(5)
                                      .Text("No prescription details available.");
                            }


                        });

                    page.Footer()
                        .AlignRight()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(t => t.FontSize(10));
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                            x.Span("    |    ");
                            x.Span($"Printed on: {DateTime.Now:dd-MMM-yyyy hh:mm tt}");
                        });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Appointment_{appointment.AppointmentId}.pdf");
        }




        [HttpPost("{id}/send-email")]
        public async Task<IActionResult> SendPrescriptionEmail(int id, [FromBody] SendPrescriptionEmailRequest request)
        {
            try
            {
                // Verify the appointment exists and get patient email
                var appointment = await _context.Appointments
                    .Include(a => a.PrescriptionDetails)
                    .FirstOrDefaultAsync(a => a.AppointmentId == id);

                if (appointment == null)
                    return NotFound("Appointment not found");

                // If patient email is not provided in request, try to get it from appointment
                var patientEmail = request.PatientEmail;
                if (string.IsNullOrEmpty(patientEmail) && appointment.UpdatedBy != null)
                {
                    // Assuming your Patient entity has an Email property
                    patientEmail = appointment.UpdatedBy;
                }

                if (string.IsNullOrEmpty(patientEmail))
                    return BadRequest("Patient email is required");

                // Generate the PDF
                var pdfBytes = await GeneratePdfBytes(id);

                if (pdfBytes == null)
                    return BadRequest("Failed to generate PDF");

                // Send email with attachment
                await _emailService.SendEmailAsync(
                    patientEmail,
                    request.Subject,
                    request.Body,
                    pdfBytes,
                    $"Prescription_{id}.pdf"
                );

                _logger.LogInformation($"Prescription email sent successfully to {patientEmail} for appointment {id}");

                return Ok(new { message = "Prescription sent successfully to patient's email" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending prescription email for appointment {id}");
                return StatusCode(500, "An error occurred while sending the email");
            }
        }


        // Extract your PDF generation logic to a separate method
        private async Task<byte[]> GeneratePdfBytes(int appointmentId)
        {
            var appointment = await (
                from a in _context.Appointments
                join d in _context.Doctors on a.DoctorId equals d.DoctorId
                join p in _context.Patients on a.PatientId equals p.PatientId
                where a.AppointmentId == appointmentId
                select new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    DoctorId = a.DoctorId,
                    DoctorName = d.DoctorName,
                    PatientId = a.PatientId,
                    PatientName = p.PatientName,
                    VisitType = a.VisitType,
                    Notes = a.Notes,
                    Diagnosis = a.Diagnosis,
                    AppointDate = a.AppointDate,
                    PrescriptionDetails = (
                        from pd in _context.PrescriptionDetails
                        join m in _context.Medicines on pd.MedicineId equals m.MedicineId into meds
                        from med in meds.DefaultIfEmpty()
                        where pd.AppointmentId == a.AppointmentId
                        select new PrescriptionDetailDto
                        {
                            PrescriptionDetailId = pd.PrescriptionDetailId,
                            MedicineId = pd.MedicineId,
                            MedicineName = med != null ? med.MedicineName : null,
                            Dosage = pd.Dosage,
                            Notes = pd.Notes,
                            StartDate = pd.StartDate,
                            EndDate = pd.EndDate
                        }
                    ).ToList()
                }
            ).FirstOrDefaultAsync();

            if (appointment == null)
                return null;

            // Your existing PDF generation code
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.PageColor("#F8F7F7");
                    page.Size(PageSizes.A4);

                    page.Header().PaddingTop(30).PaddingLeft(25).AlignLeft()
                        .Text("Prescription Report")
                        .Bold().FontSize(20);

                    page.Content()
                        .PaddingVertical(10)
                        .PaddingTop(3)
                        .PaddingLeft(25)
                        .Column(column =>
                        {
                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Patient: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.PatientName}");
                            });

                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Doctor: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.DoctorName}");
                            });

                            column.Item().Padding(2).PaddingBottom(2).Text(text =>
                            {
                                text.Span("Date: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.AppointDate:dd-MMM-yyyy}");
                            });

                            column.Item().Padding(2).PaddingBottom(10).Text(text =>
                            {
                                text.Span("Visit Type: ").Bold();
                                text.Span(" ");
                                text.Span(" ");
                                text.Span(" ");
                                text.Span($"{appointment.VisitType}");
                            });

                            column.Item().Padding(2).PaddingBottom(5).PaddingTop(5).Text("Prescriptions:").Bold().FontSize(14);

                            if (appointment.PrescriptionDetails.Any())
                            {
                                column.Item().Padding(5).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Medicine").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Dosage").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("Start Date").Bold();
                                        header.Cell().Border(1).BorderColor(Colors.Black).Padding(5).AlignCenter().Text("End Date").Bold();
                                    });

                                    foreach (var pd in appointment.PrescriptionDetails)
                                    {
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.MedicineName ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.Dosage ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.StartDate?.ToString("dd-MMM-yyyy") ?? "-");
                                        table.Cell().Border(1).BorderColor(Colors.Black).Padding(2).AlignCenter().Text(pd.EndDate?.ToString("dd-MMM-yyyy") ?? "-");
                                    }
                                });
                            }
                            else
                            {
                                column.Item()
                                    .Border(1)
                                    .BorderColor(Colors.Black)
                                    .Padding(5)
                                    .Text("No prescription details available.");
                            }
                        });

                    page.Footer()
                        .AlignRight()
                        .Text(x =>
                        {
                            x.DefaultTextStyle(t => t.FontSize(10));
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                            x.Span("    |    ");
                            x.Span($"Printed on: {DateTime.Now:dd-MMM-yyyy hh:mm tt}");
                        });
                });
            }).GeneratePdf();

            return pdfBytes;
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
