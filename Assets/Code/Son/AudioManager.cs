using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public List<GameObject> soundtracks = new List<GameObject>();

    [Header("Ambient Noises")]
    public AK.Wwise.Event forestSountrack;
    public AK.Wwise.Event baseSoundtrack;
    public AK.Wwise.Event underwaterSoundtrack;
    public AK.Wwise.Event stormSoundtrack;
    public bool forestIsPlaying;
    public bool baseIsPlaying;
    public bool underwaterIsPlaying;
    public bool stormIsPlaying;
    public bool alertIsPlaying;
    public uint underwaterID;
    public uint playingID;

    [Header("Base Sounds")]
    public AK.Wwise.Event porteOuverture;
    public AK.Wwise.Event porteFermeture;
    public AK.Wwise.Event robotEnv;
    public AK.Wwise.Event teleporteurEnv;
    public AK.Wwise.Event elevatorSound;
    public AK.Wwise.Event elevatorSoundStop;
    public AK.Wwise.Event cubiculeOpen;
    public AK.Wwise.Event cubiculeClose;

    [Header("UI Sounds")]
    //Inventaire
    public AK.Wwise.Event inventaireOuvrir;
    public AK.Wwise.Event inventaireSwap;
    public AK.Wwise.Event inventairePickUp;
    public AK.Wwise.Event inventairePlace;
    public AK.Wwise.Event inventaireFermer;

    //Crafting
    public AK.Wwise.Event craftingOuvrir;
    public AK.Wwise.Event craftingFermer;
    public AK.Wwise.Event craftingUpgrade;
    public AK.Wwise.Event craftingWeapon;
    public AK.Wwise.Event craftingEquipment;
    public AK.Wwise.Event craftingBuilding;
    public AK.Wwise.Event craftingRessource;

    //Map
    public AK.Wwise.Event carteOuvrir;
    public AK.Wwise.Event carteFermer;

    //Vente
    public AK.Wwise.Event sellingScreenOpen;
    public AK.Wwise.Event sellingScreenClose;
    public AK.Wwise.Event sellingScreenSell;
    public AK.Wwise.Event sellingScreenCasino;

    //Upgrade
    public AK.Wwise.Event upgradeOuvrir;
    public AK.Wwise.Event upgradeFermer;

    //Building
    public AK.Wwise.Event buildingOuvrir;
    public AK.Wwise.Event buildingFermer;

    //Misc
    public AK.Wwise.Event uiTexte;
    public AK.Wwise.Event uiTexteFin;
    public AK.Wwise.Event hotbar;
    public AK.Wwise.Event menuSwitch;
    public AK.Wwise.Event newDiscovery;
    public AK.Wwise.Event textMessage;
    public AK.Wwise.Event yesButton;
    public AK.Wwise.Event noButton;

    //QuickMenu
    public AK.Wwise.Event quickeMenuOpen;
    public AK.Wwise.Event quickMenuClose;
    public AK.Wwise.Event quickMenuPick;

    //GameOver
    public AK.Wwise.Event backToBase;
    public AK.Wwise.Event backToMainMenu;
    public AK.Wwise.Event gameOverTimer;
    public AK.Wwise.Event gameOverDeath;

    //Pause menu
    public AK.Wwise.Event pauseMenuOpen;
    public AK.Wwise.Event pauseMenuClose;

    //Main menu
    public AK.Wwise.Event mainMenuDesktop;
    public AK.Wwise.Event mainMenuPlay;
    public AK.Wwise.Event mainMenuHover;

    [Header("Player Sounds")]
    public AK.Wwise.Event playerSpawn;

    //Player UI
    public AK.Wwise.Event noStamina;
    public AK.Wwise.Event noOxygen;
    public AK.Wwise.Event timerRunningOut;
    public AK.Wwise.Event lowHealth;
    public AK.Wwise.Event gainHealth;
    public AK.Wwise.Event fullHealth;

    //Consummable
    public AK.Wwise.Event healLeaf;
    public AK.Wwise.Event cleanseFruit;
    public AK.Wwise.Event invisibleFruit;

    //Poison
    public AK.Wwise.Event poison;
    public AK.Wwise.Event poisonStop;

    //Shield
    public AK.Wwise.Event fullShield;
    public AK.Wwise.Event emptyShield;
    public AK.Wwise.Event shieldDamage;

    //Player state
    public AK.Wwise.Event playerTakeDamange;
    public AK.Wwise.Event playerClimb;
    public AK.Wwise.Event playerClimbStop;
    public AK.Wwise.Event playerHang;

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
    public AK.Wwise.Event bubblePlant;
    public AK.Wwise.Event bubblePlantStop;

    //Multitool
    public AK.Wwise.Event multitoolCharge;
    public AK.Wwise.Event multitoolChargeStop;

    //General
    public AK.Wwise.Event pickUp;
    public AK.Wwise.Event teleport;
    public AK.Wwise.Event aim;
    public AK.Wwise.Event aimStop;
    public AK.Wwise.Event rain;
    public AK.Wwise.Event thunder;

    [Header("Music Sounds")]
    public AK.Wwise.Event alerte;
    public AK.Wwise.Event alerteStop;
    public AK.Wwise.Event mainMenuMusic;
    public AK.Wwise.Event intro;

    [Header("Hazards Sounds")]
    public AK.Wwise.Event spikeTrap;
    public AK.Wwise.Event thorns;
    public AK.Wwise.Event urchin;
    public AK.Wwise.Event bubbles;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void StopSoundtrack()
    {
        AkSoundEngine.StopPlayingID(playingID);
    }

    public void PlayForest()
    {
        forestSountrack.Post(soundtracks[0]);
    }

    public uint PlayUnderwater()
    {
        return underwaterSoundtrack.Post(soundtracks[1]);
    }

    public void PlayBase()
    {
        baseSoundtrack.Post(soundtracks[2]);
    }

    public void PlayStorm()
    {
        stormSoundtrack.Post(soundtracks[3]);
    }

    public uint PlaySound(AK.Wwise.Event myEvent, GameObject obj)
    {
         return myEvent.Post(obj);
    }
}
