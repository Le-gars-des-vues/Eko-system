using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using System.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float TimeLeft;
    public float initialTime;
    public bool TimerOn = false;
    [SerializeField] List<Sprite> timeLeft = new List<Sprite>();
    public bool eightMinLeft = false;
    public bool sixMinLeft = false;
    public bool fourMinLeft = false;
    public bool twoMinLeft = false;

    public PlayerPermanent player;

    //public List<GameObject> theRooms = new List<GameObject>();
    public List<Planters> planters = new List<Planters>();
    public List<Teleporter> teleporter = new List<Teleporter>();
    public List<Enclos> enclosure = new List<Enclos>();

    [Header("New Cycle Variables")]
    [SerializeField] GameObject newCycleScreen;
    [SerializeField] TextMeshProUGUI newCycleText;
    [SerializeField] TextMeshProUGUI terminationText;
    private string textToWrite;
    public int cycleCount;
    [SerializeField] TextMeshProUGUI cycleMenuText;

    [Header("Spawner Variables")]
    [SerializeField] List<GameObject> dogSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> frogSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> flySpawner = new List<GameObject>();
    [SerializeField] List<GameObject> fishSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> sharkSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> jellyfishSpawner = new List<GameObject>();

    [SerializeField] List<GameObject> caeruletamSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> infpisumSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> macrebosiaSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> coralliumSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> aquaboolbusSpawner = new List<GameObject>();

    [SerializeField] List<GameObject> tugnstoneSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> magnetyneSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> rubiolSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> petralucenSpawner = new List<GameObject>();

    [SerializeField] int maxDogCount;
    [SerializeField] int maxFlyCount;
    [SerializeField] int maxFrogCount;
    [SerializeField] int maxFishCount;
    [SerializeField] int maxJellyfishCount;
    [SerializeField] int maxSharkCount;

    [SerializeField] int maxInfpisumCount;
    [SerializeField] int maxMacrebosiaCount;
    [SerializeField] int maxCaeruletamCount;
    [SerializeField] int maxAquaboolbusCount;
    [SerializeField] int maxCoralliumCount;

    [SerializeField] int maxRubiolCount;
    [SerializeField] int maxLushaliteCount;
    [SerializeField] int maxTugnstoneCount;
    [SerializeField] int maxPetralucenCount;

    [Header("Creature Variables")]
    public List<GameObject> dogs = new List<GameObject>();
    public List<GameObject> flys = new List<GameObject>();
    public List<GameObject> frogs = new List<GameObject>();
    public List<GameObject> fishes = new List<GameObject>();
    public List<GameObject> jellyfishes = new List<GameObject>();
    public List<GameObject> sharks = new List<GameObject>();

    [Header("Storm Variables")]
    [SerializeField] float timeToFullStorm;
    [SerializeField] float timer;
    GameObject storm;
    [SerializeField] ParticleSystem rainFront;
    [SerializeField] ParticleSystem rainGround;
    [SerializeField] ParticleSystem rainBack;
    [SerializeField] Volume stormVolume;
    [SerializeField] Animator lightningFlash;
    [SerializeField] float lightningCooldown = 20;
    [SerializeField] float lightningTime;
    public bool isStorm;
    uint stormSoundID;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        SceneLoader.allScenesLoaded += StartScript;
    }

    void StartScript()
    {
        TimeLeft = initialTime;
        TimerOn = false;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        cycleCount = 0;
        cycleMenuText.text = "DAY " + cycleCount.ToString("000") + "\n_________";

        storm = GameObject.Find("Storm");
        rainFront = storm.transform.GetChild(0).GetComponent<ParticleSystem>();
        rainGround = storm.transform.GetChild(1).GetComponent<ParticleSystem>();
        rainBack = storm.transform.GetChild(2).GetComponent<ParticleSystem>();
        stormVolume = storm.transform.GetChild(3).GetComponent<Volume>();
        lightningFlash = storm.transform.GetChild(4).GetComponent<Animator>();

        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach(GameObject spawner in spawners)
        {
            switch (spawner.GetComponent<Spawner>().objectName)
            {
                case "Dog":
                    dogSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = dogSpawner.Count;
                    break;
                case "Fly":
                    flySpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = flySpawner.Count;
                    break;
                case "Frog":
                    frogSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = frogSpawner.Count;
                    break;
                case "Fish":
                    fishSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = fishSpawner.Count;
                    break;
                case "Jellyfish":
                    jellyfishSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = jellyfishSpawner.Count;
                    break;
                case "Shark":
                    sharkSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = sharkSpawner.Count;
                    break;
                case "Macrebosia":
                    macrebosiaSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = macrebosiaSpawner.Count;
                    break;
                case "Caeruletam":
                    caeruletamSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = caeruletamSpawner.Count;
                    break;
                case "Infpisum":
                    infpisumSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = infpisumSpawner.Count;
                    break;
                case "Corallium":
                    coralliumSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = coralliumSpawner.Count;
                    break;
                case "Aquaboolbus":
                    aquaboolbusSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = aquaboolbusSpawner.Count;
                    break;
                case "Magnetyne":
                    magnetyneSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = magnetyneSpawner.Count;
                    break;
                case "Rubiol":
                    rubiolSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = rubiolSpawner.Count;
                    break;
                case "Tugnstone":
                    tugnstoneSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = tugnstoneSpawner.Count;
                    break;
                case "Petralucen":
                    petralucenSpawner.Add(spawner);
                    spawner.GetComponent<Spawner>().index = petralucenSpawner.Count;
                    break;
            }
            spawner.gameObject.name = spawner.GetComponent<Spawner>().objectName + " spawner " + spawner.GetComponent<Spawner>().index;
        }

        SpawnNewObjects(true);
    }

    void Update()
    {
        if (SceneLoader.instance.isLoading) return;

        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);

                if (TimeLeft < 120 && !twoMinLeft)
                {
                    twoMinLeft = true;
                    QuickMenu.instance.frame.sprite = timeLeft[4];
                    //QuickMenu.instance.anim.SetBool("isBlinking", true);
                }
                else if (TimeLeft < 240 && !fourMinLeft)
                {
                    QuickMenu.instance.frame.sprite = timeLeft[3];
                    fourMinLeft = true;
                }
                else if (TimeLeft < 360 && !sixMinLeft)
                {
                    QuickMenu.instance.frame.sprite = timeLeft[2];
                    sixMinLeft = true;
                }
                else if (TimeLeft < 480 && !eightMinLeft)
                {
                    QuickMenu.instance.frame.sprite = timeLeft[1];
                    eightMinLeft = true;
                }

                if (twoMinLeft && !Base.instance.isInside)
                {
                    if (!isStorm)
                    {
                        Storm(true);
                    }
                }
            }
            else
            {
                Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                if (!player.isInBase)
                {
                    player.Death();
                    AudioManager.instance.PlaySound(AudioManager.instance.gameOverTimer, Camera.main.gameObject);
                }
                else
                {
                    Debug.Log("You are safe, for now.");
                }
            }
        }

        if (twoMinLeft)
        {
            timer += Time.deltaTime;

            if (isStorm)
            {
                AudioManager.instance.soundtracks[3].GetComponent<TempeteRTPC>().rtpcValue -= Time.deltaTime;
                stormVolume.weight = Mathf.Lerp(0, 1, timer / timeToFullStorm);
                var emissionF = rainFront.emission;
                var emissionG = rainGround.emission;
                var emissionB = rainBack.emission;
                emissionF.rateOverTime = Mathf.Lerp(0, 500, timer / timeToFullStorm);
                emissionG.rateOverTime = Mathf.Lerp(0, 625, timer / timeToFullStorm);
                emissionB.rateOverTime = Mathf.Lerp(0, 1000, timer / timeToFullStorm);

                if (Time.time - lightningTime > lightningCooldown)
                {
                    lightningTime = Time.time;
                    lightningFlash.SetTrigger("Lightning");
                    float minCooldown = Mathf.Lerp(20, 2, timer / timeToFullStorm);
                    float maxCooldown = Mathf.Lerp(30, 5, timer / timeToFullStorm);
                    lightningCooldown = Random.Range(minCooldown, maxCooldown);
                    AudioManager.instance.PlaySound(AudioManager.instance.thunder, gameObject);
                }
            }
        }
    }

    public void Storm(bool isTrue)
    {
        isStorm = isTrue;
        if (isTrue)
        {
            lightningTime = Time.time;
            rainFront.Play();
            rainGround.Play();
            rainBack.Play();
            Debug.Log("Started Storm");
            AudioManager.instance.PlayStorm();
        }
        else
        {
            AkSoundEngine.StopPlayingID(stormSoundID);
            rainFront.Stop();
            rainGround.Stop();
            rainBack.Stop();
            stormVolume.weight = 0;
            Debug.Log("Stopped Storm");
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        QuickMenu.instance.timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartNewCycle()
    {
        StartCoroutine(NewCycle());
        Debug.Log("Started new cycle!");
    }

    void ResetTimer()
    {
        TimeLeft = initialTime;
        QuickMenu.instance.frame.sprite = timeLeft[0];
        timer = 0;
        twoMinLeft = false;
        fourMinLeft = false;
        sixMinLeft = false;
        eightMinLeft = false;
    }

    public IEnumerator NewCycle()
    {
        newCycleScreen.SetActive(true);
        if (GameObject.Find("Vente").GetComponent<Vente>().profit < gameObject.GetComponent<Quota>().quota)
        {
            textToWrite = "[WARNING : INSUFFICIENT FUNDS]\nTERMINATION IMMINENT";
            newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", true);
            yield return new WaitForSeconds(1.1f);
            terminationText.text = "";
            AudioManager.instance.PlaySound(AudioManager.instance.uiTexte, Camera.main.gameObject);
            foreach (char letter in textToWrite.ToCharArray())
            {
                terminationText.text += letter;
                yield return new WaitForSeconds(0.03f);
            }
            AudioManager.instance.PlaySound(AudioManager.instance.uiTexteFin, Camera.main.gameObject);
            yield return new WaitForSeconds(2f);
            player.Death();
            GameObject.Find("GameOverScreen").transform.Find("BackToBase").gameObject.SetActive(false);
        }
        else
        {
            Storm(false);
            AudioManager.instance.soundtracks[3].GetComponent<TempeteRTPC>().rtpcValue = 120;
            this.gameObject.GetComponent<Quota>().nouveauQuota();
            cycleCount++;
            switch (cycleCount)
            {
                case 2:
                    Tutorial.instance.day2 = true;
                    break;
                case 3:
                    Tutorial.instance.day3 = true;
                    break;
                default:
                    break;
            }
            cycleMenuText.text = "DAY " + cycleCount.ToString("000") + "\n_________";
            textToWrite = "////////////\n\nSYSTEM.REBOOT\nTERRAFORMA CORP.\n\nNEW TRANSMISSION\n.\n.\n.\n.\n\nGOOD MORNING EMPLOYEE 1212781827!\n.\n.\n.\n.\n\nCYCLE : " + cycleCount.ToString("000") + "\n.\n.\n\nQUOTA: 0 / " + gameObject.GetComponent<Quota>().quota.ToString() + " $\n\nEND TRANSMISSION\n\n/////////////";
            newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", true);
            yield return new WaitForSeconds(1.1f);
            newCycleText.text = "";
            AudioManager.instance.PlaySound(AudioManager.instance.uiTexte, Camera.main.gameObject);
            foreach (char letter in textToWrite.ToCharArray())
            {
                newCycleText.text += letter;
                yield return new WaitForSeconds(0.03f);
            }
            AudioManager.instance.PlaySound(AudioManager.instance.uiTexteFin, Camera.main.gameObject);
            yield return new WaitForSeconds(2f);
            newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", false);
            yield return new WaitForSeconds(2f);

            newCycleScreen.SetActive(false);

            ResetTimer();

            Debug.Log("New Cycle");
            for (int i = 0; i <= planters.Count - 1; i++)
            {
                planters[i].Grow();
            }
            for (int i = 0; i <= enclosure.Count - 1; i++)
            {
                enclosure[i].Grow();
            }

            for (int i = 0; i <= teleporter.Count - 1; i++)
            {
                if (!teleporter[i].isPoweredUp)
                    teleporter[i].isPoweredUp = true;
            }
            QuickMenu.instance.CheckForOpenTeleporter();
            SpawnNewObjects(false);
        }
    }

    void SpawnNewObjects(bool start)
    {
        //Dog
        int targetNumber = maxDogCount;
        int alreadyInGame = 0;
        List<Spawner> availableSpawners = new List<Spawner>();

        foreach (GameObject spawner in dogSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        int numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(dogs);
                availableSpawners.Remove(spawner);
            }
        }

        //Fly
        targetNumber = maxFlyCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in flySpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(flys);
                availableSpawners.Remove(spawner);
            }
        }

        //Frog
        targetNumber = maxFrogCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in frogSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(frogs);
                availableSpawners.Remove(spawner);
            }
        }

        //Fish
        targetNumber = maxFishCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in fishSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(fishes);
                availableSpawners.Remove(spawner);
            }
        }

        //Jellyfish
        targetNumber = maxJellyfishCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in jellyfishSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(jellyfishes);
                availableSpawners.Remove(spawner);
            }
        }

        //Shark
        targetNumber = maxSharkCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in sharkSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && (spawner.GetComponent<Spawner>().objectSpawned.GetComponentInChildren<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf))
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn(sharks);
                availableSpawners.Remove(spawner);
            }
        }

        //Infpisum
        targetNumber = maxInfpisumCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in infpisumSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //Macrebosia
        targetNumber = maxMacrebosiaCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in macrebosiaSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //Caeruletam
        targetNumber = maxCaeruletamCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in caeruletamSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //Corallium
        targetNumber = maxCoralliumCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in coralliumSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //Aquaboolbus
        targetNumber = maxAquaboolbusCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in aquaboolbusSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //rubiol
        targetNumber = maxRubiolCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in rubiolSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //magnetyne
        targetNumber = maxLushaliteCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in magnetyneSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //tugnstone
        targetNumber = maxTugnstoneCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in tugnstoneSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }

        //Petralucen
        targetNumber = maxPetralucenCount;
        alreadyInGame = 0;
        availableSpawners.Clear();
        foreach (GameObject spawner in petralucenSpawner)
        {
            if (start)
            {
                spawner.GetComponent<Spawner>().canSpawn = true;
                availableSpawners.Add(spawner.GetComponent<Spawner>());
            }
            else
            {
                if (spawner.GetComponent<Spawner>().objectSpawned != null && spawner.GetComponent<Spawner>().objectSpawned.GetComponent<HarvestableRessourceNode>().isHarvested)
                {
                    spawner.GetComponent<Spawner>().canSpawn = true;
                    availableSpawners.Add(spawner.GetComponent<Spawner>());
                }
                else
                {
                    spawner.GetComponent<Spawner>().canSpawn = false;
                    alreadyInGame++;
                }
            }
        }
        numberToSpawn = targetNumber - alreadyInGame;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                var spawner = availableSpawners[Random.Range(0, availableSpawners.Count - 1)];
                spawner.Spawn();
                availableSpawners.Remove(spawner);
            }
        }
    }
}
