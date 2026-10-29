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
		// Restore 2026-10: "Player" has no movement script; "Player2" carries PlayerMovement + the FreeLook camera.
		// Small random offset so two CharacterControllers don't spawn inside each other.
		Vector3 spawn = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player2"), spawn, Quaternion.identity);
	}

	/*public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}*/
}