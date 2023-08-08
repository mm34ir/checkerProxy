using Microsoft.VisualBasic;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CheckerProxy
{
    public partial class Form1 : Form
    {
        List<String> sp = new List<String>();
        public Form1()
        {
            InitializeComponent();
            progressBar1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string tex = Clipboard.GetText();
            if (tex != null)
            {
                Regex regex = new Regex("(?:[0-9]{1,3}\\.){3}[0-9]{1,3}[\\t:][0-9]{2,}");
                MatchCollection matches = regex.Matches(tex);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        string vr = match.Value.Replace("\t",":");
                        sp.Add(vr);
                    } 
                    sp.Distinct();
                    listBox1.Items.Clear();
                    listBox1.Items.AddRange(sp.ToArray());
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var cur = listBox1.SelectedItem;
            // listBox1.Items.Remove(cur);
            listBox1.Items.Clear();
            sp.Remove(cur.ToString());
            listBox1.Items.AddRange(sp.ToArray());
        }

        private async Task<bool> CheckProxy(string proxyAddress)
        {
            WebProxy proxy = new WebProxy(proxyAddress);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.google.com");
            request.Proxy = proxy;
            request.Timeout = 2000;
            request.ReadWriteTimeout = 2000;

            try
            {
                var resp = await request.GetResponseAsync();
                HttpWebResponse response = (HttpWebResponse)resp;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            progressBar1.Maximum = sp.Count - 1;
            progressBar1.Visible = true;

            for (int i = 0; i < sp.Count; i++)
            {
                if (await CheckProxy(sp[i]))
                {
                    listBox1.Items.Add(sp[i]);
                }
                progressBar1.Value = i;
            }
            sp.Clear();
            sp = listBox1.Items.Cast<String>().ToList();
            progressBar1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string s = "";
            foreach (string el in listBox1.Items)
            {
                s += el + '\n';
            }
            File.WriteAllText("proxy.txt", s);
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            Clipboard.SetText(listBox1.SelectedItem.ToString());
        }
    }
}