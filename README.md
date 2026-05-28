# MT_MinecraftTask

# Unity Voxel Sandbox Demo

Unity version: 6000.4.7f1

## Overview

This project is a small Minecraft-like voxel sandbox created as a technical interview task.

The focus of the project is implementation quality and performance rather than gameplay complexity.

## Features

- Procedurally generated voxel world
- Random world generation using Perlin noise
- Chunk-based terrain system
- Optimized mesh generation with visible-face culling
- Multiple block types based on height:
  - Stone
  - Grass
  - Snow
- First-person player controller
- Block mining with hold input
- Block placement with grid snapping
- Placement preview
- World height/depth limits
- Chunk loading and unloading around the player
- Chunk object pooling
- Save/load of world modifications
- Inventory based on mined blocks

## Optimization Notes

The world is not built using one GameObject per block.

Instead, blocks are stored as lightweight voxel data inside chunks. Each chunk generates a combined mesh containing only visible faces.

This reduces:
- GameObject count
- Renderer count
- Collider count
- Draw calls
- Runtime overhead

Chunk meshes are rebuilt only when needed. Runtime block edits mark affected chunks as dirty and the actual mesh rebuild is batched.

Chunks outside of the player view distance are pooled instead of destroyed.

## Controls

- WASD - Move
- Mouse - Look
- Space - Jump
- Left Mouse Button Hold - Mine block
- Right Mouse Button - Place block
- F5 - Save world
- F9 - Load world
- 1 - Select Stone
- 2 - Select Grass
- 3 - Select Snow
- B - Toggle placement preview
- F3 - Toggle debug overlay

## Technical Architecture

### Voxel Data

Each block is represented by an enum value:

```csharp
public enum EBlockType : byte
{
    Air = 0,
    Stone = 1,
    Grass = 2,
    Snow = 3
}
```

Each chunk stores voxel data in a 3D array and owns one generated mesh.

The chunk size is currently: 16 x 64 x 16

## Mesh Generation

The mesh generator iterates through voxel data and adds a face only when the neighbor block is air.

Neighbor checks work across chunk borders using world-space block lookup.

## Save System

The save system does not serialize the full world.

The terrain is regenerated from the saved seed and only player modifications are saved.

This keeps the save data small and deterministic.