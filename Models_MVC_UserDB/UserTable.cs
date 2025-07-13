using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserDB
{
    public partial class UserTable
    {
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "UserName must be between 1 and 100 characters")]
        public string? UserName { get; set; }
        
        [StringLength(1, ErrorMessage = "UserSex must be 1 character")]
        public string? UserSex { get; set; }
        
        public DateTime? UserBirthDay { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string? UserMobilePhone { get; set; }
    }
}
