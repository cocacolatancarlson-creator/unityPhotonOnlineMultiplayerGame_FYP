# Warping Playground: online multiplayer game, Unity + Photon PUN 2

My final-year project at UTAR (2022): an online multiplayer arena game for 2–4 players in the browser, built with
Unity 2020.3 and Photon PUN 2. Each player is an armoured 3D character with their own camera.

**Play the final game in your browser:** https://cocacolatancarlson-creator.github.io/unityPhotonOnlineMultiplayerGame_FYP/
(the August 2022 build, recovered in October 2026: see `tools/recover_final_build`).

## What is in this repository

This repository, uploaded in 2025, holds an earlier prototype of the game: a lobby, rooms, a host who starts the
match, and the networked 3D character. The final project's source was not recovered.

| Where | What |
|---|---|
| `tools/recover_final_build` | How the final 2022 build was made playable again without its source: one Photon setting changed (ws → wss). |
| tag `v2022-original` | The prototype exactly as uploaded. The committed source could not be played: the spawned 3D character had no movement script or follow camera. |
| branch `restore-2022-playable` | Four small fixes so the prototype plays again, no new features. See `RESTORE.md`. |
| branch `ai-upgrade-2026-10` | The prototype upgraded to Unity 6.3 LTS; mouse turning as the 2022 report describes; room-list fix with 3 unit tests; error screens. See `UPGRADE.md`. |

I directed the work and set its rules; Claude Code, my AI coding agent, wrote the code and ran the builds and tests.
Compare the original prototype with the upgrade: https://github.com/cocacolatancarlson-creator/unityPhotonOnlineMultiplayerGame_FYP/compare/v2022-original...ai-upgrade-2026-10
