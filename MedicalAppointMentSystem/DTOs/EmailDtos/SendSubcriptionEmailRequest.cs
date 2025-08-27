namespace MedicalAppointMentSystem.DTOs.EmailDtos
{
    public class SendPrescriptionEmailRequest
    {
        public int AppointmentId { get; set; }
        public string PatientEmail { get; set; }
        public string Subject { get; set; } = "Your Prescription";
        public string Body { get; set; } = "Please find your prescription attached.";
    }

}
