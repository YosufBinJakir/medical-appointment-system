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
    public class AppointmentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AppointmentsController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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
                                };

                                // Get pagination info from first row
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
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
    }
}
