using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.DTOs
{
    public class PrescriptionDetailGridDto
    {
        public string? MedicineName { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? Dosage { get; set; }
        public string? VisitType { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }
    }
}
