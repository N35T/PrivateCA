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

//async Task<string> GetCertContentAsync(CsrDTO data, string workingOn) {
//    var csrPath = Path.Combine(workingOn, "csrContent");
//    var extPath = Path.Combine(workingOn, "extContent");

//    await File.WriteAllTextAsync(csrPath, data.CsrContent);
//    await File.WriteAllTextAsync(extPath, data.ExtContent);

//    var outPath = Path.Combine(workingOn, "certContent");

//    OpenSSL.SignCSRWithCAKey(csrPath, extPath, outPath, caPath, caName, data.Password);

//    var certContent = await File.ReadAllTextAsync(outPath);
//    return certContent;
//}

//void CheckIfCaExists() {
//    var caKeyName = OpenSSL.GetCAKeyName(caName, caPath);
//    var caCertName = OpenSSL.GetCACertName(caName, caPath);
//    var password = app.Configuration["Password"]!;

//    if (!Path.Exists(caKeyName)) {
//        OpenSSL.GenerateCAPrivateKey(caName, caPath, password);
//    }

//    if (!Path.Exists(caCertName)) {
//        OpenSSL.GenerateRootCertificate(caName, caPath, app.Configuration["Issuer"]!, password);
//    }
//}

app.UseHttpsRedirection();

app.MapPost("/signcsr", async ([FromBody]CsrDTO data) => {
    //CheckIfCaExists();

    //var guid = Guid.NewGuid().ToString();
    //var workingOn = Path.Combine(workerpath, guid);
    //var certContent = "";

    //Directory.CreateDirectory(workingOn);

    //try {
    //    certContent = await GetCertContentAsync(data, workingOn);
    //} finally {
    //    Directory.Delete(workingOn, true);
    //}

    //return new CsrResponseDTO(certContent);
    privateCA.CheckIfCaExists(app.Configuration["Issuer"]!, app.Configuration["Password"]!);
    return privateCA.Sign(data);
});

app.MapGet("/pubkey", async () => {
    privateCA.CheckIfCaExists(app.Configuration["Issuer"]!, app.Configuration["Password"]!);
    var pubkey = await File.ReadAllTextAsync(OpenSSL.GetCACertName(caName, capath));
    return pubkey;
});

app.Run();