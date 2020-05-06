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
        private int customerID;

        /*private int price;
        private string name;
        private string link;*/

        public CustomerAgent(int id)
        {
            this.customerID = id;
        }

        private void openLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        public void getDatabase(TabPage instance, int id)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_seller;";

            string query = "SELECT * FROM items WHERE id=" + id;

            MySqlConnection databaseConnection = new MySqlConnection(connectionString);
            MySqlCommand commandDatabase = new MySqlCommand(query, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            MySqlDataReader reader;

            //string outPut = "";

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
                        box.Text = "Produkt agenta nr " + customerID;
                        box.Name = "groupBoxAgent" + customerID;

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
                        //dynamicLinkLabel.BackColor = Color.Red;
                        dynamicLinkLabel.ForeColor = Color.Black;
                        dynamicLinkLabel.ActiveLinkColor = Color.Black;
                        dynamicLinkLabel.VisitedLinkColor = Color.Black;
                        dynamicLinkLabel.LinkColor = Color.Black;
                        dynamicLinkLabel.DisabledLinkColor = Color.Black;
                        dynamicLinkLabel.Text = "Link do sklepu";
                        dynamicLinkLabel.Name = "linkAgent" + customerID;
                        dynamicLinkLabel.Font = new Font("Calibri", 12);
                        dynamicLinkLabel.Links.Add(0, dynamicLinkLabel.Text.Length, reader.GetString(3));
                        dynamicLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(openLink);
                        dynamicLinkLabel.Dock = DockStyle.Top;

                        box.Controls.Add(dynamicLinkLabel);
                        box.Controls.Add(mylab1);
                        box.Controls.Add(mylab);

                        instance.Controls.Add(box);
                    }
                }
                else
                {
                    Console.WriteLine("Brak wierszy");
                }

                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //return outPut;
        }

        public string getInfo()
        {
            return ("Agent nr " + customerID + " wrócił z przedmiotem");
        }
    }
}
