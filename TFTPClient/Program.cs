using System.Net;
using TFTPClient;

Client client = new Client();

//client.Upload("text4.txt");
//client.Download(@"text2.txt", @"D:\");


while (true)
{
    Console.Write("tftp >>> ");
    var command = Console.ReadLine().Split(" ");

    if (command[0].Equals("disconnect"))
    {
        Console.WriteLine("Завершение работы...");
        break;
    }

    switch (command[0])
    {
        case "upload":
            client.Upload(command[1]);
            break;
        case "download":
            client.Download(command[1], command[2]);
            break;
        case "help":
            Console.WriteLine("download\nupload\ndisconnect");
            break;
        default:
            Console.WriteLine("Неизвестная команда!");
            break;
    }
}

Console.Read();