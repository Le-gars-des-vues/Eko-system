using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.IK;

public class PlayerPermanent : MonoBehaviour
{
    [Header("Oxygen Variable")]
    public float maxOxygen;
    public float currentOxygen;
    public float oxygenDepleteRate;
    public float oxygenRegainRate;
    public Slider oxygenSlider;

    [Header("Health Variables")]
    public float maxHp;
    public float currentHp;
    public Slider hpSlider;

    [Header("Hunger Variables")]
    public float maxHunger;
    public float currentHunger;
    public float hungerDepleteRate;
    public Slider hungerSlider;

    [Header("Thirst Variables")]
    public float maxThirst;
    public float currentThirst;
    public float thirstDepleteRate;
    public Slider thirstSlider;

    [Header("Stamina Variables")]
    public float maxStamina;
    public float currentStamina;
    public float staminaRegainRate;
    public Slider staminaSlider;
    private float staminaCountdown;

    [Header("Exploration Variables")]
    public bool survivalMode;
    public GameObject objectInRightHand = null;
    public GameObject objectInLeftHand = null;
    public bool isFacingRight;

    [Header("Inventory Variables")]
    public bool inventoryOpen = false;
    [SerializeField] private GameObject playerInventory;
    [SerializeField] private GameObject storageInventory;
    [SerializeField] private List<GameObject> itemList = new List<GameObject>();
    [SerializeField] private float gridOffset;

    [Header("Ragdoll Variables")]
    [SerializeField] private Animator anim;
    [SerializeField] private List<Collider2D> colliders;
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<HingeJoint2D> joints;
    [SerializeField] private List<LimbSolver2D> limbs;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private CapsuleCollider2D playerCollider;

    private VinePlayerController vineController;

    private void Awake()
    {
        isFacingRight = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetToMax();
        ToggleRagdoll(false);
        playerInventory.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        storageInventory.GetComponent<Image>().color = new Color(255, 255, 255, 0);
        playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
        storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
        vineController = GetComponent<VinePlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //Vaariable simplement la pour le playtest, quand elle est active, les jauges de faim et de soif du joueur descendent lentement
        if (survivalMode)
        {
            currentHunger -= hungerDepleteRate * Time.deltaTime;
            SetBar(hungerSlider, currentHunger);
            currentThirst -= thirstDepleteRate * Time.deltaTime;
            SetBar(thirstSlider, currentThirst);
        }

        //Dans l'eau, l'oxygen descend
        if (GetComponent<WaterPlayerController>().enabled == true)
        {
            currentOxygen -= oxygenDepleteRate * Time.deltaTime;
            SetBar(oxygenSlider, currentOxygen);
        }
        else if (GetComponent<WaterPlayerController>().enabled == false && currentOxygen < maxOxygen)
        {
            currentOxygen += oxygenRegainRate * Time.deltaTime;
            SetBar(oxygenSlider, currentOxygen);
        }

        //Timer de 2 seconds avec que le joueur commence a regagner de la stamina, reset a chaque fois qu'il utilise de la stamina
        if (Time.time - staminaCountdown > 2 && currentStamina < maxStamina)
        {
            currentStamina += staminaRegainRate * Time.deltaTime;
            SetBar(staminaSlider, currentStamina);
        }

        if (vineController.enabled)
        {
            if (GetInput().x != 0)
            {
                CheckDirectionToFace(GetInput().x > 0);
            }
        }
        else
        {
            Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            if ((difference.x < 0f && isFacingRight) || (difference.x > 0f && !isFacingRight))
            {
                Turn();
            }
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!inventoryOpen)
            {
                inventoryOpen = true;
                playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
                playerInventory.GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
            else
            {
                inventoryOpen = false;
                playerInventory.GetComponent<Image>().color = new Color(255, 255, 255, 0);
                playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            }
        }
    }

    //Pour remettre tout les values au maximum
    public void ResetToMax()
    {
        currentOxygen = maxOxygen;
        currentHp = maxHp;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;
        SetMaxBar(oxygenSlider, maxOxygen);
        SetMaxBar(hpSlider, maxHp);
        SetMaxBar(hungerSlider, maxHunger);
        SetMaxBar(thirstSlider, maxThirst);
        SetMaxBar(staminaSlider, maxStamina);
    }

    //Pour changer les slider, bar est le slider qu'on veut changer et value est la valeur qu'on veut lui donner
    public void SetBar(Slider bar, float value)
    {
        bar.value = value;
    }

    //Pour changer la valeur maximum des slider, meme fonctionnement que la fonction precedente
    public void SetMaxBar(Slider bar, float value)
    {
        bar.maxValue = value;
        bar.value = value;
    }

    //Suite de fonction pour changer les valeurs des stats du joueurs, il suffit d'ecrir la valeur du changement (25 ou -16 par exemple)
    public void ChangeStamina(float value)
    {
        currentStamina += value;
        SetBar(staminaSlider, currentStamina);
        staminaCountdown = Time.time;
    }

    public void ChangeHp(float value)
    {
        currentHp += value;
        SetBar(hpSlider, currentHp);
    }

    public void ChangeHunger(float value)
    {
        currentHunger += value;
        SetBar(hungerSlider, currentHunger);
    }

    public void ChangeThirst(float value)
    {
        currentThirst += value;
        SetBar(thirstSlider, currentThirst);
    }

    public void ChangeOxygen(float value)
    {
        currentOxygen += value;
        SetBar(oxygenSlider, currentOxygen);
    }

    public void ToggleRagdoll(bool ragdollOn)
    {
        anim.enabled = !ragdollOn;
        playerRb.simulated = !ragdollOn;
        playerCollider.enabled = !ragdollOn;
        foreach(var col in colliders)
        {
            col.enabled = ragdollOn;
        }
        foreach (var rb in rbs)
        {
            rb.simulated = ragdollOn;
        }
        foreach (var limb in limbs)
        {
            limb.weight = ragdollOn ? 0 : 1;
        }
        foreach (var joint in joints)
        {
            joint.enabled = ragdollOn;
        }
    }

    public void EquipObject(GameObject obj, bool isRightHand)
    {
        if (isRightHand)
            objectInRightHand = obj;
        else
            objectInLeftHand = obj;
    }

    public void UnequipObject(bool isRightHand)
    {
        if (isRightHand)
            objectInRightHand = null;
        else
            objectInLeftHand = null;
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
    }

    public void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}