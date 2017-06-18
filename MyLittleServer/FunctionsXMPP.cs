using Sharp.Xmpp;
using Sharp.Xmpp.Extensions;

namespace MyLittleServer
{
    public partial class Form1
    {
        public void OnFileTransferProgress(object sender, FileTransferProgressEventArgs e)
        {
            stringForLogTextBox = "Receiving " + e.Transfer.Name + "..." + e.Transfer.Transferred + "/" + e.Transfer.Size + " Bytes";

            if (e.Transfer.Transferred == e.Transfer.Size)
            {
                fileName = e.Transfer.Name;
                fileSize = e.Transfer.Size;
                readyToWait = true;
            }
        }

        public string OnFileTransferRequest(FileTransfer transfer)
        {
            Jid fromJid = new Jid(hostname, username, "client");
            if (transfer.From == fromJid)
            {
                workMode = transfer.Description;

                return transfer.Name;
            }
            else
            {
                return null;
            }
        }
    }
}
