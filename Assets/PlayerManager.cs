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
		// Spawn the 3D character ("Player"), as the FYP report describes; Editor/UpgradeSetup.cs gives it the
		// prototype's movement and camera. Small random offset so two CharacterControllers don't overlap.
		Vector3 spawn = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
	}

	/*public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}*/
}