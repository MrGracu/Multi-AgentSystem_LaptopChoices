using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_AgentSystem_LaptopChoices
{
    class SellerAgent
    {
        private int agentID;
        private RichTextBox console;
        private string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_seller;";

        public SellerAgent(int id, RichTextBox writeConsole, ref Random rnd)
        {
            this.agentID = id;
            this.console = writeConsole;

            string query = "CREATE TABLE IF NOT EXISTS agent_" + agentID + "_ProductsTable (" +
                              "id INTEGER PRIMARY KEY AUTO_INCREMENT," +
                              "id_items INTEGER NOT NULL," +
                              "amount INTEGER NOT NULL DEFAULT 0," +
                              "price INTEGER NOT NULL," +
                              "negotiation_percentage INTEGER NOT NULL," +
                              "FOREIGN KEY(id_items) REFERENCES items(id)" +
                            ") ENGINE = InnoDB DEFAULT CHARACTER SET utf8 COLLATE utf8_polish_ci;";

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;

            try
            {
                databaseConnection.Open();
                MySqlDataReader reader = commandDatabase.ExecuteReader();
                databaseConnection.Close();

                query = "SELECT * FROM agent_" + agentID + "_ProductsTable";
                commandDatabase = new MySqlCommand(query, databaseConnection);
                commandDatabase.CommandTimeout = 60;
                databaseConnection.Open();
                reader = commandDatabase.ExecuteReader();
                bool hasRows = true;
                if (!reader.HasRows)
                {
                    hasRows = false;
                }
                databaseConnection.Close();

                if (!hasRows)
                {
                    query = "SELECT * FROM items";
                    commandDatabase = new MySqlCommand(query, databaseConnection);
                    commandDatabase.CommandTimeout = 60;
                    databaseConnection.Open();
                    reader = commandDatabase.ExecuteReader();
                    int amount = 0;
                    List<string[]> rows = new List<string[]>();
                    while (reader.Read())
                    {
                        string[] row = { reader.GetString(0), reader.GetString(2) };
                        rows.Add(row);
                        ++amount;
                    }
                    databaseConnection.Close();

                    for(int i = 0; i < rows.Count; ++i)
                    {
                        int j = rnd.Next(rows.Count);
                        string[] temp = rows[j];
                        rows[j] = rows[i];
                        rows[i] = temp;
                    }

                    query = "INSERT INTO agent_" + agentID + "_ProductsTable(`id_items`, `amount`, `price`, `negotiation_percentage`) VALUES";
                    for(int i = 0; i < rows.Count; ++i)
                    {
                        int wynik = Convert.ToInt32(Math.Round((double.Parse(rows[i][1]) * ((90 + rnd.Next(21)) / 100.0)), MidpointRounding.AwayFromZero));
                        query += " (" + rows[i][0] + ", " + ((rows.Count * 0.75) > i ? rnd.Next(1, 9) : 0) + ", " + wynik + ", " + rnd.Next(21) + ")";
                        if ((i + 1) < rows.Count) query += ",";
                        else query += ";";
                    }
                    commandDatabase = new MySqlCommand(query, databaseConnection);
                    commandDatabase.CommandTimeout = 60;
                    databaseConnection.Open();
                    MySqlDataReader myReader = commandDatabase.ExecuteReader();
                    databaseConnection.Close();
                    output("Agent nr " + agentID + ": Utworzono bazę danych z produktami", Color.DarkGray);
                    rows.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void RunAgent(ref bool stopAgent, ref List<object> recieve, ref List<object> response, ref bool isBusy)
        {
            output("Sprzadawca nr " + agentID + " startuje...", Color.Blue);
            while (true)
            {
                if (stopAgent) return;
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

        public void GetDatabase(int id)
        {
            string query = "SELECT * FROM items WHERE id=" + id;

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            try
            {
                // Open the database
                databaseConnection.Open();

                // Execute the query
                reader = commandDatabase.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        //string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };

                        GroupBox box = new GroupBox();
                        box.Dock = DockStyle.Top;
                        box.Text = "Produkt agenta nr " + agentID;
                        box.Name = "groupBoxAgent" + agentID;
                        box.Height = 120;

                        Label mylab = new Label();
                        mylab.Text = ("Model: " + reader.GetString(1));
                        mylab.Dock = DockStyle.Top;
                        mylab.Font = new Font("Calibri", 12);
                        mylab.ForeColor = Color.Green;

                        Label mylab1 = new Label();
                        mylab1.Text = ("Cena: " + reader.GetString(2) + " zł");
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
                        dynamicLinkLabel.Name = "linkAgent" + agentID;
                        dynamicLinkLabel.Font = new Font("Calibri", 12);
                        dynamicLinkLabel.Links.Add(0, dynamicLinkLabel.Text.Length, reader.GetString(3));
                    }
                }
                else
                {
                    Console.WriteLine("No rows");
                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
