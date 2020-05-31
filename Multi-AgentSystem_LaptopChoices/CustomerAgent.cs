using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Multi_AgentSystem_LaptopChoices
{
    class CustomerAgent
    {
        private int agentID;
        private int maxLaps;
        private RichTextBox console;
        private string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_customer;";

        private int price;
        private string name;
        private string link;

        public CustomerAgent(int id, int maxLaps, RichTextBox writeConsole)
        {
            this.agentID = id;
            this.maxLaps = maxLaps;
            this.console = writeConsole;

            string query = "CREATE TABLE IF NOT EXISTS agent_" + agentID + "_KnowledgeTable (" +
                              "id INTEGER PRIMARY KEY AUTO_INCREMENT," +

                              "how_many_data INTEGER NOT NULL," +
                              "preferred_laptop INTEGER NOT NULL," +
                              "size_of_laptop INTEGER NOT NULL," +
                              "laptop_usage INTEGER NOT NULL," +
                              "laptop_battery_usage INTEGER NOT NULL," +
                              "laptop_durability INTEGER NOT NULL," +
                              "night_usage INTEGER NOT NULL," +
                              "cd_player INTEGER NOT NULL," +

                              "product_name varchar(120) NOT NULL," +
                              "price INTEGER NOT NULL," +
                              "is_sold enum('0','1') NOT NULL" +
                            ") ENGINE = InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader reader = commandDatabase.ExecuteReader();
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RunAgent(ref bool stopAgent, ref List<AgentTask> sellerAgentsTab, ref List<string[]> customersProducts, ref int[] parameters, ref int[] priority, ref int maxPrice, ref int minPrice)
        {
            output("Klient nr " + agentID + ": Startuje...", Color.Blue);

            bool haveItem = false;
            for (int i = 0; i < maxLaps; i++)
            {
                if (stopAgent) return;

                foreach (AgentTask seller in sellerAgentsTab)
                {
                    if (stopAgent) return;

                    if (!seller.IsBusy(ref parameters))
                    {
                        output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " nie jest zajęty, przekazuję parametry i oczekuję propozycji sprzedawcy...", Color.OrangeRed);

                        bool endOfConversation = false;
                        do
                        {
                            if (stopAgent) return;

                            /* WAIT FOR RESPONSE THAT SELLER HAVE PRODUCT */
                            while (seller.response.Count == 0)
                            {
                                if (stopAgent) return;
                            }

                            while (seller.response[0] == null) { }
                            string[] res = (string[])seller.response[0];
                            if (res[0] != "-1")
                            {
                                /* SELLER HAVE PRODUCT - CHECK DO THE PRODUCT MEET THE REQUIREMENTS */
                                bool goodPriority = false;
                                if (int.Parse(res[4]) >= priority[0] && int.Parse(res[5]) >= priority[1] && int.Parse(res[6]) >= priority[2] && int.Parse(res[7]) >= priority[3] &&
                                   int.Parse(res[8]) >= priority[4] && int.Parse(res[9]) >= priority[5] && int.Parse(res[10]) >= priority[6] && int.Parse(res[11]) >= priority[7]) goodPriority = true;

                                if(goodPriority)
                                {
                                    seller.response.Clear();
                                    seller.recieve.Add(true);
                                    
                                    while (seller.response.Count == 0) //Wait for response that seller recieved customer response
                                    {
                                        if (stopAgent) return;
                                    }
                                    
                                    /* PRODUCT MEET THE REQUIREMENTS - NEGOTIATE THE PRICE */
                                    price = Int32.Parse(res[2]);
                                    output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " znalazł pasujący przedmiot w cenie " + price + " zł, rozpoczynam negocjacje od najniższej (" + minPrice + " zł)", Color.OrangeRed);

                                    price = minPrice;
                                    bool priceAccepted = false;
                                    do
                                    {
                                        seller.response.Clear();
                                        seller.recieve.Add(price);
                                        while (seller.response.Count == 0) //Wait for response did price is accepted
                                        {
                                            if (stopAgent) return;
                                        }

                                        if (price != 0)
                                        {
                                            while (seller.response[0] == null) { }
                                            priceAccepted = (bool)seller.response[0];

                                            if (!priceAccepted)
                                            {
                                                ++price;
                                                if (price > maxPrice)
                                                {
                                                    price = 0;
                                                    output("Klient nr " + agentID + ": Przerywam negocjacje, ponieważ cena była by większa od maksymalnej", Color.OrangeRed);
                                                }
                                                //else output("Klient nr " + agentID + ": Poprzednia propozycja została odrzucona, więc oferuję sprzedawcy cenę " + price + " zł", Color.OrangeRed);
                                            }
                                        }
                                        else
                                        {
                                            price = -1;
                                            priceAccepted = false;
                                        }
                                    } while (!priceAccepted && price != -1);

                                    seller.response.Clear();
                                    if (priceAccepted)//if (price > minPrice && price < maxPrice)
                                    {
                                        output("Klient nr " + agentID + ": Przedmiot został wybrany i wynegocjowany na " + price + " zł", Color.OrangeRed);
                                        name = res[0];
                                        link = res[3];

                                        //Save product in customer knowledge database if not exist
                                        string query = "SELECT * FROM agent_" + agentID + "_KnowledgeTable" +
                                            " WHERE `how_many_data`='" + parameters[0] + "' AND `preferred_laptop`='" + parameters[1] + "' AND `size_of_laptop`='" + parameters[2] + "' AND `laptop_usage`='" + parameters[3] + "' AND `laptop_battery_usage`='" + parameters[4] + "' AND `laptop_durability`='" + parameters[5] + "' AND `night_usage`='" + parameters[6] + "' AND `cd_player`='" + parameters[7] + "' AND `product_name`='" + res[0] + "' AND `price`='" + price + "'";
                                        Console.WriteLine(query);

                                        MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                                        MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
                                        commandDatabase.CommandTimeout = 60;
                                        MySqlDataReader reader;

                                        try
                                        {
                                            databaseConnection.Open();
                                            reader = commandDatabase.ExecuteReader();
                                            if (!reader.HasRows)
                                            {
                                                query = "INSERT INTO agent_" + agentID + "_KnowledgeTable(`how_many_data`, `preferred_laptop`, `size_of_laptop`, `laptop_usage`, `laptop_battery_usage`, `laptop_durability`, `night_usage`, `cd_player`, `product_name`, `price`, `is_sold`) VALUES" +
                                                       "('" + parameters[0] + "', '" + parameters[1] + "', '" + parameters[2] + "', '" + parameters[3] + "', '" + parameters[4] + "', '" + parameters[5] + "', '" + parameters[6] + "', '" + parameters[7] + "', '" + res[0] + "', '" + price + "', '0');";
                                                databaseConnection = new MySqlConnection(connectionString);
                                                commandDatabase = new MySqlCommand(query, databaseConnection);
                                                commandDatabase.CommandTimeout = 60;
                                                databaseConnection.Open();
                                                reader = commandDatabase.ExecuteReader();
                                                databaseConnection.Close();
                                            }
                                            databaseConnection.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }

                                        haveItem = true;
                                        endOfConversation = true;
                                        //seller.recieve.Add(true);
                                    }
                                    else
                                    {
                                        output("Klient nr " + agentID + ": Negocjacje ceny się nie powiodły, czekam na inne propozycje", Color.OrangeRed);
                                        seller.recieve.Add(false);
                                    }
                                }
                                else
                                {
                                    output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " znalazł pasujący przedmiot, ale nie spełniał on wymagań priorytetów, więc czekam na inne propozycje", Color.OrangeRed);
                                    seller.recieve.Add(false);
                                    seller.response.Clear();
                                }
                            }
                            else
                            {
                                output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " nie spełnił wymagań, opuszczam", Color.OrangeRed);
                                seller.response.Clear();
                                endOfConversation = true;
                            }
                        } while (!endOfConversation);

                        if (haveItem) break;
                    }
                    else output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " jest zajęty", Color.OrangeRed);
                }
                if (haveItem) break;
            }

            if (haveItem)
            {
                output("Klient nr " + agentID + ": Wróciłem z przedmiotem", Color.Purple);
                customersProducts.Add(new string[] { agentID.ToString(), price.ToString(), name, link });
            }
            else
            {
                output("Klient nr " + agentID + ": Nie znalazłem przedmiotu", Color.Purple);
            }
        }

        private void output(string text, Color? c = null)
        {
            console.Invoke((MethodInvoker)delegate
            {
                console.SelectionStart = console.TextLength;
                console.SelectionLength = 0;
                console.SelectionColor = Color.DarkGray;
                console.AppendText("[" + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] ");
                console.SelectionColor = c ?? console.ForeColor;
                console.AppendText(text + Environment.NewLine);
                console.SelectionStart = console.Text.Length;
                console.ScrollToCaret();
            });
        }
    }
}
