using System.Linq;
using NUnit.Framework;
using Portfolio.Core;

public class RoomListCacheTests
{
	[Test]
	public void PartialUpdate_KeepsRoomsThatWereNotMentioned()
	{
		RoomListCache<string> cache = new RoomListCache<string>();
		cache.Apply("A", false, "A");
		cache.Apply("B", false, "B");

		// Photon then sends only the change for C: A and B must survive.
		cache.Apply("C", false, "C");

		CollectionAssert.AreEquivalent(new[] { "A", "B", "C" }, cache.Rooms.ToArray());
	}

	[Test]
	public void RemovedRoom_IsDropped()
	{
		RoomListCache<string> cache = new RoomListCache<string>();
		cache.Apply("A", false, "A");
		cache.Apply("B", false, "B");

		cache.Apply("A", true, null);

		CollectionAssert.AreEquivalent(new[] { "B" }, cache.Rooms.ToArray());
	}

	[Test]
	public void UpdatedRoom_ReplacesTheOldEntry()
	{
		RoomListCache<string> cache = new RoomListCache<string>();
		cache.Apply("A", false, "A v1");

		cache.Apply("A", false, "A v2");

		Assert.AreEqual(1, cache.Count);
		Assert.AreEqual("A v2", cache.Rooms.Single());
	}
}
