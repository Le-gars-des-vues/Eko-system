using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Base : MonoBehaviour
{
    public static Base instance;

    Vector2 door;
    GameObject player;
    [SerializeField] Transform baseEntryPoint;
    public Transform baseSpawnPoint;
    public Transform trainingRoom;

    [SerializeField] float distanceOpenThreshold;
    [SerializeField] float baseEntryThreshold;
    [SerializeField] Animator leftDoorAnim;
    [SerializeField] Animator rightDoorAnim;
    bool outsideDoorOpened;
    bool insideDoorOpened;
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
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

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
            //background.SetActive(false);
            Teleport(false, true, baseSpawnPoint.position);
        }
        buildButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            if ((player.GetComponent<PlayerPermanent>().spawnAtBase && Tutorial.instance.readyToGoOut) || !player.GetComponent<PlayerPermanent>().spawnAtBase)
            {
                if (!isInside)
                {
                    //Debug.Log(Vector2.Distance(player.transform.position, door));
                    if (Vector2.Distance(player.transform.position, door) < distanceOpenThreshold && !outsideDoorOpened)
                    {
                        outsideDoorOpened = true;
                        AudioManager.instance.PlaySound(AudioManager.instance.porteOuverture, leftDoorAnim.gameObject);
                        leftDoorAnim.SetBool("isOpen", true);
                        rightDoorAnim.SetBool("isOpen", true);
                    }
                    else if (Vector2.Distance(player.transform.position, door) > distanceOpenThreshold && outsideDoorOpened)
                    {
                        outsideDoorOpened = false;
                        AudioManager.instance.PlaySound(AudioManager.instance.porteFermeture, leftDoorAnim.gameObject);
                        leftDoorAnim.SetBool("isOpen", false);
                        rightDoorAnim.SetBool("isOpen", false);
                    }
                }
                else
                {
                    //Debug.Log(Vector2.Distance(player.transform.position, door));
                    if (Vector2.Distance(player.transform.position, baseEntryPoint.position) < distanceOpenThreshold && !insideDoorOpened)
                    {
                        if (GameManager.instance.TimeLeft > 0)
                        {
                            insideDoorOpened = true;
                            AudioManager.instance.PlaySound(AudioManager.instance.porteOuverture, insideDoorsAnim.gameObject);
                            insideDoorsAnim.SetBool("isOpen", true);
                        }
                    }
                    else if (Vector2.Distance(player.transform.position, baseEntryPoint.position) > distanceOpenThreshold && insideDoorOpened)
                    {
                        insideDoorOpened = false;
                        AudioManager.instance.PlaySound(AudioManager.instance.porteFermeture, insideDoorsAnim.gameObject);
                        insideDoorsAnim.SetBool("isOpen", false);
                    }
                }

                if (Input.GetKeyDown(KeyCode.W))
                {
                    if (Vector2.Distance(player.transform.position, door) < baseEntryThreshold)
                    {
                        Teleport(false, true, baseEntryPoint.position);
                        if (GameManager.instance.isStorm)
                            GameManager.instance.Storm(false);
                    }
                    else if (Vector2.Distance(player.transform.position, baseEntryPoint.position) < baseEntryThreshold)
                    {
                        if (GameManager.instance.TimeLeft > 0)
                        {
                            Teleport(true, false, door);
                            if (!GameManager.instance.TimerOn)
                                GameManager.instance.TimerOn = true;
                        }
                    }
                }
            }
        }
    }

    public void Teleport(bool outside, bool inBase, Vector2 target)
    {
        player.transform.position = target;
        player.GetComponent<PlayerPermanent>().ResetFeetPosition();
        if (outside)
        {
            pixelLight.intensity = 1f;
            baseBackground.SetActive(false);
            isInside = false;
            //background.SetActive(true);
        }
        else if (inBase)
        {
            //background.SetActive(false);
            baseBackground.SetActive(true);
            isInside = true;
            pixelLight.intensity = 0.03f;
        }
        else if (!outside && !inBase)
        {
            //background.SetActive(false);
            baseBackground.SetActive(false);
            //isInside = false;
            pixelLight.intensity = 0.03f;
        }
    }

    public IEnumerator Dissolve(List<SpriteRenderer> sprites, float dissolveTime, bool isTrue)
    {
        float elapsedTime = 0;
        if (isTrue)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, sprites[0].gameObject);
            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                foreach (SpriteRenderer sprite in sprites)
                    sprite.material.SetFloat("_Transparency", dissolveAmount);
                yield return null;
            }
        }
        else
        {
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, sprites[0].gameObject);
            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(1f, 0.01f, elapsedTime / dissolveTime);
                foreach (SpriteRenderer sprite in sprites)
                    sprite.material.SetFloat("_Transparency", dissolveAmount);
                yield return null;
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - 4.5f, 0), 0.5f);
    }
}
