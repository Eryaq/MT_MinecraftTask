# MT_MinecraftTask

# Unity Voxel Sandbox Demo

Unity version: 6000.4.7f1

## Overview

This project is a small Minecraft-like voxel sandbox created as a technical interview task.

The focus of the project is implementation quality and performance rather than gameplay complexity.

The world is procedurally generated using Perlin noise and supports runtime voxel editing, chunk streaming, inventory management, and save/load functionality.

## Features

## World Generation

- Procedurally generated voxel world
- Deterministic seeded world generation
- Multiple terrain layers:
  - Stone
  - Grass
  - Snow
- Circular chunk streaming around the player
- Configurable view distance
- Runtime chunk loading and unloading

## Voxel System

- Chunk-based voxel storage
- Combined chunk mesh generation
- Visible-face culling
- Cross-chunk neighbor lookup
- Runtime voxel destruction and placement
- Dirty chunk rebuild batching
- Chunk generation queue to reduce frame spikes
- Chunk object pooling

## Gameplay

- First-person controller
- Block mining with hold interaction
- Block placement with placement preview
- Inventory system for collected blocks
- Block selection using hotkeys (1/2/3)
- Save and load system
- Player position persistence

## UI / Debug

- Crosshair
- Inventory HUD
- Mining progress indicator
- Toggleable debug overlay (F3)
- Placement preview toggle (B)
- Distance fog to hide chunk streaming

## Controls

- WASD - Move
- Mouse - Look
- Space - Jump
- Left Mouse Button Hold - Mine block
- Right Mouse Button - Place block
- 1 - Select Stone
- 2 - Select Grass
- 3 - Select Snow
- B - Toggle placement preview
- F3 - Toggle debug overlay
- F5 - Save world
- F9 - Load world

## Technical Architecture

## Chunk System

The world is divided into chunks.

Each chunk:
- stores voxel data
- generates a combined mesh
- owns a single renderer and collider

This avoids using one GameObject per voxel and significantly reduces runtime overhead.

Current chunk size: 16 x 64 x 16

## Mesh Generation

Only visible voxel faces are added to the mesh.

Faces between solid neighboring blocks are skipped.

Neighbor checks work across chunk borders using world-space voxel lookup.

This greatly reduces:
- vertex count
- triangle count
- renderer overhead
- draw calls

## Runtime Optimization

Several runtime optimizations were implemented:
- Chunk pooling instead of destroy/create
- Circular chunk streaming
- Dirty chunk rebuild batching
- Chunk generation queue distributed across frames
- Shared mesh/material usage
- Limited mesh rebuild scope

## Save System

The save system does not serialize the entire world.

Instead:
- the world is regenerated from the saved seed
- only modified voxels are stored
- player position is persisted separately

This keeps save files lightweight and deterministic.

## Performance Notes

Typical runtime performance in editor:

- ~300–600 FPS
- ~80 loaded chunks
- ~180k rendered triangles

(depending on camera direction and terrain visibility)

## Possible Future Improvements

- Greedy meshing
- Multithreaded chunk generation
- Texture atlas
- Async save system
- LOD chunk rendering