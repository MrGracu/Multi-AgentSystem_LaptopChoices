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
                    output("Sprzedawca nr " + agentID + ": Utworzono bazę danych z produktami", Color.DarkGray);
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
            output("Sprzadawca nr " + agentID + ": Startuje...", Color.Blue);
            while (true)
            {
                if (stopAgent) return;

                if (recieve.Count > 0)
                {
                    output("Sprzedawca nr " + agentID + ": Otrzymałem parametry i rozpoczynam rozmowę z klientem", Color.Blue);
                    int[] parameters = (int[])recieve[0];
                    int price = 0;

                    string specificationQuery = "SELECT itemsspecification.id_preferencesOptions, itemsspecification.priority FROM itemsspecification WHERE (";

                    string query = "SELECT itemsspecification.id_items, name, amount, agent_" + agentID + "_productstable.price, link, agent_" + agentID + "_productstable.negotiation_percentage, COUNT(itemsspecification.id_items) AS `colsAmout`, SUM(itemsspecification.priority) AS `points` FROM itemsSpecification " +
                                    "INNER JOIN items ON itemsspecification.id_items = items.id " +
                                    "INNER JOIN agent_" + agentID + "_productstable ON items.id = agent_" + agentID + "_productstable.id_items " +
                                    "WHERE (";

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        query += " itemsspecification.id_preferencesOptions = " + parameters[i];
                        specificationQuery += " itemsspecification.id_preferencesOptions = " + parameters[i];
                        if((i + 1) < parameters.Length)
                        {
                            query += " OR";
                            specificationQuery += " OR";
                        }
                        else
                        {
                            query += ") AND itemsspecification.priority >= '1' AND amount > 0 GROUP BY itemsspecification.id_items HAVING colsAmout = " + parameters.Length + " ORDER BY amount DESC"; // " ORDER BY points DESC";
                            specificationQuery += ") ";
                        }
                    }

                    Console.WriteLine(query);
                    response.Clear();

                    List<string[]> foundProducts = new List<string[]>();

                    MySqlConnection databaseConnection = new MySqlConnection(connectionString);
                    MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
                    commandDatabase.CommandTimeout = 60;
                    databaseConnection.Open();
                    MySqlDataReader reader = commandDatabase.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            foundProducts.Add(new string[] { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5) });
                        }
                    }
                    databaseConnection.Close();

                    if (foundProducts.Count > 0)
                    {
                        int selectedID = 0;
                        bool consumerSelected = false;
                        output("Sprzedawca nr " + agentID + ": Znalazłem produkty pasujące (" + foundProducts.Count + "), przekazuję jeden klientowi...", Color.Blue);
                        for (int i = 0; i < foundProducts.Count; i++)
                        {
                            /* SEND PRODUCT AND HIS SPECIFICATION */
                            Console.WriteLine(specificationQuery + "AND itemsspecification.id_items = " + foundProducts[i][0]);

                            string[] specification = new string[parameters.Length];
                            int pos = 0;

                            commandDatabase = new MySqlCommand((specificationQuery + "AND itemsspecification.id_items = " + foundProducts[i][0]), databaseConnection);
                            commandDatabase.CommandTimeout = 60;
                            databaseConnection.Open();
                            reader = commandDatabase.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    specification[pos] = reader.GetString(1);
                                    ++pos;
                                }
                            }
                            databaseConnection.Close();

                            string[] responseSpecification = new string[4 + specification.Length];
                            responseSpecification[0] = foundProducts[i][1];
                            responseSpecification[1] = foundProducts[i][2];
                            responseSpecification[2] = foundProducts[i][3];
                            responseSpecification[3] = foundProducts[i][4];
                            specification.CopyTo(responseSpecification, 4);

                            recieve.Clear();
                            response.Add(responseSpecification);

                            /* RECEIVE DECISION THAT THE PRODUCT MEETS THE REQUIREMENTS  */
                            while (recieve.Count == 0)
                            {
                                if (stopAgent) return;
                            }

                            while (recieve[0] == null) { }
                            consumerSelected = (bool)recieve[0];
                            selectedID = i;

                            if (consumerSelected)
                            {
                                recieve.Clear();
                                response.Add(true);

                                /* IF SELECTED THEN WAIT FOR PRICE SUGGESTIONS */
                                int minimumPrice = Convert.ToInt32(Math.Round((double.Parse(foundProducts[i][3]) * ((100 - int.Parse(foundProducts[i][5])) / 100.0)), MidpointRounding.AwayFromZero));
                                do
                                {
                                    while (recieve.Count == 0)
                                    {
                                        if (stopAgent) return;
                                        Task.Delay(500);
                                    }
                                    while (recieve[0] == null) { }
                                    price = (int)recieve[0];

                                    if (price == 0) consumerSelected = false;
                                    else
                                    {
                                        if (price < minimumPrice)
                                        {
                                            recieve.Clear();
                                            response.Add(false);
                                        }
                                        else
                                        {
                                            recieve.Clear();
                                            response.Add(true);
                                            break;
                                        }
                                    }
                                } while (price != 0);

                                if(price > 0) break;
                            }
                            else output("Sprzedawca nr " + agentID + ": Klient odrzucił przedmiot, sprawdzam czy mam inny, aby przekazać...", Color.Blue);
                        }

                        if(consumerSelected)
                        {
                            output("Sprzedawca nr " + agentID + ": Klient wybrał przedmiot, sprzedaję...", Color.Blue);
                            //Change in database amount of items
                            /*query = "UPDATE agent_" + agentID + "_productstable SET amount=" + (int.Parse(foundProducts[selectedID][2]) - 1) + " WHERE id_items = " + foundProducts[selectedID][0];
                            Console.WriteLine(query);
                            commandDatabase = new MySqlCommand(query, databaseConnection);
                            commandDatabase.CommandTimeout = 60;
                            databaseConnection.Open();
                            reader = commandDatabase.ExecuteReader();
                            databaseConnection.Close();*/

                            output("Sprzedawca nr " + agentID + ": Zaktualizowano ilość produktów", Color.Blue);
                        }
                        else
                        {
                            output("Sprzedawca nr " + agentID + ": Klient NIE ZNALAZŁ odpowiedniego przedmiotu", Color.Blue);
                            response.Add(new string[] { "-1" });
                        }
                        recieve.Clear();
                    }
                    else
                    {
                        recieve.Clear();
                        response.Add(new string[] { "-1" });
                    }
                    isBusy = false;
                }
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
