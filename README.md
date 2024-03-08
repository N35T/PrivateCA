# PrivateCA
A software suite, used to manage our private certification authority for SSL certificates in our local network

To get started issuing your own certificates, there are only a few steps:
- [DNS Setup](#dns-setup)
- [Server Setup](#server-setup)
- [Creating your CA](#creating-your-ca)
- [Issuing Certificates](#issuing-certificates)
- [Trusting the Certificates](#trusting-the-certificates)

## DNS Setup
tbd PIHOLE

## Server Setup
HTTP and HTTPS communicates on port 80 and 443 by default. This means that regulary, only a single process can on the machine can serve a website on one of those ports.

Domains can only be pointed to an ip adress and default to port 80 when `http://` or port 443 when `https://` is used. This rules out using different ports to expose different services!

To solve this problem you can use a simple webserver like [NGINX](https://docs.nginx.com/nginx/admin-guide/web-server/). This listens to requests on port 80/443, looks at the incoming domain of the request and routes it to the corresponding port of the service.

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
tbd
