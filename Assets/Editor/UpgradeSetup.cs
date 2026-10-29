using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// One-off, repeatable edits for the AI-assisted upgrade, kept as code so the change is reviewable.
//   Menu: Portfolio > Wire Player Combat, or -executeMethod UpgradeSetup.WirePlayerCombat
public static class UpgradeSetup
{
	const string PlayerPrefab = "Assets/Resources/PhotonPrefabs/Player2.prefab";

	[MenuItem("Portfolio/Wire Player Combat")]
	public static void WirePlayerCombat()
	{
		GameObject root = PrefabUtility.LoadPrefabContents(PlayerPrefab);
		try
		{
			PlayerCombat combat = root.GetComponent<PlayerCombat>();
			if (combat == null)
			{
				combat = root.AddComponent<PlayerCombat>();
			}

			Transform hud = root.transform.Find("HUD");
			if (hud == null)
			{
				hud = BuildHud(root.transform);
			}

			SerializedObject so = new SerializedObject(combat);
			so.FindProperty("hud").objectReferenceValue = hud.gameObject;
			so.FindProperty("healthBar").objectReferenceValue = hud.Find("HealthBar/Fill").GetComponent<Image>();
			so.FindProperty("crosshair").objectReferenceValue = hud.Find("Crosshair").GetComponent<Image>();
			so.ApplyModifiedPropertiesWithoutUndo();

			if (root.transform.Find("NameTag") == null)
			{
				BuildNameTag(root.transform);
			}

			PrefabUtility.SaveAsPrefabAsset(root, PlayerPrefab);
		}
		finally
		{
			PrefabUtility.UnloadPrefabContents(root);
		}
		Debug.Log("UpgradeSetup: PlayerCombat and HUD wired on " + PlayerPrefab);
	}

	// The Game scene holds a copy of the armour model whose own Camera (depth 0) draws over the
	// Cinemachine-driven Main Camera (depth -1): every client saw the same fixed view, and the
	// crosshair did not line up with Camera.main. Only the Main Camera should render.
	[MenuItem("Portfolio/Disable Stray Scene Cameras")]
	public static void DisableStrayCameras()
	{
		UnityEngine.SceneManagement.Scene scene = UnityEditor.SceneManagement.EditorSceneManager.OpenScene(
			"Assets/Scenes/Game.unity", UnityEditor.SceneManagement.OpenSceneMode.Single);
		foreach (Camera cam in Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
		{
			if (cam.CompareTag("MainCamera") || !cam.enabled)
				continue;
			cam.enabled = false;
			Debug.Log("UpgradeSetup: disabled stray camera on " + cam.gameObject.name);
		}
		UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
	}

	static Transform BuildHud(Transform parent)
	{
		Sprite uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

		GameObject hud = new GameObject("HUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
		hud.transform.SetParent(parent, false);
		hud.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
		CanvasScaler scaler = hud.GetComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(1280, 720);

		// Crosshair: a small dot in the middle of the screen, where the hitscan ray goes.
		Image dot = NewImage("Crosshair", hud.transform, uiSprite, new Color(1f, 1f, 1f, 0.9f));
		RectTransform dotRect = dot.rectTransform;
		dotRect.anchorMin = dotRect.anchorMax = new Vector2(0.5f, 0.5f);
		dotRect.sizeDelta = new Vector2(8f, 8f);

		// Health bar: bottom-left, a dark frame with a green fill.
		Image frame = NewImage("HealthBar", hud.transform, uiSprite, new Color(0f, 0f, 0f, 0.6f));
		RectTransform frameRect = frame.rectTransform;
		frameRect.anchorMin = frameRect.anchorMax = new Vector2(0f, 0f);
		frameRect.pivot = new Vector2(0f, 0f);
		frameRect.anchoredPosition = new Vector2(24f, 24f);
		frameRect.sizeDelta = new Vector2(300f, 22f);

		Image fill = NewImage("Fill", frame.transform, uiSprite, new Color(0.25f, 0.85f, 0.35f, 1f));
		fill.type = Image.Type.Filled;
		fill.fillMethod = Image.FillMethod.Horizontal;
		fill.fillAmount = 1f;
		RectTransform fillRect = fill.rectTransform;
		fillRect.anchorMin = Vector2.zero;
		fillRect.anchorMax = Vector2.one;
		fillRect.offsetMin = new Vector2(3f, 3f);
		fillRect.offsetMax = new Vector2(-3f, -3f);

		return hud.transform;
	}

	static void BuildNameTag(Transform parent)
	{
		GameObject go = new GameObject("NameTag", typeof(TextMeshPro));
		go.transform.SetParent(parent, false);
		go.transform.localPosition = new Vector3(0f, 1.5f, 0f);   // just above the 2 m capsule
		TextMeshPro text = go.GetComponent<TextMeshPro>();
		text.rectTransform.sizeDelta = new Vector2(4f, 0.6f);
		text.fontSize = 3f;
		text.alignment = TextAlignmentOptions.Center;
		text.color = Color.white;
		text.text = "Player";

		NameTag tag = go.AddComponent<NameTag>();
		SerializedObject so = new SerializedObject(tag);
		so.FindProperty("label").objectReferenceValue = text;
		so.ApplyModifiedPropertiesWithoutUndo();
	}

	static Image NewImage(string name, Transform parent, Sprite sprite, Color color)
	{
		GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
		go.transform.SetParent(parent, false);
		Image image = go.GetComponent<Image>();
		image.sprite = sprite;
		image.color = color;
		image.raycastTarget = false;
		return image;
	}
}
