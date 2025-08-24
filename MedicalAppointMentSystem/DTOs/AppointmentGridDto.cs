using MedicalAppointMentSystem.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.DTOs
{
    public class AppointmentGridDto
    {
       public int AppointmentId { get; set; }
        public string? DoctorName { get; set; } 
        public string? PatientName { get; set; }
        public string? VisitType { get; set; }
        public string? Diagnosis { get; set; } = default!;
        public DateTime AppointDate { get; set; }
        //public ICollection<PrescriptionDetail>? PrescriptionDetails { get; set; } = default!;
    }
}
