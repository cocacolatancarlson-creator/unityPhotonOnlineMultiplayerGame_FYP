# AI-assisted upgrade, October 2026

Branch `ai-upgrade-2026-10`, cut from `restore-2022-playable` (which is cut from tag `v2022-original`).
Code written by Claude Code, directed and reviewed by Tan Carlson. Each change is its own commit.

## What changed

| Change | Why | Verified |
|---|---|---|
| Unity 2020.3 → **6.3 LTS (6000.3.27f1)** | Current LTS; inside Photon PUN 2's supported range | First open: 0 compile errors; API Updater rewrote 6 lines in Photon views |
| MCP for Unity v10.3.0 (pinned tag) | Lets Claude Code read the console, edit prefabs and run tools in the live editor | Used to wire combat and HUD; console checked after each step |
| `RoomListCache` + Launcher | Photon sends room-list *changes*; the 2022 code redrew only the changes, so rooms could vanish | 3 unit tests |
| Closed / hidden / full rooms leave the list; error screens for disconnect and failed join | Rooms that can't be joined; 2022 code handled only a failed create | Code review |
| `Health` + `PlayerCombat` + `PlayerManager.Die()` | Finishes the shooting / health / respawn loop the 2022 code left commented out | 3 unit tests + two-client play test |
| HUD (crosshair, health bar), name tags, hit marker | Readable multiplayer | Two-client play test |
| **Stray camera disabled** | A copy of the armour model in the Game scene had its own Camera (depth 0) drawing over the gameplay camera (depth -1): every client saw one fixed view and the crosshair missed the real aim ray. Present since 2022 | Two-client play test: each client now has its own following camera |
| WebGL build script, texture cap, 1280x720 canvas | Runs from any static host; 56 MB total | Build succeeded |

## Verification (29 Oct 2026, two browser tabs on one PC, Photon Cloud)

| Step | Result |
|---|---|
| EditMode tests in Unity 6.3 | 6/6 passed |
| WebGL build, Unity 6000.3.27f1 | Succeeded (the 3 logged "errors" are `-nographics` "No graphic device" and Unity cloud-analytics timeouts) |
| Create, list, join, host-only Start | Pass |
| Each client has its own third-person camera; name tags readable | Pass (after the stray-camera fix) |
| Client 2 shoots the host | Pass: host health bar drops |
| Further hits | Pass: host dies and respawns at a new point with full health |

Screenshots: `game_portfolio/evidence/upgraded_2026_*`.

## Known, not done

- 15 CS0618 deprecation warnings remain inside Photon PUN 2.40 (third-party). Upgrading to PUN 2.56 needs the Asset Store package.
- Button text in the Unity 6 build looks underlined (TextMesh Pro font asset carried over from 2022); not yet investigated.
- No CI pipeline yet.
