using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom : MonoBehaviour
{
    [SerializeField] Base theBase;
    [SerializeField] Robot robot;

    PlayerPermanent player;
    bool isInRange;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E) && ArrowManager.instance.targetObject == gameObject)
        {
            if (isInRange && !DialogueManager.instance.dialogueRunning)
            {
                PromptManager.instance.CreateNewPrompt(new Prompt("Go Back to base?", false, "Yes", "No"));
                PromptManager.onButtonClick = TeleportToBase;
            }
        }
    }

    void TeleportToBase()
    {
        StartCoroutine(player.GetComponent<PlayerPermanent>().Dissolve(2f, true, GameObject.Find("Base").GetComponent<Base>().baseSpawnPoint.position));
        robot.TeleportToBase();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive)
                ArrowManager.instance.PlaceArrow(transform.position, "BACK TO BASE", new Vector2(0, -2), gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive)
                ArrowManager.instance.PlaceArrow(transform.position, "BACK TO BASE", new Vector2(0, -2), gameObject);
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
