import requests

with open("data.txt") as f:
    data = f.read()
                    
vals = {"Content": data,
    "Name": "test",
    "Owner": 3}

url = "http://localhost:50268/api/File"

x = requests.post(url, json=vals)

print(x.text)