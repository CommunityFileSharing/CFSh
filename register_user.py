import requests

vals = {"Password": "admin",
    "Username": "admin",
    "FirstName": "lorem",
    "LastName": "ipsum"}

url = "http://localhost:5000/api/Users/register"

x = requests.post(url, json=vals)

print(x.text)