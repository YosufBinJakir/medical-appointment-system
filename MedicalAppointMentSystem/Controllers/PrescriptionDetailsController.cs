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

namespace MedicalAppointMentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;


        public PrescriptionDetailsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: api/PrescriptionDetails
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<PrescriptionDetail>>> GetPrescriptionDetails()
        //{
        //    return await _context.PrescriptionDetails.ToListAsync();
        //}
        //public async Task<(List<AppointmentGridDto> Data, int TotalCount)> GetAppointmentsAsync(
        //string? patientName, string? doctorName, string? visitType, string? diagnosis, int pageNumber=1, int pageSize=1)
        //{
        //    var appointments = new List<AppointmentGridDto>();
        //    int totalCount = 0;

        //    using (var conn = _context.Database.GetDbConnection())
        //    {
        //        await conn.OpenAsync();
        //        using (var cmd = conn.CreateCommand())
        //        {
        //            cmd.CommandText = "GetAppointmentsWithFilterAndPaging";
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.Add(new SqlParameter("@PatientName", (object?)patientName ?? DBNull.Value));
        //            cmd.Parameters.Add(new SqlParameter("@DoctorName", (object?)doctorName ?? DBNull.Value));
        //            cmd.Parameters.Add(new SqlParameter("@VisitType", (object?)visitType ?? DBNull.Value));
        //            cmd.Parameters.Add(new SqlParameter("@Diagnosis", (object?)diagnosis ?? DBNull.Value));
        //            cmd.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
        //            cmd.Parameters.Add(new SqlParameter("@PageSize", pageSize));

        //            using (var reader = await cmd.ExecuteReaderAsync())
        //            {
        //                // First Resultset → Data
        //                while (await reader.ReadAsync())
        //                {
        //                    appointments.Add(new AppointmentGridDto
        //                    {
        //                        PatientName = reader["PatientName"].ToString()!,
        //                        DoctorName = reader["DoctorName"].ToString()!,
        //                        Date = Convert.ToDateTime(reader["AppointDate"]),
        //                        VisitType = reader["VisitType"].ToString()!,
        //                        Diagnosis = reader["Diagnosis"].ToString()!
        //                    });
        //                }

        //                // Move to second resultset → TotalCount
        //                if (await reader.NextResultAsync() && await reader.ReadAsync())
        //                {
        //                    totalCount = Convert.ToInt32(reader["TotalCount"]);
        //                }
        //            }
        //        }
        //    }

        //    return (appointments, totalCount);
        //}
        public async Task<IActionResult> GetPrescriptionDetails(
        [FromQuery] string? searchInput = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("connection");
                var prescriptionDetails = new List<PrescriptionDetailGridDto>();
                var paginationInfo = new PaginationInfoDto();

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("GetPrescriptionsWithSearch", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SearchInput", (object?)searchInput ?? DBNull.Value);
                        command.Parameters.AddWithValue("@PageNumber", pageNumber);
                        command.Parameters.AddWithValue("@PageSize", pageSize);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var prescriptionDetail = new PrescriptionDetailGridDto
                                {
                                   // AppointmentId = reader.GetInt32("AppointmentId"),
                                    PatientName = reader.GetString("PatientName"),
                                    DoctorName = reader.GetString("DoctorName"),
                                    AppointmentDate = reader.GetDateTime("AppointDate"),
                                    VisitType = reader.GetString("VisitType"),
                                    Diagnosis = reader.IsDBNull("Diagnosis") ? null : reader.GetString("Diagnosis"),
                                    MedicineName = reader.GetString("MedicineName"),
                                    Dosage = reader.GetString("Dosage"),
                                    StartDate = reader.GetDateTime("StartDate"),
                                    EndDate = reader.GetDateTime("EndDate"),
                                    Notes = reader.GetString("Notes"),

                                };

                                // Get pagination info from first row
                                if (prescriptionDetails.Count == 0)
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

                                prescriptionDetails.Add(prescriptionDetail);
                            }
                        }
                    }
                }

                var result = new PrescriptionResultDto
                {
                    Data = prescriptionDetails,
                    Pagination = paginationInfo,
                    Success = true,
                    Message = prescriptionDetails.Any() ? "prescriptions retrieved successfully" : "No prescriptions found"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An error occurred while retrieving prescriptions",
                    Error = ex.Message
                });
            }
        }

        // GET: api/PrescriptionDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PrescriptionDetail>> GetPrescriptionDetail(int id)
        {
            var prescriptionDetail = await _context.PrescriptionDetails.FindAsync(id);

            if (prescriptionDetail == null)
            {
                return NotFound();
            }

            return prescriptionDetail;
        }

        // PUT: api/PrescriptionDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescriptionDetail(int id, PrescriptionDetail prescriptionDetail)
        {
            if (id != prescriptionDetail.PrescriptionDetailId)
            {
                return BadRequest();
            }

            _context.Entry(prescriptionDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrescriptionDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/PrescriptionDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PrescriptionDetail>> PostPrescriptionDetail(PrescriptionDetail prescriptionDetail)
        {
            _context.PrescriptionDetails.Add(prescriptionDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPrescriptionDetail", new { id = prescriptionDetail.PrescriptionDetailId }, prescriptionDetail);
        }

        // DELETE: api/PrescriptionDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescriptionDetail(int id)
        {
            var prescriptionDetail = await _context.PrescriptionDetails.FindAsync(id);
            if (prescriptionDetail == null)
            {
                return NotFound();
            }

            _context.PrescriptionDetails.Remove(prescriptionDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PrescriptionDetailExists(int id)
        {
            return _context.PrescriptionDetails.Any(e => e.PrescriptionDetailId == id);
        }
    }
}
