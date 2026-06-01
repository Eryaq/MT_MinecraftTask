using MT_MinecraftTask.Player;
using TMPro;
using UnityEngine;

namespace MT_MinecraftTask.UI
{
    public class MiningProgressHUD : MonoBehaviour
    {
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _text;

        private void Update()
        {
            if (_interactor == null || _root == null || _text == null)
                return;

            bool isVisible = _interactor.IsMining && _interactor.MiningProgress01 > 0f;

            _root.SetActive(isVisible);

            if (!isVisible)
                return;

            int percent = Mathf.RoundToInt(_interactor.MiningProgress01 * 100f);
            _text.text = $"Mining {_interactor.CurrentMiningBlock} {percent}%";
        }
    }
}
