using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagementGraphQL.Data.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Rate { get; set; }

        [Required]
        public string Comment { get; set; }


        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        [Required]
        public Employee Employee { get; set; }
    }
}
