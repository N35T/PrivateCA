using System.Net.Http.Json;
using PrivateCA.Core.CA;
using PrivateCA.Core.DTOs;

namespace PrivateCA.Client;

public class PrivateCAApi : IPrivateCAApi {

    private const string CaLocation = "https://peter.n35t.local/signcsr";

    private readonly HttpClient _httpClient;

    public PrivateCAApi() {
        _httpClient = new HttpClient {
            BaseAddress = new Uri(CaLocation)
        };
    }

    public async Task<CsrResponseDTO> SignCsrAsync(CsrDTO data) {
        var response = await _httpClient.PostAsJsonAsync("",data);
        response.EnsureSuccessStatusCode();

        var csrResponse = await response.Content.ReadFromJsonAsync<CsrResponseDTO>();
        return csrResponse ?? throw new ApplicationException("Csr Response was empty");
    }

}
