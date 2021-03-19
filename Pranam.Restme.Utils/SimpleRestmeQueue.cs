using System.Collections.Generic;

namespace Pranam
{
    public class SimpleRestmeQueue<TA>
    {
        public bool ExecutionWaitRequired = false;

        public List<TA> QueueItems;
    }
}