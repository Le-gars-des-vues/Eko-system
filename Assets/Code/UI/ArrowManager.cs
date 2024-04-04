using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArrowManager : MonoBehaviour
{
    public static ArrowManager instance;

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

    [SerializeField] bool followTarget;
    public Transform target;
    [SerializeField] float yOffset;
    [SerializeField] float xOffset;


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        text.text = textToWrite;
        textB.text = textToWrite;
    }

    // Start is called before the first frame update
    void OnActivation()
    {
        speed /= timeToFill;
        transform.Find("Visual").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        arrowPos = transform.Find("Visual").transform.localPosition;
        text.GetComponent<RectTransform>().localPosition = new Vector3(arrowPos.x, arrowPos.y + 0.22f, arrowPos.z);
        textB.GetComponent<RectTransform>().localPosition = new Vector3(arrowPos.x, arrowPos.y + 0.22f, arrowPos.z);
        timer = 0;
    }

    private void OnDeactivation()
    {
        transform.Find("Visual").transform.localPosition = arrowPos;
        transform.Find("Visual").GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);
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

        if (followTarget)
            transform.position = new Vector2(target.position.x + xOffset, target.position.y + yOffset);
    }

    public void PlaceArrow(Vector2 pos, string textToWrite, Vector2 offset, float _timeToFill = 0.02f)
    {
        timeToFill = _timeToFill;
        text.text = textToWrite;
        textB.text = textToWrite;
        OnActivation();
        gameObject.transform.position = pos + offset;
    }

    public void RemoveArrow()
    {
        OnDeactivation();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (followTarget)
            Gizmos.DrawSphere(new Vector2(target.position.x + xOffset, target.position.y + yOffset), 0.1f);
    }
}
