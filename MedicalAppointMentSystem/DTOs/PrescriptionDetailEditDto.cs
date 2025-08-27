namespace MedicalAppointMentSystem.DTOs
{
    public class PrescriptionDetailEditDto
    {
      
        public int? MedicineId { get; set; }
        public string? Dosage { get; set; }
        public string? Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
