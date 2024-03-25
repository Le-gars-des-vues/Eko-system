using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Arrow : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float timeToFill = 0.02f;
    [SerializeField] float speed = 1;
    [SerializeField] float timer;
    public bool readyToActivate;
    bool holdingKey;
    bool changedSpeed = false;
    Vector3 arrowPos;

    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro textB;
    [SerializeField] string textToWrite = "PICK UP";


    private void Start()
    {
        text.text = textToWrite;
        textB.text = textToWrite;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        speed /= timeToFill;
        arrowPos = transform.Find("Visual").transform.localPosition;
        text.GetComponent<RectTransform>().localPosition = new Vector3(arrowPos.x, arrowPos.y + 0.22f, arrowPos.z);
        textB.GetComponent<RectTransform>().localPosition = new Vector3(arrowPos.x, arrowPos.y + 0.22f, arrowPos.z);
    }

    private void OnDisable()
    {
        transform.Find("Visual").transform.localPosition = arrowPos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E) || Input.GetKeyDown(KeyCode.E))
        {
            holdingKey = true;
            if (timer < timeToFill)
            {
                timer += Time.deltaTime;
            }
            else
            {
                readyToActivate = true;
                anim.SetFloat("animSpeed", 0);
            }
        }
        else
        {
            holdingKey = false;
            readyToActivate = false;
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
                anim.SetFloat("animSpeed", 0);
        }

        if (holdingKey && !changedSpeed)
        {
            changedSpeed = true;
            anim.SetFloat("animSpeed", speed);
        }
        else if (!holdingKey && changedSpeed)
        {
            changedSpeed = false;
            anim.SetFloat("animSpeed", -speed);
        }
    }
}
