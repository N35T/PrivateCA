# PrivateCA
A software suite, used to manage our private certification authority for SSL certificates in our local network

To get started issuing your own certificates, there are only a few steps:
- [DNS Setup](#dns-setup)
- [Server Setup](#server-setup)
- [Creating your CA](#creating-your-ca)
- [Issuing Certificates](#issuing-certificates)
- [Trusting the Certificates](#trusting-the-certificates)

## DNS Setup
In order to have custom domain names without having to access a public domain, you will have to setup your own DNS server.

This is possible with a [Pi-hole](https://github.com/pi-hole/pi-hole) for example. The Pi-hole gives you the extra advantage of also having network-wide ad blocking.

It can be installed on a Linux hardware with the following one line command:

```bash
curl -sSL https://install.pi-hole.net | bash
```
From there, you will be guided through the installation process of the Pi-hole.

Once the installation is done you will need to login on your router first and configure the Pi-Hole as the preferred DNS-Server. On one side for the internet access: <br>
Here you have to enter the IP-address of your Pi-Hole (both IPv4 and IPv6).

![Tab for configuring DNS-Server for the internet access on the router](assets/screenshots/dns_config_router1.png)

Similar goes for the IP-address-configurations for your home network, so other devices know where to ask for resolving your local domains: 

![Tab for configuring DNS-Server in the home-network on the router](assets/screenshots/dns_config_router2.png)

### IPv4

![Tab for configuring the IPv4 local DNS-Server](assets/screenshots/dns_config_ipv4.png)

### IPv6

![Tab for configuring the IPv6 local DNS-Server](assets/screenshots/dns_config_ipv6.png)

> **Note:** There are different router types, with different interfaces. In this example we will be using a Fritz-Box. If you have a different router, the configurations might be located differently.

After configuring your router you can go to the web-interface of your Pi-hole and add your local domain records. Simply go to `Local DNS` and `DNS Records` on the side nav. Here you can add your local domain and link it to the IP-address of your local server.

![Local DNS records page of the Pi-Hole](assets/screenshots/local_dns_records_pihole.png)

Now when you enter the url of your local domain the Pi-Hole connects you with your local server. 

## Server Setup
HTTP and HTTPS communicates on port 80 and 443 by default. This means that regularly, only a single process can on the machine can serve a website on one of those ports.

Domains can only be pointed to an ip address and default to port 80 when `http://` or port 443 when `https://` is used. This rules out using different ports to expose different services!

To solve this problem you can use a simple proxy like [NGINX](https://docs.nginx.com/nginx/admin-guide/web-server/). This listens to requests on port 80/443, looks at the incoming domain of the request and routes it to the corresponding port of the service.

Example: Service1 running on port 5001, Service2 running on port 5002

Create the file `/etc/nginx/sites-enabled/service1.yourdomain.local.conf`
```bash
server {
    server_name service1.yourdomain.local;
    location / {
            proxy_set_header   X-Real-IP $remote_addr;
            proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_pass         http://127.0.0.1:5001/;
            proxy_http_version 1.1;
            proxy_set_header   Upgrade $http_upgrade;
            proxy_set_header   Connection "upgrade";
    }

    listen 80;
}
```

Create the file `/etc/nginx/sites-enabled/service2.yourdomain.local.conf`
```bash
server {
    server_name service2.yourdomain.local;
    location / {
            proxy_set_header   X-Real-IP $remote_addr;
            proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_pass         http://127.0.0.1:5002/;
            proxy_http_version 1.1;
            proxy_set_header   Upgrade $http_upgrade;
            proxy_set_header   Connection "upgrade";
    }

    listen 80;
}
```

Now run `nginx -s reload` in your bash!

Both services should now be accessible through their domains!

## Creating your CA
tbd

## Issuing Certificates
tbd

## Trusting the Certificates

To continue, get your `example.ca.crt` file from your Certification Authority.

### Windows
To install the certificate on windows, right click the `.crt` file and choose `Install Certificate`.

When prompted for the Certification Store, choose `Place all certificates in the following store`.
Choose `Trusted Root Certification Authorities store` for the certificate.

You can learn more about this process [here](https://learn.microsoft.com/en-us/skype-sdk/sdn/articles/installing-the-trusted-root-certificate)!

Restart your browser and enjoy your SSL.

### Linux

Copy your certificate in the `ca-certificates` folder with
```bash
sudo cp example.ca.crt /usr/local/share/ca-certificates
```

And then update your ca certs
```bash
# IF not installed:
sudo apt-get install -y ca-certificates


sudo update-ca-certificates
```

See [here](https://ubuntu.com/server/docs/security-trust-store) for more information