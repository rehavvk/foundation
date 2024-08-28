using System.Threading.Tasks;
using UnityEngine;

namespace Rehawk.Foundation.Misc
{
    public class TaskYieldInstruction : CustomYieldInstruction
    {
        private readonly Task task;
        
        public TaskYieldInstruction(Task task)
        {
            this.task = task;
        }

        public override bool keepWaiting => !task.IsCompleted;
    }
}