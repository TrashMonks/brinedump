using System;
using System.IO;

using Newtonsoft.Json;

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
			JSON = new JsonTextWriter(writer) { Formatting = Formatting.Indented, IndentChar = '\t', Indentation = 1 };
			JSON.WriteStartObject();

			Write();

			JSON.WriteEndObject();
			//JSON.Close();
			writer.Flush();
			writer.Dispose();
			BDLog($"Finished writing {FileName}.");
		}

		protected abstract void Write();
	}
}
