# Restoring the 2022 version so it runs and can be played

Branch `restore-2022-playable`, cut from tag `v2022-original`. Same engine line (Unity 2020.3 LTS),
same Photon PUN 2.40, no new features. Only what stops the original from being played is changed.

| # | Problem found in the 2022 version | Evidence | Fix |
|---|---|---|---|
| 1 | The spawned player cannot move. `PlayerManager` spawns `PhotonPrefabs/Player`, the character model, which has only `PhotonView` + `PhotonTransformView` and no movement script. | `Assets/Resources/PhotonPrefabs/Player.prefab` component list | Spawn `Player2`, the prefab that carries `PlayerMovement`, a `CharacterController` and the Cinemachine FreeLook camera. |
| 2 | Every player spawns at (0,0,0), so a second player starts inside the first. | `PlayerManager.CreateController` | Small random offset (±3 m). |
| 3 | "Loading" stays visible behind the title menu. The Loading panel starts active but its `open` flag is saved as false, so `OpenMenu("title")` never closes it. | Screenshot of the 2022 WebGL build; `Menu.unity` (`menuName: loading`, `open: 0`) | `Launcher.Start` opens the Loading menu through `MenuManager`, so the flag and the panel agree. |

## Editor version

Built with **Unity 2020.3.49f1** instead of 2020.3.25f1. It is the same 2020.3 LTS line, and its release notes say
"Scripting: Addressed CVE-2025-59489" (Unity security advisory, October 2025). Opening the project bumped only
editor-tooling packages (Rider, Visual Studio, VS Code, Collab, Test Framework). The gameplay packages
(Cinemachine 2.6.11, TextMesh Pro 3.0.6, Timeline 1.4.8) and Photon PUN 2.40 are unchanged.
The untouched 2022 original is still tag `v2022-original`, and the original 2022 WebGL build is kept beside the repo.

## Build tooling (no gameplay change)

`Assets/Editor/PortfolioBuild.cs` makes the WebGL build from the command line:
- 1280x720 canvas (the 2022 build was a fixed 1920x1024) and uncompressed output, so it runs from any static web host.
- WebGL-only texture cap of 512 px on the armour model's 95 maps in `Resources`. The model is not the spawned
  player, but a copy of it stands in the Game scene, so the maps ship; at 2048 px the WebGL data file was about
  141.6 MB (138,268 KiB), now 20.1 MB (20,061,820 bytes).
  The `.meta` files gain a WebGL platform override; other platforms keep full size.

## Verification (29 Oct 2026, two browser tabs on one PC, Photon Cloud)

| Step | Result |
|---|---|
| Build, Unity 2020.3.49f1, `PortfolioBuild.WebGL` | Succeeded, 0 errors |
| Title menu, no "Loading" overlap | Pass |
| Create room `room1` (client 1) | Pass: room screen with player list, host-only Start |
| Find Room (client 2) | Pass: `room1` listed |
| Join (client 2) | Pass: both clients list two players; client 2 has no Start button |
| Start (host) | Pass: both clients load the level, two capsule players spawn |
| Move and jump (client 1) | Pass: client 2 sees client 1's character move |

Screenshots: `game_portfolio/evidence/restored_2022_*.jpg`.

## Known issue left as found (fixed in the upgrade branch)

The camera does not follow your own character. The Game scene holds a copy of the armour model whose own
Camera (depth 0) draws over the Cinemachine-driven Main Camera (depth -1), so every client sees the same fixed
overview. The game is still playable from that view, so the restore leaves it alone; `ai-upgrade-2026-10` fixes it.
