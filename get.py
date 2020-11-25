import requests
import json

#uri = "http://localhost:50268/api/File/51"

with open("test.base64") as f:
    data = json.load(f)

with open("test.realbase64", 'w') as f:
    f.write(data["content"])



