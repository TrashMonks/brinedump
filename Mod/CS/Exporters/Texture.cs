using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.IO;

using XRL.UI;
using UnityEngine;
using static TrashMonks.Brinedump.Static;

namespace TrashMonks.Brinedump
{
	public class TextureExporter : Exporter
	{
		public TextureExporter(string path) : base(path)
		{
		}

		public override void Export()
		{
			GameManager.Instance.uiQueue.queueTask(() => GameManager.Instance.StartCoroutine(ExportTask()));
		}

		private IEnumerator ExportTask()
		{
			BDLog("Exporting textures...");
			var files = Directory.GetFiles("*", SearchOption.AllDirectories).ToDictionary(x => x.FullName, x => false);
			var file = new StringBuilder();
			var dirPath = new StringBuilder();
			int n = 0, m = 10;

			foreach (var info in Resources.LoadAll<exTextureInfo>("TextureInfo"))
			{
				if (n++ >= m)
				{
					// aim for around 30-50 fps
					var t = Time.deltaTime;
					if (t > 0.03) m--;
					else if (t < 0.02) m++;

					n = 0;
					yield return null;
				}

				if (info == null) continue;

				try
				{
					file.Clear();
					dirPath.Clear().Append(Directory.FullName);
					if (dirPath.EndsWith('/') || dirPath.EndsWith('\\'))
					{
						dirPath.Remove(dirPath.Length - 1, 1);
					}

					var end = false;
					var parts = info.name.Substring(24).Split('_');
					for (int i = 0, l = parts.Length; i < l; i++)
					{
						var part = parts[i];
						if (!end && (Char.IsLower(part[0]) || i == l - 1)) // banking on files lc and dirs uc
						{
							end = true;
						}

						if (end) file.Compound(part, '_');
						else dirPath.Compound(part, Path.DirectorySeparatorChar);
					}

					var dir = new DirectoryInfo(dirPath.ToString());
					dir.Create();
					dir.Refresh();

					Loading.SetLoadingStatus($"Writing {file}...");
					var path = dirPath.Append(Path.DirectorySeparatorChar).Append(file).ToString();
					var bytes = Kobold.SpriteManager.GetUnitySprite(info).texture.EncodeToPNG();

					if (files.ContainsKey(path))
					{
						files[path] = true;
						var old = File.ReadAllBytes(path);
						if (bytes.SequenceEqual(old)) continue;
						else BDLog($"{file} changed.");
					}
					else
					{
						BDLog($"{file} new.");
					}

					File.WriteAllBytes(path, bytes);
				}
				catch (Exception e)
				{
					BDLog($"Error writing texture: {info.name}", e);
					break;
				}
			}

			foreach (var pair in files)
			{
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