using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class SyncButton : UdonSharpBehaviour
{
    public GameObject toggleObject;
    public double delayTime = 5;
    [UdonSynced,FieldChangeCallback(nameof(ServerEventTimeSync))] public double serverEventTimeSync = 0;

    public double ServerEventTimeSync
    {
        get => serverEventTimeSync;
        set
        {
            serverEventTimeSync = value;
            text_serverTick.text = serverEventTimeSync.ToString();
        }
    }

    [UdonSynced] bool isToggle = false;
    [UdonSynced] bool isReady = false;
    [Header("For Debug")]
    [SerializeField]public int serverTick;
    [SerializeField] double serverTime = 0;

    public TextMeshProUGUI text_tick;
    public TextMeshProUGUI text_time;

    public TextMeshProUGUI text_serverTick;

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        if(player.isLocal)
        {
            if(isToggle)
            {
                toggleObject.SetActive(true);
            }
            else
            {
                toggleObject.SetActive(false);
            }
        }
    }


    public override void Interact()
    {
        if(Networking.LocalPlayer.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        ServerEventTimeSync = Networking.GetServerTimeInSeconds() + delayTime;
        isReady = true;
        RequestSerialization();
    }

    void Update()
    {
        if (serverTime > ServerEventTimeSync && isReady)
        {
            isReady = false;
            Debug.Log("ServerTime : " + Networking.GetServerTimeInSeconds());
            ServerEventTimeSync = 0; 
            if(isToggle)
            {
                isToggle = false;
                toggleObject.SetActive(false);
            }
            else
            {
                isToggle = true;
                toggleObject.SetActive(true);
            }
        }
        
        serverTime = Networking.GetServerTimeInSeconds();
        serverTick = Networking.GetServerTimeInMilliseconds();
        
        if (text_tick != null)
        {
            text_tick.text = serverTick.ToString();
        }
        else
        {
            Debug.LogWarning("text_tick is Null");
        }

        if (text_time != null)
        {
            text_time.text = serverTime.ToString();
        }
        else
        {
            Debug.LogWarning("text_time is Null");
        }
    }
}