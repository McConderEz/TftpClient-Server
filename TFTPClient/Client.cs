using GSF.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tftp.Net;

namespace TFTPClient
{
    public class Client
    {
        private TftpClient _tftpClient;
        private ITftpTransfer _transfer;
        private static AutoResetEvent TransferFinishedEvent = new AutoResetEvent(false);

        public Client()
        {
            _tftpClient = new TftpClient(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 69));
        }

        public void Download(string filename, string targetDir)
        {
            try
            {
                _transfer = _tftpClient.Download(filename);
                MemoryStream memoryStream = new MemoryStream();
                _transfer.OnFinished += (transfer) =>
                {

                    Console.WriteLine("Download completed.");

                    var path = filename.Split(new char[] { '/', '\\'} );

                    using (FileStream fileStream = new FileStream(targetDir + path[path.Length - 1], FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        memoryStream.CopyTo(fileStream);
                    }
                };
                _transfer.Start(memoryStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Upload(string filename)
        {
            try
            {
                _transfer = _tftpClient.Upload(filename);
                MemoryStream memoryStream = new MemoryStream();
                using (FileStream fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    fileStream.CopyTo(memoryStream);
                }
                _transfer.OnFinished += (transfer) =>
                {
                    Console.WriteLine("Upload completed.");
                };
                _transfer.Start(memoryStream);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }

}


