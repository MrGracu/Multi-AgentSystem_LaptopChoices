using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_AgentSystem_LaptopChoices
{
    public partial class Form1 : Form
    {
        bool programRunning = false;
        Random random = new Random();

        List<CustomerAgent> customerAgentsTab = new List<CustomerAgent>();

        private void output(string text, Color? c = null)
        {
            outConsole.SelectionStart = outConsole.TextLength;
            outConsole.SelectionLength = 0;
            outConsole.SelectionColor = Color.DarkGray;
            outConsole.AppendText("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] ");
            outConsole.SelectionColor = c ?? outConsole.ForeColor;
            outConsole.AppendText(text + Environment.NewLine);
            outConsole.SelectionStart = outConsole.Text.Length;
            outConsole.ScrollToCaret();
            //outConsole.Refresh();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void switchProgram()
        {
            if (programRunning)
            {
                programRunning = false;
                output("Zatrzymuję...", Color.Green);
                startStopProgram.Text = "Start";
            }
            else
            {
                tabPage4.Controls.Clear();

                programRunning = true;
                output("Uruchamiam...", Color.Green);
                startStopProgram.Text = "Stop";

                for (int i = 1; i <= customerAgentsNumber.Value; i++)
                {
                    CustomerAgent customer = new CustomerAgent(i);

                    customer.getDatabase(tabPage4, i);
                    output(customer.getInfo(), Color.Blue);

                    customerAgentsTab.Add(customer);
                }

                output("Wszyscy agenci wrócili", Color.Green);
                switchProgram();

                tabControl1.SelectedIndex = 3;
            }
        }

        private void startStopProgram_Click(object sender, EventArgs e)
        {
            switchProgram();
        }

        private void clearConsole_Click(object sender, EventArgs e)
        {
            outConsole.Text = "";
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(programRunning && e.TabPageIndex < 2) e.Cancel = true;
        }
    }
}
