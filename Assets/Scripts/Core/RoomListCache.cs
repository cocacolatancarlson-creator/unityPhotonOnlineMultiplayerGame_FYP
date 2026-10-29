using System.Collections.Generic;

namespace Portfolio.Core
{
	// Photon sends OnRoomListUpdate as a list of CHANGES, not the full list.
	// The 2022 Launcher cleared the UI and redrew only the changes, so rooms vanished.
	// This keeps the full list keyed by room name and applies each change to it.
	public sealed class RoomListCache<T>
	{
		readonly Dictionary<string, T> rooms = new Dictionary<string, T>();

		public int Count => rooms.Count;

		public IEnumerable<T> Rooms => rooms.Values;

		public void Apply(string name, bool removed, T room)
		{
			if (removed)
			{
				rooms.Remove(name);
			}
			else
			{
				rooms[name] = room;
			}
		}

		public void Clear()
		{
			rooms.Clear();
		}
	}
}
