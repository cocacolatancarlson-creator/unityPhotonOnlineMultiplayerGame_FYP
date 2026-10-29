# Online multiplayer game: Unity + Photon PUN 2

My final-year project at UTAR (2022), uploaded here in 2025: a lobby, rooms, a host who starts the match,
and networked third-person players, built with Unity 2020.3 and Photon PUN 2.

**Play it in your browser:** https://cocacolatancarlson-creator.github.io/unityPhotonOnlineMultiplayerGame_FYP/

## Restored and upgraded with Claude Code (October 2026)

| Where | What |
|---|---|
| tag `v2022-original` | The project exactly as uploaded. The committed source could not be played: the spawned character had no movement script. |
| branch `restore-2022-playable` | Three small fixes so it plays again, nothing added. See `RESTORE.md`. |
| branch `ai-upgrade-2026-10` | Upgraded to Unity 6.3 LTS; combat, health and respawn finished; own camera per player; 6 unit tests. See `UPGRADE.md`. |

I directed the work and set its rules; Claude Code, my AI coding agent, wrote the code and ran the builds and the two-player tests.
Compare the original with the upgrade: https://github.com/cocacolatancarlson-creator/unityPhotonOnlineMultiplayerGame_FYP/compare/v2022-original...ai-upgrade-2026-10
