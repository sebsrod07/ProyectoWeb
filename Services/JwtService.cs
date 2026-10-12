using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
namespace Services;
public class JwtService
{
    private readonly string secretKey;
    private readonly string issuer;
    private readonly string audience;
    private readonly UserManager<IdentityUser> userManager;
    public JwtService(IConfiguration config,UserManager<IdentityUser> UserManager)
    {
        secretKey=config["jwt:secretKey"]??throw new Exception("No existe SecretKey");
        issuer=config["jwt:issuer"]??throw new Exception("No Existe Issuer");
        audience=config["jwt:audience"]??throw new Exception("No existe Audience");
        this.userManager=UserManager;
    }
    public async Task<string> createJwt(IdentityUser user)
    {
        var roles=await userManager.GetRolesAsync(user);
        var userClaims=await userManager.GetClaimsAsync(user);
        var key=new SymmetricSecurityKey (Encoding.UTF8.GetBytes(secretKey));
        var credentials=new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
        var claims =new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub,user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        };
        foreach(var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role,role));
        }
        foreach(Claim claim in userClaims)
        {
            claims.Add(claim); 
            
        }
        var token=new JwtSecurityToken(
            issuer:this.issuer,
            audience:audience,
            claims:claims,
            expires:DateTime.Now.AddHours(1),
            signingCredentials:credentials
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}