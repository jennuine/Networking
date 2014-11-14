using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour 
{
	string gameName = "A_LT_okein_Serv_er_hochabdlksj";
	bool refresh = false;
	float refreshReqLength = 3.0f;
	HostData[] hostData;

	private void startServer()
	{
		Network.InitializeServer (4, 25002, false);
		MasterServer.RegisterHost (gameName, "Game Name", "Server");
	}


	void onServerInialized()
	{
		Debug.Log ("Server Initialized!");
		SpawnPlayer();
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

	private void SpawnPlayer()
	{
		Debug.Log("Spawning Player. . .");
		Network.Instantiate (Resources.Load ("Prefabs/Player"), new Vector3(0f, 2.5f, 0f), Quaternion.identity,0);
	}
	

	public void OnGUI()
	{
		if (Network.isServer)
			GUILayout.Label("Server");
		else if (Network.isClient)
			GUILayout.Label("Client");

		if (Network.isClient)
		{
			if (GUI.Button (new Rect(Screen.width / 2 - 75f, 25f, 150f, 30f), "Spawn"))
				SpawnPlayer ();

		}

		if (!Network.isClient || !Network.isServer)
		{
			if (GUI.Button(new Rect(25f,25f, 150f, 30f), "Start New Server"))
			{
				startServer();
			}

			if (GUI.Button(new Rect(25f,65f, 150f, 30f), "Refresh Server List"))
			{
				StartCoroutine("RefreshHostList");
			}

			if (hostData != null)
			{
				for(int i = 0; i < hostData.Length; i++)
				{
					if (GUI.Button (new Rect(Screen.width/2, 65f + (30f * i ), 300f, 30f), hostData[i].gameName))
					 {
						Network.Connect (hostData[i]);
					}

				}

			}
		}
	}

}
