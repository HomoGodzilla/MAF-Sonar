 
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MAF.Adapters.Output.Firebase;
using MAF.Core.Application;
using MAF.Ports.Input;
using MAF.Ports.Output;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MAF.Adapters.Input.WebServer.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<ITreeOutput, FirebaseAdapter>();

builder.Services.AddScoped<ITreeInput, TreeApplication>();

builder.Services.AddControllers();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});


//Autenticação
var key = Encoding.ASCII.GetBytes(Key.SecretKey);

builder.Services.AddAuthentication(x => {

        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;    
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;    
    }

).AddJwtBearer( x=>

    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();


//Config swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen( x =>
{
    x.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        Name = "Authorization", 
        Type = SecuritySchemeType.ApiKey, 
        Scheme = "Bearer", 
        BearerFormat = "JWT", 
        In = ParameterLocation.Header
    });

    x.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {

        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MAF_Api");
        options.RoutePrefix = "swagger";

    });
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();