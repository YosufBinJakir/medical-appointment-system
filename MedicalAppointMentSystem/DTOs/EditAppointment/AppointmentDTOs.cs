namespace MedicalAppointMentSystem.DTOs.EditAppointment
{
    public class AppointmentDto
    {
        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }
        public string? DoctorName { get; set; } = default!;
        public int PatientId { get; set; }
        public string? PatientName { get; set; } = default!;
        public string? VisitType { get; set; }
        public string? Notes { get; set; }
        public string? Diagnosis { get; set; }
        public DateTime AppointDate { get; set; }
        public List<PrescriptionDetailDto> PrescriptionDetails { get; set; } = new();
    }

    public class PrescriptionDetailDto
    {
        public int PrescriptionDetailId { get; set; }
        public int? MedicineId { get; set; }
        public string? MedicineName { get; set; }
        public string? Dosage { get; set; }
        public string? Notes { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

}
