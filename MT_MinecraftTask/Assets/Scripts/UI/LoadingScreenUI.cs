using MT_MinecraftTask.World;
using TMPro;
using UnityEngine;

namespace MT_MinecraftTask.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private WorldManager _worldManager;
        [SerializeField] private TMP_Text _loadingText;

        private void Update()
        {
            int percent = Mathf.RoundToInt(_worldManager.InitialLoadingProgress01 * 100f);
            _loadingText.text = $"Loading world... {percent}%";
        }
    }
}
