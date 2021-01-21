using System.Collections.Generic;
using System.IO;
using LitJson;

using XRL;
using XRL.Core;

namespace TrashMonks.Brinedump
{
	[HasModSensitiveStaticCache]
	public static class Config
	{
		public static string FilePath => DataManager.SavePath("Brinedump.json");
		public static readonly string DefaultConfig = "{\n\t\"Paths\": {\n\t\t\"Text\": \"%SavePath%/Trash/Text\",\n\t\t\"Textures\": \"%SavePath%/Trash/Textures\"\n\t}\n}\n";

		static Dictionary<string, string> _Paths;
		public static Dictionary<string, string> Paths {
			get {
				if (_Paths == null) Read();
				return _Paths;
			}
		}

		/* TODO: Defaults should be written per-value, 
		 * LitJson's writing of human readable files is more broken every time I use it however, wait until newtonsoft is usable/netstandard in mod domain */
		[ModSensitiveCacheInit]
		public static void Write() {
			if (File.Exists(FilePath)) return;
			File.WriteAllText(FilePath, DefaultConfig);
		}

		public static void Read() {
			_Paths = new Dictionary<string, string>();

			Write();
			var stream = new StreamReader(FilePath);
			var reader = new JsonReader(stream);
			while (reader.Read()) {
				if (reader.Token == JsonToken.PropertyName) {
					if ("Paths".Equals(reader.Value)) ReadPaths(reader);
				}
			}
			reader.Close();
			stream.Dispose();
		}

		public static void ReadPaths(JsonReader reader) {
			while (reader.Read()) {
				if (reader.Token == JsonToken.ObjectEnd) break;
				if (reader.Token == JsonToken.PropertyName) {
					var key = reader.Value as string;
					reader.Read();
					_Paths[key] = SubstitutePaths(reader.Value as string);
				}
			}
		}

		public static string SubstitutePaths(string str) {
			str = str.Replace("%SavePath%", XRLCore.SavePath);
			return str.Replace("%DataPath%", XRLCore.DataPath);
		}
	}

}
