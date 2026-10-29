# AI-assisted upgrade, October 2026

Branch `ai-upgrade-2026-10`, which starts from the restore branch's first fix (`ff9e0a9`); `restore-2022-playable` is cut from tag `v2022-original`.
Code written by Claude Code, directed by Tan Carlson. Each change is its own commit.

The game design is the one in the FYP report (Warping Playground): each player is an armoured 3D character
with its own third-person camera. W moves forward, Space jumps, S turns back 180 degrees, and moving the
mouse turns the character (report sections 5.2.2 and 5.2.5). The upgrade keeps that design and does not add
features the report does not describe.

## What changed

| Change | Why | Verified |
|---|---|---|
| Unity 2020.3 → **6.3 LTS (6000.3.27f1)** | Current LTS; inside Photon PUN 2's supported range | First open: 0 compile errors; API Updater rewrote 6 lines in Photon views |
| **The 3D character is the player** (same fix as the restore branch, `Editor/UpgradeSetup.cs`) | The code spawns the character, but the character had no movement or camera; the prototype's movement and FreeLook camera sat on a test prefab | Two-client play test |
| **Mouse turning** (`PlayerMovement.mouseTurnSpeed`; a click locks the pointer, Esc releases it) | The report's control scheme: moving the mouse turns the character and its camera. The 2022 prototype only turned with A/D | Play test |
| `RoomListCache` + Launcher | Photon sends room-list *changes*; the 2022 code redrew only the changes, so rooms could vanish | 3 unit tests |
| Closed / hidden / full rooms leave the list; error screens for disconnect and failed join | Rooms that can't be joined; 2022 code handled only a failed create | Code review |
| Stray camera disabled, static character copy switched off | A copy of the armour model in the Game scene had its own Camera (depth 0) drawing over the gameplay camera (depth -1), so every client saw one fixed view. Present since 2022 | Two-client play test: each client has its own following camera |
| MCP for Unity v10.3.0 (pinned tag) | Lets Claude Code read the console, edit prefabs and run tools in the live editor | Installed and connected; the final edits run as batch-mode editor scripts so they can be repeated |
| WebGL build script, texture cap, 1280x720 canvas | Runs from any static host | Build succeeded |

## Removed on 29 Oct 2026

An earlier version of this branch added a hitscan shooting loop (`PlayerCombat`, `Health`, respawn), a HUD and
name tags on the capsule test prefab. None of that is in the FYP design (the report's attacks are melee, throw,
warp and laser on the 3D character), so it was removed after the report was read, together with its tests.

## Verification (29 Oct 2026, two browser tabs on one PC, Photon Cloud)

| Step | Result |
|---|---|
| EditMode tests in Unity 6.3 | 3/3 passed (`RoomListCacheTests`) |
| WebGL build, Unity 6000.3.27f1 | Succeeded, 57.4 MB (the 3 logged "errors" are IL2CPP notes that three large TextMesh Pro methods got their own C++ file) |
| Create, list, join, host-only Start | Pass |
| Each player spawns as the 3D character with its own following camera | Pass: both clients |
| W, Space, S and mouse turning | Pass: W walks, Space jumps and lands, S turns the character and camera 180 degrees, moving the mouse turns it; the other client sees the movement |

Screenshots: `game_portfolio/evidence/upgraded_2026_character_*`.

## Known, not done

- The final Warping Playground features in the report (melee, throw and recall, warp, laser, enemies, timer,
  scoring, ranking board) are not in this prototype's source, and the final project's source was not recovered.
  The report's Unity Play upload ("WarpingPlaygroundTemp", 15 Aug 2022, section 5.2.10) is still listed but no
  longer plays (tried on 29 Oct 2026).
- 15 CS0618 deprecation warnings remain inside Photon PUN 2.40 (third-party). Upgrading to PUN 2.56 needs the Asset Store package.
- Button text in the Unity 6 build looks underlined (TextMesh Pro font asset carried over from 2022); not yet investigated.
- No CI pipeline yet.
