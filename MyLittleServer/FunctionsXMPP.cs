using Matrix;
using Matrix.Xmpp.Client;
using Matrix.Xmpp.Sasl;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MyLittleServer
{
    public partial class Form1
    {
        private void xmppClient_OnLogin(object sender, Matrix.EventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Соединение с сервером XMPP установлено!");
        }

        private void xmppClient_OnAuthError(object sender, SaslEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Соединение с сервером XMPP не удалось!");
        }

        private void xmppClient_OnClose(object sender, Matrix.EventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Соединение с сервером XMPP разорвано!");
        }

        private void fm_OnFile(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Получен запрос на получение файла!");

            Jid fromJid = new Jid(xmppClient.Username,
                                  xmppClient.XmppDomain,
                                  "client");

            if (e.Jid.ToString().ToLower() == fromJid.ToString().ToLower())
            {
                workMode = e.Description;
                e.Directory = Path.GetDirectoryName(Application.ExecutablePath);
                e.Accept = true;
            }
        }

        private void fm_OnDeny(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Прием файла отклонен!");
        }

        private void fm_OnStart(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Прием файла начат!");
        }

        private void fm_OnProgress(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Ход приема файла: " + e.BytesTransmitted + "/" + e.FileSize + " байт");
        }

        private void fm_OnAbort(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Прием файла прерван!");
        }

        private void fm_OnError(object sender, ExceptionEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Ошибка приема файла!");
        }

        private void fm_OnEnd(object sender, FileTransferEventArgs e)
        {
            logTextBox_textChange(DateTime.Now.ToString("HH:mm:ss") + " Прием файла завершен!");

            using (Bitmap img = (Bitmap)Image.FromFile(e.Filename))
            {
                DecodeBarcode(img);
            }
        }
    }
}
