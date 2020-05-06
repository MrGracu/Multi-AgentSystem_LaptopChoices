using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Multi_AgentSystem_LaptopChoices
{
    public partial class Form1 : Form
    {
        bool programRunning = false;
        Random random = new Random();

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

        private void openLink(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Link.LinkData.ToString());
        }

        private void getDatabase(int agentID,int id)
        {
            string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_seller;";

            string query = "SELECT * FROM items WHERE id="+id;

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

                // All succesfully executed, now do something

                // IMPORTANT : 
                // If your query returns result, use the following processor :

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        // As our database, the array will contain : ID 0, FIRST_NAME 1,LAST_NAME 2, ADDRESS 3
                        // Do something with every received database ROW
                        //string[] row = { reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3) };

                        //outPut = (reader.GetString(1) + " " + reader.GetString(2) + " " + reader.GetString(3));


                        // Setting the location of the GroupBox
                        GroupBox box = new GroupBox();
                        //box.Location = new Point(179, 145);

                        //box.Anchor = AnchorStyles.Top;
                        box.Dock = DockStyle.Top;

                        // Setting the size of the GroupBox
                        //box.Size = new Size(329, 94);

                        // Setting text the GroupBox
                        box.Text = "Produkt agenta nr " + agentID;

                        // Setting the name of the GroupBox
                        box.Name = "groupBoxAgent"+ agentID;

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
                        dynamicLinkLabel.Name = "linkAgent"+agentID;
                        dynamicLinkLabel.Font = new Font("Calibri", 12);
                        dynamicLinkLabel.Links.Add(0,dynamicLinkLabel.Text.Length,reader.GetString(3));
                        dynamicLinkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(openLink);
                        dynamicLinkLabel.Dock = DockStyle.Top;

                        box.Controls.Add(dynamicLinkLabel);
                        box.Controls.Add(mylab1);
                        box.Controls.Add(mylab);

                        tabPage4.Controls.Add(box);
                    }
                }
                else
                {
                    Console.WriteLine("No rows found.");
                }

                // Finally close the connection
                databaseConnection.Close();
            }
            catch (Exception ex)
            {
                // Show any error message.
                MessageBox.Show(ex.Message);
            }

            //return outPut;
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
                    getDatabase(i,random.Next(1, 19));
                    output(("Agent nr " + i + " wrócił z przedmiotem"), Color.Blue);
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
