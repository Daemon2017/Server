using System;
using System.Windows.Forms;
using Sharp.Xmpp.Client;
using System.IO;
using System.Drawing;

namespace MyLittleServer
{
    public partial class Form1 : Form
    {
        string[] namesArray = new string[0];
        int[] countArray = new int[0];
        long[] idArray = new long[0];

        string fileName;
        long fileSize;
        bool readyToWait = false;

        static string hostname = "jabber.ru";
        static string username = "";
        static string password = "";

        static string stringForLogTextBox;

        static dbConnector dataBaseConn;

        XmppClient clientXMPP = new XmppClient(hostname, username, password, 5222, true);

        static string workMode;

        public Form1()
        {
            InitializeComponent();

            clientXMPP.FileTransferRequest = OnFileTransferRequest;
            clientXMPP.FileTransferProgress += OnFileTransferProgress;
            clientXMPP.FileTransferSettings.ForceInBandBytestreams = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (clientXMPP.Connected == true)
            {
                clientXMPP.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
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

            try
            {
                clientXMPP.Connect("server");
            }
            catch
            {
                MessageBox.Show("Не удалось соединиться с сервером, проверьте логин/пароль!",
                                "Ошибка",
                                MessageBoxButtons.OK);
            }
        }

        private void logTextBox_textChange(string text)
        {
            log_textBox.Invoke((MethodInvoker)delegate
            {
                log_textBox.Text = text;
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (readyToWait == true)
            {
                bool success = false;

                while (success != true)
                {
                    FileInfo file = new FileInfo(fileName);

                    if (file.Length == fileSize)
                    {
                        success = true;

                        using (Bitmap img = (Bitmap)Image.FromFile(fileName))
                        {
                            DecodeBarcode(img);
                        }

                        readyToWait = false;
                    }
                }
            }

            logTextBox_textChange(stringForLogTextBox);

            dataGridView1.Invoke((MethodInvoker)delegate
            {
                dataGridView1.DataSource = null;
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = dataBaseConn.GetComments("SELECT * FROM things ORDER BY `id` ASC");
            });
        }
    }
}
