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
currentRound = len(glob.glob("./*")) - 1

services = []
for i in range(1, 1 + SERVICE_COUNT):
    services.append({
        "serviceId": i,
        "serviceName": f"service{i}",
        "flagVariants": 1,
        "firstBloods": []
    })

teams = []
for i in range(1, 1 + TEAM_COUNT):
    teams.append({
        "teamName": f"Team {i}",
        "teamId": i,
        "logoUrl": "https://ctftime.org//media/team/discord_icon_only.png",
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


def generate(scoreboard, currentRound):
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
                "defenseScore": scoreboard["teams"][t]["serviceDetails"][s]["defenseScore"] + defense,
                "serviceLevelAgreementScore": scoreboard["teams"][t]["serviceDetails"][s]["serviceLevelAgreementScore"] + sla,
                "serviceStatus": "OK",  # todo: make random ?
                "message": None
            }

            attack_sum += attack
            defense_sum += defense
            sla_sum += sla

        scoreboard["teams"][t]["attackScore"] = scoreboard["teams"][t]["attackScore"] + attack_sum
        scoreboard["teams"][t]["defenseScore"] += defense_sum
        scoreboard["teams"][t]["serviceLevelAgreementScore"] += sla_sum
        scoreboard["teams"][t]["totalScore"] += attack_sum + \
            defense_sum + sla_sum

    return scoreboard


def save(scoreboard, currentRound):
    # with open('EnoLandingPageBackend/ClientApp/src/assets/scoreboard.json', 'w') as f:
    with open('scoreboard.json', 'w') as f:
        json.dump(scoreboard, f)
    # with open(f"EnoLandingPageBackend/ClientApp/src/assets/scoreboard{currentRound}.json", 'w') as f:
    with open(f"scoreboard{currentRound}.json", 'w') as f:
        json.dump(scoreboard, f)

    print(f"Created scoreboard for round {currentRound}")


startTimestamp = datetime.now()
endTimestamp = datetime.now()

save(scoreboard, currentRound)

while True:
    scoreboard = generate(scoreboard, currentRound)

    time.sleep(ROUND_LENGTH_SECONDS)

    currentRound += 1
    startTimestamp = endTimestamp
    endTimestamp = datetime.now().astimezone()
    scoreboard["currentRound"] = currentRound
    scoreboard["startTimestamp"] = startTimestamp.isoformat()
    scoreboard["endTimestamp"] = endTimestamp.isoformat()

    save(scoreboard, currentRound)
    requests.post("http://localhost:5000/api/scoreboardinfo/scoreboard?adminSecret=secret",
                  json=scoreboard)
