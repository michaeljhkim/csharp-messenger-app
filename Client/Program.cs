using System.Net.Sockets;

// Define the TCP Client
TcpClient client = new TcpClient();

// Connect the client to the Server using defined Address and Port in Constants
client.Connect(Constants.Address, Constants.PORT);
Console.WriteLine("Connected to the server!");

// Define instances of Messenger for stream reading and writing
Messenger inStream = new Messenger();
Messenger outStream = new Messenger();

// List to store outgoing messages from the client
List<string> outgoingMessages = new List<string>();

// Task to continuously read input from the console and add it to the outgoingMessages list
var input_messages = new TaskFactory().StartNew(async () => {
    while (true) {
        string msg = Console.ReadLine() ?? string.Empty;
        outgoingMessages.Add(msg);
    }
});

// Main loop to read incoming packets and send outgoing packets 
while (true) {
    ReadPackets();
    SendPackets();
}

// Function to read incoming packets from the server
void ReadPackets() {
    NetworkStream stream = client.GetStream();
    for (int i = 0; i < 10; i++) {
        if (stream.DataAvailable) {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            (int opcode, string message) = inStream.ParseMessagePacket(buffer.Take(bytesRead).ToArray());
            Console.WriteLine($"Received: [{opcode}] - {message}");
        }
    }
}

// Function to send packets to the server
void SendPackets() {
    if (outgoingMessages.Count > 0) {
        var msg = outgoingMessages[0];
        var packet = outStream.CreateMessagePacket(10, msg);
        client.GetStream().Write(packet);
        outgoingMessages.RemoveAt(0);
    }
}