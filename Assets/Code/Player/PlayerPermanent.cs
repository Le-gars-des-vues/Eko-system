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
    private bool isInvincible;
    private Color invisible;
    private List<Material> ogMaterials = new List<Material>();
    [SerializeField] private Material flashMaterial;
    [SerializeField] private List<SpriteRenderer> playerGFX = new List<SpriteRenderer>();
    [SerializeField] private float flashWhiteDuration;
    [SerializeField] private float invincibilityDuration;
    [SerializeField] private float invisibilityDuration;
    [SerializeField] private float hitStunDuration;
    public float knockBackForce;

    [Header("Stamina Variables")]
    public float maxStamina;
    public float currentStamina;
    public float staminaRegainRate;
    public Slider staminaSlider;
    private float staminaCountdown;
    public bool staminaDepleted = false;

    /*
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
    */

    [Header("Exploration Variables")]
    public bool survivalMode;
    public GameObject objectInRightHand = null;
    //public GameObject objectInLeftHand = null; Utilisation de la main gauche
    public bool isFacingRight;
    public List<GameObject> ressourcesNear = new List<GameObject>();
    public float nearestRessourceDistance = 10;
    private GameObject nearestRessource;
    public bool isThrowing = false;
    [SerializeField] MultiTool multiTool;
    public bool isUsingMultiTool;

    [Header("Inventory Variables")]
    public bool inventoryOpen = false;
    [SerializeField] private GameObject playerInventory;
    [SerializeField] private GameObject storageInventory;
    [SerializeField] private GameObject market;
    [SerializeField] private GameObject crafting;
    [SerializeField] private float gridOffset;
    public bool isInBase;
    private bool hasBuiltStorage = false;

    [Header("Ragdoll Variables")]
    [SerializeField] private Animator anim;
    [SerializeField] private List<Collider2D> colliders;
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<HingeJoint2D> joints;
    [SerializeField] private List<LimbSolver2D> limbs;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private CapsuleCollider2D playerCollider;

    private VinePlayerController vineController;
    private GameObject theBase;

    [Header("Upgrade Variables")]
    public bool hasDoubleJump;

    private void Awake()
    {
        isFacingRight = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //On assigne tout les elements relevant
        oxygenSlider = GameObject.Find("oxygenBar").GetComponent<Slider>();
        hpSlider = GameObject.Find("healthBar").GetComponent<Slider>();
        staminaSlider = GameObject.Find("staminaBar").GetComponent<Slider>();

        playerInventory = GameObject.Find("GridInventaire");
        storageInventory = GameObject.Find("GridStorage");
        market = GameObject.Find("Vente");
        crafting = GameObject.Find("Crafting");

        //Au depart du jeu, on set tout les bars au max et on desactive le ragdoll
        ResetToMax();
        ToggleRagdoll(false);

        ShowOrHideInventory(false, true, true);

        //On va chercher le script de vigne
        vineController = GetComponent<VinePlayerController>();
        invisible = new Color(255, 255, 255, 0);

        theBase = GameObject.FindGameObjectWithTag("Base");
    }

    // Update is called once per frame
    void Update()
    {
        /*
        //Variable simplement la pour le playtest, quand elle est active, les jauges de faim et de soif du joueur descendent lentement
        if (survivalMode)
        {
            currentHunger -= hungerDepleteRate * Time.deltaTime;
            SetBar(hungerSlider, currentHunger);
            currentThirst -= thirstDepleteRate * Time.deltaTime;
            SetBar(thirstSlider, currentThirst);
        }
        */
        if (theBase.GetComponent<Base>().isInside)
        {
            if (!isInBase)
                isInBase = true;
            else
                isInBase = false;
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
        if (Time.time - staminaCountdown > 2 && currentStamina <= maxStamina)
        {
            currentStamina += staminaRegainRate * Time.deltaTime;
            SetBar(staminaSlider, currentStamina);
            if (currentStamina >= maxStamina)
                staminaDepleted = false;
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
                ShowOrHideInventory(true, CanOpenStorage(), isInBase);
            else
                ShowOrHideInventory(false, CanOpenStorage(), isInBase);
        }
        
        if (ressourcesNear.Count >= 1)
        {
            foreach (var ressource in ressourcesNear)
            {
                float distance = Vector2.Distance(transform.position, ressource.transform.position);
                if (distance < nearestRessourceDistance)
                {
                    if (nearestRessource != null)
                    {
                        ressource.GetComponent<PickableObject>().isSelected = false;
                    }
                    nearestRessourceDistance = distance;
                    nearestRessource = ressource;
                    ressource.GetComponent<PickableObject>().isSelected = true;
                }
            }
        }
        else
            nearestRessourceDistance = 10;
    }

    //Pour remettre tout les values au maximum
    public void ResetToMax()
    {
        currentOxygen = maxOxygen;
        currentHp = maxHp;
        //currentHunger = maxHunger;
        //currentThirst = maxThirst;
        currentStamina = maxStamina;
        SetMaxBar(oxygenSlider, maxOxygen);
        SetMaxBar(hpSlider, maxHp);
        //SetMaxBar(hungerSlider, maxHunger);
        //SetMaxBar(thirstSlider, maxThirst);
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
        if (currentStamina <= 0)
            staminaDepleted = true;
    }

    public void ChangeHp(float value, bool isLosingHp, GameObject otherObject = null)
    {
        if (isLosingHp)
        {
            if (!isInvincible)
            {
                isInvincible = true;

                //Hitstun
                StartCoroutine(Hitstun(hitStunDuration));

                //Si le joueur n'est pas mort, il flash blanc et profite d'un moment d'invincibilite
                if (currentHp > 0)
                {
                    StartCoroutine(FlashWhite(playerGFX, flashWhiteDuration));
                    StartCoroutine(InvicibilityFrames(invisibilityDuration));
                }
                Vector2 direction = (transform.position - otherObject.transform.position).normalized;
                playerRb.AddForce(new Vector2(direction.x, 0.2f) * knockBackForce, ForceMode2D.Impulse);

                if (currentHp + value > maxHp)
                    currentHp = maxHp;
                else
                    currentHp += value;

                SetBar(hpSlider, currentHp);
            }
        }
        else
        {
            if (currentHp + value > maxHp)
                currentHp = maxHp;
            else
                currentHp += value;

            SetBar(hpSlider, currentHp);
        }
    }

    /*
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
    */

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

    public void EquipObject(GameObject obj)
    {
        objectInRightHand = obj;
    }

    public void UnequipObject()
    {
        objectInRightHand = null;
    }

    public void EquipMultiTool(bool equipped)
    {
        if (equipped)
        {
            objectInRightHand = multiTool.gameObject;
            multiTool.UseMultiTool(true);
        }
        else
        {
            objectInRightHand = null;
            multiTool.UseMultiTool(false);
        }
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
    
    public void ShowOrHideInventory(bool show, bool storage, bool isInBase)
    {
        if (show && playerInventory != null)
        {
            inventoryOpen = true;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            if (storage && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            }
            if (isInBase)
            {
                if (market != null)
                    market.GetComponent<RectTransform>().localPosition = new Vector2(market.GetComponent<RectTransform>().localPosition.x, market.GetComponent<RectTransform>().localPosition.y + gridOffset);
                if (crafting != null)
                    crafting.GetComponent<RectTransform>().localPosition = new Vector2(crafting.GetComponent<RectTransform>().localPosition.x, crafting.GetComponent<RectTransform>().localPosition.y + gridOffset);
            }
        }
        else if (!show && playerInventory != null)
        {
            inventoryOpen = false;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            if (storage && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            }
            if (isInBase)
            {
                if (market != null)
                    market.GetComponent<RectTransform>().localPosition = new Vector2(market.GetComponent<RectTransform>().localPosition.x, market.GetComponent<RectTransform>().localPosition.y - gridOffset);
                if (crafting != null)
                    crafting.GetComponent<RectTransform>().localPosition = new Vector2(crafting.GetComponent<RectTransform>().localPosition.x, crafting.GetComponent<RectTransform>().localPosition.y - gridOffset);
            }
        }
    }
    

    //Arrete le temps pour la duree choisit
    private IEnumerator Hitstun(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;
    }

    //Flash en blanc en changeant le materiel du joueur
    private IEnumerator FlashWhite(List<SpriteRenderer> spriteList, float duration)
    {
        foreach (var sprite in spriteList)
        {
            ogMaterials.Add(sprite.material);
            sprite.material = flashMaterial;
        }
        yield return new WaitForSecondsRealtime(duration);
        int i = 0;
        foreach (var sprite in spriteList)
        {
            sprite.material = ogMaterials[i];
            i++;
        }
    }

    IEnumerator InvicibilityFrames(float duration)
    {
        yield return new WaitForSeconds(flashWhiteDuration);
        //Le joueur flash pendant quelques secondes entre visible et invisible
        for (float i = 0; i < invincibilityDuration; i += Time.deltaTime)
        {
            // Si le joueur est deja invisible, il devient visible et vice-versa
            foreach (var sprite in playerGFX)
            {
                if (sprite.color == invisible)
                {
                    sprite.color = new Color(1, 1, 1, 1);
                }
                else
                {
                    sprite.color = invisible;
                }
            }
            yield return new WaitForSeconds(duration);
        }

        //On reactive le collider et on s'assure que le joueur est a nouveau visible
        foreach (var sprite in playerGFX)
            sprite.color = new Color(1, 1, 1, 1);

        //Le joueur peut a nouveau subir du degat
        isInvincible = false;
    }

    public bool CanOpenStorage()
    {
        return isInBase && hasBuiltStorage;
    }

    private Vector2 GetInput()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public void ResetFeetPosition()
    {
        gameObject.transform.Find("RightLegSolver").transform.Find("RightLegSolver_Target").GetComponent<PlayerLegAnimation>().ResetPosition();
        gameObject.transform.Find("LeftLegSolver").transform.Find("LeftLegSolver_Target").GetComponent<PlayerLegAnimation>().ResetPosition();
    }

    //Je met ça la pour mettre une manière de voir si on est dans la base ou non, tu peux changer ça si tu veux! -Pascal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Base")
        {
            isInBase = true;
            Debug.Log(isInBase);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Base")
        {
            isInBase = false;
            Debug.Log(isInBase);
        }
    }
}
