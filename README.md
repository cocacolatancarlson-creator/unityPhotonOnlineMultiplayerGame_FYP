# Warping Playground: online multiplayer game, Unity + Photon PUN 2

My final-year project at UTAR (2022): an online multiplayer arena game for 2–4 players in the browser, built with
Unity 2020.3 and Photon PUN 2. Each player is an armoured 3D character with their own camera. This repository,
uploaded in 2025, holds an earlier prototype of it: a lobby, rooms, a host who starts the match, and the
networked 3D character. The final project was not recovered; the showcase shows it through the report's screenshots.

**Play it in your browser:** https://cocacolatancarlson-creator.github.io/unityPhotonOnlineMultiplayerGame_FYP/

## Restored and upgraded with Claude Code (October 2026)

| Where | What |
|---|---|
| tag `v2022-original` | The prototype exactly as uploaded. The committed source could not be played: the spawned 3D character had no movement script or follow camera. |
| branch `restore-2022-playable` | Four small fixes so it plays again, no new features: the prototype's own movement and camera move onto the 3D character. See `RESTORE.md`. |
| branch `ai-upgrade-2026-10` | Upgraded to Unity 6.3 LTS; mouse turning as the 2022 report describes; room-list fix with 3 unit tests; error screens. See `UPGRADE.md`. |

I directed the work and set its rules; Claude Code, my AI coding agent, wrote the code and ran the builds and the two-player tests.
Compare the original with the upgrade: https://github.com/cocacolatancarlson-creator/unityPhotonOnlineMultiplayerGame_FYP/compare/v2022-original...ai-upgrade-2026-10
