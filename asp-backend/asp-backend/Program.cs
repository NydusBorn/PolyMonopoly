using asp_backend;
using asp_backend.Contexts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication("Auth")
    .AddScheme<AuthenticationSchemeOptions, Auth>("Auth", options => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    // Fine for in development, removes db to stay in compliance with current schema
    await Statics._userContext.Database.EnsureDeletedAsync();
    await Statics._userContext.Database.EnsureCreatedAsync();
}
else
{
    // If modification of db is required in production, then use the Migrate method (you must consult ef core documentation on migrations,
    // and also, if you started with EnsureCreated, then you must roll your own migrations manually (first create a new db via migrations (that fits the schema), then use your code to migrate db to db))
    await Statics._userContext.Database.MigrateAsync();
}

//TODO: Guest removal subroutine

await app.RunAsync();