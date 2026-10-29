using Photon.Pun;
using Portfolio.Core;
using UnityEngine;
using UnityEngine.UI;

// Finishes the shooting / health / death loop that the 2022 PlayerController left commented out.
// Hitscan from the screen centre; the victim's owner applies the damage, so each client is authoritative
// over its own character (same rule as movement).
[RequireComponent(typeof(PhotonView))]
public class PlayerCombat : MonoBehaviourPun
{
	[SerializeField] float maxHealth = 100f;
	[SerializeField] float damage = 25f;
	[SerializeField] float range = 100f;
	[SerializeField] float fireCooldown = 0.25f;
	[SerializeField] Image healthBar;          // optional: local player's on-screen bar

	Health health;
	PlayerManager playerManager;
	float nextFireTime;
	bool dying;

	void Awake()
	{
		health = new Health(maxHealth);
		object[] data = photonView.InstantiationData;
		if (data != null && data.Length > 0)
		{
			PhotonView managerView = PhotonView.Find((int)data[0]);
			if (managerView != null)
			{
				playerManager = managerView.GetComponent<PlayerManager>();
			}
		}
	}

	void Start()
	{
		if (!photonView.IsMine && healthBar != null)
		{
			healthBar.transform.root.gameObject.SetActive(false);
		}
	}

	void Update()
	{
		if (!photonView.IsMine)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			Cursor.lockState = CursorLockMode.Locked;   // keep the mouse in the game for the orbit camera
		}

		if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
		{
			nextFireTime = Time.time + fireCooldown;
			Fire();
		}

		if (transform.position.y < -10f)   // fell out of the world (also from the 2022 notes)
		{
			Die();
		}
	}

	void Fire()
	{
		Camera cam = Camera.main;
		if (cam == null)
			return;

		Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		RaycastHit[] hits = Physics.RaycastAll(ray, range);
		System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
		foreach (RaycastHit hit in hits)
		{
			PlayerCombat target = hit.collider.GetComponentInParent<PlayerCombat>();
			if (target == this)
				continue;   // the ray starts behind our own character in third person
			if (target != null)
			{
				target.photonView.RPC(nameof(RPC_TakeDamage), target.photonView.Owner, damage);
			}
			return;   // first thing that isn't us stops the shot
		}
	}

	[PunRPC]
	void RPC_TakeDamage(float amount)
	{
		if (!photonView.IsMine)
			return;

		bool killed = health.TakeDamage(amount);
		if (healthBar != null)
		{
			healthBar.fillAmount = health.Fraction;
		}
		if (killed)
		{
			Die();
		}
	}

	void Die()
	{
		// A fall and a killing shot can land in the same frame; respawn only once.
		if (dying || playerManager == null)
			return;
		dying = true;
		playerManager.Die();
	}
}
