using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Doctor
    {
        public int? DoctorId {  get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Phone { get; set; }
        [Required]
        public String Email { get; set; }
        [Required]
        public String Qualification { get; set; }
        [Required]
        public String Specialization { get; set; }
        public Boolean IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
    }

}
