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

        private bool ValidatePreferences()
        {
            bool result = true;

            if (string.IsNullOrEmpty(how_many_dataSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(preferred_laptopSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(size_of_laptopSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(laptop_usageSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(laptop_battery_usageSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(laptop_durabilitySelect.Text)) result = false;
            else if (string.IsNullOrEmpty(night_usageSelect.Text)) result = false;
            else if (string.IsNullOrEmpty(cd_playerSelect.Text)) result = false;

            if(!result)
            {
                MessageBox.Show("Należy wypełnić wszystkie parametry w zakładce \"Preferencje\"", "Niepoprawne dane Preferencji");
                return result;
            }
            else if (min_price.Value > max_price.Value)
            {
                MessageBox.Show("Minimalna kwota nie może być większa od maksymalej", "Niepoprawne dane Preferencji");
                return false;
            }

            /*if (string.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("No Item is Selected");
            }
            else
            {
                MessageBox.Show("Item Selected is:" + comboBox1.Text);
            }*/
            //Item m = comboBox.Items[comboBox.SelectedIndex];
            //if(comboBox.SelectedIndex > -1) //somthing was selected
            //Item m = comboBox.SelectedItem;

            return result;
        }

        private void SwitchProgram()
        {
            if (programRunning)
            {
                programRunning = false;
                output("Zatrzymuję...", Color.Green);
                startStopProgram.Text = "Start";
                preferencesContainer.Enabled = true;
                customerAgentsTab.Clear();
            }
            else
            {
                if (!ValidatePreferences()) return;

                resultBox.Controls.Clear();

                preferencesContainer.Enabled = false;
                programRunning = true;
                output("Uruchamiam...", Color.Green);
                startStopProgram.Text = "Stop";

                for (int i = 1; i <= customerAgentsNumber.Value; i++)
                {
                    CustomerAgent customer = new CustomerAgent(i, resultBox, output);

                    customer.GetDatabase(i);

                    customerAgentsTab.Add(customer);
                }
                
                output("Wszyscy agenci wrócili", Color.Green);
                //SwitchProgram();

                //tabControl1.SelectedIndex = 3;
            }
        }

        private void startStopProgram_Click(object sender, EventArgs e)
        {
            SwitchProgram();
        }

        private void clearConsole_Click(object sender, EventArgs e)
        {
            outConsole.Text = "";
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(programRunning && e.TabPageIndex < 1) e.Cancel = true;
        }
    }
}
