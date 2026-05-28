using MT_MiencraftTask.Player;
using TMPro;
using UnityEngine;

namespace MT_MiencraftTask.UI
{
    public class MiningProgressHUD : MonoBehaviour
    {
        [SerializeField] private PlayerVoxelInteractor _interactor;
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _text;

        private void Update()
        {
            bool isVisible = _interactor.IsMining && _interactor.MiningProgress01 > 0f;

            _root.SetActive(isVisible);

            if (!isVisible)
                return;

            int percent = Mathf.RoundToInt(_interactor.MiningProgress01 * 100f);
            _text.text = $"Mining {_interactor.CurrentMiningBlock} {percent}%";
        }
    }
}
