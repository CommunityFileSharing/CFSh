import requests

vals = {"Id": 1,
    "UserId": 1,
    "FreeSpace": 35234333,
    "UsedSpace": 0,
    "IP": "127.0.0.1",
    "Port": 8888}

url = "http://localhost:50268/api/Thicc"

x = requests.post(url, json=vals)

print(x.text)