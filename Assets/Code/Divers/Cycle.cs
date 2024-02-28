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

    void Start()
    {
        TimeLeft = initialTime;
        TimerTxt = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();
        TimerOn = true;
        theCharacter = GameObject.FindGameObjectWithTag("Player");
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
        else
        {
            if (!theCharacter.GetComponent<PlayerPermanent>().isInBase && TimerOn == false)
            {
                TimeLeft = initialTime;
                TimerOn=true;
                Debug.Log("New Cycle");
                this.gameObject.GetComponent<Quota>().nouveauQuota();
            }
        }
    }

    void updateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
