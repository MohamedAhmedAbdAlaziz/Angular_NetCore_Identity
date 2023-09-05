using System.Security.Claims;
using API.DTOs.Account;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly JWTService _jwtService;
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;

    public AccountController(JWTService jwtService,
     SignInManager<User> signInManager,
    UserManager<User> userManager
    )
    {
        _jwtService = jwtService;
        _signInManager = signInManager;
        _userManager = userManager;
    }
    [Authorize]
    [HttpGet("refresh-user-token")]
    public async Task<ActionResult<UserDto>> RefreshUserToken(){
 var user= await _userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
   return CreateApplicationUserDto(user);

    }
   [HttpPost("login")]
   public async Task<ActionResult<UserDto>> Login(LoginDto model){
    var user= await _userManager.FindByNameAsync(model.UserName);
    if(user== null){
        return Unauthorized("Invalid username or password");
    }
    if(user.EmailConfirmed==false){
         return Unauthorized("please confirm you email");
    }
    var result =await _signInManager.CheckPasswordSignInAsync(user,model.Password ,false);
    if(!result.Succeeded)
    return Unauthorized("Invalid username or password");
  
  return CreateApplicationUserDto(user);
  
   }
   [HttpPost("register")]
    public async Task<ActionResult<UserDto>> register(RegisterDto model){
    
    if(await CheckExistsAsync(model.Email)){
        return BadRequest($"An existes account is using {model.Email}, email address, Please try with anouther email address");
    }
        var userToAdd = new User 
    {
          FirstName= model.FirstName.ToLower(),
        LastName= model.LastName.ToLower(),
        UserName= model.Email.ToLower(),
          Email= model.Email.ToLower(),
          EmailConfirmed=true
    }  ;
    var result= await _userManager.CreateAsync(userToAdd,model.Password);
    if(!result.Succeeded)
    {
        return BadRequest(result.Errors);
    }
    return Ok("your account has been created, you can login");
  
   }


   #region 
   private UserDto CreateApplicationUserDto(User user){
    return new UserDto{
       FirstName=user.FirstName,
       LastName= user.LastName,
       JWT= _jwtService.CreateJWT(user)
    };
   }
   private async Task<bool> CheckExistsAsync(string email){
    return await _userManager.Users.AnyAsync(x=>x.Email==email.ToLower());
    
       }
   #endregion

}