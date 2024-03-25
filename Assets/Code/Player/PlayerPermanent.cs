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
    public bool isInAirPocket;

    [Header("Health Variables")]
    public float maxHp;
    public float currentHp;
    public Slider hpSlider;
    private bool isInvincible;
    private Color invisible;
    private List<Material> ogMaterials = new List<Material>();
    [SerializeField] private Material flashMaterial;
    public List<SpriteRenderer> playerGFX = new List<SpriteRenderer>();
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
    public float staminaCountdown;
    private float staminaTime;
    public bool staminaDepleted = false;

    [Header("Shield Variables")]
    public float maxShield = 0;
    public float currentShield = 0;
    public float shieldRegainRate;
    public Slider shieldSlider;
    public float shieldCountdown;
    private float shieldTime;

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
    //public bool survivalMode;
    public GroundPlayerController groundPlayerController;
    public WaterPlayerController waterPlayerController;
    public VinePlayerController vinePlayerController;

    public GameObject objectInRightHand = null;
    public bool isFacingRight;

    public List<GameObject> objectsNear = new List<GameObject>();
    public float nearestObjectDistance = 10;
    public GameObject nearestObject;
    public bool isThrowing = false;

    [SerializeField] MultiTool multiTool;
    public bool isUsingMultiTool;
    [SerializeField] bool spawnAtBase = true;

    public float minDistanceToHarvest;
    public float timeToHarvest;

    public bool isPoisoned;
    public bool colliderShapeIsChanged;

    [Header("Inventory Variables")]
    public bool inventoryOpen = false;
    [SerializeField] private GameObject playerInventory;
    [SerializeField] private GameObject storageInventory;
    [SerializeField] private float gridOffset;
    public bool isInBase;

    [Header("Ragdoll Variables")]
    [SerializeField] private Animator anim;
    [SerializeField] private List<Collider2D> colliders;
    [SerializeField] private List<Rigidbody2D> rbs;
    [SerializeField] private List<HingeJoint2D> joints;
    [SerializeField] private List<LimbSolver2D> limbs;
    [SerializeField] private Rigidbody2D playerRb;
    public CapsuleCollider2D playerCollider;
    public CapsuleCollider2D underWaterCollider;
    [SerializeField] private List<GameObject> bones;
    [SerializeField] private List<Vector3> bonesPosition;
    [SerializeField] private List<Quaternion> bonesRotation;

    private VinePlayerController vineController;
    private GameObject theBase;
    public GameObject gameOverScreen;

    [Header("Upgrade Variables")]
    public bool hasMultitool;
    public bool hasDoubleJump;
    public GameObject flyBackpack;
    public bool hasPunch;
    public GameObject dogFist;
    public bool hasOxygenMask;
    public GameObject frogMask;
    public bool hasShield;
    public bool hasOptics;
    public float hpMultiplier;
    public float staminaMultiplier;
    public float harvestDistanceMultiplier;
    public float harvestTimeDivider;

    [Header("UI Variables")]
    [SerializeField] CinemachineVirtualCamera vcam;
    [SerializeField] private GameObject map;
    [SerializeField] private GameObject noMap;
    [SerializeField] private GameObject market;
    [SerializeField] private GameObject crafting;
    [SerializeField] private GameObject building;
    [SerializeField] private GameObject upgrade;
    [SerializeField] private GameObject room;
    public bool mapIsOpen = true;
    public bool marketIsOpen = false;
    public bool craftingIsOpen = false;
    public bool buildingIsOpen = false;
    public bool upgradeIsOpen = false;
    public bool roomManageIsOpen = false;
    public bool menuIsOpen = false;
    public bool uiOpened;
    public bool cameraTrigger = false;
    [SerializeField] Texture2D[] cursorImages;

    [Header("Base Variables")]
    public bool hasBuiltMap = false;
    public bool hasBuiltStorage = false;

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
        shieldSlider = GameObject.Find("shieldBar").GetComponent<Slider>();
        GameObject.Find("shieldBar").SetActive(false);

        playerInventory = GameObject.Find("Inventaire");
        storageInventory = GameObject.Find("Storage");
        market = GameObject.Find("Vente");
        crafting = GameObject.Find("Crafting");
        building = GameObject.Find("RoomCrafting");
        map = GameObject.Find("Map");
        map.SetActive(false);
        noMap = GameObject.Find("NoMap");
        upgrade = GameObject.Find("Upgrades");
        room = GameObject.Find("RoomMenu");
        gameOverScreen = GameObject.Find("GameOverScreen");
        //Au depart du jeu, on set tout les bars au max et on desactive le ragdoll
        for (int i = 0; i < bones.Count; i++)
        {
            bonesPosition.Add(bones[i].transform.position);
            bonesRotation.Add(bones[i].transform.rotation);
        }

        //On va chercher le script de vigne
        vineController = GetComponent<VinePlayerController>();
        invisible = new Color(255, 255, 255, 0);

        theBase = GameObject.FindGameObjectWithTag("Base");
        vcam = GameObject.Find("Virtual Camera").GetComponent<CinemachineVirtualCamera>();
        Reset();
        ToggleRagdoll(false);
    }

    // Update is called once per frame
    void Update()
    {
        uiOpened = IsInUI();

        if (uiOpened && !cameraTrigger)
        {
            StartCoroutine(MoveCamera("GoUp"));
            cameraTrigger = true;
        }
        else if (!uiOpened && cameraTrigger && !buildingIsOpen)
        {
            StartCoroutine(MoveCamera("GoDown"));
            cameraTrigger = false;
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
            {
                if (isUsingMultiTool)
                {
                    Debug.Log("Unequipping multitool");
                    EquipMultiTool(false);
                }
                isInBase = true;
            }
        }
        else
        {
            if (isInBase)
                isInBase = false;
        }

        //Dans l'eau, l'oxygen descend
        if (GetComponent<WaterPlayerController>().enabled == true && !isInAirPocket)
        {
            currentOxygen -= oxygenDepleteRate * Time.deltaTime;
            SetBar(oxygenSlider, currentOxygen);
        }
        else if ((GetComponent<WaterPlayerController>().enabled == false || isInAirPocket) && currentOxygen < maxOxygen)
        {
            currentOxygen += oxygenRegainRate * Time.deltaTime;
            SetBar(oxygenSlider, currentOxygen);
        }

        //Timer de 2 seconds avec que le joueur commence a regagner de la stamina, reset a chaque fois qu'il utilise de la stamina
        if (Time.time - staminaTime > staminaCountdown && currentStamina <= maxStamina)
        {
            currentStamina += staminaRegainRate * Time.deltaTime;
            SetBar(staminaSlider, currentStamina);
            if (currentStamina >= maxStamina)
                staminaDepleted = false;
        }

        if (hasShield)
        {
            if (Time.time - shieldTime > shieldCountdown && currentShield <= maxShield)
            {
                currentShield += shieldRegainRate * Time.deltaTime;
                SetBar(shieldSlider, currentShield);
            }
        }

        if (!uiOpened)
        {
            CheckForClosestObject();
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
        }

        //Open UI
        if (Input.GetKeyDown(KeyCode.I) && !marketIsOpen && !craftingIsOpen)
            ShowOrHideInventory();

        if (Input.GetKeyDown(KeyCode.M) && !marketIsOpen && !craftingIsOpen)
            ShowOrHideMap();

        if (Input.GetKeyDown(KeyCode.U) && !marketIsOpen && !craftingIsOpen)
            ShowOrHideUpgrades();

        if (Input.GetKeyDown(KeyCode.LeftAlt) && !marketIsOpen && !craftingIsOpen && hasMultitool)
        {
            if (isInBase)
            {
                ShowOrHideBuilding();
            }
        }

        if (currentHp <= 0)
        {
            Death();
        }
    }

    //Pour remettre tout les values au maximum
    public void Reset()
    {
        if (spawnAtBase)
        {
            transform.position = theBase.GetComponent<Base>().baseSpawnPoint.position;
            theBase.GetComponent<Base>().isInside = true;
        }

        //currentHunger = maxHunger;
        //currentThirst = maxThirst;
        //SetMaxBar(hungerSlider, maxHunger);
        //SetMaxBar(thirstSlider, maxThirst);

        currentOxygen = maxOxygen;
        currentHp = maxHp;
        currentStamina = maxStamina;
        currentShield = maxShield;
        SetMaxBar(oxygenSlider, maxOxygen);
        SetMaxBar(hpSlider, maxHp);
        SetMaxBar(staminaSlider, maxStamina);
        SetMaxBar(shieldSlider, maxShield);

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
        if (objectInRightHand != null && !isUsingMultiTool)
        {
            objectInRightHand.GetComponent<PickableObject>().isPickedUp = false;
            UnequipObject();
        }

        gameOverScreen.SetActive(true);
        gameOverScreen.GetComponent<GameOverScreen>().player = gameObject;
    }

    public void CheckForClosestObject()
    {
        //Pick up ressources
        if (objectsNear.Count >= 1)
        {
            foreach (var obj in objectsNear)
            {
                if (obj != null)
                {
                    float distance = Vector2.Distance(transform.position, obj.transform.position);

                    if (distance < nearestObjectDistance || nearestObject == obj)
                    {
                        nearestObjectDistance = distance;
                        nearestObject = obj;
                        obj.GetComponent<PickableObject>().isSelected = true;
                    }
                    else
                        obj.GetComponent<PickableObject>().isSelected = false;
                }
            }
        }
        else
            nearestObjectDistance = 10;
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
        if (buildingIsOpen)
            ShowOrHideBuilding();
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
        staminaTime = Time.time;
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

                if (hasShield)
                {
                    shieldTime = Time.time;
                    if (Mathf.Abs(value) <= currentShield)
                    {
                        currentShield += value;
                        SetBar(shieldSlider, currentShield);
                    }
                    else if (Mathf.Abs(value) > currentShield)
                    {
                        float restOfDamage = Mathf.Abs(value) - currentShield;
                        currentShield -= currentShield;
                        SetBar(shieldSlider, currentShield);

                        currentHp -= restOfDamage;
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
            Cursor.SetCursor(cursorImages[0], new Vector2(40, 35), CursorMode.Auto);
            multiTool.UseMultiTool(true);
        }
        else
        {
            objectInRightHand = null;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
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
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);

            if (mapIsOpen)
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
        }
        else
        {
            inventoryOpen = false;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            if (CanOpenStorage() && storageInventory != null)
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
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
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y + gridOffset);

            if (mapIsOpen)
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
        }
        else
        {
            inventoryOpen = false;
            playerInventory.GetComponent<RectTransform>().localPosition = new Vector2(playerInventory.GetComponent<RectTransform>().localPosition.x, playerInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);
            if (CanOpenStorage() && storageInventory != null)
                storageInventory.GetComponent<RectTransform>().localPosition = new Vector2(storageInventory.GetComponent<RectTransform>().localPosition.x, storageInventory.GetComponent<RectTransform>().localPosition.y - gridOffset);

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
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
        }
        else
        {
            if (crafting != null)
                crafting.GetComponent<RectTransform>().localPosition = new Vector2(crafting.GetComponent<RectTransform>().localPosition.x, crafting.GetComponent<RectTransform>().localPosition.y - gridOffset);
            craftingIsOpen = false;
        }
    }

    public void ShowOrHideBuilding()
    {
        if (!buildingIsOpen)
        {
            if (!cameraTrigger)
            {
                cameraTrigger = true;
                StartCoroutine(MoveCamera("ZoomOut"));
            }
            else
            {
                vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.5f;
                StartCoroutine(MoveCamera("ZoomOut"));
            }
            if (building != null)
                building.GetComponent<RectTransform>().localPosition = new Vector2(building.GetComponent<RectTransform>().localPosition.x, building.GetComponent<RectTransform>().localPosition.y + gridOffset);
            buildingIsOpen = true;

            if (inventoryOpen)
                ShowOrHideInventory();
            if (mapIsOpen)
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
        }
        else
        {
            if (cameraTrigger)
            {
                cameraTrigger = false;
                StartCoroutine(MoveCamera("ZoomIn"));
            }
            if (building != null)
                building.GetComponent<RectTransform>().localPosition = new Vector2(building.GetComponent<RectTransform>().localPosition.x, building.GetComponent<RectTransform>().localPosition.y - gridOffset);
            buildingIsOpen = false;
        }
    }

    public void ShowOrHideRoomManagement()
    {
        if (!roomManageIsOpen)
        {
            if (room != null)
                room.GetComponent<RectTransform>().localPosition = new Vector2(room.GetComponent<RectTransform>().localPosition.x, room.GetComponent<RectTransform>().localPosition.y + gridOffset);
            roomManageIsOpen = true;

            if (mapIsOpen)
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }
        }
        else
        {
            if (room != null)
                room.GetComponent<RectTransform>().localPosition = new Vector2(room.GetComponent<RectTransform>().localPosition.x, room.GetComponent<RectTransform>().localPosition.y - gridOffset);
            roomManageIsOpen = false;
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
                ShowOrHideMap();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
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
            if (marketIsOpen)
                ShowOrHideMarket();
            if (craftingIsOpen)
                ShowOrHideCrafting();
            if (upgradeIsOpen)
                ShowOrHideUpgrades();
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }

            if (hasBuiltMap)
                map.SetActive(true);
            else
                noMap.SetActive(true);
            mapIsOpen = true;
        }
        else
        {
            if (hasBuiltMap)
                map.SetActive(false);
            else
                noMap.SetActive(false);
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
            if (marketIsOpen)
                ShowOrHideMarket();
            if (craftingIsOpen)
                ShowOrHideCrafting();
            if (roomManageIsOpen)
                ShowOrHideRoomManagement();
            if (buildingIsOpen)
            {
                if (isUsingMultiTool)
                    EquipMultiTool(false);
                ShowOrHideBuilding();
            }

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

    IEnumerator MoveCamera(string effect)
    {
        float timer = 0;
        float duration = 0.1f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            if (effect == "GoUp")
            {
                if (vcam != null)
                    vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = Mathf.Lerp(0.5f, 0.75f, timer / duration);
            }
            else if (effect == "GoDown")
            {
                if (vcam != null)
                    vcam.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = Mathf.Lerp(0.75f, 0.5f, timer / duration);
            }
            else if (effect == "ZoomIn")
            {
                if (vcam != null)
                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(20f, 8.2f, timer / duration);
            }
            else if (effect == "ZoomOut")
            {
                if (vcam != null)
                    vcam.m_Lens.OrthographicSize = Mathf.Lerp(8.2f, 20f, timer / duration);
            }
            yield return null;
        }
    }

    public IEnumerator Poison(float duration, float tickDamage, float tickInterval)
    {
        if (hasShield && currentShield > 0)
        {
            yield return null;
        }
        else
        {
            hpSlider.gameObject.GetComponent<Animator>().SetBool("isPoisoned", true);
            float timer = 0;
            while (timer < duration)
            {
                timer += tickInterval;
                ChangeHp(-tickDamage, false);
                yield return new WaitForSeconds(tickInterval);
            }
            hpSlider.gameObject.GetComponent<Animator>().SetBool("isPoisoned", false);
        }
    }

    public void ChangeColliderShape(bool circle)
    {
        if (circle)
        {
            colliderShapeIsChanged = true;
            playerCollider.direction = CapsuleDirection2D.Horizontal;
            playerCollider.size = new Vector2(playerCollider.size.x, 1);
        }
        else
        {
            colliderShapeIsChanged = false;
            playerCollider.direction = CapsuleDirection2D.Vertical;
            playerCollider.size = new Vector2(playerCollider.size.x, 2);
        }
    }

    public bool CanOpenStorage()
    {
        return isInBase && hasBuiltStorage;
    }

    bool IsInUI()
    {
        return inventoryOpen || mapIsOpen || upgradeIsOpen || marketIsOpen || roomManageIsOpen || craftingIsOpen;
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
