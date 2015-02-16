using UnityEngine;
using System.Collections;

public class NetworkManagerScript : MonoBehaviour
{

    public bool TestServerSpawn;

    public string gameName = "SnubbleJr_Networking";

    private GameObject playerPrefab;
    private Transform spawnObject;

    HostData[] hostData;

    float btnX, btnY, btnW, btnH;

    bool refreshing = false;

    PlayerManagerBehaviour playerManagerBehaviour;

    //stating server
    void Start()
    {
        btnX = Screen.width * 0.05f;
        btnY = Screen.width * 0.05f;
        btnW = Screen.width * 0.1f;
        btnH = Screen.width * 0.1f;

        playerManagerBehaviour = GameObject.Find("Player Manager").GetComponent<PlayerManagerBehaviour>();
    }

    void OnGUI()
    {
        if (!Network.isClient && !Network.isServer)
        {
            if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server"))
            {
                StartServer();
            }

            if (GUI.Button(new Rect(btnX, btnY * 1.2f + btnH, btnW, btnH), "RefreshHosts"))
            {
                print("Refreshing Host list");
                refreshHostList();
            }

            if (hostData != null)
            {
                for (int i = 0; i < hostData.Length; i++)
                {
                    if (GUI.Button(new Rect(btnX * 1.5f + btnW, btnY * 1.2f + (btnH * i), btnW * 3, btnH * 0.5f), hostData[i].gameName))
                    {
                        Network.Connect(hostData[i]);
                    }
                }
            }
        }
    }

    //starting server
    void StartServer()
    {
        Network.InitializeServer(2, 25001, !Network.HavePublicAddress());
        MasterServer.RegisterHost(gameName, "ProjectFootStool", "server test");
    }

    void refreshHostList()
    {
        MasterServer.RequestHostList(gameName);
        refreshing = true;
    }

    //initialising server
    void OnServerInitialized()
    {
        print("Server Initialized");

        if (TestServerSpawn)
        {
            spawnPlayer();
        }
    }

    void OnPlayerDisconnected(NetworkPlayer player)
    {
        Debug.Log("Clean up after player " + player);
        Network.RemoveRPCs(player);
        Network.DestroyPlayerObjects(player);
    }

    void OnConnectedToServer()
    {
        spawnPlayer();
    }

    void OnMasterServerEvent(MasterServerEvent mse)
    {
        if (mse == MasterServerEvent.RegistrationSucceeded)
        {
            print("Registrated with Master server under name: " + gameName);
        }
    }

    void spawnPlayer()
    {
        //Network.Instantiate(playerPrefab, spawnObject.position, Quaternion.identity, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (refreshing && MasterServer.PollHostList().Length > 0)
        {
            refreshing = false;
            hostData = MasterServer.PollHostList();
        }
    }
}
