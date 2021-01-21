using XRL.Wish;
using XRL.UI;

namespace TrashMonks.Brinedump
{
	[HasWishCommand]
	public static class Wishes
	{
		[WishCommand(Command = "brinedump:gameobjects")]
		public static void Export() {
			Loading.LoadTask($"Writing GameObjects.json...", new GameObjectExporter(Config.Paths["Text"]).Export);
		}

		[WishCommand(Command = "brinedump:textures")]
		public static void ExportTextures() {
			new TextureExporter(Config.Paths["Textures"]).Export();
		}
	}
}
