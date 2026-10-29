# AI-assisted upgrade, October 2026

Branch `ai-upgrade-2026-10`, cut from `restore-2022-playable` (which is cut from tag `v2022-original`).
Code written by Claude Code, directed and reviewed by Tan Carlson. Each line says whether it is verified.

## Done in code

| Change | Why | Verified |
|---|---|---|
| `Portfolio.Core.RoomListCache` + Launcher uses it | Photon sends room-list *changes*; the 2022 code redrew only the changes, so rooms vanished from the list | 3 unit tests pass (.NET 8 test run, 29 Oct); not yet in Unity |
| Closed, hidden and full rooms leave the list | They can't be joined | not yet |
| Error screens for disconnect and failed join | 2022 code only handled a failed create | not yet |
| `Portfolio.Core.Health` + `PlayerCombat` | Finishes the shooting / health / death loop the 2022 code left commented out; hitscan, owner applies damage | 3 unit tests pass for Health; combat not yet in Unity |
| `PlayerManager.Die()` respawn | Was commented out in 2022 | not yet |
| EditMode test assembly `Portfolio.Core.Tests` | First automated tests in the project | runs in .NET; Unity Test Runner not yet |

## Still to do (needs the Unity 6.3 editor)

1. Open in Unity 6000.3 LTS; upgrade PUN 2.40 to a Unity-6 version if it does not compile (2.48+ supports Unity 6).
2. Install MCP for Unity (CoplayDev/unity-mcp) so Claude Code can read the console, run tests and enter Play mode.
3. Wire `PlayerCombat` onto `Player2`, add a crosshair and a health bar.
4. WebGL build with `PortfolioBuild.WebGL`, two-client play test, screenshots.
