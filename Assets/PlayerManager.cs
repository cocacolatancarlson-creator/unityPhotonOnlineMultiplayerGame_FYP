using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
	PhotonView PV;

	GameObject controller;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
	}

	void Start()
	{
		if (PV.IsMine)
		{
			CreateController();
		}
	}

	void CreateController()
	{
		//Transform spawnpoint = SpawnManager.Instance.GetSpawnpoint();
		//controller = 
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),Vector3.zero,Quaternion.identity);
	}

	/*public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}*/
}