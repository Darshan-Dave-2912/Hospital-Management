using System.ComponentModel.DataAnnotations;

namespace Admin.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required]
        public String Departmentname { get; set; }
        [Required]
        public String Description { get; set; }        
        public bool IsActive { get; set; }      
        public DateTime Created { get; set; }       
        public DateTime Modified { get; set; }
        public int? UserId { get; set; }
    }
}
