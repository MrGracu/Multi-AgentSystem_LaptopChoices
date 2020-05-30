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
        private Panel resultBox;
        private RichTextBox console;
        private string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=agents_customer;";

        private int price;
        private string name;
        private string link;

        public CustomerAgent(int id, int maxLaps, Panel instance, RichTextBox writeConsole)
        {
            this.agentID = id;
            this.maxLaps = maxLaps;
            this.resultBox = instance;
            this.console = writeConsole;
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

                            while (seller.response.Count == 0)
                            {
                                if (stopAgent) return;
                            }

                            string[] res = (string[])seller.response[0];
                            if (res[0] != "-1")
                            {
                                price = Int32.Parse(res[2]);
                                output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " znalazł pasujący przedmiot w cenie " + price + " zł", Color.OrangeRed);

                                if (price > minPrice && price < maxPrice)
                                {
                                    output("Klient nr " + agentID + ": Przedmiot został wybrany", Color.OrangeRed);
                                    name = res[0];
                                    link = res[3];
                                    haveItem = true;
                                    endOfConversation = true;
                                    seller.recieve.Add(true);
                                }
                                else
                                {
                                    output("Klient nr " + agentID + ": Przedmiot nie mieścił sie w cenie, czekam na inne propozycje", Color.OrangeRed);
                                    seller.recieve.Add(false);
                                }
                                seller.response.Clear();
                            }
                            else
                            {
                                output("Klient nr " + agentID + ": Sprzedawca nr " + seller.id + " nie spełnił wymagań, opuszczam", Color.OrangeRed);
                                endOfConversation = true;
                                seller.response.Clear();
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
