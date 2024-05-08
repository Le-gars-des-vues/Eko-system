using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace System.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader instance;
        public static event Action allScenesLoaded;

        [SerializeField] Slider loadingBar;
        [SerializeField] float fillSpeed;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroup;

        float targetProgress;
        public bool isLoading;

        public readonly SceneGroupManager manager = new SceneGroupManager();

        private void Awake()
        {
            if (instance != null)
                Destroy(this);
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }


            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        private void Start()
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }

        private void Update()
        {
            if (!isLoading) return;

            float currentProgress = loadingBar.value;
            float progressDifference = Mathf.Abs(currentProgress - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.value = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * dynamicFillSpeed);
        }

        public async Task LoadSceneGroup(int index)
        {
            loadingBar.value = 0;
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroup.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.progressed += target => targetProgress = MathF.Max(target, targetProgress);

            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroup[index], progress);
            allScenesLoaded?.Invoke();
            Debug.Log("Script started!");
            allScenesLoaded = null;
            EnableLoadingCanvas(false);
        }

        void EnableLoadingCanvas(bool enable = true)
        {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }

        public async void LoadGame()
        {
            await LoadSceneGroup(0);
        }

        public async void LoadSandbox()
        {
            await LoadSceneGroup(2);
        }

        public async void LoadForest1()
        {
            await LoadSceneGroup(3);
        }

        public async void LoadGym_1()
        {
            await LoadSceneGroup(4);
        }

        public async void LoadMainMenu()
        {
            await LoadSceneGroup(1);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> progressed;

        const float RATIO = 1f;

        public void Report(float value)
        {
            progressed?.Invoke(value / RATIO);
        }
    }
}

