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
		// "Player2" carries PlayerMovement + the FreeLook camera; the random offset keeps players apart.
		// The manager's ViewID travels as instantiation data so the character can find its manager to respawn.
		Vector3 spawn = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
		controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player2"), spawn, Quaternion.identity,
			0, new object[] { PV.ViewID });
	}

	// Finishes the respawn that the 2022 code had commented out.
	public void Die()
	{
		PhotonNetwork.Destroy(controller);
		CreateController();
	}
}