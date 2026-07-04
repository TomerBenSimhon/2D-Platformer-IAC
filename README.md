# Blue Sword Ninja

A 2D action platformer. Solo project — all code and mechanics built by me.

**Engine:** Unity 2022.3.55f1
**Role:** Solo developer

[Portfolio](https://tomerbensimhon.github.io)

## Overview

One or two sentences on what the game is and what you were going for — tone, core loop, what makes it fun.

## Systems

### Player Controller
Fluid movement with variable jump height and adaptive gravity — falling applies stronger gravity than rising, for snappier, more readable jump arcs. Includes:
- **Jump buffering** — a jump pressed slightly before landing still registers
- **Coyote time** — jumps still fire for a few frames after leaving a platform edge

Assets/Scripts/Player/PlayerMovement.cs

### The Sword
A dual-purpose tool, used for both traversal and combat:
- Thrown at wooden walls, it sticks and becomes a temporary platform
- Can be recalled back to the player at any time
- Powers a dash attack
- The same throw stuns enemies on hit

Assets/Scripts/Player/SwordPlatformBehavior.cs
Assets/Scripts/Player/SwordProjectileBehavior.cs
Assets/Scripts/Player/PlayerActions.cs

### Blob Enemy AI
A finite state machine with three states — **Patrol**, **Search**, **Attack** — so the enemy's behavior changes based on whether it has spotted the player, lost track of them, or is engaging directly.

Assets/Scripts/Enemy


## Built with

Unity 2022.3.55f1 · C#
