using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Ambient Noises")]
    public AK.Wwise.RTPC soundtrackRTPC;
    [SerializeField] [Range(0, 30)] float rtpcValue;
    public AK.Wwise.Event forestSountrack;
    public bool forestIsPlaying;
    uint playingID;

    [Header("Base Sounds")]
    public AK.Wwise.Event porteOuverture;
    public AK.Wwise.Event porteFermeture;

    [Header("UI Sounds")]
    //Inventaire
    public AK.Wwise.Event inventaireOuvrir;
    public AK.Wwise.Event inventaireSwap;
    public AK.Wwise.Event inventairePickUp;
    public AK.Wwise.Event inventairePlace;

    //Crafting
    public AK.Wwise.Event craftingOuvrir;
    public AK.Wwise.Event craftingFermer;

    //Map
    public AK.Wwise.Event carteOuvrir;
    public AK.Wwise.Event upgradeOuvrir;

    //Vente
    public AK.Wwise.Event sellingScreenOpen;
    public AK.Wwise.Event sellingScreenClose;
    public AK.Wwise.Event sellingScreenSell;

    //Misc
    public AK.Wwise.Event uiFermer;
    public AK.Wwise.Event uiTexte;
    public AK.Wwise.Event uiTexteFin;
    public AK.Wwise.Event hotbar;
    public AK.Wwise.Event menuSwitch;

    //QuickMenu
    public AK.Wwise.Event quickeMenuOpen;
    public AK.Wwise.Event quickMenuClose;
    public AK.Wwise.Event quickMenuPick;
    public AK.Wwise.Event info;
    public AK.Wwise.Event messages;
    public AK.Wwise.Event teleportMenu;

    //GameOver
    public AK.Wwise.Event backToBase;
    public AK.Wwise.Event backToMainMenu;
    public AK.Wwise.Event gameOverTimer;
    public AK.Wwise.Event gameOverDeath;

    [Header("Player Sounds")]
    //Player UI
    public AK.Wwise.Event noStamina;
    public AK.Wwise.Event noOxygen;
    public AK.Wwise.Event timerRunningOut;
    public AK.Wwise.Event lowHealth;
    public AK.Wwise.Event gainHealth;

    //Player state
    public AK.Wwise.Event playerTakeDamange;
    public AK.Wwise.Event playerClimb;
    public AK.Wwise.Event playerClimbStop;

    //VO Player
    public AK.Wwise.Event voAttack;
    public AK.Wwise.Event voDeath;
    public AK.Wwise.Event voDamage;
    public AK.Wwise.Event voDrown;
    public AK.Wwise.Event voJump;
    public AK.Wwise.Event voBreath;


    [Header("Exploration Sounds")]
    //Underwater
    public AK.Wwise.Event nage;
    public AK.Wwise.Event waterSplash;

    //Multitool
    public AK.Wwise.Event multitoolCharge;
    public AK.Wwise.Event multitoolChargeStop;

    //General
    public AK.Wwise.Event pickUp;
    public AK.Wwise.Event teleport;
    public AK.Wwise.Event aim;
    public AK.Wwise.Event aimStop;
    public AK.Wwise.Event storm;

    [Header("Hazards Sounds")]
    public AK.Wwise.Event spikeTrap;
    public AK.Wwise.Event thorns;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    private void Update()
    {
        soundtrackRTPC.SetValue(gameObject, rtpcValue);
    }

    public void StopSoundtrack()
    {
        AkSoundEngine.StopPlayingID(playingID);
    }

    public void PlaySoundtrack(AK.Wwise.Event myEvent)
    {
        AkSoundEngine.StopPlayingID(playingID);
        playingID = myEvent.Post(gameObject);
    }

    public uint PlaySound(AK.Wwise.Event myEvent, GameObject obj)
    {
         return myEvent.Post(obj);
    }
}
