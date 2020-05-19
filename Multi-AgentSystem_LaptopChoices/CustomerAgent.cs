﻿using System;
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
        private Panel resultBox;
        private RichTextBox console;
        private string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_seller;";

        /*private int price;
        private string name;
        private string link;*/

        public CustomerAgent(int id, int maxLaps, Panel instance, RichTextBox writeConsole)
        {
            this.agentID = id;
            this.maxLaps = maxLaps;
            this.resultBox = instance;
            this.console = writeConsole;
        }

        public void RunAgent(ref bool stopAgent, ref List<object> recieve, ref List<object> response, ref bool isBusy)
        {
            output("Klient nr " + agentID + " startuje...", Color.Blue);
            GetDatabase(agentID);
            /*while (true)
            {
                if (stopAgent) return;
            }*/
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

        private void OpenLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void SelectProduct(object sender, EventArgs e)
        {
            //Here select product
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
                        dynamicLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OpenLink);
                        dynamicLinkLabel.Dock = DockStyle.Top;

                        Button dynamicButton = new Button();
                        dynamicButton.Height = 30;
                        dynamicButton.Text = "Wybierz tę ofertę klienta nr " + agentID;
                        dynamicButton.Name = "buttonAgent" + agentID;
                        dynamicButton.Font = new Font("Calibri", 12);
                        dynamicButton.Click += new EventHandler(SelectProduct);
                        dynamicButton.Dock = DockStyle.Top;

                        box.Controls.Add(dynamicButton);
                        box.Controls.Add(dynamicLinkLabel);
                        box.Controls.Add(mylab1);
                        box.Controls.Add(mylab);

                        resultBox.Invoke((MethodInvoker)delegate { resultBox.Controls.Add(box); });
                    }

                    output("Klient nr " + agentID + " wrócił z przedmiotem", Color.Purple);
                    Console.WriteLine(agentID);
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
