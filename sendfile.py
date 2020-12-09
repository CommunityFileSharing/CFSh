import requests

with open("test_enc.txt") as f:
    data = f.read()
                    
vals = {"Content": data,
    "Name": "test",
    "Owner": 1}

url = "http://localhost:5000/api/File"

x = requests.post(url, json=vals)

print(x.text)