using XRL;
using XRL.Core;
using XRL.World;

namespace TrashMonks.Brinedump
{
	public static partial class Static
	{
		public static XRLGame GAME => XRLCore.Core.Game;
		public static GameObject PLAYER => GAME.Player.Body;
		public static Zone ZONE => GAME.ZoneManager.ActiveZone;

		public const string DESC_DEFAULT = "A hideous specimen.";
	}
}
