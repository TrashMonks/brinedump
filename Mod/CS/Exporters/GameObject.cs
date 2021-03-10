using System;
using System.Linq;

using XRL;
using XRL.Core;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;
using XRL.World.Parts.Mutation;

using static TrashMonks.Brinedump.Static;

namespace TrashMonks.Brinedump
{
	public class GameObjectExporter : JSONExporter
	{
		public GameObjectExporter(string path) : base("GameObjects", path) { }

		/// <summary>
		/// Filter out blueprints that should not be serialised.
		/// </summary>
		bool BlueprintPredicate(GameObjectBlueprint blueprint) {
			if (!blueprint.HasPart("Render")) return false;
			if (blueprint.IsBaseBlueprint()) return false;
			if (blueprint.DescendsFrom("Widget")) return false;
			if (blueprint.DescendsFrom("DataBucket")) return false;

			return true;
		}

		// Uses non-thread safe pools for events & game objects, don't thread this.
		protected override void Write() {
			var factory = GameObjectFactory.Factory;
			var blueprints = factory.BlueprintList.Where(BlueprintPredicate)
				.OrderBy(x => x.Name)
				.ToList();

			for (int i = 0; i < blueprints.Count; i++) {
				var blueprint = NormalizeBlueprint(blueprints[i]);
				if (i % 20 == 0) Event.ResetPool(); // Reset pool so we don't balloon memory/past max pool size.

				try {
					var obj = factory.CreateObject(Blueprint: blueprint, BonusModChance: -100, SetModNumber: -1, Context: "Initialization");
					obj.RemovePart("Examiner"); // Mystifies the descriptions we want.
					obj.RemovePart("TerrainNotes"); // Expects a CurrentCell.

					JSON.WriteStartObject(blueprint.Name);
					JSON.WriteProperty("Tier", blueprint.Tier);
					WriteXPValue(obj);
					WriteDescription(obj);
					WriteMutations(obj);
					WriteSkills(obj);

					obj.pBrain?.PerformReequip(Silent: true);
					WriteStatistics(obj);

					JSON.WriteEndObject();
					factory.Pool(obj); // Clear & return created object to pool for re-use.
				} catch (Exception ex) {
					BDLog("GameObjectExporter:" + blueprint.Name, ex);
				}
			}
		}

		/// <summary>
		/// Remove elements of chance by including/excluding all.
		/// TODO: Deep clone and preserve original.
		/// </summary>
		GameObjectBlueprint NormalizeBlueprint(GameObjectBlueprint blueprint, bool max = false) {
			if (blueprint.Inventory == null) return blueprint;

			foreach (var item in blueprint.Inventory) {
				if (item.Chance <= 0 || item.Chance >= 100) continue;
				item.Chance = max ? 100 : 0;
			}

			return blueprint;
		}

		void WriteDescription(GameObject obj) {
			var description = obj.GetPart<Description>();
			if (description == null) return;

			JSON.WriteProperty("Description", description.Short);
		}

		void WriteMutations(GameObject obj) {
			var mutations = obj.GetPartsDescendedFrom<BaseMutation>();
			if (!mutations.Any()) return;

			JSON.WriteStartObject("Mutations");
			foreach (var mutation in mutations) {
				JSON.WriteProperty(mutation.Name, mutation.BaseLevel);
			}
			JSON.WriteEndObject();
		}

		void WriteSkills(GameObject obj) {
			var skills = obj.GetPartsDescendedFrom<BaseSkill>();
			if (!skills.Any()) return;

			JSON.WriteStartArray("Skills");
			foreach (var skill in skills) {
				JSON.WriteValue(skill.Name);
			}
			JSON.WriteEndArray();
		}

		void WriteXPValue(GameObject obj) {
			var xp = obj.GetStat("XPValue");
			if (xp == null) return;

			var value = obj.GetIntProperty("*XPValue", xp.Value);
			if (value == 0) return;

			JSON.WriteStartObject("XPValue");
			var level = obj.GetStatValue("Level");
			var lt = Math.Ceiling(level / 5d) * 5;
			var tier = level / 5;

			JSON.WriteProperty("Award <= " + (lt - 1), value);
			JSON.WriteProperty("Award >= " + lt, value / 2);
			JSON.WriteProperty("Award >= " + (lt + 5), value / 10);
			JSON.WriteProperty("Award >= " + (lt + 10), 0);

			JSON.WriteEndObject();
		}

		void WriteStatistics(GameObject obj) {
			JSON.WriteStartObject("Statistics");

			foreach (var stat in obj.Statistics) {
				WriteStatistic(stat.Value);
			}

			JSON.WriteEndObject();
		}

		void WriteStatistic(Statistic stat) {
			JSON.WriteStartObject(stat.Name);

			JSON.WriteProperty("Base", stat.BaseValue);
			JSON.WriteProperty("Bonus", stat.Bonus);
			JSON.WriteProperty("Penalty", stat.Penalty);
			JSON.WriteProperty("Boost", stat.Boost);
			JSON.WriteProperty("Value", stat.Value);
			WriteStringValue(stat);

			if (stat.TryGetCombatValue(out int combat)) {
				JSON.WriteProperty("Combat", combat);
			}

			JSON.WriteEndObject();
		}

		static string[] StatBlacklist = new[] { "*XP", "XPValue" };
		void WriteStringValue(Statistic stat) {
			var value = stat.sValue;
			if (string.IsNullOrEmpty(value)) return;
			if (StatBlacklist.Contains(stat.Name)) return;

			JSON.WriteStartObject("StringValue");

			var tier = 1 + stat.Owner.GetStatValue("Level") / 5;
			value = value.Replace("(t)", tier.ToString());
			value = value.Replace("(t+1)", (tier + 1).ToString());
			value = value.Replace("(t-1)", (tier - 1).ToString());

			var values = value.Split(',');
			var formula = string.Join("+", values);
			var min = values.Aggregate(0, (s, v) => s + v.RollMin());
			var max = values.Aggregate(0, (s, v) => s + v.RollMax());

			var factor = stat.Boost > 0 ? 0.25f : 0.2f;
			var minBoost = (int)Math.Ceiling(min * factor * stat.Boost);
			var maxBoost = (int)Math.Ceiling(max * factor * stat.Boost);
			if (stat.Boost != 0)
				formula += DiceForRange(minBoost, maxBoost);

			JSON.WriteProperty("Formula", formula);
			JSON.WriteProperty("Range", min == max ? $"{min}" : $"{min}...{max}");
			JSON.WriteProperty("Mean", (min + max) / 2f);
			if (stat.Boost != 0) {
				JSON.WriteProperty("BoostRange", minBoost == maxBoost ? $"{minBoost}" : $"{minBoost}...{maxBoost}");
				JSON.WriteProperty("BoostMean", (minBoost + maxBoost) / 2f);
			}

			JSON.WriteEndObject();
		}
	}
}
