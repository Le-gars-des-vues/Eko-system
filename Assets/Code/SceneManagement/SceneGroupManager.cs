using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace System.SceneManagement
{
    public class SceneGroupManager
    {
        public static SceneGroupManager instance;

        public event Action<string> OnSceneLoaded = delegate { };
        public event Action<string> OnSceneUnloaded = delegate { };
        public event Action OnSceneGroupLoaded = delegate { };

        SceneGroup ActiveSceneGroup;

        public async Task LoadScenes(SceneGroup group, IProgress<float> progress, bool reloadDupScene = false) 
        {
            ActiveSceneGroup = group;
            var loadedScenes = new List<string>();

            await UnloadScenes();

            int scenesCount = SceneManager.sceneCount;

            for (int i = 0; i < scenesCount; i++)
            {
                loadedScenes.Add(SceneManager.GetSceneAt(i).name);
            }

            var totalScenesToLoad = ActiveSceneGroup.Scenes.Count;
            var operationGroup = new AsyncOperationGroup(totalScenesToLoad);

            for (var i = 0; i < totalScenesToLoad; i++)
            {
                var sceneData = group.Scenes[i];
                if (!reloadDupScene && loadedScenes.Contains(sceneData.name)) continue;

                var operation = SceneManager.LoadSceneAsync(sceneData.Reference.Path, LoadSceneMode.Additive);

                operationGroup.Operations.Add(operation);

                OnSceneLoaded.Invoke(sceneData.name);

                while (!operationGroup.isDone)
                {
                    progress?.Report(operationGroup.Progress);
                    await Task.Delay(100);
                }

                Scene activeScene = SceneManager.GetSceneByName(ActiveSceneGroup.FindSceneNameByType(SceneType.ActiveScene));
                if (activeScene.IsValid())
                {
                    SceneManager.SetActiveScene(activeScene);
                }
                OnSceneGroupLoaded.Invoke();
            }
        }
        public async Task UnloadScenes() 
        {
            var scenes = new List<string>();
            var activeScene = SceneManager.GetActiveScene().name;

            int sceneCount = SceneManager.sceneCount;

            for (var i = sceneCount - 1; i > 0; i--)
            {
                var sceneAt = SceneManager.GetSceneAt(i);
                if (!sceneAt.isLoaded) continue;

                var sceneName = sceneAt.name;
                if (sceneName == "Bootstrapper") continue;
                scenes.Add(sceneName);
            }

            var operationGroup = new AsyncOperationGroup(scenes.Count);
            foreach (var scene in scenes)
            {
                var operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null) continue;

                operationGroup.Operations.Add(operation);
                OnSceneUnloaded.Invoke(scene);
            }

            while (!operationGroup.isDone)
            {
                await Task.Delay(100);
            }
        }
    }

    public readonly struct AsyncOperationGroup
    {
        public readonly List<AsyncOperation> Operations;

        public float Progress => Operations.Count == 0 ? 0 : Operations.Average(o => o.progress);
        public bool isDone => Operations.All(o => o.isDone);
        public AsyncOperationGroup(int initialCapacity)
        {
            Operations = new List<AsyncOperation>(initialCapacity);
        }
    }
}
