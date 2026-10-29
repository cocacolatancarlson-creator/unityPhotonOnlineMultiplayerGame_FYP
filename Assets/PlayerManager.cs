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
		// Restore 2026-10: spawn the 3D character ("Player"), as committed and as the FYP report describes;
		// Editor/RestoreSetup.cs gives it the prototype's movement and camera. Small random offset so two
		// CharacterControllers don't spawn inside each other.
		Vector3 spawn = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawn, Quaternion.identity);
	}

	/*public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}*/
}