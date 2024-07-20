import requests
import json
from config import SCOREBOARD_URL, WEBHOOK_URL

d = requests.get(SCOREBOARD_URL).json()

try:
    cache = json.load(open("cache.json", "r"))
except:
    cache = []

for service in d["services"]:
    name = service["serviceName"]
    for fb in service["firstBloods"]:
        out = f"{fb['teamName']} got first blood on flag store #{fb['flagVariantId'] + 1} on service {name}!"
        if out in cache:
            print(f"Skipping duplicate entry: {out}")
        else:
            print(out)
            data = {
                "content": out,
                "username": "Firstblood-Bot"
            }
            try:
                result = requests.post(WEBHOOK_URL, json=data)
                cache.append(out)
            except Exception as e:
                print(e)

json.dump(cache, open("cache.json", "w"))
