using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tftp.Net;


namespace TFTPServer
{
    public class Server
    {
        private TftpServer _tftpServer;
        private static string _serverDirectory;


        public Server()
        {
            _serverDirectory = "C:\\tftp";
            _tftpServer = new TftpServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 69));
            _tftpServer.OnReadRequest += new TftpServerEventHandler(server_OnReadRequest);
            _tftpServer.OnWriteRequest += new TftpServerEventHandler(server_OnWriteRequest);
            _tftpServer.Start();


            Console.WriteLine($"Сервер TFTP запущен для директории: {_serverDirectory}");

        }

        static void server_OnReadRequest(ITftpTransfer transfer, EndPoint client)
        {
            try
            {
                String path = Path.Combine(_serverDirectory, transfer.Filename);
                FileInfo file = new FileInfo(path);
                OutputTransferStatus(transfer, "Accepting request from " + client);
                StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));

                if (!file.FullName.StartsWith(_serverDirectory, StringComparison.InvariantCultureIgnoreCase))
                {
                    CancelTransfer(transfer, TftpErrorPacket.AccessViolation);
                }
                else if (!file.Exists)
                {
                    CancelTransfer(transfer, TftpErrorPacket.FileNotFound);
                }
                else
                {
                    OutputTransferStatus(transfer, "Accepting request from " + client);
                    StartTransfer(transfer, new FileStream(file.FullName, FileMode.Open, FileAccess.Read));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void StartTransfer(ITftpTransfer transfer, Stream stream)
        {
            transfer.OnProgress += new TftpProgressHandler(transfer_OnProgress);
            transfer.OnError += new TftpErrorHandler(transfer_OnError);
            transfer.OnFinished += new TftpEventHandler(transfer_OnFinished);
            stream.Position = 0;
            transfer.Start(stream);


        }
        static void server_OnWriteRequest(ITftpTransfer transfer, EndPoint client)
        {
            try
            {
                String file = Path.Combine(_serverDirectory, transfer.Filename);
                OutputTransferStatus(transfer, "Accepting write request from " + client);
                StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
                if (File.Exists(file))
                {
                    CancelTransfer(transfer, TftpErrorPacket.FileAlreadyExists);
                }
                else
                {
                    OutputTransferStatus(transfer, "Accepting write request from " + client);
                    StartTransfer(transfer, new FileStream(file, FileMode.CreateNew));
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void CancelTransfer(ITftpTransfer transfer, TftpErrorPacket reason)
        {
            OutputTransferStatus(transfer, "Cancelling transfer: " + reason.ErrorMessage);
            transfer.Cancel(reason);
        }

        static void transfer_OnError(ITftpTransfer transfer, TftpTransferError error)
        {
            OutputTransferStatus(transfer, "Error: " + error);
        }

        static void transfer_OnFinished(ITftpTransfer transfer)
        {
            OutputTransferStatus(transfer, "Finished");
        }

        static void transfer_OnProgress(ITftpTransfer transfer, TftpTransferProgress progress)
        {
            OutputTransferStatus(transfer, "Progress " + progress);
        }

        private static void OutputTransferStatus(ITftpTransfer transfer, string message)
        {
            Console.WriteLine("[" + transfer.Filename + "] " + message);
        }
    }
}

