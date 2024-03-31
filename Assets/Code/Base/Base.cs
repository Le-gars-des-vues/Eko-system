using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Base : MonoBehaviour
{
    Vector2 door;
    GameObject player;
    [SerializeField] Transform baseEntryPoint;
    public Transform baseSpawnPoint;
    public Transform trainingRoom;

    [SerializeField] float distanceOpenThreshold;
    [SerializeField] float baseEntryThreshold;
    [SerializeField] Animator leftDoorAnim;
    [SerializeField] Animator rightDoorAnim;
    [SerializeField] Animator insideDoorsAnim;
    [SerializeField] GameObject buildButton;

    [SerializeField] GameObject background;
    [SerializeField] GameObject baseBackground;
    [SerializeField] Light2D pixelLight;

    public bool isSceneLoaded = true;

    public bool isInside;

    // Start is called before the first frame update
    void Awake()
    {
        background = GameObject.Find("ParallaxBackground");
        if (isSceneLoaded)
        {
            transform.position = new Vector2(135.3f, 4.6f);
        }
        door = new Vector2(transform.position.x, transform.position.y - 4.5f);
        leftDoorAnim = transform.Find("Outside").Find("LeftDoor").GetComponent<Animator>();
        rightDoorAnim = transform.Find("Outside").Find("RightDoor").GetComponent<Animator>();
        //buildButton.SetActive(true);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<PlayerPermanent>().spawnAtBase)
        {
            Teleport(false, true, baseSpawnPoint.position);
        }
        buildButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if (!isInside)
            {
                //Debug.Log(Vector2.Distance(player.transform.position, door));
                if (Vector2.Distance(player.transform.position, door) < distanceOpenThreshold)
                {
                    leftDoorAnim.SetBool("isOpen", true);
                    rightDoorAnim.SetBool("isOpen", true);
                }
                else
                {
                    leftDoorAnim.SetBool("isOpen", false);
                    rightDoorAnim.SetBool("isOpen", false);
                }
            }
            else
            {
                //Debug.Log(Vector2.Distance(player.transform.position, door));
                if (Vector2.Distance(player.transform.position, baseEntryPoint.position) < distanceOpenThreshold)
                {
                    insideDoorsAnim.SetBool("isOpen", true);
                }
                else
                {
                    insideDoorsAnim.SetBool("isOpen", false);
                }
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (Vector2.Distance(player.transform.position, door) < baseEntryThreshold)
                {
                    Teleport(false, true, baseEntryPoint.position);
                }
                else if (Vector2.Distance(player.transform.position, baseEntryPoint.position) < baseEntryThreshold)
                {
                    Teleport(true, false, door);
                }
            }
        }
        if (isInside && player == null)
            player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Teleport(bool outside, bool inBase, Vector2 target)
    {
        if (outside)
        {
            pixelLight.intensity = 1f;
            baseBackground.SetActive(false);
            isInside = false;
            background.SetActive(true);
        }
        else if (inBase)
        {
            background.SetActive(false);
            baseBackground.SetActive(true);
            isInside = true;
            pixelLight.intensity = 0.03f;
        }
        else if (!outside && !inBase)
        {
            background.SetActive(false);
            baseBackground.SetActive(false);
            isInside = false;
            pixelLight.intensity = 0.03f;
        }
        player.transform.position = target;
        player.GetComponent<PlayerPermanent>().ResetFeetPosition();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - 4.5f, 0), 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            player = collision.gameObject;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isInside)
            player = null;
    }
}
