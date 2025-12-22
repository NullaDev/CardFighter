using System;
using GameLogic.Buff;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Card.Engine
{
    public class EntityActionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(EntityAction);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            var action = new EntityAction();

            var selectorToken = obj["Selector"];
            if (selectorToken != null)
            {
                var selectorType = selectorToken["Type"]?.ToString();
                action.Selector = selectorType switch
                {
                    "empty" => selectorToken.ToObject<EmptySelector>(serializer),
                    "all" => selectorToken.ToObject<AllSelector>(serializer),
                    "self" => selectorToken.ToObject<SelfSelector>(serializer),
                    "player" => selectorToken.ToObject<PlayerSelector>(serializer),
                    "range" => selectorToken.ToObject<RangeSelector>(serializer),
                    _ => throw new Exception("Unknown selector type: " + selectorType)
                };
            }
            else
            {
                action.Selector = new EmptySelector();
            }

            var filtersToken = obj["Filters"];
            if (filtersToken != null)
            {
                foreach (var f in filtersToken)
                {
                    var type = f["Type"]?.ToString();
                    EntityFilter filter = type switch
                    {
                        "first_n" => f.ToObject<FirstNFilter>(serializer),
                        "last_n" => f.ToObject<LastNFilter>(serializer),
                        "exclude_self" => f.ToObject<ExcludeSelfFilter>(serializer),
                        "is_alive" => f.ToObject<IsAliveFilter>(serializer),
                        "health" => f.ToObject<HealthFilter>(serializer),
                        "entity_type" => f.ToObject<TypeFilter>(serializer),
                        "condition" => f.ToObject<ConditionFilter>(serializer),
                        "has_buff" => f.ToObject<HasBuffFilter>(serializer),
                        "buff_param" => f.ToObject<BuffParamFilter>(serializer),
                        "name" => f.ToObject<NameFilter>(serializer),
                        "at_player_cached_pos" => f.ToObject<AtPlayerCachedPosFilter>(serializer),
                        _ => throw new Exception("Unknown filter type: " + type)
                    };
                    action.Filters.Add(filter);
                }
            }

            var processorsToken = obj["Processors"];
            if (processorsToken != null)
            {
                foreach (var p in processorsToken)
                {
                    var type = p["Type"]?.ToString();
                    EntityProcessor processor = type switch
                    {
                        "add_buff" => new AddBuffProcessor
                        {
                            Buffs = BuffEffectRule.ParseBuffs(p["Buffs"], serializer)
                        },
                        "move" => p.ToObject<MoveProcessor>(serializer),
                        "turn" => p.ToObject<TurnProcessor>(serializer),
                        "damage" => p.ToObject<DamageProcessor>(serializer),
                        "ratio_damage" => p.ToObject<RatioDamageProcessor>(serializer),
                        "add_cost" => p.ToObject<AddCostProcessor>(serializer),
                        "add_armor" => p.ToObject<AddArmorProcessor>(serializer),
                        "move_attack" => p.ToObject<MoveAttackProcessor>(serializer),
                        "kill" => p.ToObject<KillProcessor>(serializer),
                        "clear_buff" => p.ToObject<ClearBuffProcessor>(serializer),
                        "remove_buff" => p.ToObject<RemoveBuffProcessor>(serializer),
                        "summon" => p.ToObject<SummonProcessor>(serializer),
                        "execute_action" => new ExecuteActionProcessor
                        {
                            Action = p["Action"]?.ToObject<EntityAction>(serializer)
                        },
                        _ => throw new Exception("Unknown processor type: " + type)
                    };

                    action.Processors.Add(processor);
                }
            }

            return action;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}