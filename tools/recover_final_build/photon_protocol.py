"""Find (and optionally patch) PUN's connection protocol inside a Unity WebGL .data file.
   Read-only:  python photon_protocol.py <in.data> <AppIdRealtime>
   Patch:      python photon_protocol.py <in.data> <AppIdRealtime> <out.data> <new protocol int>
   ExitGames ConnectionProtocol: Udp=0, Tcp=1, WebSocket=4, WebSocketSecure=5.
   Unity serialization: string = int32 length + bytes + pad to 4; bool = 1 byte + pad to 4; int = int32."""
import struct
import sys

import UnityPy

src, app_id = sys.argv[1], sys.argv[2].encode()
out = sys.argv[3] if len(sys.argv) > 3 else None
new_protocol = int(sys.argv[4]) if len(sys.argv) > 4 else None

d = open(src, "rb").read()
assert d[:16] == b"UnityWebData1.0\0"
head_end = struct.unpack_from("<I", d, 16)[0]
entries, pos = [], 20
while pos < head_end:
    off, size, nlen = struct.unpack_from("<III", d, pos)
    pos += 12
    entries.append([d[pos:pos + nlen].decode(), off, size])
    pos += nlen

bundle_entry = next(e for e in entries if e[0] == "data.unity3d")
env = UnityPy.load(d[bundle_entry[1]:bundle_entry[1] + bundle_entry[2]])
needle = struct.pack("<i", len(app_id)) + app_id


def fields_after(raw, pos):
    def s(p):
        n = struct.unpack_from("<i", raw, p)[0]
        assert 0 <= n < 512, f"bad length {n}"
        p += 4 + n
        return raw[p - n:p].decode("utf-8", "replace"), p + (-p % 4)
    def b(p):
        return raw[p], p + 4
    def i(p):
        return struct.unpack_from("<i", raw, p)[0], p + 4
    out_fields = {}
    for name, f in [("AppIdRealtime", s), ("AppIdFusion", s), ("AppIdChat", s), ("AppIdVoice", s),
                    ("AppVersion", s), ("UseNameServer", b), ("FixedRegion", s), ("Server", s), ("Port", i),
                    ("ProxyServer", s), ("Protocol", i), ("EnableProtocolFallback", b), ("AuthMode", i),
                    ("EnableLobbyStatistics", b), ("NetworkLogging", i)]:
        at = pos
        val, pos = f(pos)
        out_fields[name] = (val, at)
    return out_fields


target = None
for obj in env.objects:
    if obj.type.name != "MonoBehaviour":
        continue
    raw = obj.get_raw_data()
    k = raw.find(needle)
    if k < 0:
        continue
    fields = fields_after(raw, k)
    shown = {n: ("<set>" if n.startswith("AppId") and v else v) for n, (v, _) in fields.items()}
    print(f"object {obj.path_id} in {obj.assets_file.name}: {shown}")
    target = (obj, raw, fields)

if target is None:
    sys.exit("AppSettings not found")
if out is None:
    sys.exit(0)

obj, raw, fields = target
old, at = fields["Protocol"]
patched = bytearray(raw)
struct.pack_into("<i", patched, at, new_protocol)
obj.set_raw_data(bytes(patched))
new_bundle = env.file.save(packer="original")
print(f"Protocol {old} -> {new_protocol}; bundle {bundle_entry[2]} -> {len(new_bundle)} bytes")

# Rebuild the UnityWebData container with the new bundle; every other entry is copied byte for byte.
blobs = [new_bundle if e[0] == "data.unity3d" else d[e[1]:e[1] + e[2]] for e in entries]
header_len = 20 + sum(12 + len(e[0].encode()) for e in entries)
header = bytearray(b"UnityWebData1.0\0" + struct.pack("<I", header_len))
offset = header_len
for e, blob in zip(entries, blobs):
    name = e[0].encode()
    header += struct.pack("<III", offset, len(blob), len(name)) + name
    offset += len(blob)
with open(out, "wb") as fh:
    fh.write(bytes(header))
    for blob in blobs:
        fh.write(blob)
print("wrote", out, offset, "bytes")
