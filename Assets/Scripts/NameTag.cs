using Photon.Pun;
using TMPro;
using UnityEngine;

// Floating nickname above each player, turned to face the local camera.
// Hidden for your own character so it does not block the third-person view.
public class NameTag : MonoBehaviour
{
	[SerializeField] TMP_Text label;

	PhotonView owner;

	void Start()
	{
		owner = GetComponentInParent<PhotonView>();
		if (owner == null || owner.IsMine)
		{
			gameObject.SetActive(false);
			return;
		}
		label.text = owner.Owner != null ? owner.Owner.NickName : "Player";
	}

	void LateUpdate()
	{
		Camera cam = Camera.main;
		if (cam != null)
		{
			transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
		}
	}
}
