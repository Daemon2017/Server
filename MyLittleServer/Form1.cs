using System;
using System.Windows.Forms;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Sasl;
using System.IO;

namespace MyLittleServer
{
    public partial class Form1 : Form
    {
        static dbConnector dataBaseConn;

        static string workMode;

        XmppClient xmppClient = new XmppClient();
        FileTransferManager ftm = new FileTransferManager();

        public Form1()
        {
            InitializeComponent();

            string lic = @"";
            Matrix.License.LicenseManager.SetLicense(lic);

            string[] xmppConfig = new string[3];
            xmppConfig = File.ReadAllLines("XMPP.cfg");
            xmppClient.SetXmppDomain(xmppConfig[0]);
            xmppClient.SetUsername(xmppConfig[1]);
            xmppClient.Password = xmppConfig[2];
            xmppClient.Resource = "server";
            xmppClient.Port = 5222;
            xmppClient.StartTls = true;
            xmppClient.OnLogin += new EventHandler<Matrix.EventArgs>(xmppClient_OnLogin);
            xmppClient.OnAuthError += new EventHandler<SaslEventArgs>(xmppClient_OnAuthError);
            xmppClient.OnClose += new EventHandler<Matrix.EventArgs>(xmppClient_OnClose);

            ftm.XmppClient = xmppClient;
            ftm.Blocking = true;
            ftm.OnDeny += fm_OnDeny;
            ftm.OnAbort += fm_OnAbort;
            ftm.OnError += fm_OnError;
            ftm.OnEnd += fm_OnEnd;
            ftm.OnStart += fm_OnStart;
            ftm.OnProgress += fm_OnProgress;
            ftm.OnFile += fm_OnFile;          
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            xmppClient.Close();
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            try
            {
                dataBaseConn = new dbConnector();
            }
            catch (Exception)
            {
                MessageBox.Show("Не найдена база данных! Проверьте запущен ли сервер БД",
                               "Нет подключения к БД",
                               MessageBoxButtons.OK);
            }

            xmppClient.Open();
        }

        private void logTextBox_textChange(string text)
        {
            log_textBox.Invoke((MethodInvoker)delegate
            {
                log_textBox.Text += text + "\r\n";
            });
        }

        private void timer1_Tick(object sender, System.EventArgs e)
        {
            dataGridView1.Invoke((MethodInvoker)delegate
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = dataBaseConn.GetComments("SELECT * FROM things ORDER BY `id` ASC");
            });
        }
    }
}
