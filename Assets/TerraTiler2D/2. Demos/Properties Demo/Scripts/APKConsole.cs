using UnityEngine;
using UnityEngine.UI;

namespace TerraTiler2D
{
    public class APKConsole : MonoBehaviour
    {
        public Text StackText;
        public Text ConsoleTextPrefab;
        public Scrollbar Scrollbar;

        private bool isPaused = false;

        private int refreshFrame;
        private bool shouldRefresh = false;

        private string previousDebug = "";

        // Start is called before the first frame update
        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (string.Equals(previousDebug, logString))
            {
                return;
            }

            Text newText = GameObject.Instantiate(ConsoleTextPrefab.gameObject, transform).GetComponent<Text>();

            string debugString = "";

            if (!isPaused)
            {
                switch (type)
                {
                    case LogType.Error:
                        debugString += "<color=red>ERROR: </color>";
                        break;
                    case LogType.Assert:
                        break;
                    case LogType.Warning:
                        debugString += "<color=yellow>WARNING: </color>";
                        break;
                    case LogType.Log:
                        debugString += "<color=green>LOG: </color>";
                        break;
                    case LogType.Exception:
                        debugString += "<color=orange>EXCEPTION: </color>";
                        break;
                    default:
                        break;
                }

                debugString += logString;

                newText.text = debugString;
                previousDebug = logString;

                newText.gameObject.GetComponent<Button>().onClick.AddListener(
                    () => { HandleStack(debugString + "\n" + stackTrace); }
                );
            }

            if (!shouldRefresh)
            {
                shouldRefresh = true;
                refreshFrame = Time.frameCount;
            }
            
        }

        public void HandleStack(string stackString)
        {
            StackText.text = stackString;
        }

        // Update is called once per frame
        void Update()
        {
            if (shouldRefresh)
            {
                if (Time.frameCount - refreshFrame >= 2)
                {
                    Scrollbar.value = 0.0f;
                    shouldRefresh = false;
                }
            }
        }

        public void TogglePauseConsole()
        {
            isPaused = !isPaused;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
        }
    }
}