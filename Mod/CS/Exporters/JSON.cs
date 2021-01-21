using System;
using System.IO;

using LitJson;

using static TrashMonks.Brinedump.Static;

namespace TrashMonks.Brinedump
{
	public abstract class JSONExporter : Exporter
	{
		public string FileName { get; set; }
		protected JsonWriter JSON { get; set; }

		public JSONExporter(string fileName, string path) : base(path) {
			FileName = fileName;
		}

		public override void Export() {
			var file = Path.Combine(Directory.FullName, String.Format("{0}.{1:yyyyMMddHHmmss}.json", FileName, DateTime.Now));
			var writer = new StreamWriter(file);
			BDLog($"Writing {FileName}...");
			// This pretty print's pretty garbage, tries to align everything in one pass and inevitably ends up havin' to indent more later.
			// Switch over to newtonsoft when the netstandard facade is in the mod script domain.
			// Bloody hell, f#$¤ this pretty print, just use an external tool.
			JSON = new JsonWriter(writer) { PrettyPrint = false };
			JSON.WriteObjectStart();

			Write();

			JSON.WriteObjectEnd();
			//JSON.Close();
			writer.Flush();
			writer.Dispose();
			BDLog($"Finished writing {FileName}.");
		}

		protected abstract void Write();
	}
}
