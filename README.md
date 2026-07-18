# EnoLandingPage

## docker-compose
```yaml
version: '3'

services:
  enolandingpage:
    build: .
    environment:
      - "EnoLandingPage__Title=FoobarCTF"
      - "EnoLandingPage__StartTime=2020-11-22T15:00:00Z"
      - "EnoLandingPage__RegistrationCloseOffset=48"
      - "EnoLandingPage__CheckInBeginOffset=12"
      - "EnoLandingPage__CheckInEndOffset=2"
      - "EnoLandingPage__HetznerVulnboxType=cx11"
      - "EnoLandingPage__HetznerCloudApiToken=..."
      - "EnoLandingPage__HetznerVulnboxImage=..."
      - "EnoLandingPage__HetznerVulnboxPubkey=..."
      - "EnoLandingPage__HetznerVulnboxLocation=fsn1 nbg1 hel1"
      - "EnoLandingPage__HetznerVulnboxLocationMapPath=/app/vulnboxes.json"
      - "EnoLandingPage__OAuthClientId=..."
      - "EnoLandingPage__OAuthClientSecret=..."
      - "EnoLandingPage__AdminSecret=..." 
    ports:
      - "5001:80"
    volumes:
      - ./sessions:/root/.aspnet/DataProtection-Keys
      - ./data:/app/data
      - ./scoreboard:/app/wwwroot/scoreboard
```

## VM Placement
If you want to place VMs in specific Hetzner data centers, creating a mapping in `vulnboxes.json` and adjust the `HetznerVulnboxLocationMapPath` accordingly. 
The team-IDs in there are 0-indexed, so to map team1's VM, use `"0": "hel1"` in the JSON. If a key is not found, `HetznerVulnboxLocation` is used as the default.

`HetznerVulnboxLocation` may hold several locations, separated by spaces or commas (e.g. `fsn1 nbg1 hel1`).
They are tried in order: if Hetzner reports that a location cannot place the VM - because it is out of
capacity for the configured `HetznerVulnboxType` - the next one is attempted. Other failures (invalid API
token, exceeded quota, unknown image, ...) fail immediately without trying the remaining locations, since
they would fail the same way everywhere.

Teams pinned via `vulnboxes.json` always use exactly that one location and do **not** fall back.

## Reverse Proxy Configuration
The reverse proxy must set the [XFP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto) and allow https connections.

## Development Configuration
When you want to run the page in development mode (most notably without https), add the following environment variable:
```
"ASPNETCORE_ENVIRONMENT=Development"
```
