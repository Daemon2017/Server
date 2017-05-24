using Sharp.Xmpp.Extensions;
using System.Drawing;
using System.IO;

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
            if (transfer.From == username + "@" + hostname + "/client")
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
