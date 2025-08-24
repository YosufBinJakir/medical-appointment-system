namespace MedicalAppointMentSystem.Entities
{
    public partial class Doctor : BaseEntity
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = default!;


    }
}
