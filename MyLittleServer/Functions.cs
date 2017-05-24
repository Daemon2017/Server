using BarcodeLib.BarcodeReader;
using HtmlAgilityPack;
using System;
using System.Drawing;
using System.Text;

namespace MyLittleServer
{
    public partial class Form1
    {
        private void DecodeBarcode(Bitmap img)
        {
            string[] results = BarcodeReader.read(img, BarcodeReader.EAN13);

            if (results != null)
            {
                var sb = new StringBuilder(results[0]);
                string changer = "4";
                var temp = changer.ToCharArray(0, 1);
                sb[0] = temp[0];
                results[0] = sb.ToString();

                ConnectToWebSite(results[0]);
            }
        }

        private void ConnectToWebSite(string message)
        {
            // Проверяем нет ли вещи с таким штрихом-кодом в базе            
            if (dataBaseConn.checkBarcodeExisting(Convert.ToInt64(message)))
            {
                dataBaseConn.updateData(workMode, Convert.ToInt64(message));
                stringForLogTextBox = "Barcode есть, изменил количество";
            }
            else if (!dataBaseConn.checkBarcodeExisting(Convert.ToInt64(message)))
            {
                stringForLogTextBox = "Barcode нет, парсю имя";

                string responseData = "";

                HtmlWeb hw = new HtmlWeb();
                HtmlDocument doc = hw.Load(@"https://ean13.org/?query=" + message + "&page=search");
                var nodes = doc.DocumentNode.SelectNodes("//div[@class='collist-2']");

                if (nodes != null)
                {
                    foreach (HtmlNode node in nodes)
                    {
                        responseData = node.InnerText;
                    }

                    responseData = responseData.Remove(0, 13);
                    responseData = responseData.Replace('"', ' ');
                    responseData = responseData.Replace('\'', ' ');
                    responseData = responseData.Replace('{', ' ');
                    responseData = responseData.Replace('}', ' ');
                }

                // Если в базе сайта нет названия - даем предмету характерное название
                if (responseData == "")
                {
                    Namer f = new Namer();
                    f.ShowDialog();

                    responseData = f.newName;
                }

                if (responseData != null)
                {
                    // Проверяем нет ли вещи с таким именем в базе
                    if (workMode == "INC")
                    {
                        if (!dataBaseConn.checkNameExisting(responseData))
                        {
                            stringForLogTextBox = "Имени нет, вставляю " + responseData;
                            dataBaseConn.insertData(responseData, 1, Convert.ToInt64(message));
                        }
                    }
                }
            }
        }
    }
}
