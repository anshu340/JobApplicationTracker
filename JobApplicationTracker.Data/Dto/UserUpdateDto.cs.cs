using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JobApplicationTracker.Data.Dto
{


    public class UserUpdateDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Location { get; set; }

        public int? UserType { get; set; }


    }

}
