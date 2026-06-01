using MT_MinecraftTask.Voxels;
using System;
using UnityEngine;

namespace MT_MinecraftTask.World
{
    [Serializable]
    public struct WorldModification
    {
        public Vector3Int WorldPosition;
        public EBlockType BlockType;

        public WorldModification(Vector3Int worldPosition, EBlockType blockType)
        {
            WorldPosition = worldPosition;
            BlockType = blockType;
        }
    }
}
