# Tequila Sunrise Project Backlog

This document serves as the central place for tracking tasks that need to be implemented in the project. Each task will be assigned a unique identifier (TS-X) and categorized by milestone.

## How to Use This Backlog

1. Tasks are organized by milestone
2. Each task has a unique identifier (TS-X)
3. To start working on a task:
   - Create a GitHub issue for it
   - Create a branch following the pattern `TS-X-brief-description`
   - Move the task to "In Progress" in your issue tracker
4. Update the status of tasks as they progress

## Current Sprint

<!-- Tasks currently being worked on in the current sprint -->

| ID    | Title | Description | Priority | Assignee | Status |
|-------|-------|-------------|----------|----------|--------|
| TS-4  | Create AR plane detection and visualization | Implement plane detection with visual indicators | High | @zahidplanet | In Progress |
| TS-6  | Import avatar model and configure import settings | Import and set up the TS_PixelAvatarMain model | High | @zahidplanet | In Progress |
| TS-7  | Create avatar animation controller and transitions | Set up animation states and transitions | High | @zahidplanet | In Progress |

## Milestone 1: Project Setup and Core AR Functionality

| ID    | Title | Description | Priority | Status |
|-------|-------|-------------|----------|--------|
| TS-1  | Initialize repository setup and documentation | Set up GitHub repo with proper documentation and workflow | High | Done |
| TS-2  | Configure Unity project with AR Foundation | Set up Unity project with AR Foundation and URP | High | Done |
| TS-3  | Implement AR session and camera setup | Configure AR camera and session settings | High | Done |
| TS-4  | Create AR plane detection and visualization | Implement plane detection with visual indicators | High | In Progress |
| TS-5  | Set up mobile build configurations | Configure build settings for iOS and Android | Medium | Ready |

## Milestone 2: Avatar Implementation

| ID    | Title | Description | Priority | Status |
|-------|-------|-------------|----------|--------|
| TS-6  | Import avatar model and configure import settings | Import and set up the TS_PixelAvatarMain model | High | In Progress |
| TS-7  | Create avatar animation controller and transitions | Set up animation states and transitions | High | In Progress |
| TS-8  | Implement character controller with physics | Create character controller with collisions | High | Backlog |
| TS-9  | Create mobile joystick control | Implement touchscreen joystick for movement | Medium | Backlog |
| TS-10 | Implement jump button functionality | Create jump button and jumping physics | Medium | Backlog |
| TS-11 | Create avatar placement in AR space | Allow user to place avatar on detected surfaces | High | Backlog |
| TS-12 | Add avatar scaling and positioning | Implement proper scaling for AR context | Medium | Backlog |

## Milestone 3: Motorcycle Implementation

| ID    | Title | Description | Priority | Status |
|-------|-------|-------------|----------|--------|
| TS-13 | Import motorcycle model and configure import settings | Import and set up the TS_Motorcycle model | High | Backlog |
| TS-14 | Implement motorcycle physics with wheel colliders | Create physics-based motorcycle with wheel colliders | High | Backlog |
| TS-15 | Create motorcycle controller with steering | Implement steering and lean mechanics | High | Backlog |
| TS-16 | Implement flat surface detection algorithm | Create algorithm to find suitable flat surfaces | Medium | Backlog |
| TS-17 | Create motorcycle spawning logic | Spawn motorcycle on suitable flat surface | Medium | Backlog |
| TS-18 | Implement avatar-motorcycle interaction | Create mounting/dismounting functionality | High | Backlog |
| TS-19 | Add motorcycle controls UI | Create UI for motorcycle controls | Medium | Backlog |

## Milestone 4: Physics and Interaction Refinement

| ID    | Title | Description | Priority | Status |
|-------|-------|-------------|----------|--------|
| TS-20 | Refine avatar movement on uneven surfaces | Improve avatar movement across different surfaces | Medium | Backlog |
| TS-21 | Improve motorcycle lean physics | Enhance leaning mechanics for realistic feel | Medium | Backlog |
| TS-22 | Enhance collision detection and response | Improve collision handling between objects | High | Backlog |
| TS-23 | Optimize surface mapping accuracy | Improve AR surface detection accuracy | Medium | Backlog |
| TS-24 | Fine-tune control responsiveness | Enhance feel and responsiveness of controls | Medium | Backlog |
| TS-25 | Add physics debugging tools | Create tools to visualize and debug physics | Low | Backlog |

## Milestone 5: Polish and Optimization

| ID    | Title | Description | Priority | Status |
|-------|-------|-------------|----------|--------|
| TS-26 | Add VFX for avatar movement and jumps | Create visual effects for movement | Low | Backlog |
| TS-27 | Create motorcycle engine effects | Add visual and audio effects for motorcycle | Low | Backlog |
| TS-28 | Optimize performance for low-end devices | Improve performance on mobile devices | High | Backlog |
| TS-29 | Improve UI feedback and responsiveness | Enhance UI responsiveness and feedback | Medium | Backlog |
| TS-30 | Fix identified bugs and issues | Address bugs from testing | High | Backlog |
| TS-31 | Prepare build for release | Final preparations for app release | High | Backlog |

## Future Features (Backlog)

<!-- Ideas for future development beyond the current milestones -->

- Multiplayer support
- Custom avatar skins
- Additional vehicles
- In-app tutorial
- Achievement system 