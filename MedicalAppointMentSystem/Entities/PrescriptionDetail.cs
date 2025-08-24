using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.Entities
{
    public partial class PrescriptionDetail : BaseEntity
    {
        [Key]
        public int PrescriptionDetailId { get; set; }
        public int? MedicineId { get; set; }
        public string? Dosage { get; set; } = default!;
        public string? Notes { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? AppointmentId { get; set; }
        public Appointment? Appointment { get; set; } = default!;
    }
}
