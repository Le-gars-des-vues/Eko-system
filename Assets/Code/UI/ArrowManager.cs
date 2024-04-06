using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager instance;

    [SerializeField] Animator anim;
    [SerializeField] float timeToFill = 0.02f;
    [SerializeField] float timer;
    public bool readyToActivate;

    float moveSpeed = 10f;
    Vector2 arrowPos;
    Vector2 arrowOffset;

    [SerializeField] TextMeshPro text;
    [SerializeField] TextMeshPro textB;

    public GameObject targetObject;
    [SerializeField] PlayerPermanent player;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetObject != null)
            transform.position = Vector2.Lerp(transform.position, arrowPos + arrowOffset, moveSpeed * Time.deltaTime);

        if (Input.GetKey(KeyCode.E) || Input.GetKeyDown(KeyCode.E))
        {
            if (timer < timeToFill)
                timer += Time.deltaTime;
            else
                readyToActivate = true;
        }
        else
        {
            readyToActivate = false;
            if (timer > 0)
                timer -= Time.deltaTime;
        }

        float motion = Mathf.Lerp(0, 1, timer / timeToFill);
        anim.SetFloat("motionTime", motion);
    }

    public void PlaceArrow(Vector2 pos, string textToWrite, Vector2 offset, GameObject objectToTarget, float _timeToFill = 0.02f)
    {
        timeToFill = _timeToFill;
        text.text = textToWrite;
        textB.text = textToWrite;
        targetObject = objectToTarget;

        transform.Find("Visual").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        timer = 0;

        arrowOffset = offset;
        Vector2 diff = pos - (Vector2)targetObject.transform.position;
        arrowPos = (Vector2)targetObject.transform.position + diff;
        gameObject.transform.position = pos + offset;
    }

    public void RemoveArrow()
    {
        transform.Find("Visual").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
        targetObject = null;
        text.text = "";
        textB.text = "";
    }
}
