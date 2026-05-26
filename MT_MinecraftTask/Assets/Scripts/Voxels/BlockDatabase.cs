using System.Collections.Generic;
using UnityEngine;

namespace MT_MiencraftTask.Voxels
{
    [CreateAssetMenu(menuName = "Minecraft Demo/Block Database")]
    public class BlockDatabase : ScriptableObject
    {
        [SerializeField] private BlockDefinition[] _definitions;

        private Dictionary<EBlockType, BlockDefinition> _lookup;

        public BlockDefinition Get(EBlockType type)
        {
            _lookup ??= BuildLookup();

            if (_lookup.TryGetValue(type, out var definition))
                return definition;

            Debug.LogError($"Missing block definition for {type}");
            return null;
        }

        private Dictionary<EBlockType, BlockDefinition> BuildLookup()
        {
            var result = new Dictionary<EBlockType, BlockDefinition>();

            foreach (var definition in _definitions)
                result[definition.Type] = definition;

            return result;
        }

        public float GetMiningTime(EBlockType type)
        {
            BlockDefinition definition = Get(type);

            if (definition == null)
                return 1f;

            return definition.MiningTime;
        }
    }
}
