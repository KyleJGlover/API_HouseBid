var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Adds option to add middleware 

//app.UseHttpsRedirection();
// Authentication/Authorization

app.UseAuthorization();
// For routing
app.MapControllers();

app.Run();
