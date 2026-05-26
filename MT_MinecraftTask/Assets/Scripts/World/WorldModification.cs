using MT_MiencraftTask.Voxels;
using System;
using UnityEngine;

namespace MT_MiencraftTask.World
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
