using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    [SerializeField] Base theBase;

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
        if (Input.GetKey(KeyCode.E) && ArrowManager.instance.readyToActivate)
        {
            if (isInRange)
            {
                PromptManager.instance.CreateNewPrompt(new Prompt("Go Back to base?", false, "Yes", "No"));
                PromptManager.onButtonClick = TeleportToBase;
            }
        }
    }

    void TeleportToBase()
    {
        StartCoroutine(player.GetComponent<PlayerPermanent>().Dissolve(2f, true, GameObject.Find("Base").GetComponent<Base>().baseSpawnPoint.position));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            ArrowManager.instance.PlaceArrow(transform.position, "CRAFTING BENCH", new Vector2(0, 1), 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            ArrowManager.instance.RemoveArrow();
        }
    }
}
