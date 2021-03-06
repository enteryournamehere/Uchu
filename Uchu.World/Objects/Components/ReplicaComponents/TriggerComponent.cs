using System;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class TriggerComponent : ReplicaComponent
    {
        public Trigger Trigger { get; private set; }
        
        public override ComponentId Id => ComponentId.TriggerComponent;

        protected TriggerComponent()
        {
            Listen(OnStart, async () =>
            {
                if (!GameObject.Settings.TryGetValue("trigger_id", out var triggerIds)) return;

                Logger.Information($"{GameObject} Attempting trigger: {triggerIds}");
                
                var str = (string) triggerIds;
                var split = str.Split(':');

                if (split.Length != 2)
                {
                    Logger.Error($"{GameObject} Failed to parse trigger: {triggerIds}");
                    
                    return;
                }
                
                var fileId = int.Parse(split[0]);
                var triggerId = int.Parse(split[1]);

                await LoadTriggerAsync(fileId, triggerId);
            });
        }

        public async Task LoadTriggerAsync(int fileId, int triggerId)
        {
            var trigger = Zone.ZoneInfo.TriggerDictionary[fileId, triggerId];
                
            if (trigger == default)
            {
                trigger = Zone.ZoneInfo.TriggerDictionary[triggerId, fileId];

                if (trigger == default)
                {
                    Logger.Error($"{GameObject} Failed to find trigger: {triggerId}:{fileId}");

                    return;
                }
            }
                
            if (trigger.Enabled == 0) return;

            await LoadTriggerAsync(trigger);
        }

        public async Task LoadTriggerAsync(Trigger trigger)
        {
            Trigger = trigger;
            
            Logger.Information($"{GameObject} Trigger: {Trigger.FileId}:{Trigger.Id}");

            foreach (var @event in trigger.Events)
            {
                PhysicsComponent physics;
                switch (@event.Id)
                {
                    case "OnCreate":
                        await ExecuteEventAsync(@event);
                        break;
                    case "OnDestroy":
                        Listen(OnDestroyed, async () =>
                        {
                            await ExecuteEventAsync(@event);
                        });
                        break;
                    case "OnEnter":
                        physics = GameObject.AddComponent<PhysicsComponent>();
                            
                        Listen(physics.OnEnter, async other =>
                        {
                            Logger.Information($"Enter: {other.GameObject}");

                            await ExecuteEventAsync(@event, other.GameObject);
                        });
                        break;;
                    case "OnExit":
                        physics = GameObject.AddComponent<PhysicsComponent>();
                            
                        Listen(physics.OnLeave, async other =>
                        {
                            Logger.Information($"Left: {other.GameObject}");
                                
                            await ExecuteEventAsync(@event, other.GameObject);
                        });
                        break;
                    default:
                        Logger.Error($"Unsupported event type: {@event.Id}!");
                        break;
                }
            }
        }

        private async Task ExecuteEventAsync(TriggerEvent @event, params object[] arguments)
        {
            foreach (var command in @event.Commands)
            {
                await ExecuteTriggerCommandAsync(command, arguments);
            }
        }

        private async Task ExecuteTriggerCommandAsync(TriggerCommand command, params object[] arguments)
        {
            Logger.Information($"TRIGGER: {command.Id} -> {string.Join(", ", arguments)}");
            
            switch (command.Id)
            {
                case "SetPhysicsVolumeEffect":
                    await SetPhysicsVolumeEffectAsync(command);
                    break;
                case "CastSkill":
                    await CastSkillAsync(command, arguments);
                    break;
                case "pushObject":
                    await PushObjectAsync(command, arguments);
                    break;
                case "repelObject":
                    await RepealObjectAsync(command, arguments);
                    break;
            }

            GameObject.Serialize(GameObject);
        }

        private async Task PushObjectAsync(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = new Vector3
                {
                    X = float.Parse(parameters[0]),
                    Y = float.Parse(parameters[1]),
                    Z = float.Parse(parameters[2])
                }
            });
        }

        private async Task RepealObjectAsync(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = forward * float.Parse(parameters[0])
            });
        }

        private async Task CastSkillAsync(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;
            
            target.SendChatMessage($"Laser!");
            
            var parameters = command.Arguments.Split(',');

            var skill = int.Parse(parameters[0]);

            var skillComponent = GameObject.AddComponent<SkillComponent>();

            var _ = Task.Run(async () =>
            {
                try
                {
                    await skillComponent.CalculateSkillAsync(skill, target);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private async Task SetPhysicsVolumeEffectAsync(TriggerCommand command)
        {
            if (!GameObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) return;

            var arguments = command.Arguments.Split(',');
                    
            physicsComponent.IsEffectActive = true;

            var effectTypeInfo = typeof(PhantomPhysicsEffectType);

            var effectType = (PhantomPhysicsEffectType) Enum.Parse(effectTypeInfo, arguments[0]);

            physicsComponent.EffectType = effectType;

            var amount = float.Parse(arguments[1]);

            physicsComponent.EffectAmount = amount;

            if (arguments.Length > 2)
            {
                var direction = new Vector3
                {
                    X = float.Parse(arguments[2]),
                    Y = float.Parse(arguments[3]),
                    Z = float.Parse(arguments[4])
                };

                physicsComponent.EffectDirection = direction;
            }
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            var hasId = Trigger != default;

            writer.WriteBit(hasId);

            if (hasId) writer.Write(Trigger.Id);
        }
    }
}