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
    [SerializeField] float distanceOpenThreshold;
    [SerializeField] float baseEntryThreshold;
    [SerializeField] Animator leftDoorAnim;
    [SerializeField] Animator rightDoorAnim;
    [SerializeField] GameObject buildButton;

    [SerializeField] GameObject background;
    [SerializeField] GameObject baseBackground;
    [SerializeField] Light2D pixelLight;

    public bool isSceneLoaded = true;

    public bool isInside;

    // Start is called before the first frame update
    void Awake()
    {
        if (isSceneLoaded)
        {
            transform.position = new Vector2(135.3f, 4.6f);
        }
        door = new Vector2(transform.position.x, transform.position.y - 4.5f);
        leftDoorAnim = transform.Find("Outside").Find("LeftDoor").GetComponent<Animator>();
        rightDoorAnim = transform.Find("Outside").Find("RightDoor").GetComponent<Animator>();
        buildButton.SetActive(true);
    }

    private void Start()
    {
        background = GameObject.Find("ParallaxBackground");
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
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

            if (Input.GetKeyDown(KeyCode.W))
            {
                if (Vector2.Distance(player.transform.position, door) < baseEntryThreshold)
                {
                    pixelLight.intensity = 0.03f;
                    background.SetActive(false);
                    isInside = true;
                    player.transform.position = baseEntryPoint.position;
                    player.GetComponent<PlayerPermanent>().ResetFeetPosition();
                    baseBackground.SetActive(true);
                }
                else if (Vector2.Distance(player.transform.position, baseEntryPoint.position) < baseEntryThreshold)
                {
                    pixelLight.intensity = 1f;
                    baseBackground.SetActive(false);
                    isInside = false;
                    player.transform.position = door;
                    player.GetComponent<PlayerPermanent>().ResetFeetPosition();
                    background.SetActive(true);
                }
            }
        }
        if (isInside && player == null)
            player = GameObject.FindGameObjectWithTag("Player");
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
