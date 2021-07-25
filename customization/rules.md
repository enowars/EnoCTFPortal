# Rules

- Vulnboxes and VPN servers are provided by us, you don't have to provide or take care of anything.
- A round lasts 60 seconds, flags are valid for several rounds.
- Flag format: <code> ENO[A-Za-z0-9+\/=]{48}</code>
- Flag submission: <code>nc 10.0.13.37 1337</code>
- You will find an [Arkime](https://github.com/arkime/arkime) installation on your vulnbox located in /services/EnoMoloch. Arkime is a traffic analysis tool, not a vulnerable service.

## Setup

We will be providing hosted vulnboxes for all teams. You can start your vulnbox through the registration page after the start of the CTF. **Note that you must check in before the start of the CTF, otherwise you will not be able to start your vulnbox!**

You can download an OpenVPN configuration file which allows you to access your vulnbox as well as the rest of the competition network. You will be able to access your vulnbox immediately after the start, whereas the rest of the competition network will only be reachable after the network has opened.

The vulnbox has the IP address <code>10.1.teamID.1</code>, your team network will be assigned IP addresses from the IP range <code>10.1.teamID.130</code> to <code>10.1.teamID.254</code>. While we are blocking direct access to your team VPN from other teams, your device will be reachable from the vulnbox and thus might be targetable by teams who get remote code execution on your vulnbox. **Please take measures to protect your device used to access the network, e.g. by setting up a firewall!**

![network layout diagram](./assets/customization/enowars5_network.png)

For security reasons, the access to Arkime/Moloch is blocked over the network and it is only accessible from localhost. You can use SSH port forwarding to access your Arkime/Moloch by running <code>ssh -L 8005:localhost:8005 root@<your vulnbox IP></code>. Then you will be able to access it on your local machine by opening <code>http://localhost:8005</code> and logging in with username and password <code>moloch</code>.

### Self-Hosting

Note that we do not provide official support for self-hosting your vulnbox. However, this still should be easily doable if you have experience with setting up self-hosted vulnboxes.

There will be a wireguard config to which traffic for the <code>10.1.teamID.0/25</code> network will be routed. You can either use that directly on your vulnbox and set the address to <code>10.1.teamID.1</code>, or use a separate router and assign it any unused IP address, e.g. <code>10.1.teamID.2</code>. In that case you need to forward traffic for <code>10.1.teamID.1</code> to your vulnbox.

The vulnbox should be running Ubuntu 20.04 with `docker` and `docker-compose` installed and configured.

You will find all services in <code>/services/</code> on the vulnbox. Each service has a separate `docker-compose.yml` used for managing that service.

You can download your wireguard config for the vulnbox and OpenVPN config for your team before the start of the CTF. If you are able to reach your vulnbox from the team VPN, it should be reachable from the rest of the network as well.

When the CTF starts, start your own vulnbox as you would when using a cloud-hosted vulnbox, copy the services from <code>/services/</code> to your own vulnbox. **Make sure to stop the wireguard client running on the vulnbox! Otherwise the cloud-hosted and self-hosted vulnbox will both try to connect to the server, leading to connection issues.**

## Attack Info

The endpoint https://5.enowars.com/api/attackinfo delivers a JSON that is updated at the start of every round and has the following format:

```
{
	"availableTeams": [
		"10.1.52.1"
	],
	"services": {
		"service_1": {
			"10.1.52.1": {
				"7": [
					[ "user73" ],
					[ "user5" ]
				],
				"8": [
					[ "user96" ],
					[ "user314" ]
				]
			}
		}
	}
}
```

The `availableTeams` field contains a list of team IPs that were at least partially up in the previous round.

The `services` field will, for some services, provide you with additional information that may be helpful or necessary to exploit a given service. This is typically something like the username of the account containing the flag, but the exact format depends on the service. These are grouped by service, team IP, round, and type of flag.


## Scoring

We are currently using the scoring formula by [Faust CTF](https://2019.faustctf.net/information/rules/).

## Social Conduct

The vulnerable services of your opponents are your only valid targets. Do not engage anything else!

Do not attempt to exhaust resources on your opponents' vulnboxes, for example by sending excessive amounts of requests or exploiting vulnerabilities leading to a denial of service.
