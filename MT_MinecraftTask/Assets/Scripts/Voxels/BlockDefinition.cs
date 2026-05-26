using UnityEngine;

namespace MT_MiencraftTask.Voxels
{
    [System.Serializable]
    public class BlockDefinition
    {
        public EBlockType Type;
        public Material Material;
        public float MiningTime = 1f;
    }
}
