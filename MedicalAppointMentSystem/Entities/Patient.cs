using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.Entities
{
    public partial class Patient : BaseEntity
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; } = default!;
        public string? Email { get; set; } = default!;

    }
}
