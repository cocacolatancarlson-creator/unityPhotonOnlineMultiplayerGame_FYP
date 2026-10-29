# Restoring the 2022 version so it runs and can be played

Branch `restore-2022-playable`, cut from tag `v2022-original`. Same engine line (Unity 2020.3 LTS),
same Photon PUN 2.40, no new features. Only what stops the original from being played is changed.

| # | Problem found in the 2022 version | Evidence | Fix |
|---|---|---|---|
| 1 | The spawned player cannot move. `PlayerManager` spawns `PhotonPrefabs/Player`, the armoured 3D character the FYP report describes, but that prefab has only `PhotonView` + `PhotonTransformView`: no movement script and no follow camera. The prototype's `PlayerMovement` and FreeLook camera sit on a test prefab, `Player2`. | `Assets/Resources/PhotonPrefabs/Player.prefab` component list; FYP report, sections 5.2.2 (spawning) and 5.2.5 (character controller) | Keep spawning the 3D character. `Assets/Editor/RestoreSetup.cs` gives it the prototype's own `PlayerMovement` with the prototype's tuning (gravity 10, jump 7, double jump x1.5), a `CharacterController` sized to the mesh, and the FreeLook rig copied from `Player2`, re-targeted at the character. The character's own Camera is switched off. |
| 1b | A static copy of the character stands in the Game scene with its own Camera (depth 0), which draws over the Cinemachine-driven Main Camera (depth -1), so nobody's camera follows their own player. | `Game.unity`; 2022 build screenshot | `RestoreSetup` switches that static copy off. |
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
- WebGL-only texture cap of 512 px on the armour model's 95 maps in `Resources`. The model is the spawned
  player, so the maps ship; at 2048 px the WebGL data file was about
  141.6 MB (138,268 KiB), now 20.1 MB (20,061,820 bytes).
  The `.meta` files gain a WebGL platform override; other platforms keep full size.

## Verification (29 Oct 2026, two browser tabs on one PC, Photon Cloud)

| Step | Result |
|---|---|
| Build, Unity 2020.3.49f1, `PortfolioBuild.WebGL` | Succeeded, 0 errors (data 19,607 KB, wasm 20,505 KB) |
| Title menu, no "Loading" overlap | Pass |
| Create room (client 1), Find Room + Join (client 2) | Pass: both clients list two players; only the host has Start |
| Start (host) | Pass: both clients load the level and each player spawns as the armoured 3D character |
| Own camera | Pass: each client's camera follows its own character from behind |
| W forward, A/D turn (client 2) | Pass: the character walks and turns; client 1 sees it move |
| Space jump | Pass: jumps and lands in about 1.4 s (prototype tuning) |

Screenshots: `game_portfolio/evidence/restored_2022_character_*.jpg` and `restored_2022_room_two_players.jpg`.
Earlier capsule-based screenshots (superseded on 29 Oct) are kept in `evidence/_superseded_capsule_2026-10-29/`.

## What the restore does not add

The final Warping Playground features in the report (melee, throw and recall, warp, laser, enemies, timer,
scoring, ranking board) are not in this prototype's source, and the final project's source was not recovered.
The report's Unity Play upload ("WarpingPlaygroundTemp", 15 Aug 2022, section 5.2.10) is still listed but no
longer plays (tried on 29 Oct 2026). The restore makes the prototype that exists playable; it does not
recreate the final game.
