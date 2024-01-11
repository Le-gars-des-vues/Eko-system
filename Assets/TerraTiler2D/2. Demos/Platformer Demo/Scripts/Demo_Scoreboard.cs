using UnityEngine;
using UnityEngine.UI;

namespace TerraTiler2D
{
    [RequireComponent(typeof(Text))]
    public class Demo_Scoreboard : MonoBehaviour
    {
        private Text myText;

        private void Start()
        {
            myText = GetComponent<Text>();
            EventManager.GetInstance().AddListener<Demo_EarnedScoreEvent>(UpdateScore);
        }

        private void UpdateScore(Demo_EarnedScoreEvent evt)
        {
            myText.text = evt.score.ToString();
        }
    }
}
