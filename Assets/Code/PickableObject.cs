using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private Material ogMaterial;
    public Material flashMaterial;
    private Color ogColor;

    private GameObject rightHand;
    private GameObject leftHand;
    private PlayerPermanent player;

    public bool hasFlashed;
    public bool isPickedUp = false;

    // Start is called before the first frame update
    void Start()
    {
        ogColor = GetComponent<SpriteRenderer>().color;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        rightHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_4").Find("bone_5").gameObject;
        leftHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_6").Find("bone_7").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Mathf.Abs(Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position)));
        if (CanBePickedUp() && !hasFlashed)
        {
            hasFlashed = true;
            StartCoroutine(FlashWhite(GetComponent<SpriteRenderer>(), 0.05f, 5));
        }
        else if (!CanBePickedUp() && hasFlashed)
            hasFlashed = false;


        //Pick up
        if (hasFlashed && Input.GetKeyDown(KeyCode.E))
        {
            if (!isPickedUp)
            {
                if (player.objectInRightHand == null)
                {
                    isPickedUp = true;
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponent<Rigidbody2D>().simulated = false;
                    transform.position = rightHand.transform.Find("RightArmEffector").transform.position;
                    transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - rightHand.transform.rotation.z);
                    gameObject.transform.SetParent(rightHand.transform);
                    GetComponent<SpriteRenderer>().sortingOrder = 7;
                    player.EquipObject(gameObject, true);
                }
                else if (player.objectInLeftHand == null)
                {
                    isPickedUp = true;
                    GetComponent<BoxCollider2D>().enabled = false;
                    GetComponent<Rigidbody2D>().simulated = false;
                    transform.position = leftHand.transform.Find("LeftArmEffector").transform.position;
                    transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - leftHand.transform.rotation.z);
                    gameObject.transform.SetParent(leftHand.transform);
                    GetComponent<SpriteRenderer>().sortingOrder = 2;
                    player.EquipObject(gameObject, false);
                }
            }
        }
        if (gameObject.tag == "Javelin" && isPickedUp)
        {
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    //Flash en blanc en changeant le materiel du joueur
    private IEnumerator FlashWhite(SpriteRenderer sprite, float duration, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            yield return new WaitForSecondsRealtime(duration);
            //ogMaterial = sprite.material;
            sprite.color = new Color(255, 255, 255, 255);
            //sprite.material = flashMaterial;
            yield return new WaitForSecondsRealtime(duration);
            //sprite.material = ogMaterial;
            sprite.color = ogColor;
        }
        if (!CanBePickedUp())
        {
            sprite.color = ogColor;
            yield break;
        }
        if (isPickedUp)
        {
            sprite.color = ogColor;
            yield break;
        }
    }

    bool CanBePickedUp()
    {
        return Mathf.Abs(Vector2.Distance(new Vector2(GameObject.FindGameObjectWithTag("Player").transform.position.x, 0), new Vector2(transform.position.x, 0))) <= 1f;
    }
}
