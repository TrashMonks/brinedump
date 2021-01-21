using System;
using System.Collections.Generic;
using System.Reflection;

using XRL.World;
using XRL.Rules;
using LitJson;

namespace TrashMonks.Brinedump
{
	public static class Extensions
	{
		public static void WriteProperty(this JsonWriter JSON, string name, object value) => JSON.WriteProperty(name, value.ToString());
		public static void WriteProperty(this JsonWriter JSON, string name, string value) {
			JSON.WritePropertyName(name);
			JSON.Write(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, int value) {
			JSON.WritePropertyName(name);
			JSON.Write(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, double value) {
			JSON.WritePropertyName(name);
			JSON.Write(value);
		}

		public static void WriteProperty(this JsonWriter JSON, string name, bool value) {
			JSON.WritePropertyName(name);
			JSON.Write(value);
		}

		public static void WriteStartObject(this JsonWriter JSON, string name) {
			JSON.WritePropertyName(name);
			JSON.WriteObjectStart();
		}

		public static void WriteStartArray(this JsonWriter JSON, string name) {
			JSON.WritePropertyName(name);
			JSON.WriteArrayStart();
		}

		public static bool TryGetCombatValue(this Statistic stat, out int value) {
			value = 0;

			var method = typeof(Stats).GetMethod("GetCombat" + stat.Name, BindingFlags.Static | BindingFlags.Public);
			if (method == null) return false;

			value = (int)method.Invoke(null, new object[] { stat.Owner });
			return true;
		}
	}
}
