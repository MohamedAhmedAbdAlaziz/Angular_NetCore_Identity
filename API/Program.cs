using System.Text;
using API.Data;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(x=>
{ 
    x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

});
//be able to inject JWTService class inside our Controller 
builder.Services.AddScoped<JWTService>();
builder.Services.AddIdentityCore<User>(options=>
{
    options.Password.RequiredLength=6;
    options.Password.RequireDigit=false;
    options.Password.RequireLowercase=false;
    options.Password.RequireUppercase=false;
    options.Password.RequireNonAlphanumeric=false;
    options.SignIn.RequireConfirmedEmail=true;
    }).AddRoles<IdentityRole>()// be able to add roles
    .AddRoleManager<RoleManager<IdentityRole>>() // be able to male use of RoleManager
    .AddEntityFrameworkStores<Context>()// provider our context
    .AddSignInManager<SignInManager<User>>() //make use of Signin manager
    .AddUserManager<UserManager<User>>()//make use of UserManager to create users
    .AddDefaultTokenProviders();//be able to create token foe email confirmation 

// be able to authentication users using JWT 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options=> 
{ 
   options.TokenValidationParameters= new TokenValidationParameters
   {
    //validate the token based on the key we have provided inside appsetting.development.json JWT:Key
    ValidateIssuerSigningKey=true,
    //the issuser  siginin key based on jwt :key  
    IssuerSigningKey= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"])),
    //the issuser  which in here is the api project url we are using 
    ValidIssuer=builder.Configuration["JWT:Issuer"],
    //validate the issuser (who ever is isssing the JWT)
    ValidateIssuer=true,
    //dont validate Audience (angular side)
    ValidateAudience= false
   };
    
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
