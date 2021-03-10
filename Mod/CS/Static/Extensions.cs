using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

using XRL.World;
using XRL.Rules;
using Newtonsoft.Json;

namespace TrashMonks.Brinedump
{
	public static class Extensions
	{
		public static void WriteProperty(this JsonWriter JSON, string name, object value) => JSON.WriteProperty(name, value.ToString());
		public static void WriteProperty(this JsonWriter JSON, string name, string value) {
			JSON.WritePropertyName(name);
			JSON.WriteValue(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, int value) {
			JSON.WritePropertyName(name);
			JSON.WriteValue(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, double value) {
			JSON.WritePropertyName(name);
			JSON.WriteValue(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, bool value) {
			JSON.WritePropertyName(name);
			JSON.WriteValue(value);
		}

		public static void WriteStartObject(this JsonWriter JSON, string name) {
			JSON.WritePropertyName(name);
			JSON.WriteStartObject();
		}

		public static void WriteStartArray(this JsonWriter JSON, string name) {
			JSON.WritePropertyName(name);
			JSON.WriteStartArray();
		}

		// TODO: Probably better to just hardcode this
		public static bool TryGetCombatValue(this Statistic stat, out int value) {
			value = 0;

			try {
				var method = typeof(Stats).GetMethod("GetCombat" + stat.Name, BindingFlags.Static | BindingFlags.Public);
				if (method == null) return false;

				var types = method.GetParameters().Select(x => x.ParameterType);
				if (types.SequenceEqual(new[] { typeof(GameObject) }))
					value = (int)method.Invoke(null, new object[] { stat.Owner });
				else if (types.SequenceEqual(new[] { typeof(GameObject), typeof(bool) }))
					value = (int)method.Invoke(null, new object[] { stat.Owner, false });
				else return false;

				return true;
			} catch {
				return false;
			}
		}
	}
}
