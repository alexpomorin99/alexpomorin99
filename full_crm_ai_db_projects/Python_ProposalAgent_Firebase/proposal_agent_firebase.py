import os
import json
import datetime

import firebase_admin
from firebase_admin import credentials, firestore
import requests

SYSTEM_PROMPT = """You are an assistant that generates short commercial proposals for IT/CRM services.
Return concise, bullet-point style text.
"""

cred = credentials.ApplicationDefault()
firebase_admin.initialize_app(cred)
db = firestore.client()


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


def save_proposal(client_name: str, service: str, proposal_raw: str) -> None:
    db.collection('proposals').add({
        'client_name': client_name,
        'service': service,
        'proposal_raw': proposal_raw,
        'created_at': datetime.datetime.utcnow().isoformat(),
    })


if __name__ == "__main__":
    name = input("Client name: ")
    service = input("Service: ")
    proposal = generate_proposal(name, service)
    print(proposal)
    save_proposal(name, service, proposal)
