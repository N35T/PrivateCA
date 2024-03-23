using PrivateCA.Core.CA;
using PrivateCA.Core.DTOs;

namespace PrivateCA.API;
public class LocalCAApi : IPrivateCAApi {
    public LocalCAApi() { }

    private string Issuer;
    private string Password;

    private CA privateCA { get; }

    public LocalCAApi(string issuer, string password) {
        Issuer = issuer;
        Password = password;
        privateCA = new CA("./caWorker/", issuer, password);
    }

    public Task<CsrResponseDTO> SignCsrAsync(CsrDTO data) {
        privateCA.CheckIfCaExists(Issuer, Password);
        return privateCA.Sign(data);
    }
}
