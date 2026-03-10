import os
import json
import requests

SYSTEM_PROMPT = """You are an assistant that generates short commercial proposals for IT/CRM services.
Return concise, bullet-point style text.
"""

def generate_proposal(client_name: str, service: str) -> str:
    api_key = os.getenv("OPENAI_API_KEY", "YOUR_API_KEY_HERE")
    url = "https://api.openai.com/v1/chat/completions"
    headers = {"Authorization": f"Bearer {api_key}", "Content-Type": "application/json"}
    data = {
        "model": "gpt-4.1-mini",
        "messages": [
            {"role": "system", "content": SYSTEM_PROMPT},
            {"role": "user", "content": f"Client: {client_name}. Service: {service}."}
        ]
    }
    resp = requests.post(url, headers=headers, data=json.dumps(data), timeout=60)
    return resp.text

if __name__ == "__main__":
    name = input("Client name: ")
    service = input("Service: ")
    print(generate_proposal(name, service))
