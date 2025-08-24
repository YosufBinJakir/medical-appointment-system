namespace MedicalAppointMentSystem.DTOs
{
    public class AppointmentResultDto
    {
        public List<AppointmentGridDto> Data { get; set; } = new();
        public PaginationInfoDto Pagination { get; set; } = new();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
