using System.ComponentModel.DataAnnotations;

namespace FreeCourse.Web.Models;

public class SigninInput
{
    [Required]
    [Display(Name="Email")]
    public string Email { get; set; }
    
    [Required]
    [Display(Name="Password")]
    public string Password { get; set; }
    
    [Required]
    [Display(Name="Remember me")]
    public bool RememberMe { get; set; }
    
}