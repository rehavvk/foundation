using System.Threading.Tasks;
using Rehawk.Foundation.Misc;

namespace Rehawk.Foundation.Extensions
{
    public static class TaskExtensions
    {
        public static TaskYieldInstruction CoroutineWait(this Task task)
        {
            return new TaskYieldInstruction(task);
        }
    }
}