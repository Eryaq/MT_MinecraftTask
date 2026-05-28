using MT_MiencraftTask.Player;
using MT_MiencraftTask.Voxels;
using TMPro;
using UnityEngine;

namespace MT_MiencraftTask.UI
{
    public class InventoryHUD : MonoBehaviour
    {
        [SerializeField] private PlayerBlockInventory _inventory;
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private TMP_Text _text;

        private void OnEnable()
        {
            if (_inventory != null)
                _inventory.InventoryChanged += Refresh;

            if (_interactor != null)
                _interactor.SelectedBlockChanged += Refresh;

            Refresh();
            Refresh();
        }

        private void OnDisable()
        {
            if (_inventory != null)
                _inventory.InventoryChanged -= Refresh;

            if (_interactor != null)
                _interactor.SelectedBlockChanged -= Refresh;
        }

        private void Refresh()
        {
            EBlockType selected = _interactor.SelectedPlacementBlock;

            _text.text =
                $"Stone: {Format(EBlockType.Stone, selected)} {_inventory.GetAmount(EBlockType.Stone)}\n" +
                $"Grass: {Format(EBlockType.Grass, selected)} {_inventory.GetAmount(EBlockType.Grass)}\n" +
                $"Snow:  {Format(EBlockType.Snow, selected)} {_inventory.GetAmount(EBlockType.Snow)}\n\n" +
                $"Selected: {selected}";
        }

        private string Format(EBlockType type, EBlockType selected)
        {
            return type == selected ? ">" : " ";
        }
    }
}
