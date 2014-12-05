using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	string gameName = "A_LT_okein_Serv_er_hochabdlksj";
	//bool refresh = false;
	float refreshReqLength = 3.0f;
	HostData[] hostData;
	
	private void startServer()
	{
		Network.InitializeServer (4, 25002, false);
		MasterServer.RegisterHost (gameName, "Game Name", "Server");
		SpawnServerPlayer();
	}
	
	
	void onServerInialized()
	{
		Debug.Log ("Server Initialized!");
		SpawnClientPlayer();
	}
	
	void onMasterServerEvent(MasterServerEvent mse)
	{
		if (mse == MasterServerEvent.RegistrationSucceeded) 
			Debug.Log("Registration Successful!");
		
	}
	
	void OnPlayerDisconnected(NetworkPlayer player)
	{
		Debug.Log("Player disconnected from: " + player.ipAddress + ":" + player.port);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	void OnApplicationQuit()
	{
		if (Network.isServer)
		{
			Network.Disconnect(200);
			MasterServer.UnregisterHost();
		}
		
		if (Network.isClient)
			Network.Disconnect(200);
	}
	
	public IEnumerator RefreshHostList()
	{
		Debug.Log ("Refreshing. . .");
		MasterServer.RequestHostList (gameName);
		
		float timeStarted = Time.time;
		float timeEnd = Time.time + refreshReqLength;
		
		while (Time.time < timeEnd)
		{
			hostData = MasterServer.PollHostList ();
			yield return new WaitForEndOfFrame();
		}
		
		if (hostData == null || hostData.Length == 0)
			Debug.Log ("No active servers have been found");
		else
			Debug.Log (hostData.Length + " have been found");
	}
	
	private void SpawnServerPlayer()
	{
		Debug.Log("Spawning Player. . .");
		Network.Instantiate (Resources.Load ("Prefabs/PlayerShipCam"), new Vector3(0f, 2.5f, 0f), Quaternion.identity,0);
	}

	private void SpawnClientPlayer()
	{
		Debug.Log("Spawning Player. . .");
		GameObject ship = (GameObject)Network.Instantiate (Resources.Load ("Prefabs/PlayerShip"), new Vector3(0f, 2.5f, 0f), Quaternion.identity,0);
		GameObject obj = new GameObject("PlayerCam");
		obj.AddComponent<Camera>();
		obj.transform.parent = ship.transform;
		obj.transform.localPosition = new Vector3(0, 0.1941137f, -0.07031611f);
	}
	
	
	public void OnGUI()
	{
		
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			if (!Network.isClient || !Network.isServer)
			{
				if (GUI.Button(new Rect(25f,25f, 150f, 30f), "Start New Server"))
				{
					startServer();
				}
				
				if (GUI.Button(new Rect(25f,65f, 150f, 30f), "Connect"))
				{
					StartCoroutine("RefreshHostList");
				}
				
				if (hostData != null)
				{
					for(int i = 0; i < hostData.Length; i++)
					{
						if (GUI.Button (new Rect(Screen.width/2, 65f + (30f * i ), 300f, 30f), "Connect to " + hostData[i].gameName))
						{
							Network.Connect (hostData[i]);
						}
						
					}
					
				}
			}
		}
		else
		{
			if (Network.isServer)
				GUILayout.Label("Server");
			else if (Network.isClient)
			{
				GUILayout.Label("Client");
				
				if (GUI.Button (new Rect(Screen.width / 2 - 75f, 25f, 150f, 30f), "Spawn"))
					SpawnClientPlayer ();
			}
				//string ipAddress = Network.player.ipAddress.ToString();
			//string port = Network.player.port.ToString();
			
			//GUI.Label(new Rect(140, 20, 250, 40), "IP Address: " + ipAddress + ":" + port);
			
			if (GUI.Button(new Rect(10,10,100,50), "Disconnect"))
			{
				if (Network.isServer)
				{
					Network.Disconnect(200);
					MasterServer.UnregisterHost();
					
				}
				
				if (Network.isClient)
					Network.Disconnect(200);
			}
			
			
		}
	}
	
}
