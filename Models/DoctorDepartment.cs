using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class DoctorDepartment
    {
        public int? DoctorDepartmentId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public int DepartmentId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public int UserID { get; set; }
    }

    public class DoctorDropDown
    {
        public int DoctorID { get; set; }
        public string name { get; set; }
    }

    public class DepartmentDropDown
    {
        public int DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }
}
