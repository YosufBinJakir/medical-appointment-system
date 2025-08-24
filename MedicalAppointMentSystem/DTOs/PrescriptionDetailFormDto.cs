using MedicalAppointMentSystem.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.DTOs
{
    public class PrescriptionDetailFormDto
    {
 
        public int? MedicineId { get; set; } = default!;
        public string? Dosage { get; set; } = default!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Notes { get; set; }

    }
}
