using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_AgentSystem_LaptopChoices
{
    public partial class Form1 : Form
    {
        bool programRunning = false;
        Random random = new Random();
        bool stopTasks = false;

        string connectionStringSeller = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_seller;";
        string connectionStringCustomer = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_customer;";

        List<Task> customerAgentsTab = new List<Task>();
        List<AgentTask> sellerAgentsTab = new List<AgentTask>();

        List<string[]> customersProducts = new List<string[]>(); //(4) [0] - customer id, [1] - price, [2] - name, [3] - product link;

        int[] parameters = new int[8];
        int[] priority = new int[8];
        int minPrice, maxPrice;

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

            return result;
        }

        private async Task<bool> WaitForCustomers()
        {
            bool finished = false;
            while (!finished)
            {
                finished = true;
                foreach (var agent in customerAgentsTab)
                {
                    if (!agent.IsCompleted) finished = false;
                }
                await Task.Delay(500);
            }

            if(finished && programRunning)
            {
                output("Wszyscy agenci wrócili", Color.YellowGreen);
                SwitchProgram();
            }

            return finished;
        }

        private void CustomersFinished()
        {
            WaitForCustomers();
        }

        private void SwitchProgram()
        {
            if (programRunning)
            {
                programRunning = false;
                output("Zatrzymuję...", Color.Green);
                startStopProgram.Text = "Start";
                preferencesContainer.Enabled = true;
                stopTasks = true;
                ShowItems();
            }
            else
            {
                if (!ValidatePreferences()) return;

                stopTasks = false;
                customerAgentsTab.Clear();
                sellerAgentsTab.Clear();
                resultBox.Controls.Clear();
                customersProducts.Clear();

                preferencesContainer.Enabled = false;
                programRunning = true;
                output("Uruchamiam...", Color.Green);
                startStopProgram.Text = "Stop";

                parameters[0] = 1 + how_many_dataSelect.SelectedIndex;
                priority[0] = how_many_dataPriority.Value;
                parameters[1] = 4 + preferred_laptopSelect.SelectedIndex;
                priority[1] = preferred_laptopPriority.Value;
                parameters[2] = 7 + size_of_laptopSelect.SelectedIndex;
                priority[2] = size_of_laptopPriority.Value;
                parameters[3] = 10 + laptop_usageSelect.SelectedIndex;
                priority[3] = laptop_usagePriority.Value;
                parameters[4] = 14 + laptop_battery_usageSelect.SelectedIndex;
                priority[4] = laptop_battery_usagePriority.Value;
                parameters[5] = 17 + laptop_durabilitySelect.SelectedIndex;
                priority[5] = laptop_durabilityPriority.Value;
                parameters[6] = 20 + night_usageSelect.SelectedIndex;
                priority[6] = night_usagePriority.Value;
                parameters[7] = 23 + cd_playerSelect.SelectedIndex;
                priority[7] = cd_playerPriority.Value;

                minPrice = Decimal.ToInt32(min_price.Value);
                maxPrice = Decimal.ToInt32(max_price.Value);
                
                for (int i = 1; i <= sellerAgentsNumber.Value; i++)
                {
                    SellerAgent temp = new SellerAgent(i, outConsole, ref random);
                    AgentTask agentTask = new AgentTask();
                    Task tempTask = Task.Run(() => temp.RunAgent(ref stopTasks, ref agentTask.recieve, ref agentTask.response));

                    agentTask.SetAgentTask(ref tempTask);
                    agentTask.id = i;

                    sellerAgentsTab.Add(agentTask);
                }

                for (int i = 1; i <= customerAgentsNumber.Value; i++)
                {
                    CustomerAgent temp = new CustomerAgent(i, Decimal.ToInt32(maxLapsNumber.Value), outConsole);

                    customerAgentsTab.Add(Task.Run(() => temp.RunAgent(ref stopTasks, ref sellerAgentsTab, ref customersProducts, ref parameters, ref priority, ref maxPrice, ref minPrice)));
                }

                CustomersFinished();
            }
        }

        private void OpenLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void SelectProduct(object sender, EventArgs e)
        {
            //Here select product
            int agentID = int.Parse((sender as Button).Name.Substring(11));
            output("Wybrano przedmiot klienta nr " + agentID, Color.Green);
            resultBox.Controls.Clear();
        }

        public void ShowItems()
        {
            for (int i = customersProducts.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < customersProducts.Count; j++)
                {
                    if(i != j && int.Parse(customersProducts[i][1]) >= int.Parse(customersProducts[j][1]) && customersProducts[i][2] == customersProducts[j][2] && customersProducts[i][3] == customersProducts[j][3])
                    {
                        output("Znaleziono takie same produkty u agentów, więc usuwam droższe", Color.Red);
                        customersProducts.RemoveAt(i);
                    }
                }
            }

            foreach (string[] product in customersProducts)
            {
                GroupBox box = new GroupBox();
                box.Dock = DockStyle.Top;
                box.Text = "Produkt agenta nr " + product[0];
                box.Name = "groupBoxAgent" + product[0];
                box.Height = 120;

                Label mylab = new Label();
                mylab.Text = ("Model: " + product[2]);
                mylab.Dock = DockStyle.Top;
                mylab.Font = new Font("Calibri", 12);
                mylab.ForeColor = Color.Green;

                Label mylab1 = new Label();
                mylab1.Text = ("Cena: " + product[1] + " zł");
                mylab1.Dock = DockStyle.Top;
                mylab1.Font = new Font("Calibri", 12);
                mylab1.ForeColor = Color.Green;

                LinkLabel dynamicLinkLabel = new LinkLabel();
                dynamicLinkLabel.ForeColor = Color.Black;
                dynamicLinkLabel.ActiveLinkColor = Color.Black;
                dynamicLinkLabel.VisitedLinkColor = Color.Black;
                dynamicLinkLabel.LinkColor = Color.Black;
                dynamicLinkLabel.DisabledLinkColor = Color.Black;
                dynamicLinkLabel.Text = "Link do sklepu";
                dynamicLinkLabel.Name = "linkAgent" + product[0];
                dynamicLinkLabel.Font = new Font("Calibri", 12);
                dynamicLinkLabel.Links.Add(0, dynamicLinkLabel.Text.Length, product[3]);
                dynamicLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OpenLink);
                dynamicLinkLabel.Dock = DockStyle.Top;

                Button dynamicButton = new Button();
                dynamicButton.Height = 30;
                dynamicButton.Text = "Wybierz tę ofertę klienta nr " + product[0];
                dynamicButton.Name = "buttonAgent" + product[0];
                dynamicButton.Font = new Font("Calibri", 12);
                dynamicButton.Click += new EventHandler(SelectProduct);
                dynamicButton.Dock = DockStyle.Top;

                box.Controls.Add(dynamicButton);
                box.Controls.Add(dynamicLinkLabel);
                box.Controls.Add(mylab1);
                box.Controls.Add(mylab);

                resultBox.Controls.Add(box);
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

        private void clearCustomersDatabaseButton_Click(object sender, EventArgs e)
        {
            string query = "SHOW TABLES LIKE 'agent_%';";

            MySqlConnection databaseConnection = new MySqlConnection(connectionStringCustomer);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                query = "";
                databaseConnection.Open();
                MySqlDataReader reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    query = "DROP TABLE IF EXISTS";
                    while (reader.Read())
                    {
                        query += " " + reader.GetString(0) + ",";
                    }
                    query = query.Remove(query.Length - 1, 1) + ";";
                }
                databaseConnection.Close();

                if (query.Length > 0)
                {
                    commandDatabase = new MySqlCommand(query, databaseConnection);
                    commandDatabase.CommandTimeout = 60;
                    databaseConnection.Open();
                    reader = commandDatabase.ExecuteReader();
                    databaseConnection.Close();
                    output("Zresetowano bazy danych klientów", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void clearSellersDatabaseButton_Click(object sender, EventArgs e)
        {
            string query = "SHOW TABLES LIKE 'agent_%';";

            MySqlConnection databaseConnection = new MySqlConnection(connectionStringSeller);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                query = "";
                databaseConnection.Open();
                MySqlDataReader reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    query = "DROP TABLE IF EXISTS";
                    while (reader.Read())
                    {
                        query += " " + reader.GetString(0) + ",";
                    }
                    query = query.Remove(query.Length - 1, 1) + ";";
                }
                databaseConnection.Close();

                if(query.Length > 0)
                {
                    commandDatabase = new MySqlCommand(query, databaseConnection);
                    commandDatabase.CommandTimeout = 60;
                    databaseConnection.Open();
                    reader = commandDatabase.ExecuteReader();
                    databaseConnection.Close();
                    output("Zresetowano bazy danych sprzedawców", Color.Red);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
