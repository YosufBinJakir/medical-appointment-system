using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicalAppointMentSystem.Entities
{
    public partial class Medicine : BaseEntity
    {
        public int MedicineId { get; set; }
        public string MedicineName { get; set; } = default!;
    }
}
