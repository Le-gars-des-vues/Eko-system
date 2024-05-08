using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class StartNewCycle : MonoBehaviour
{
    bool isInRange;
    bool startedANewCycle;
    float newCycleTime;
    [SerializeField] VisualEffect SmokeVFX;
    [SerializeField] Animator anim;
    bool isAnimating;

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.E) && ArrowManager.instance.targetObject == gameObject && !startedANewCycle && !DialogueManager.instance.dialogueRunning)
            {
                string promptText = GameObject.Find("Vente").GetComponent<Vente>().profit >= GameManager.instance.gameObject.GetComponent<Quota>().quota ? "Start a new day?" : "Start a new day? \n\nWARNING : INSUFFICIENT PROFITS!";
                PromptManager.instance.CreateNewPrompt(new Prompt(promptText, false, "Yes", "No"));
                PromptManager.onButtonClick += GameManager.instance.StartNewCycle;
                PromptManager.onButtonClick += ResetNewCycle;
            }

            if (Time.time - newCycleTime > 5.1f && startedANewCycle)
                startedANewCycle = false;
        }
    }

    void ResetNewCycle()
    {
        startedANewCycle = true;
        newCycleTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "START NEW CYCLE", new Vector2(0, 1), gameObject);
            if (!isAnimating)
            {
                isAnimating = true;
                anim.SetBool("isOpen", true);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "START NEW CYCLE", new Vector2(0, 1), gameObject);
            if (!isAnimating)
            {
                isAnimating = true;
                anim.SetBool("isOpen", true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
            if (isAnimating)
            {
                isAnimating = false;
                anim.SetBool("isOpen", false);
            }
        }
    }

    void StartSmoke()
    {
        SmokeVFX.Play();
    }

    void StopSmoke()
    {
        SmokeVFX.Stop();
    }
}
