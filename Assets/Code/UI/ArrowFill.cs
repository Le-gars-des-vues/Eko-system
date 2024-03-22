using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowFill : MonoBehaviour
{
    [SerializeField] Animator anim;
    [SerializeField] float timeToFill = 0.02f;
    [SerializeField] float speed = 1;
    [SerializeField] float timer;
    public bool readyToActivate;
    bool holdingKey;
    bool changedSpeed = false;
    Vector3 arrowPos;

    private void Start()
    {
        arrowPos = transform.Find("Visual").transform.localPosition;
        transform.Find("Visual").transform.localPosition = arrowPos;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        //anim = GetComponent<Animator>();
        speed /= timeToFill;
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
