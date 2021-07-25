import os
import json
import random
from datetime import date, datetime
import time
import requests
import json
import glob

directory = r'./plain-scoreboard'
for filename in os.listdir(directory):
    if filename.endswith(".json"):
        print(os.path.join(directory, filename))
        with open(os.path.join(directory, filename), 'r') as f:
            lines = f.read()
            req = requests.post("http://localhost:5000/api/scoreboardinfo/scoreboard?adminSecret=secret",
                      data=lines, headers={"Content-Type":"application/json"})
            print(req.text)
    else:
        continue