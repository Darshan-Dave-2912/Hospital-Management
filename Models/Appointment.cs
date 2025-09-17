using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Appointment
    {
        public int AppointmentId {  get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public DateTime? AppointmentDate { get; set; }
        [Required]
        public String AppointmentStatus { get; set; }
        [Required]
        public String Description { get; set; }
        [Required]
        public String SpecialRemarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
        [Required]
        public int TotalConsultedAmount { get; set; }
    }

    public class DoctorDropDownModel
    {
        public int DoctorID { get; set; }
        public string name { get; set; }
    }

    public class PatientDropDownModel
    {
        public int PatientID { get; set; }
        public string Name { get; set; }
    }
}
