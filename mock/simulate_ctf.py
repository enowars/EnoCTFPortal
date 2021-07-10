# -*- coding: utf-8 -*-
import json
import random
from datetime import date, datetime
import time
import requests
import json
import glob


SERVICE_COUNT = 11
TEAM_COUNT = 100
ROUND_LENGTH_SECONDS = 15

try:
    with open("currentRound.info", 'r') as f:
        line = f.readline().strip()
        currentRound = int(line) - 1

except:
    currentRound = 0


services = []
for i in range(1, 1 + SERVICE_COUNT):
    services.append({
        "serviceId": i,
        "serviceName": f"ServiceName {i}",
        "flagVariants": random.randint(1, 3),
        "firstBloods": []
    })

teams = []
for i in range(1, 1 + TEAM_COUNT):
    teams.append({
        "teamName": f"Team {i}",
        "teamId": i,
        "logoUrl": random.choice(["https://ctftime.org//media/team/discord_icon_only.png", ""]),
        "countryCode": "US",
        "totalScore": 0,
        "attackScore": 0,
        "defenseScore": 0,
        "serviceLevelAgreementScore": 0,
        "serviceDetails": [
            {
                "serviceId": s["serviceId"],
                "attackScore": 0,
                "defenseScore": 0,
                "serviceLevelAgreementScore": 0,
                "serviceStatus": "OK",
                "message": None
            }
            for s in services
        ]
    })

scoreboard = {
    "currentRound": currentRound,
    "startTimestamp": datetime.now().isoformat(),
    "endTimestamp": datetime.now().isoformat(),
    "roundLength": ROUND_LENGTH_SECONDS,
    "dnsSuffix": "",
    "services": services,
    "teams": teams
}


def generateScoreboard(scoreboard, currentRound):
    for t in range(0, TEAM_COUNT):
        attack_sum = 0
        defense_sum = 0
        sla_sum = 0

        for s in range(0, SERVICE_COUNT):
            attack = random.randint(0, 5)
            defense = random.randint(0, 5)
            sla = random.randint(0, 5)

            scoreboard["teams"][t]["serviceDetails"][s] = {
                "serviceId": scoreboard["teams"][t]["serviceDetails"][s]["serviceId"],
                "attackScore": scoreboard["teams"][t]["serviceDetails"][s]["attackScore"] + attack,
                # "attackScoreDelta": attack,
                "defenseScore": scoreboard["teams"][t]["serviceDetails"][s]["defenseScore"] - defense,
                # "defenseScoreDelta": defense,
                "serviceLevelAgreementScore": scoreboard["teams"][t]["serviceDetails"][s]["serviceLevelAgreementScore"] + sla,
                # "serviceLevelAgreementScoreDelta": sla,
                "serviceStatus":  random.choice(["OK", "OK", "OK", "OK", "OK", "OK", "MUMBLE", "OFFLINE", "RECOVERING", "INTERNAL_ERROR"]),
                "message": random.choice(["""Traceback (most recent call last):
  File "tb.py", line 15, in <module>
    a()
  File "tb.py", line 3, in a
    j = b(i)
  File "tb.py", line 9, in b
    c()
  File "tb.py", line 13, in c
    error()
NameError: name 'error' is not defined""", None])
            }

            attack_sum += attack
            defense_sum += defense
            sla_sum += sla

        scoreboard["teams"][t]["attackScore"] += attack_sum
        # scoreboard["teams"][t]["attackScoreDelta"] = attack_sum
        scoreboard["teams"][t]["defenseScore"] += defense_sum
        # scoreboard["teams"][t]["defenseScoreDelta"] = defense_sum
        scoreboard["teams"][t]["serviceLevelAgreementScore"] += sla_sum
        # scoreboard["teams"][t]["serviceLevelAgreementScoreDelta"] = sla_sum
        scoreboard["teams"][t]["totalScore"] += attack_sum + defense_sum + sla_sum
        # scoreboard["teams"][t]["totalScoreDelta"] = attack_sum + defense_sum + sla_sum

    for s in range(0, SERVICE_COUNT):
        service = scoreboard["services"][s]
        firstBloods = []

        for variant in range(0, service["flagVariants"]):
            if random.randint(0, 3) != 0: continue

            team = random.choice(scoreboard["teams"])

            firstBloods.append({
                "teamId": team["teamId"],
                "teamName": team["teamName"],
                "timestamp": "2021-06-05 12:12:12",
                "roundId": 1,
                "flagVariantId": variant,
            })

        scoreboard["services"][s] = {
            "serviceId": service["serviceId"],
            "serviceName": service["serviceName"],
            "flagVariants": service["flagVariants"],
            "firstBloods": firstBloods,
        }

    return scoreboard


def generateAttackInfo(attackInfo, currentRound):
    attackInfo = json.loads("""{
  "availableTeams": [
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1",
    "10.1.9.1"
  ],
  "services": {
    "gosship": {
      "10.1.9.1": {
        "619": { "0": ["olive-holy-elijah-108777"], "1": ["SEaSoN-rIVer-myTH-208810"] },
        "620": { "0": ["glaze-spark-james-093894"], "1": ["PeRiDOt-SwAMP-KitteN-656692"] }
      }
    },
    "orcano": {
      "10.1.9.1": {
        "619": { "0": [":i886806348:i-815075466"], "1": [":i2091792785:i744951361"] },
        "620": { "0": [":i-996168990:i1318233268"], "1": [":i1950306626:i2113958100"] }
      }
    },
    "shatranj": {
      "10.1.9.1": {
        "619": { "0": ["Q97UzzFYY5Zgyw"], "1": ["635OCU12naEQzQ"] },
        "620": { "0": ["fuCexsesVsoGeQ"], "1": ["CuWTqHC5l4QPKQ"] }
      }
    },
    "stldoctor": {
      "10.1.9.1": {
        "619": { "0": ["Model 5af679f3f8.. is kinda sus"], "1": ["User 46b3988c4d.. is kinda sus"] },
        "620": { "0": ["Model 267fd7767d.. is kinda sus"], "1": ["User 40cf917aa8.. is kinda sus"] }
      }
    }
  }
}
""")

    return attackInfo


def save(scoreboard, currentRound):
    # with open('EnoLandingPageBackend/ClientApp/src/assets/scoreboard.json', 'w') as f:
    with open('scoreboard.json', 'w') as f:
        json.dump(scoreboard, f)
    # with open(f"EnoLandingPageBackend/ClientApp/src/assets/scoreboard{currentRound}.json", 'w') as f:
    with open(f"scoreboard{currentRound}.json", 'w') as f:
        json.dump(scoreboard, f)


startTimestamp = datetime.now()
endTimestamp = datetime.now()

# save(scoreboard, currentRound)

while True:
    scoreboard = generateScoreboard(scoreboard, currentRound)
    attackInfo = generateAttackInfo({}, currentRound)

    currentRound += 1
    startTimestamp = endTimestamp
    endTimestamp = datetime.now().astimezone()
    scoreboard["currentRound"] = currentRound
    scoreboard["startTimestamp"] = startTimestamp.isoformat()
    scoreboard["endTimestamp"] = endTimestamp.isoformat()

    # todo: debug
    time.sleep(6)

    # save(scoreboard, currentRound)
    try:
        requests.post("http://localhost:5000/api/scoreboardinfo/scoreboard?adminSecret=secret",
                      json=scoreboard)
        requests.post("http://localhost:5000/api/attackinfo?adminSecret=secret",
                      json=attackInfo)
    except:
        print("There was a problem sending this round...")
    print(f"Posted scoreboard for round {currentRound}")
    with open("currentRound.info", 'w') as f:
        f.write(str(currentRound))
    time.sleep(ROUND_LENGTH_SECONDS)
