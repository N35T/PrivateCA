using Microsoft.AspNetCore.Mvc;
using PrivateCA.Core.CA;
using PrivateCA.Core.DTOs;
using PrivateCA.Core.OpenSSL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

var workerpath = app.Configuration["workerpath"]!;
var capath = app.Configuration["capath"]!;
var caName = app.Configuration["CaName"]!;
var privateCA = new CA(workerpath, capath, caName);

app.UseHttpsRedirection();

app.MapPost("/signcsr", async ([FromBody]CsrDTO data) => {
    privateCA.CheckIfCaExists(app.Configuration["Issuer"]!, app.Configuration["Password"]!);
    return await privateCA.Sign(data);
});

app.MapGet("/pubkey", async () => {
    privateCA.CheckIfCaExists(app.Configuration["Issuer"]!, app.Configuration["Password"]!);
    var pubkey = await File.ReadAllTextAsync(OpenSSL.GetCACertName(caName, capath));
    return pubkey;
});

app.Run();