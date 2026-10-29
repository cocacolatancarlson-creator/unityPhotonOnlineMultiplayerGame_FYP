# Recovering the final 2022 build (October 2026)

The final Warping Playground build was published on Unity Play in August 2022 (the link is in the FYP report,
section 5.2.10). By October 2026 it no longer played there, and the final project's source was not recovered.
This folder holds the tool used to make that build play again without its source.

## What was found

| Step | Result |
|---|---|
| Download the four build files from Unity Play (about 31 MB, gzip, built 15 Aug 2022, Unity 2020.3.25f1) | Kept unchanged with SHA-256 hashes |
| Decompress and run them from a local http server | Loads, connects to Photon over `ws://`, menu and match work |
| Why Unity Play fails | The game connects over plain `ws://` (Photon ports 9093/9090). Browsers block `ws://` on https pages, such as Unity Play and GitHub Pages |
| Rewrite the socket URL to `wss://` in the hosting page | Fails: the name server rejects the login ("Data parsing error … encrypted: True"), because the client still runs the plain-ws handshake |
| Read `PhotonServerSettings` from the build | `AppSettings.Protocol = 4` (WebSocket), region `Asia` |

## The fix

`photon_protocol.py` changes that one serialized value from 4 to 5 (WebSocketSecure) inside `data.unity3d`
(a UnityFS bundle in the WebGL `.data` container), re-packs the bundle with UnityPy and rebuilds the
`UnityWebData1.0` container; every other entry is copied byte for byte.

    uv run --with UnityPy python photon_protocol.py myTempBuild.data <AppIdRealtime>                       # read only
    uv run --with UnityPy python photon_protocol.py myTempBuild.data <AppIdRealtime> patched.data 5        # patch

## Checks (29 Oct 2026)

- The patched build connects to Photon's name server and master server over `wss://`.
- Played in a browser: menu, room, start, and a match with HP and MP bars, the 180-second timer and enemy fire.

## Hashes (SHA-256)

Downloaded from Unity Play (gzip):

    12c58b51227070c1a4e77db730d2f1df6ef9443d1968eb8b67ed0da21bfb1362  game.data
    1b4c18f3bcde4fde521b0e795e147a91c1daadc563342baf11ee8b5bca9a946f  game.wasm
    269fd33d13f7bca7f6eeeef54053a8b0c80b41663c9c218b6500fea4f134dcc5  framework.js
    070001727b761dd07e180ccf5eb9e600aa6455348875b86c19f211550efac5e2  loader.js

Decompressed, as served on the showcase:

    f202bd51e56f56c87d70085a679e4f585a9a37832b025bf9a236b20145da167c  myTempBuild.data (unpatched)
    e7d540e7a404bb014abeef80d0c5014b4c0f79c6880a99b00cbb4e98d48fdf08  myTempBuild.data (patched, served)
    ef3736112944d7d8c37c917c308f2894dcdb3d8952cfc4d89ad68c2926bb5994  myTempBuild.wasm
    226a72bcf97d137a83d35bc2b2b24d235b03ce79453b101748fa2f7a5bb84f65  myTempBuild.framework.js

Recovered by Claude Code at Tan Carlson's direction.
