using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Patient
    {
        public int PatientId { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public String Gender { get; set; }
        [Required]
        public String Email { get; set; }
        [Required]
        public String Phone { get; set; }
        [Required]
        public String Address { get; set; }
        [Required]
        public String City { get; set; }
        [Required]
        public String State { get; set; }
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
    }
}
