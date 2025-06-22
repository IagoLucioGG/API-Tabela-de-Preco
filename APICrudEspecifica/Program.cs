using APICrudEspecifica.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Serviço de limpeza diária
builder.Services.AddHostedService<CleanupHistoricoService>();

// Autorização baseada em permissões
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Consultar", policy =>
        policy.Requirements.Add(new PermissionRequirement("CONSULTAR")));

    options.AddPolicy("Cadastrar", policy =>
        policy.Requirements.Add(new PermissionRequirement("CADASTRAR")));

    options.AddPolicy("Editar", policy =>
        policy.Requirements.Add(new PermissionRequirement("EDITAR")));

    options.AddPolicy("Deletar", policy =>
        policy.Requirements.Add(new PermissionRequirement("DELETAR")));
});




// Adiciona suporte para acessar o HttpContext no handler
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

// Configura o DbContext com SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configura autenticação JWT
var key = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Ajuste conforme necessário
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
    };

    options.Events = new JwtBearerEvents
    {
        OnChallenge = context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";

            var json = System.Text.Json.JsonSerializer.Serialize(new
            {
                mensagem = "Token inválido ou não fornecido."
            });

            return context.Response.WriteAsync(json);
        }
    };
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
