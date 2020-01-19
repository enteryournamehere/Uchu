using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.MissionSystem
{
    public class MissionInstance
    {
        public Player Player { get; }
        
        public int MissionId { get; }
        
        public MissionTaskBase[] Tasks { get; private set; }

        public static Dictionary<MissionTaskType, Type> TaskTypes { get; private set; }
        
        static MissionInstance()
        {
            TaskTypes = new Dictionary<MissionTaskType, Type>();
            
            var types = typeof(MissionInstance).Assembly.GetTypes().Where(
                t => t.BaseType == typeof(MissionTaskBase)
            );
            
            foreach (var type in types)
            {
                if (type.IsAbstract) continue;
                
                var instance = (MissionTaskBase) Activator.CreateInstance(type);

                try
                {
                    TaskTypes[instance.Type] = type;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                }
            }
        }
        
        public MissionInstance(Player player, int missionId)
        {
            Player = player;

            MissionId = missionId;
        }

        public async Task LoadAsync()
        {
            await using var cdClient = new CdClientContext();

            var clientTasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();

            var tasks = new List<MissionTaskBase>();

            foreach (var clientTask in clientTasks)
            {
                var taskType = (MissionTaskType) (clientTask.TaskType ?? 0);

                if (!TaskTypes.TryGetValue(taskType, out var type))
                {
                    Logger.Error($"No {nameof(MissionTaskBase)} for {taskType} found.");
                    
                    continue;
                }

                var instance = (MissionTaskBase) Activator.CreateInstance(type);

                Debug.Assert(clientTask.Uid != null, "clientTask.Uid != null");
                
                await instance.LoadAsync(Player, MissionId, clientTask.Uid.Value);

                tasks.Add(instance);
            }

            Tasks = tasks.ToArray();
        }

        public async Task StartAsync()
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // Setup new mission
            //

            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );
            
            var tasks = await cdClient.MissionTasksTable.Where(
                t => t.Id == MissionId
            ).ToArrayAsync();
            
            if (mission != default) return;

            await ctx.Missions.AddAsync(new Mission
            {
                CharacterId = Player.ObjectId,
                MissionId = MissionId,
                Tasks = tasks.Select(task => new MissionTask
                {
                    TaskId = task.Uid ?? 0
                }).ToList()
            });

            await ctx.SaveChangesAsync();

            await UpdateMissionStateAsync(MissionState.Active);

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );

            MessageMissionTypeState(MissionLockState.New, clientMission.Definedsubtype, clientMission.Definedtype);
        }

        public async Task CompleteAsync(int rewardItem = default)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            //
            // If this mission is not already accepted, accept it and move on to complete it.
            //
            
            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );

            if (mission == default)
            {
                await StartAsync();
                
                mission = await ctx.Missions.FirstOrDefaultAsync(
                    m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
                );
            }

            //
            // Save changes to be able to update its state.
            //

            await ctx.SaveChangesAsync();

            await UpdateMissionStateAsync(MissionState.Unavailable, true);

            //
            // Get character mission to complete.
            //
            
            if (mission.State == (int) MissionState.Completed) return;
            
            mission.CompletionCount++;
            mission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();
            mission.State = (int) MissionState.Completed;
            
            await ctx.SaveChangesAsync();

            //
            // Inform the client it's now complete.
            //

            await UpdateMissionStateAsync(MissionState.Completed);

            //
            // Update player based on rewards.
            //

            await SendRewardsAsync(rewardItem);

            var _ = Task.Run(async () =>
            {
                var component = Player.GetComponent<MissionInventoryComponent>();

                await component.MissionCompleteAsync(MissionId);
            });
        }

        public async Task SendRewardsAsync(int rewardItem)
        {
            await using var ctx = new UchuContext();
            await using var cdClient = new CdClientContext();

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );
            
            if (clientMission.IsMission ?? true)
            {
                // Mission

                Player.Currency += clientMission.Rewardcurrency ?? 0;

                Player.UniverseScore += clientMission.LegoScore ?? 0;
            }
            else
            {
                var character = await ctx.Characters.FirstAsync(
                    c => c.CharacterId == Player.ObjectId
                );
                
                //
                // Achievement
                //
                // These rewards have the be silent, as the client adds them itself.
                //

                character.Currency += clientMission.Rewardcurrency ?? 0;
                character.UniverseScore += clientMission.LegoScore ?? 0;

                //
                // The client adds currency rewards as an offset, in my testing. Therefore we
                // have to account for this offset.
                //

                Player.HiddenCurrency += clientMission.Rewardcurrency ?? 0;

                await ctx.SaveChangesAsync();
            }

            var stats = Player.GetComponent<Stats>();

            await stats.BoostBaseHealth((uint) (clientMission.Rewardmaxhealth ?? 0));
            await stats.BoostBaseImagination((uint) (clientMission.Rewardmaximagination ?? 0));

            //
            // Get item rewards.
            //

            var inventory = Player.GetComponent<InventoryManagerComponent>();

            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );

            var repeat = mission.CompletionCount != 0;
            
            var rewards = new (Lot, int)[]
            {
                ((repeat ? clientMission.Rewarditem1repeatable : clientMission.Rewarditem1) ?? 0,
                    (repeat ? clientMission.Rewarditem1repeatcount : clientMission.Rewarditem1count) ?? 1),

                ((repeat ? clientMission.Rewarditem2repeatable : clientMission.Rewarditem2) ?? 0,
                    (repeat ? clientMission.Rewarditem2repeatcount : clientMission.Rewarditem2count) ?? 1),

                ((repeat ? clientMission.Rewarditem3repeatable : clientMission.Rewarditem3) ?? 0,
                    (repeat ? clientMission.Rewarditem3repeatcount : clientMission.Rewarditem3count) ?? 1),

                ((repeat ? clientMission.Rewarditem4repeatable : clientMission.Rewarditem4) ?? 0,
                    (repeat ? clientMission.Rewarditem4repeatcount : clientMission.Rewarditem4count) ?? 1),
            };

            var emotes = new[]
            {
                clientMission.Rewardemote ?? -1,
                clientMission.Rewardemote2 ?? -1,
                clientMission.Rewardemote3 ?? -1,
                clientMission.Rewardemote4 ?? -1
            };

            foreach (var i in emotes.Where(e => e != -1))
            {
                await Player.UnlockEmoteAsync(i);
            }

            var isMission = clientMission.IsMission ?? true;
            
            if (rewardItem <= 0)
            {
                foreach (var (rewardLot, rewardCount) in rewards)
                {
                    var lot = rewardLot;
                    var count = rewardCount;
                    
                    if (lot == default || count == default) continue;

                    if (isMission)
                    {
                        var _ = Task.Run(async () =>
                        {
                            await inventory.AddItemAsync(lot, (uint) count);
                        });
                    }
                    else
                    {
                        var _ = Task.Run(async () =>
                        {
                            await Task.Delay(10000);
                            
                            await inventory.AddItemAsync(lot, (uint) count);
                        });
                    }
                }
            }
            else
            {
                var (lot, count) = rewards.FirstOrDefault(l => l.Item1 == rewardItem);

                if (lot != default && count != default)
                {
                    var _ = Task.Run(async () =>
                    {
                        await inventory.AddItemAsync(lot, (uint) count);
                    });
                }
            }
        }

        public async Task<bool> IsCompleteAsync()
        {
            foreach (var task in Tasks)
            {
                var isComplete = await task.IsCompleteAsync();

                if (!isComplete) return false;
            }

            return true;
        }

        public async Task SoftCompleteAsync()
        {
            await using var cdClient = new CdClientContext();

            var clientMission = await cdClient.MissionsTable.FirstAsync(
                m => m.Id == MissionId
            );

            if (clientMission.IsMission ?? false)
            {
                await UpdateMissionStateAsync(MissionState.ReadyToComplete);
                
                return;
            }

            await CompleteAsync();
        }

        public async Task<MissionState> GetMissionStateAsync()
        {
            await using var ctx = new UchuContext();
            
            var mission = await ctx.Missions.FirstOrDefaultAsync(
                m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
            );

            return (MissionState) mission.State;
        }
        
        public async Task UpdateMissionStateAsync(MissionState state, bool sendingRewards = false)
        {
            await using (var ctx = new UchuContext())
            {
                var mission = await ctx.Missions.FirstOrDefaultAsync(
                    m => m.CharacterId == Player.ObjectId && m.MissionId == MissionId
                );

                mission.State = (int) state;

                await ctx.SaveChangesAsync();
            }
            
            Player.Message(new NotifyMissionMessage
            {
                Associate = Player,
                MissionId = MissionId,
                MissionState = state,
                SendingRewards = sendingRewards
            });
        }

        public void MessageMissionTypeState(MissionLockState state, string subType, string type)
        {
            Player.Message(new SetMissionTypeStateMessage
            {
                Associate = Player,
                LockState = state,
                SubType = subType,
                Type = type
            });
        }
    }
}