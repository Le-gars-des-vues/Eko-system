using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Robot : MonoBehaviour
{
    [SerializeField] TextMeshPro speechBubbleText;
    [SerializeField] float walkSpeed;
    bool isWalking;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoRight()
    {
        rb.AddForce(walkSpeed * Vector2.right);
    }

    void GoLeft()
    {
        rb.AddForce(walkSpeed * Vector2.left);
    }

    public IEnumerator Speech(string textToWrite)
    {
        speechBubbleText.text = "";
        foreach (char letter in textToWrite.ToCharArray())
        {
            speechBubbleText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }
        yield return null;
    }
}
