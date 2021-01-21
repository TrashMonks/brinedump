using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using XRL.UI;
using UnityEngine;

using static TrashMonks.Brinedump.Static;

namespace TrashMonks.Brinedump
{
	public class TextureExporter : Exporter
	{
		Dictionary<string, Texture2D> Textures;

		public TextureExporter(string path) : base(path) { }

		public override void Export() {
			GameManager.Instance.uiQueue.queueTask(() => {
				BDLog("Exporting textures...");
				LoadTextures();
				Task.Factory.StartNew(ExportTask);
			});
		}

		public void LoadTextures() {
			BDLog("Loading texture info...");
			Textures = new Dictionary<string, Texture2D>();

			var infoList = Resources.LoadAll<exTextureInfo>("TextureInfo");
			foreach (var info in infoList) {
				if (info == null) continue;

				var sprite = Kobold.SpriteManager.GetUnitySprite(info);
				if (sprite == null) continue;

				Textures[info.name] = sprite.texture;
			}
		}

		private void ExportTask() {
			BDLog("Writing texture files...");
			var files = Directory.GetFiles("*", SearchOption.AllDirectories).ToDictionary(x => x.FullName, x => false);
			foreach (var pair in Textures) {
				try {
					var name = pair.Key.Substring(24);
					var ul = name.IndexOf('_');
					var file = name.Substring(ul + 1);

					var dir = new DirectoryInfo(Path.Combine(Directory.FullName, name.Substring(0, ul)));
					dir.Create();
					dir.Refresh();

					Loading.SetLoadingStatus($"Writing {file}...");
					var path = Path.Combine(dir.FullName, file);
					var bytes = pair.Value.EncodeToPNG();

					if (files.ContainsKey(path)) {
						files[path] = true;
						var old = File.ReadAllBytes(path);
						if (bytes.SequenceEqual(old)) continue;
						else BDLog($"{file} changed.");
					} else {
						BDLog($"{file} new.");
					}

					File.WriteAllBytes(path, bytes);
				} catch (Exception e) {
					BDLog($"Error writing texture: {pair.Key}", e);
					break;
				}
			}
			foreach (var pair in files) {
				if (pair.Value) continue;
				var relative = pair.Key.Replace(Directory.FullName, "");
				BDLog($"{relative} foreign.");
			}
			Loading.SetLoadingStatus("Done!");
			Loading.SetLoadingStatus(null);
			BDLog("Finished exporting textures.");
		}
	}
}
