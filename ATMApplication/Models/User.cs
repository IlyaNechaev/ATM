using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public Guid Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string FirstName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string MiddleName { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "int")]
        [Required]
        public int Age { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Login { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        [Required]
        public string Email { get; set; }

        [Column("Password", TypeName = "nvarchar(512)")]
        public string PasswordHash { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; }

        public List<Card> Cards { get; set; }
    }

    [Flags]
    public enum Gender
    {
        MALE,
        FEMALE
    }
}
