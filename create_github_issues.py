#!/usr/bin/env python3
import re
import subprocess
import os
import time
import argparse

def read_backlog_file(file_path='BACKLOG.md'):
    """Read the BACKLOG.md file."""
    with open(file_path, 'r') as file:
        content = file.read()
    return content

def parse_tasks(content):
    """Parse the tasks from the backlog content."""
    milestones = {}
    current_milestone = None
    
    # Find all milestone sections
    milestone_pattern = r'## Milestone (\d+): ([^\n]+)'
    milestone_matches = re.finditer(milestone_pattern, content)
    
    for match in milestone_matches:
        milestone_number = match.group(1)
        milestone_name = match.group(2)
        milestone_id = f"TS-M{milestone_number}"
        milestones[milestone_id] = {
            'name': milestone_name,
            'tasks': []
        }
        
        # Find the start position of this milestone section
        milestone_start = match.end()
        
        # Find the end position (next milestone or end of file)
        next_milestone = re.search(r'## Milestone \d+:', content[milestone_start:])
        if next_milestone:
            milestone_end = milestone_start + next_milestone.start()
        else:
            # Look for other section markers
            next_section = re.search(r'## ', content[milestone_start:])
            if next_section:
                milestone_end = milestone_start + next_section.start()
            else:
                milestone_end = len(content)
        
        # Extract the milestone content
        milestone_content = content[milestone_start:milestone_end]
        
        # Extract tasks using regex
        task_pattern = r'\| (TS-\d+)\s+\| ([^|]+)\s*\| ([^|]+)\s*\| ([^|]+)\s*\| ([^|]+)\s*\|'
        task_matches = re.finditer(task_pattern, milestone_content)
        
        for task_match in task_matches:
            task_id = task_match.group(1)
            title = task_match.group(2).strip()
            description = task_match.group(3).strip()
            priority = task_match.group(4).strip()
            status = task_match.group(5).strip()
            
            milestones[milestone_id]['tasks'].append({
                'id': task_id,
                'title': title,
                'description': description,
                'priority': priority,
                'status': status,
                'milestone': milestone_id
            })
    
    return milestones

def create_labels(milestones, dry_run=False):
    """Create necessary labels in GitHub."""
    if dry_run:
        print("[DRY RUN] Would create milestone, priority, and status labels")
        return True
        
    try:
        # Create milestone labels
        for milestone_id in milestones.keys():
            label_name = f"milestone:{milestone_id}"
            color = "0366d6"  # Blue color for milestones
            
            # Create the label
            cmd = [
                'gh', 'label', 'create', label_name,
                '--color', color,
                '--description', f"Tasks for {milestone_id}"
            ]
            
            # Check if label exists first
            check_cmd = ['gh', 'label', 'list']
            result = subprocess.run(check_cmd, capture_output=True, text=True)
            
            if label_name not in result.stdout:
                result = subprocess.run(cmd, capture_output=True, text=True)
                if result.returncode == 0:
                    print(f"Created label: {label_name}")
                else:
                    print(f"Failed to create label: {label_name}")
                    print(f"Error: {result.stderr}")
            else:
                print(f"Label {label_name} already exists")
        
        # Create priority labels
        priorities = ["high", "medium", "low"]
        colors = ["d73a4a", "fbca04", "0e8a16"]  # Red, Yellow, Green
        
        for priority, color in zip(priorities, colors):
            label_name = f"priority:{priority}"
            
            # Check if label exists
            check_cmd = ['gh', 'label', 'list']
            result = subprocess.run(check_cmd, capture_output=True, text=True)
            
            if label_name not in result.stdout:
                cmd = [
                    'gh', 'label', 'create', label_name,
                    '--color', color,
                    '--description', f"{priority.capitalize()} priority tasks"
                ]
                
                result = subprocess.run(cmd, capture_output=True, text=True)
                if result.returncode == 0:
                    print(f"Created label: {label_name}")
                else:
                    print(f"Failed to create label: {label_name}")
                    print(f"Error: {result.stderr}")
            else:
                print(f"Label {label_name} already exists")
        
        # Create status labels
        statuses = ["backlog", "ready", "in progress", "review", "done"]
        colors = ["ededed", "c5def5", "bfdadc", "c2e0c6", "0e8a16"]  # Gray, Light Blue, Teal, Light Green, Green
        
        for status, color in zip(statuses, colors):
            label_name = f"status:{status}"
            
            # Check if label exists
            check_cmd = ['gh', 'label', 'list']
            result = subprocess.run(check_cmd, capture_output=True, text=True)
            
            if label_name not in result.stdout:
                cmd = [
                    'gh', 'label', 'create', label_name,
                    '--color', color,
                    '--description', f"Tasks in {status.capitalize()} status"
                ]
                
                result = subprocess.run(cmd, capture_output=True, text=True)
                if result.returncode == 0:
                    print(f"Created label: {label_name}")
                else:
                    print(f"Failed to create label: {label_name}")
                    print(f"Error: {result.stderr}")
            else:
                print(f"Label {label_name} already exists")
                
        return True
    except Exception as e:
        print(f"Error creating labels: {e}")
        return False

def create_milestone(name, description, dry_run=False):
    """Create a milestone in GitHub."""
    if dry_run:
        print(f"[DRY RUN] Would create milestone: {name}")
        return True
        
    try:
        # Check if the milestone already exists
        result = subprocess.run(
            ['gh', 'api', 'repos/:owner/:repo/milestones', '--jq', f'.[] | select(.title=="{name}")'],
            capture_output=True, text=True
        )
        
        if result.stdout.strip():
            print(f"Milestone '{name}' already exists, skipping creation.")
            return True
        
        # Create the milestone
        result = subprocess.run(
            ['gh', 'api', '--method', 'POST', 'repos/:owner/:repo/milestones', 
             '-f', f'title={name}', '-f', f'description={description}'],
            capture_output=True, text=True
        )
        
        if result.returncode == 0:
            print(f"Created milestone: {name}")
            return True
        else:
            print(f"Failed to create milestone: {name}")
            print(f"Error: {result.stderr}")
            return False
    except Exception as e:
        print(f"Error creating milestone: {e}")
        return False

def create_issue(task, milestone_name, dry_run=False):
    """Create a GitHub issue for the task."""
    if dry_run:
        print(f"[DRY RUN] Would create issue: [{task['id']}] {task['title']}")
        return True
        
    try:
        # Check if the issue already exists
        result = subprocess.run(
            ['gh', 'issue', 'list', '--search', f'{task["id"]} in:title'],
            capture_output=True, text=True
        )
        
        if task["id"] in result.stdout:
            print(f"Issue {task['id']} already exists, skipping creation.")
            return True
        
        # Prepare the issue body
        body = f"""
## Description
{task['description']}

## Priority
{task['priority']}

## Acceptance Criteria
- [ ] Implementation complete
- [ ] Tests added/updated
- [ ] Documentation updated
- [ ] Code review completed

## Related Tasks
- Milestone: {milestone_name}
"""
        
        # Create labels string
        # Convert to lowercase and replace spaces with hyphens
        priority_label = f"priority:{task['priority'].lower()}"
        milestone_label = f"milestone:{task['milestone']}"
        status_label = f"status:{task['status'].lower().replace(' ', '-')}"
        
        # Create the issue
        title = f"[{task['id']}] {task['title']}"
        
        # Use gh issue create command with individual label flags
        cmd = [
            'gh', 'issue', 'create', 
            '--title', title,
            '--body', body
        ]
        
        # Add label flags individually
        cmd.extend(['--label', milestone_label])
        cmd.extend(['--label', priority_label])
        if task['status'].lower() != 'backlog':
            cmd.extend(['--label', status_label])
        
        # Run the command
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            print(f"Created issue: {title}")
            return True
        else:
            print(f"Failed to create issue: {title}")
            print(f"Error: {result.stderr}")
            return False
    except Exception as e:
        print(f"Error creating issue: {e}")
        return False

def create_project(dry_run=False):
    """Create a GitHub project for the repository."""
    if dry_run:
        print("[DRY RUN] Would create project: Tequila Sunrise Development")
        return True
        
    try:
        # Check if a project named "Tequila Sunrise Development" already exists
        result = subprocess.run(
            ['gh', 'project', 'list'],
            capture_output=True, text=True
        )
        
        if "Tequila Sunrise Development" in result.stdout:
            print("Project 'Tequila Sunrise Development' already exists.")
            return True
        
        # Create the project with correct syntax
        result = subprocess.run(
            ['gh', 'project', 'create', 
             '--title', 'Tequila Sunrise Development'],
            capture_output=True, text=True
        )
        
        if result.returncode == 0:
            print("Created project: Tequila Sunrise Development")
            return True
        else:
            print("Failed to create project")
            print(f"Error: {result.stderr}")
            return False
    except Exception as e:
        print(f"Error creating project: {e}")
        return False

def main():
    # Parse command line arguments
    parser = argparse.ArgumentParser(description='Create GitHub issues from BACKLOG.md')
    parser.add_argument('--debug', action='store_true', help='Run in debug mode (only process a few tasks)')
    parser.add_argument('--dry-run', action='store_true', help='Dry run (do not create issues)')
    args = parser.parse_args()
    
    debug_mode = args.debug
    dry_run = args.dry_run
    
    if dry_run:
        print("Running in DRY RUN mode - no issues will be created")
    
    if debug_mode:
        print("Running in DEBUG mode - only processing a few tasks")
        
    print("Starting GitHub issue creation from BACKLOG.md...")
    
    # Read the backlog file
    content = read_backlog_file()
    
    # Parse the tasks
    milestones = parse_tasks(content)
    
    # Create a project board
    create_project(dry_run)
    
    # Create necessary labels
    create_labels(milestones, dry_run)
    
    # Process each milestone
    for milestone_id, milestone_data in milestones.items():
        print(f"\nProcessing milestone: {milestone_id} - {milestone_data['name']}")
        
        # Create the milestone
        if create_milestone(milestone_data['name'], f"Milestone {milestone_id}", dry_run):
            # Process each task in the milestone
            for i, task in enumerate(milestone_data['tasks']):
                create_issue(task, milestone_data['name'], dry_run)
                # Add a small delay to prevent API rate limiting
                time.sleep(1)
                
                # In debug mode, only process 2 tasks per milestone
                if debug_mode and i >= 1:
                    print(f"Debug mode: Stopping after processing 2 tasks in {milestone_id}")
                    break
        
        # In debug mode, only process the first milestone
        if debug_mode:
            print("Debug mode: Stopping after processing first milestone")
            break
    
    print("\nCompleted GitHub issue creation from BACKLOG.md")

if __name__ == "__main__":
    main() 