using UnityEngine;

namespace MT_MinecraftTask.Voxels
{
    [System.Serializable]
    public class BlockDefinition
    {
        public EBlockType Type;
        public Material Material;
        public float MiningTime = 1f;
    }
}
