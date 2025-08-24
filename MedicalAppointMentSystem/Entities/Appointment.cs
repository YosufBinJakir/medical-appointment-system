using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.Entities
{
    public partial class Appointment : BaseEntity
    {
        [Key]
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
        public string? VisitType { get; set; } = default!;
        public string? Notes { get; set; } = default!;
        public string? Diagnosis { get; set; } = default!;
        public DateTime AppointDate { get; set; }
        public ICollection<PrescriptionDetail>? PrescriptionDetails { get; set; } =default!;


    }
}
