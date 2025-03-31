using CityInfo.API;
using CityInfo.API.DbContexts;
using CityInfo.API.Interface_Repo;
using CityInfo.API.Services;
using EmailService;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

//Configure serilog -  Log is defined in the Serilog namespace
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()   //The first thing we want to do is set the minimum log level to Debug
    .WriteTo.Console()      //we state where we want to log to, Console and to a File.
    .WriteTo.File("AppError/cityinfo.txt", rollingInterval: RollingInterval.Day) //the location of the file and the RollingInterval, in other words, how often a new file should be created with logs
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

//as we are using serilog now, commenting old approach
//builder.Logging.AddConsole(); //adding console logging

builder.Host.UseSerilog(); //adding serilog to the host


// Add services to the container.


builder.Services.Configure<EmailConfiguration>(builder.Configuration.GetSection("EmailConfiguration"));
builder.Services.AddSingleton<EmailConfiguration>();

//We've told the container that whenever we inject an IMailService in our code, we want it to provide us with an instance of EmailSender
builder.Services.AddScoped<IEmailSender, EmailSender>();

builder.Services.AddSingleton<CitiesDataStore>();

builder.Services.AddDbContext<CityInfoContext>( );

//Registering the repository
builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();



builder.Services.AddControllers(options =>
{
    //when api asks for certain representation and API not support it, then we return status code stating we didn't support it, so we use below option
    options.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson()
.AddXmlDataContractSerializerFormatters();


//registering the ProblemDetails services 
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions.Add("additionalInfo", "Additional Info exmaple");
        ctx.ProblemDetails.Extensions.Add("Server", Environment.MachineName);
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//registering the file extension content type provider
builder.Services.AddSingleton<FileExtensionContentTypeProvider>();
builder.Services.AddTransient<LocalMailService>();
var app = builder.Build();

// Configure the HTTP request pipeline.

// Exceptions occurring in other pieces of middleware that are added after the ExceptionHandler middleware can be handled by it as well
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


//Printing hello world to the screen
//app.Run(context => context.Response.WriteAsync("Hello, world!"));

app.Run();
