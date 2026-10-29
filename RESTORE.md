# Restoring the 2022 version so it runs and can be played

Branch `restore-2022-playable`, cut from tag `v2022-original`. Same engine (Unity 2020.3.25f1),
same Photon PUN 2.40, no new features. Only what stops the original from being played is changed.

| # | Problem found in the 2022 version | Evidence | Fix |
|---|---|---|---|
| 1 | The spawned player cannot move. `PlayerManager` spawns `PhotonPrefabs/Player`, the character model, which has only `PhotonView` + `PhotonTransformView` and no movement script. | `Assets/Resources/PhotonPrefabs/Player.prefab` component list | Spawn `Player2`, the prefab that carries `PlayerMovement`, a `CharacterController` and the Cinemachine FreeLook camera. |
| 2 | Every player spawns at (0,0,0), so a second player starts inside the first. | `PlayerManager.CreateController` | Small random offset (±3 m). |
| 3 | "Loading" stays visible behind the title menu. The Loading panel starts active but its `open` flag is saved as false, so `OpenMenu("title")` never closes it. | Screenshot of the 2022 WebGL build; `Menu.unity` (`menuName: loading`, `open: 0`) | `Launcher.Start` opens the Loading menu through `MenuManager`, so the flag and the panel agree. |

Verification status: not yet verified in the editor or in a build.
