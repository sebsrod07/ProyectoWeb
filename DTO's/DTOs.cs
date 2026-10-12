using System.ComponentModel.DataAnnotations;

namespace DTOs;
public class AccountDTO
{
    [Required] 
    public string Email {get;set;} = null!;
    [Required]
    public string Password {get;set;} = null!;
}
public class RoleAccountDTO
{
    [Required] 
    public string Email {get;set;} = null!;
    [Required]
    public string Password {get;set;} = null!;
    [Required]
    public string Role{get;set;} = null!;
}

