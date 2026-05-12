using System.Text;
using Inventory.Api.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

LoadDotEnv();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
var baseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is required.");
var connectionString = ResolveConnectionString(baseConnectionString);
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
        var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is required.");
        var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Jwt:Audience is required.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.OpenConnection();
        dbContext.Database.CloseConnection();
    }
    catch (NpgsqlException ex)
    {
        throw new InvalidOperationException("Could not connect to PostgreSQL with configured connection settings.", ex);
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred.",
            traceId = context.TraceIdentifier
        });
    });
});

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

static string ResolveConnectionString(string baseConnectionString)
{
    var fullConnectionStringFromEnv = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
    if (!string.IsNullOrWhiteSpace(fullConnectionStringFromEnv))
    {
        return fullConnectionStringFromEnv;
    }

    var passwordFromEnv = Environment.GetEnvironmentVariable("DB_PASSWORD");
    if (string.IsNullOrWhiteSpace(passwordFromEnv))
    {
        return baseConnectionString;
    }

    var builder = new NpgsqlConnectionStringBuilder(baseConnectionString)
    {
        Password = passwordFromEnv
    };

    return builder.ConnectionString;
}

static void LoadDotEnv()
{
    var currentDirectory = Directory.GetCurrentDirectory();
    var parentDirectory = Directory.GetParent(currentDirectory)?.FullName;

    LoadDotEnvFile(Path.Combine(currentDirectory, ".env"));
    if (!string.IsNullOrWhiteSpace(parentDirectory))
    {
        LoadDotEnvFile(Path.Combine(parentDirectory, ".env"));
    }
}

static void LoadDotEnvFile(string path)
{
    if (!File.Exists(path))
    {
        return;
    }

    foreach (var rawLine in File.ReadAllLines(path))
    {
        var line = rawLine.Trim();
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#", StringComparison.Ordinal))
        {
            continue;
        }

        var separatorIndex = line.IndexOf('=');
        if (separatorIndex <= 0)
        {
            continue;
        }

        var key = line[..separatorIndex].Trim();
        var value = line[(separatorIndex + 1)..].Trim();

        if (value.Length >= 2 &&
            ((value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal)) ||
             (value.StartsWith("'", StringComparison.Ordinal) && value.EndsWith("'", StringComparison.Ordinal))))
        {
            value = value[1..^1];
        }

        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(key)))
        {
            Environment.SetEnvironmentVariable(key, value);
        }
    }
}
