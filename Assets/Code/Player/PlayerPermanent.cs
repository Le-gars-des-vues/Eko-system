using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D.IK;
using Cinemachine;

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
    [SerializeField] private List<GameObject> bones;
    [SerializeField] private List<Vector3> bonesPosition;
    [SerializeField] private List<Quaternion> bonesRotation;

    private VinePlayerController vineController;
    private GameObject theBase;
    public GameObject gameOverScreen;

    [Header("Upgrade Variables")]
    public bool hasDoubleJump;

    [Header("UI Variables")]
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject market;
    [SerializeField] private GameObject crafting;
    [SerializeField] private GameObject upgrade;
    public bool mapIsOpen = true;
    public bool marketIsOpen = false;
    public bool craftingIsOpen = false;
    public bool upgradeIsOpen = false;
    public bool uiOpened;
    bool inventoryTrigger = false;
    bool wasOnGround = false;
    bool wasInWater = false;
    bool wasOnVine = false;

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

        playerInventory = GameObject.Find("Inventaire");
        storageInventory = GameObject.Find("Storage");
        market = GameObject.Find("Vente");
        crafting = GameObject.Find("Crafting");
        map = GameObject.Find("Map");
        upgrade = GameObject.Find("Upgrades");
        gameOverScreen = GameObject.Find("GameOverScreen");
        //Au depart du jeu, on set tout les bars au max et on desactive le ragdoll
        for (int i = 0; i < bones.Count; i++)
        {
            bonesPosition.Add(bones[i].transform.position);
            bonesRotation.Add(bones[i].transform.rotation);
        }
        Reset();
        ToggleRagdoll(false);

        //On va chercher le script de vigne
        vineController = GetComponent<VinePlayerController>();
        invisible = new Color(255, 255, 255, 0);

        theBase = GameObject.FindGameObjectWithTag("Base");
        vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        uiOpened = IsInUI();

        if (uiOpened && !inventoryTrigger)
        {
            CheckIfUiIsOpen(uiOpened);
            StartCoroutine(MoveCamera(true));
        }
        else if (!uiOpened && inventoryTrigger)
        {
            CheckIfUiIsOpen(uiOpened);
            StartCoroutine(MoveCamera(false));
        }

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

        //Check if player is in base
        if (theBase.GetComponent<Base>().isInside)
        {
            if (!isInBase)
                isInBase = true;
        }
        else
        {
            if (isInBase)
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

        //Vine controller
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
        
        //Open UI
        if (Input.GetKeyDown(KeyCode.I) && !marketIsOpen && !craftingIsOpen)
        {
            if (!inventoryOpen)
                ShowOrHideInventory();
            else
                ShowOrHideInventory();
        }

        if (Input.GetKeyDown(KeyCode.M) && !marketIsOpen && !craftingIsOpen)
        {
            ShowOrHideMap();
        }

        if (Input.GetKeyDown(KeyCode.U) && !marketIsOpen && !craftingIsOpen)
        {
            ShowOrHideUpgrades();
        }

        //Pick up ressources
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

        if (currentHp <= 0)
        {
            Death();
        }
    }

    //Pour remettre tout les values au maximum
    public void Reset()
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

        CloseUI();

        gameOverScreen.SetActive(false);
    }

    public void Death()
    {
        CloseUI();

        ToggleRagdoll(true);
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            script.enabled = false;
        }

        ItemGrid inventoryGrid = playerInventory.transform.Find("GridInventaire").gameObject.GetComponent<ItemGrid>();
        InventoryItem anItem;
        List<InventoryItem> itemsInInventory = new List<InventoryItem>();

        float inventoryWidth = inventoryGrid.GetGridSizeWidth();
        float inventoryHeight = inventoryGrid.GetGridSizeHeight();

        for (int x = 0; x < inventoryWidth; x++)
        {

            for (int y = 0; y < inventoryHeight; y++)
            {
                anItem = inventoryGrid.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    if (!itemsInInventory.Contains(anItem))
                        itemsInInventory.Add(anItem);
                }
            }
        }
        int numberOfItemToDestroy = itemsInInventory.Count / 2;
        for (int i = 0; i < numberOfItemToDestroy; i++)
        {
            int index = Random.Range(0, itemsInInventory.Count - 1);
            itemsInInventory[index].GetComponent<InventoryItem>().Delete();
            itemsInInventory.RemoveAt(index);
        }

        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<GameOverScreen>().player = gameObject;
    }

    void CloseUI()
    {
        if (inventoryOpen)
            ShowOrHideInventory();
        if (marketIsOpen)
            ShowOrHideMarket();
        if (craftingIsOpen)
            ShowOrHideCrafting();
        if (mapIsOpen)
            ShowOrHideMap();
        if (upgradeIsOpen)
            ShowOrHideUpgrades();
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
                playerRb.velocity = Vector2.zero;
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

        if (!ragdollOn)
        {
            for (int i = 0; i < bones.Count; i++)
            {
                bones[i].transform.position = bonesPosition[i];
                bones[i].transform.rotation = bonesRotation[i];
            }
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
    
    public void ShowOrHideInventory()
    {
        if (!inventoryOpen)
        {
            inventoryOpen = true;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            if (CanOpenStorage() && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            }

            if (mapIsOpen)
            {
                map.SetActive(false);
                mapIsOpen = false;
            }

            if (upgradeIsOpen)
            {
                upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y - gridOffset);
                upgradeIsOpen = false;
            }
        }
        else
        {
            inventoryOpen = false;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            if (CanOpenStorage() && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            }
        }
    }

    public void ShowOrHideInventoryNoButtons()
    {
        if (!inventoryOpen)
        {
            inventoryOpen = true;
            playerInventory.transform.Find("NextUI").gameObject.SetActive(false);
            playerInventory.transform.Find("PreviousUI").gameObject.SetActive(false);
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            if (CanOpenStorage() && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);
            }

            if (mapIsOpen)
            {
                map.SetActive(false);
                mapIsOpen = false;
            }

            if (upgradeIsOpen)
            {
                upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y - gridOffset);
                upgradeIsOpen = false;
            }
        }
        else
        {
            inventoryOpen = false;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            if (CanOpenStorage() && storageInventory != null)
            {
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            }
            playerInventory.transform.Find("NextUI").gameObject.SetActive(true);
            playerInventory.transform.Find("PreviousUI").gameObject.SetActive(true);
        }
    }

    public void ShowOrHideCrafting()
    {
        if (!craftingIsOpen)
        {
            if (crafting != null)
                crafting.GetComponent<RectTransform>().localPosition = new Vector2(crafting.GetComponent<RectTransform>().localPosition.x, crafting.GetComponent<RectTransform>().localPosition.y + gridOffset);
            craftingIsOpen = true;
            if (mapIsOpen)
            {
                map.SetActive(false);
                mapIsOpen = false;
            }
            if (upgradeIsOpen)
            {
                upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y - gridOffset);
                upgradeIsOpen = false;
            }
        }
        else
        {
            if (crafting != null)
                crafting.GetComponent<RectTransform>().localPosition = new Vector2(crafting.GetComponent<RectTransform>().localPosition.x, crafting.GetComponent<RectTransform>().localPosition.y - gridOffset);
            craftingIsOpen = false;
        }
    }

    public void ShowOrHideMarket()
    {
        if (!marketIsOpen)
        {
            if (market != null)
                market.GetComponent<RectTransform>().localPosition = new Vector2(market.GetComponent<RectTransform>().localPosition.x, market.GetComponent<RectTransform>().localPosition.y + gridOffset);
            marketIsOpen = true;

            if (mapIsOpen)
            {
                map.SetActive(false);
                mapIsOpen = false;
            }
            if (upgradeIsOpen)
            {
                upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y - gridOffset);
                upgradeIsOpen = false;
            }
        }
        else
        {
            if (market != null)
                market.GetComponent<RectTransform>().localPosition = new Vector2(market.GetComponent<RectTransform>().localPosition.x, market.GetComponent<RectTransform>().localPosition.y - gridOffset);
            marketIsOpen = false;
        }
    }

    public void ShowOrHideMap()
    {
        if (!mapIsOpen)
        {
            if (inventoryOpen)
                ShowOrHideInventory();
            /*
            if (marketIsOpen)
                ShowOrHideMarket();
            if (craftingIsOpen)
                ShowOrHideCrafting();
            */
            if (upgradeIsOpen)
                ShowOrHideUpgrades();

            map.SetActive(true);
            mapIsOpen = true;
        }
        else
        {
            map.SetActive(false);
            mapIsOpen = false;
        }
    }

    public void ShowOrHideUpgrades()
    {
        if (!upgradeIsOpen)
        {
            if (inventoryOpen)
                ShowOrHideInventory();
            if (mapIsOpen)
                ShowOrHideMap();

            upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y + gridOffset);
            upgradeIsOpen = true;
        }
        else
        {
            upgrade.GetComponent<RectTransform>().localPosition = new Vector2(upgrade.GetComponent<RectTransform>().localPosition.x, upgrade.GetComponent<RectTransform>().localPosition.y - gridOffset);
            upgradeIsOpen = false;
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

    void CheckIfUiIsOpen(bool isOpened)
    {
        if (isOpened)
        {
            inventoryTrigger = true;
            if (GetComponent<GroundPlayerController>().enabled)
            {
                wasOnGround = true;
                GetComponent<GroundPlayerController>().enabled = false;
            }
            else if (GetComponent<VinePlayerController>().enabled)
            {
                wasOnVine = true;
                GetComponent<VinePlayerController>().enabled = false;
            }
            else if (GetComponent<WaterPlayerController>().enabled)
            {
                wasInWater = true;
                GetComponent<WaterPlayerController>().enabled = false;
            }
        }
        else
        {
            inventoryTrigger = false;
            if (wasOnGround)
            {
                wasOnGround = false;
                GetComponent<GroundPlayerController>().enabled = true;
            }
            else if (wasInWater)
            {
                wasInWater = false;
                GetComponent<WaterPlayerController>().enabled = true;
            }
            else if (wasOnVine)
            {
                wasOnVine = false;
                GetComponent<VinePlayerController>().enabled = true;
            }
        }
    }

    IEnumerator MoveCamera(bool up)
    {
        float timer = 0;
        float duration = 0.1f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (up)
            {
                if (vcam != null)
                    vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = Mathf.Lerp(0.5f, 0.75f, timer / duration);
            }
            else
            {
                if (vcam != null)
                    vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = Mathf.Lerp(0.75f, 0.5f, timer / duration);
            }
            yield return null;
        }
    }

    public bool CanOpenStorage()
    {
        return isInBase && hasBuiltStorage;
    }

    bool IsInUI()
    {
        return inventoryOpen || marketIsOpen || craftingIsOpen || mapIsOpen || upgradeIsOpen;
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

    //Je met �a la pour mettre une mani�re de voir si on est dans la base ou non, tu peux changer �a si tu veux! -Pascal
    /*
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
    */
}
