using System.Collections.Generic;
using System.Threading.Tasks;

namespace Uchu.World.Systems.Behaviors
{
    public class ChainBehavior : BehaviorBase
    {
        public override BehaviorTemplateId Id => BehaviorTemplateId.Chain;
        
        public int Delay { get; set; }
        
        public BehaviorBase[] Behaviors { get; set; }

        public override async Task BuildAsync()
        {
            var actions = GetParameters();

            var behaviors = new List<BehaviorBase>();

            for (var i = 0; i < actions.Length; i++)
            {
                var behavior = await GetBehavior($"behavior {i + 1}");
                
                if (behavior == default) continue;

                behaviors.Add(behavior);
            }

            Behaviors = behaviors.ToArray();
            
            var delay = await GetParameter("chain_delay");
            
            if (delay.Value == null) return;

            Delay = (int) delay.Value;
        }

        public override async Task ExecuteAsync(ExecutionContext context, ExecutionBranchContext branchContext)
        {
            await base.ExecuteAsync(context, branchContext);
            
            var chainIndex = context.Reader.Read<uint>();

            await Behaviors[chainIndex - 1].ExecuteAsync(context, branchContext);
        }

        public override async Task CalculateAsync(NpcExecutionContext context, ExecutionBranchContext branchContext)
        {
            // TODO
            
            context.Writer.Write(1);
            
            await Behaviors[1 - 1].CalculateAsync(context, branchContext);
        }
    }
}