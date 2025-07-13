using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2022_Core8_WebApi_JWT.Models_MVC_UserLogin
{
    public partial class DbUser
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "UserName is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "UserName must be between 1 and 50 characters")]
        public string UserName { get; set; } = null!;
        
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Password must be between 3 and 100 characters")]
        public string UserPassword { get; set; } = null!;
        
        public int? UserRank { get; set; }
        
        [StringLength(1, ErrorMessage = "UserApproved must be 1 character")]
        public string? UserApproved { get; set; }
    }
}
