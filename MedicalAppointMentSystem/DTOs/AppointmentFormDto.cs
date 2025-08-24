using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.DTOs
{
    public class AppointmentFormDto
    {
        public int? PatientId { get; set; } = default!;
        public int? DoctorId { get; set; } 
        public DateTime? AppointmentDate { get; set; }
        public string? VisitType { get; set; } =default!;
        public string? Notes { get; set; } = default!;
        public string? Diagnosis { get; set; } =default!;
    }
}
