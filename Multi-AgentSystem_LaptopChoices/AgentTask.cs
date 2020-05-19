using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multi_AgentSystem_LaptopChoices
{
    class AgentTask
    {
        public Task task;
        public List<object> response;
        public List<object> recieve;
        public bool isBusy;

        public void SetAgentTask(ref Task task)
        {
            this.task = task;
        }
    }
}
