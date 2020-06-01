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
        public int id;
        public bool isBusy;

        public void SetAgentTask(ref Task task)
        {
            this.task = task;
            this.response = new List<object>();
            this.recieve = new List<object>();
        }

        public bool IsBusy(ref int[] parameters)
        {
            bool busy = false;
            if (isBusy)
            {
                busy = true;
            }
            else
            {
                isBusy = true;
                response.Clear();
                recieve.Clear();
                recieve.Add(parameters);
            }
            return busy;
        }
    }
}
