using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNewCycle : MonoBehaviour
{
    bool isInRange;
    bool startedANewCycle;
    float newCycleTime;

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            if (Input.GetKey(KeyCode.E) && ArrowManager.instance.readyToActivate && !startedANewCycle)
            {
                startedANewCycle = true;
                newCycleTime = Time.time;
                string promptText = GameObject.Find("Vente").GetComponent<Vente>().profit >= GameManager.instance.gameObject.GetComponent<Quota>().quota ? "Start a new day?" : "Start a new day? \n\nWARNING : INSUFFICIENT PROFITS!";
                PromptManager.instance.CreateNewPrompt(new Prompt(promptText, false, "Yes", "No"));
                PromptManager.onButtonClick = GameManager.instance.StartNewCycle;
            }

            if (Time.time - newCycleTime > 5.1f)
                startedANewCycle = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            ArrowManager.instance.PlaceArrow(transform.position, "START NEW CYCLE", new Vector2(0, 1), gameObject, 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
        }
    }
}
