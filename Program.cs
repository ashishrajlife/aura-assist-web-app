using aura_assist_prod.Data;
using aura_assist_prod.Data;
using aura_assist_prod.Middleware;
using aura_assist_prod.Services;
using aura_assist_prod.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
  
    builder.Services.AddAuthorization(options =>
    {
        // Policy for regular users
        options.AddPolicy("UserOnly", policy =>
            policy.RequireRole("User"));

        // Policy for influencers
        options.AddPolicy("InfluencerOnly", policy =>
            policy.RequireRole("Influencer"));

        // Policy for agencies
        options.AddPolicy("AgencyOnly", policy =>
            policy.RequireRole("Agency"));

        // Combined policies
        options.AddPolicy("UserOrInfluencer", policy =>
            policy.RequireRole("User", "Influencer"));

        options.AddPolicy("InfluencerOrAgency", policy =>
            policy.RequireRole("Influencer", "Agency"));

        options.AddPolicy("AllRoles", policy =>
            policy.RequireRole("User", "Influencer", "Agency"));
    });

// Add CORS (for Angular frontend)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAngular");
app.UseMiddleware<RoleLoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();