using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Cycle : MonoBehaviour
{
    public float TimeLeft;
    public float initialTime;
    public bool TimerOn = false;

    public GameObject theCharacter;

    public TextMeshProUGUI TimerTxt;

    public GameObject[] theRooms;

    [Header("New Cycle Variables")]
    [SerializeField] GameObject newCycleScreen;
    [SerializeField] TextMeshProUGUI newCycleText;
    private string textToWrite;
    int cycleCount;

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

    [SerializeField] int dogSpawnerFraction;
    [SerializeField] int flySpawnerFraction;
    [SerializeField] int frogSpawnerFraction;

    [SerializeField] int infpisumSpawnerFraction;
    [SerializeField] int macrebosiaSpawnerFraction;
    [SerializeField] int caeruletamSpawnerFraction;

    [SerializeField] int rubiolSpawnerFraction;
    [SerializeField] int magnetyneSpawnerFraction;
    [SerializeField] int tugnstoneSpawnerFraction;

    void Start()
    {
        TimeLeft = initialTime;
        TimerTxt = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        TimerOn = true;
        theCharacter = GameObject.FindGameObjectWithTag("Player");
        cycleCount = 1;

        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Spawner");
        foreach(GameObject spawner in spawners)
        {
            switch (spawner.GetComponent<Spawner>().objectName)
            {
                case "Dog":
                    dogSpawner.Add(spawner);
                    break;
                case "Fly":
                    flySpawner.Add(spawner);
                    break;
                case "Frog":
                    frogSpawner.Add(spawner);
                    break;
                case "Macrebosia":
                    macrebosiaSpawner.Add(spawner);
                    break;
                case "Caeruletam":
                    caeruletamSpawner.Add(spawner);
                    break;
                case "Infpisum":
                    infpisumSpawner.Add(spawner);
                    break;
                case "Magnetyne":
                    magnetyneSpawner.Add(spawner);
                    break;
                case "Rubiol":
                    rubiolSpawner.Add(spawner);
                    break;
                case "Tugnstone":
                    tugnstoneSpawner.Add(spawner);
                    break;
            }
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
        /*
        else
        {
            if (!theCharacter.GetComponent<PlayerPermanent>().isInBase && TimerOn == false)
            {
                TimeLeft = initialTime;
                TimerOn=true;
                Debug.Log("New Cycle");
                this.gameObject.GetComponent<Quota>().nouveauQuota();
                for(int i=0; i <= theRooms.Length - 1; i++)
                {
                    theRooms[i].GetComponent<RoomCrafters>().SpawnExtraItem();
                }
            }
        }
        */
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public IEnumerator NewCycle()
    {
        newCycleScreen.SetActive(true);
        this.gameObject.GetComponent<Quota>().nouveauQuota();
        textToWrite = "////////////<br><br>SYSTEM.REBOOT<br>TERRAFORMA CORP.<br><br>NEW TRANSMISSION<br>.<br>.<br>.<br>.<br><br>GOOD MORNING EMPLOYEE 1212781827!<br>.<br>.<br>.<br>.<br><br>CYCLE : " + cycleCount.ToString("000") + "<br>.<br>.<br><br>QUOTA: 0 / " + gameObject.GetComponent<Quota>().quota.ToString() + " $<br><br>END TRANSMISSION<br><br>/////////////";
        newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", true);
        yield return new WaitForSeconds(3f);
        newCycleText.text = "";
        foreach(char letter in textToWrite.ToCharArray())
        {
            newCycleText.text += letter;
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        newCycleScreen.GetComponent<Animator>().SetBool("fadeIn", false);
        TimeLeft = initialTime;
        TimerOn = true;
        Debug.Log("New Cycle");
        for (int i = 0; i <= theRooms.Length - 1; i++)
        {
            theRooms[i].GetComponent<RoomCrafters>().SpawnExtraItem();
        }
        newCycleScreen.SetActive(false);
        cycleCount++;
        SpawnNewObjects(false);
    }

    void SpawnNewObjects(bool start)
    {
        //Dog
        int targetNumber = dogSpawner.Count / dogSpawnerFraction;
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
                if (spawner.GetComponent<Spawner>().objectSpawned.GetComponent<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
                spawner.Spawn();
            }
        }

        //Fly
        targetNumber = flySpawner.Count / flySpawnerFraction;
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
                if (spawner.GetComponent<Spawner>().objectSpawned.GetComponent<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //Frog
        targetNumber = frogSpawner.Count / frogSpawnerFraction;
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
                if (spawner.GetComponent<Spawner>().objectSpawned.GetComponent<CreatureDeath>().isDead || !spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }
        //Infpisum
        targetNumber = infpisumSpawner.Count / infpisumSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //Macrebosia
        targetNumber = macrebosiaSpawner.Count / macrebosiaSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //Caeruletam
        targetNumber = caeruletamSpawner.Count / caeruletamSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //rubiol
        targetNumber = rubiolSpawner.Count / rubiolSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //magnetyne
        targetNumber = magnetyneSpawner.Count / magnetyneSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }

        //tugnstone
        targetNumber = tugnstoneSpawner.Count / tugnstoneSpawnerFraction;
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
                if (!spawner.GetComponent<Spawner>().objectSpawned.activeSelf)
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
            }
        }
    }
}
