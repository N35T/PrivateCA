using PrivateCA.Core.Commands;

namespace PrivateCA.Core;

public static class NginX {

    private const string NginXPath = "/etc/nginx/sites-enabled/";

    public static void RegisterDomain(string domain, int hostPort, string sslCertPath, string sslPrivKeyPath, string dhParamsPath) {
        var nginxConfFile = $$"""
        server {
            server_name {{domain}};
            location / {
                proxy_set_header   X-Real-IP $remote_addr;
                proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
                proxy_pass         http://127.0.0.1:{{hostPort}}/;
                proxy_http_version 1.1;
                proxy_set_header   Upgrade $http_upgrade;
                proxy_set_header   Connection "upgrade";
            }

            listen 443 ssl;
            ssl_certificate {{sslCertPath}};
            ssl_certificate_key {{sslPrivKeyPath}};
            ssl_dhparam {{dhParamsPath}};
        }

        server {
            if ($host = {{domain}}) {
                return 301 https://$host$request_uri;
            }
            
            listen 80;
            server_name {{domain}};

            return 404;
        }
        """;

        File.WriteAllText(Path.Combine(NginXPath, domain), nginxConfFile);

        "sudo nginx -s reload".Bash();
    }

}
