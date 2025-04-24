import os
import sys
import json
import requests
from typing import Dict, List

# Configuration
GITHUB_API_URL = "https://api.github.com"
REPO_OWNER = "zahidplanet"
REPO_NAME = "TequilaSunrise"
GITHUB_TOKEN = os.getenv("GITHUB_TOKEN")

if not GITHUB_TOKEN:
    print("Error: GITHUB_TOKEN environment variable not set")
    sys.exit(1)

headers = {
    "Authorization": f"token {GITHUB_TOKEN}",
    "Accept": "application/vnd.github.v3+json"
}

def create_milestone(title: str, description: str) -> int:
    """Create a milestone and return its number."""
    url = f"{GITHUB_API_URL}/repos/{REPO_OWNER}/{REPO_NAME}/milestones"
    data = {
        "title": title,
        "description": description,
        "state": "open"
    }
    response = requests.post(url, headers=headers, json=data)
    if response.status_code == 201:
        return response.json()["number"]
    else:
        print(f"Error creating milestone {title}: {response.text}")
        return None

def create_issue(title: str, body: str, milestone_id: int, labels: List[str]) -> bool:
    """Create a GitHub issue."""
    url = f"{GITHUB_API_URL}/repos/{REPO_OWNER}/{REPO_NAME}/issues"
    data = {
        "title": title,
        "body": body,
        "milestone": milestone_id,
        "labels": labels
    }
    response = requests.post(url, headers=headers, json=data)
    if response.status_code == 201:
        print(f"Created issue: {title}")
        return True
    else:
        print(f"Error creating issue {title}: {response.text}")
        return False

def create_label(name: str, color: str, description: str) -> bool:
    """Create a label if it doesn't exist."""
    url = f"{GITHUB_API_URL}/repos/{REPO_OWNER}/{REPO_NAME}/labels"
    data = {
        "name": name,
        "color": color,
        "description": description
    }
    response = requests.post(url, headers=headers, json=data)
    return response.status_code in [201, 422]  # 422 means label already exists

def setup_labels():
    """Create default labels."""
    labels = {
        "priority: high": {"color": "d73a4a", "description": "High priority task"},
        "priority: medium": {"color": "fbca04", "description": "Medium priority task"},
        "priority: low": {"color": "0e8a16", "description": "Low priority task"},
        "status: ready": {"color": "c2e0c6", "description": "Ready for development"},
        "status: in progress": {"color": "1d76db", "description": "Currently in development"},
        "status: review": {"color": "5319e7", "description": "In review phase"},
        "type: feature": {"color": "0075ca", "description": "New feature"},
        "type: bug": {"color": "b60205", "description": "Bug fix"},
        "type: enhancement": {"color": "a2eeef", "description": "Enhancement to existing feature"}
    }
    
    for name, info in labels.items():
        create_label(name, info["color"], info["description"])

def main():
    """Main function to create issues from backlog."""
    print("Setting up labels...")
    setup_labels()
    
    # Create milestones
    milestones = {
        "Project Setup and Core AR": "Initial project setup and core AR functionality implementation",
        "Avatar Implementation": "Avatar model import, animation, and controls implementation",
        "Motorcycle Implementation": "Motorcycle physics and interaction implementation",
        "Physics and Interaction": "Refinement of physics and interaction systems",
        "Polish and Optimization": "Final polish and performance optimization",
        "Monetization": "Implementation of monetization and engagement features",
        "Content Creation": "Development of content creation tools"
    }
    
    milestone_ids = {}
    for title, description in milestones.items():
        milestone_id = create_milestone(title, description)
        if milestone_id:
            milestone_ids[title] = milestone_id
    
    print("Milestones created successfully!")
    print("Please run the backlog import script next to create individual issues.")

if __name__ == "__main__":
    main() 