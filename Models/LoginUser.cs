#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace loginRegister.Models;

public class LoginUser
{
    [Required]
    [EmailAddress]
    public string Email {get;set;}

    [Required]
    [MinLength(8)]
    [DataType(DataType.Password)]
    public string Password {get;set;}
}