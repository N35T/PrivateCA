namespace PrivateCA.Client;

public record CsrDTO(string CsrContent, string ExtContent, string Password) {}

public record CsrResponseDTO(string CertContent) {}
