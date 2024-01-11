using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TerraTiler2D
{
    [RequireComponent(typeof(Text))]
    public class UITextSetter : MonoBehaviour
    {
        private Text myText;

        // Start is called before the first frame update
        void Start()
        {
            myText = GetComponent<Text>();
        }

        public void SetText(string value)
        {
            myText.text = value;
        }
        public void SetText(int value)
        {
            myText.text = value.ToString();
        }
        public void SetText(float value)
        {
            value *= 100;
            value = Mathf.Round(value);
            value /= 100;

            myText.text = value.ToString();
        }
        public void SetText(bool value)
        {
            myText.text = value.ToString();
        }
        public void SetText(Vector2 value)
        {
            myText.text = value.ToString();
        }
        public void SetText(Vector3 value)
        {
            myText.text = value.ToString();
        }
        public void SetText(Vector4 value)
        {
            myText.text = value.ToString();
        }
    }
}
