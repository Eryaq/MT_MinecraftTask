using MT_MiencraftTask.Voxels;
using System.Collections.Generic;
using UnityEngine;

namespace MT_MiencraftTask.Player
{
    public class PlayerBlockInventory : MonoBehaviour
    {
        private readonly Dictionary<EBlockType, int> _blocks = new();

        public event System.Action InventoryChanged;

        public void Add(EBlockType type, int amount = 1)
        {
            if (type == EBlockType.Air)
                return;

            _blocks.TryAdd(type, 0);
            _blocks[type] += amount;

            InventoryChanged?.Invoke();
            Debug.Log($"Inventory: {type} = {_blocks[type]}");
        }

        public bool TryConsume(EBlockType type, int amount = 1)
        {
            if (type == EBlockType.Air)
                return false;

            if (!_blocks.TryGetValue(type, out int currentAmount))
                return false;

            if (currentAmount < amount)
                return false;

            _blocks[type] -= amount;

            InventoryChanged?.Invoke();
            Debug.Log($"Inventory: {type} = {_blocks[type]}");

            return true;
        }

        public int GetAmount(EBlockType type)
        {
            return _blocks.GetValueOrDefault(type, 0);
        }

        public void Clear()
        {
            _blocks.Clear();
            InventoryChanged?.Invoke();
        }
    }
}
