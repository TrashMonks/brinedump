using System.IO;

namespace TrashMonks.Brinedump
{
	public abstract class Exporter
	{
		protected DirectoryInfo Directory { get; set; }

		public Exporter(string path) {
			Directory = new DirectoryInfo(path);
			Directory.Create();
			Directory.Refresh();
		}

		public abstract void Export();
	}
}
