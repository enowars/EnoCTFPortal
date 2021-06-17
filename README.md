# EnoPortal

This is the reusable landing page for any CTF. It includes the Scoreboard, as well as registration, Vulnbox and Network Access utilities for each team.

## Customization

For Better reusability the Frontent makes use of an IFrame Front Page.

You can also rebrand the outer frame by including CSS and supplying some Environment variables but the Main Page can be completley custom made.

This way you can use your own framework for the landing page because it is completly seperate.

## Hosting

There are two options to host the Portal, the `Live` (default) version should be used during an event. The `Archive` (static) version should be used to make the results available after the CTF is over.

### Live (for running a CTF)

```bash
# Set the Variables for you CTF according to the example.env
cp example.env .env
nano .env
# Start the Landing Page
docker-compose up -d
```

> **Reverse Proxy Configuration**
> The reverse proxy must set the [XFP header](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-Proto) and allow https connections.

### Archive (for making the scoreboard available afterwards)

```bash
# Copy your Scoreboards to /scoreboard before
# Try it out locally
docker-compose -f static.docker-compose.yml up

# Its recommended to build an image for the according CTF
docker build . -f static.Dockerfile -t ghcr.io/enowars/enoportal:<year>-<event>
docker push ghcr.io/enowars/enoportal:<year>-<event>
```
