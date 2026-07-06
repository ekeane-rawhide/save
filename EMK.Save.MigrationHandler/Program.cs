using EMK.Save.PL.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add service to project
builder.Services.AddDbContext<SaveEntities>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SaveConnection"),
    builder =>
    {
        builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
    }
));

var app = builder.Build();

app.Run();
