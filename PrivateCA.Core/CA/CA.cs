using PrivateCA.Core.DTOs;

namespace PrivateCA.Core.CA;
public class CA
{

    private readonly string Workerpath;
    private readonly string CaPath;
    private readonly string CaName;
    public CA(string workerpath, string caPath, string caName)
    {
        Workerpath = workerpath;
        CaPath = caPath;
        CaName = caName;
    }
    private async Task<string> GetCertContentAsync(CsrDTO data, string workingOn)
    {
        var csrPath = Path.Combine(workingOn, "csrContent");
        var extPath = Path.Combine(workingOn, "extContent");

        await File.WriteAllTextAsync(csrPath, data.CsrContent);
        await File.WriteAllTextAsync(extPath, data.ExtContent);

        var outPath = Path.Combine(workingOn, "certContent");

        OpenSSL.OpenSSL.SignCSRWithCAKey(csrPath, extPath, outPath, CaPath, CaName, data.Password);

        var certContent = await File.ReadAllTextAsync(outPath);
        return certContent;
    }

    public void CheckIfCaExists(string issuer, string password)
    {
        var caKeyName = OpenSSL.OpenSSL.GetCAKeyName(CaName, CaPath);
        var caCertName = OpenSSL.OpenSSL.GetCACertName(CaName, CaPath);

        if (!Path.Exists(caKeyName))
        {
            OpenSSL.OpenSSL.GenerateCAPrivateKey(CaName, CaPath, password);
        }

        if (!Path.Exists(caCertName))
        {
            OpenSSL.OpenSSL.GenerateRootCertificate(CaName, CaPath, issuer, password);
        }
    }

    public async Task<CsrResponseDTO> Sign(CsrDTO data)
    {
        var guid = Guid.NewGuid().ToString();
        var workingOn = Path.Combine(Workerpath, guid);
        var certContent = "";

        Directory.CreateDirectory(workingOn);

        try
        {
            certContent = await GetCertContentAsync(data, workingOn);
        }
        finally
        {
            Directory.Delete(workingOn, true);
        }

        return new CsrResponseDTO(certContent);
    }
}