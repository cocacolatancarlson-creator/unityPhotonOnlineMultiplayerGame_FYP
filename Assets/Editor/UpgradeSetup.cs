using Cinemachine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// One-off, repeatable edits for the AI-assisted upgrade, kept as code so the change is reviewable.
//   Menu: Portfolio > Make 3D Character Playable, or -executeMethod UpgradeSetup.MakeCharacterPlayable
public static class UpgradeSetup
{
	const string CharacterPrefab = "Assets/Resources/PhotonPrefabs/Player.prefab";
	const string TestPrefab = "Assets/Resources/PhotonPrefabs/Player2.prefab";
	const string GameScene = "Assets/Scenes/Game.unity";

	// The FYP report's player is the armoured 3D character ("Player"), which the code already spawns, but that
	// prefab never got movement or a follow camera. This wires the prototype's own PlayerMovement (with the
	// prototype's tuning) and third-person FreeLook camera (from the test prefab "Player2") onto it.
	// Same steps as RestoreSetup on the restore branch.
	[MenuItem("Portfolio/Make 3D Character Playable")]
	public static void MakeCharacterPlayable()
	{
		GameObject test = PrefabUtility.LoadPrefabContents(TestPrefab);
		GameObject root = PrefabUtility.LoadPrefabContents(CharacterPrefab);
		try
		{
			// Size a CharacterController to the character's mesh.
			Bounds b = new Bounds(root.transform.position, Vector3.zero);
			foreach (Renderer r in root.GetComponentsInChildren<Renderer>(true)) b.Encapsulate(r.bounds);
			float s = Mathf.Max(root.transform.lossyScale.y, 0.0001f);
			// Explicit == null checks: Unity's "fake null" objects slip past the ?? operator in the editor.
			CharacterController cc = root.GetComponent<CharacterController>();
			if (cc == null) cc = root.AddComponent<CharacterController>();
			cc.height = b.size.y / s;
			cc.radius = Mathf.Clamp(Mathf.Min(b.size.x, b.size.z) / (2f * s), 0.2f / s, cc.height / 2f);
			cc.center = root.transform.InverseTransformPoint(b.center);
			Debug.Log($"UpgradeSetup: character bounds size {b.size}, center {b.center}, controller h={cc.height:F2} r={cc.radius:F2}");

			// The prototype's movement script with the prototype's tuning (the script defaults float for seconds).
			PlayerMovement move = root.GetComponent<PlayerMovement>();
			if (move == null) move = root.AddComponent<PlayerMovement>();
			PlayerMovement tuned = test.GetComponentInChildren<PlayerMovement>(true);
			if (tuned != null) EditorUtility.CopySerialized(tuned, move);
			move.characterController = cc;
			Debug.Log($"UpgradeSetup: movement tuning copied from the test prefab: {tuned != null}");

			// The prototype's third-person camera rig, re-targeted at the character.
			if (root.GetComponentInChildren<CinemachineFreeLook>(true) == null)
			{
				CinemachineFreeLook source = test.GetComponentInChildren<CinemachineFreeLook>(true);
				GameObject rig = Object.Instantiate(source.gameObject, root.transform, false);
				rig.name = source.gameObject.name;
				CinemachineFreeLook freeLook = rig.GetComponent<CinemachineFreeLook>();
				freeLook.Follow = root.transform;
				freeLook.LookAt = root.transform;
			}

			// The character carried its own Camera (depth 0). With several players spawned, those cameras
			// fight over the screen; the scene's Main Camera, driven by each player's FreeLook, renders instead.
			foreach (Camera cam in root.GetComponentsInChildren<Camera>(true))
			{
				cam.enabled = false;
				Debug.Log("UpgradeSetup: disabled the character's own camera on " + cam.gameObject.name);
			}

			PrefabUtility.SaveAsPrefabAsset(root, CharacterPrefab);
		}
		finally
		{
			PrefabUtility.UnloadPrefabContents(root);
			PrefabUtility.UnloadPrefabContents(test);
		}

		// The Game scene also holds a static copy of the character placed in the level. Now that the
		// character is the player, that copy looks like an extra player, so it is switched off.
		var scene = EditorSceneManager.OpenScene(GameScene, OpenSceneMode.Single);
		foreach (Camera cam in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (cam.CompareTag("MainCamera")) continue;
			GameObject placed = PrefabUtility.GetOutermostPrefabInstanceRoot(cam.gameObject);
			if (placed == null) placed = cam.gameObject;
			placed.SetActive(false);
			Debug.Log("UpgradeSetup: switched off the static character placed in the level: " + placed.name);
		}
		EditorSceneManager.SaveScene(scene);
	}

	// The Game scene holds a copy of the armour model whose own Camera (depth 0) draws over the
	// Cinemachine-driven Main Camera (depth -1): every client saw the same fixed view.
	// Only the Main Camera should render. (Applied on 29 Oct; MakeCharacterPlayable now also hides that copy.)
	[MenuItem("Portfolio/Disable Stray Scene Cameras")]
	public static void DisableStrayCameras()
	{
		UnityEngine.SceneManagement.Scene scene = EditorSceneManager.OpenScene(GameScene, OpenSceneMode.Single);
		foreach (Camera cam in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (cam.CompareTag("MainCamera") || !cam.enabled)
				continue;
			cam.enabled = false;
			Debug.Log("UpgradeSetup: disabled stray camera on " + cam.gameObject.name);
		}
		EditorSceneManager.SaveScene(scene);
	}
}
