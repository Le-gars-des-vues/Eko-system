using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public float TimeLeft;
    public float initialTime;
    public bool TimerOn = false;
    [SerializeField] List<Sprite> timeLeft = new List<Sprite>();
    bool eightMinLeft;
    bool sixMinLeft;
    bool fourMinLeft;
    bool twoMinLeft;

    public GameObject theCharacter;

    //public List<GameObject> theRooms = new List<GameObject>();
    public List<Planters> planters = new List<Planters>();
    public List<Teleporter> teleporter = new List<Teleporter>();
    public List<Enclos> enclosure = new List<Enclos>();

    [Header("New Cycle Variables")]
    [SerializeField] GameObject newCycleScreen;
    [SerializeField] TextMeshProUGUI newCycleText;
    private string textToWrite;
    public int cycleCount;
    [SerializeField] TextMeshProUGUI cycleMenuText;

    [Header("Spawner Variables")]
    [SerializeField] List<GameObject> dogSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> frogSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> flySpawner = new List<GameObject>();

    [SerializeField] List<GameObject> caeruletamSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> infpisumSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> macrebosiaSpawner = new List<GameObject>();

    [SerializeField] List<GameObject> tugnstoneSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> magnetyneSpawner = new List<GameObject>();
    [SerializeField] List<GameObject> rubiolSpawner = new List<GameObject>();

    [SerializeField] int maxDogCount;
    [SerializeField] int maxFlyCount;
    [SerializeField] int maxFrogCount;

    [SerializeField] int maxInfpisumCount;
    [SerializeField] int maxMacrebosiaCount;
    [SerializeField] int maxCaeruletamCount;

    [SerializeField] int maxRubiolCount;
    [SerializeField] int maxLushaliteCount;
    [SerializeField] int maxTugnstoneCount;

    [Header("Creature Variables")]
    public List<GameObject> dogs = new List<GameObject>();
    public List<GameObject> flys = new List<GameObject>();
    public List<GameObject> frogs = new List<GameObject>();

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    void Start()
    {
        TimeLeft = initialTime;
        TimerOn = false;
        theCharacter = GameObject.FindGameObjectWithTag("Player");
        cycleCount = 1;
        cycleMenuText.text = "DAY " + cycleCount.ToString("000") + "\n_________";

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
            }
            spawner.gameObject.name = spawner.GetComponent<Spawner>().objectName + " spawner " + spawner.GetComponent<Spawner>().index;
        }

        SpawnNewObjects(true);
    }

    void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime;
                updateTimer(TimeLeft);

                if (TimeLeft < 480 && !eightMinLeft)
                    QuickMenu.instance.frame.sprite = timeLeft[1];
                else if (TimeLeft < 360 && !sixMinLeft)
                    QuickMenu.instance.frame.sprite = timeLeft[2];
                else if (TimeLeft < 240 && !fourMinLeft)
                    QuickMenu.instance.frame.sprite = timeLeft[3];
                else if (TimeLeft < 120 && !twoMinLeft)
                    QuickMenu.instance.frame.sprite = timeLeft[4];
            }
            else
            {
                Debug.Log("Time is UP!");
                TimeLeft = 0;
                TimerOn = false;
                if (!theCharacter.GetComponent<PlayerPermanent>().isInBase)
                {
                    theCharacter.GetComponent<PlayerPermanent>().currentHp = 0;
                    theCharacter.GetComponent<PlayerPermanent>().SetBar(theCharacter.GetComponent<PlayerPermanent>().hpSlider, theCharacter.GetComponent<PlayerPermanent>().currentHp);
                }
                else
                {
                    Debug.Log("You are safe, for now.");
                }
            }
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
    }

    public IEnumerator NewCycle()
    {
        newCycleScreen.SetActive(true);
        this.gameObject.GetComponent<Quota>().nouveauQuota();
        cycleCount++;
        cycleMenuText.text = "DAY " + cycleCount.ToString("000") + "\n_________";
        textToWrite = "////////////\n\nSYSTEM.REBOOT\nTERRAFORMA CORP.\n\nNEW TRANSMISSION\n.\n.\n.\n.\n\nGOOD MORNING EMPLOYEE 1212781827!\n.\n.\n.\n.\n\nCYCLE : " + cycleCount.ToString("000") + "\n.\n.\n\nQUOTA: 0 / " + gameObject.GetComponent<Quota>().quota.ToString() + " $\n\nEND TRANSMISSION\n\n/////////////";
        newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", true);

        yield return new WaitForSeconds(1.1f);
        newCycleText.text = "";
        foreach(char letter in textToWrite.ToCharArray())
        {
            newCycleText.text += letter;
            yield return new WaitForSeconds(0.03f);
        }

        yield return new WaitForSeconds(2f);
        newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", false);
        yield return new WaitForSeconds(2f);

        newCycleScreen.SetActive(false);
        TimeLeft = initialTime;
        QuickMenu.instance.frame.sprite = timeLeft[0];
        Debug.Log("New Cycle");
        for (int i = 0; i <= planters.Count - 1; i++)
        {
            planters[i].Grow();
        }

        for (int i = 0; i <= teleporter.Count - 1; i++)
        {
            if (!teleporter[i].isPoweredUp)
                teleporter[i].isPoweredUp = true;
        }
        QuickMenu.instance.CheckForOpenTeleporter();
        SpawnNewObjects(false);
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
    }
}
