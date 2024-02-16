using System.Net;
using TFTPClient;

Client client = new Client();

client.Upload("text3.txt");

Console.Read();