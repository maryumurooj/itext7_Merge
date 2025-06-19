using PdfMergeCompany.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register PDF merge service
builder.Services.AddScoped<IPdfMergeService, PdfMergeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseStaticFiles(); // Enable serving static files from wwwroot
app.UseAuthorization();
app.MapControllers();

app.Run();