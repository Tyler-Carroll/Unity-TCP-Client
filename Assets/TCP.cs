using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TCP : MonoBehaviour
{
    #region private members
    private TcpClient socketConnection;
    private Thread clientRecieveThread;
    #endregion


//Connect to python server (this creates a threaded call)
    void Start()
    {
        ConnectToTcpServer();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void ConnectToTcpServer() {
        try{
            //create new thread
            clientRecieveThread = new Thread (new ThreadStart(ListenForData));
            clientRecieveThread.IsBackground = true;
            clientRecieveThread.Start();
        }
        catch(Exception e) {
            Debug.Log($"On client connect exception: {e}");
        }
    }

    private void ListenForData() {
        try {
            //creates Tcp Client and connects client to localhost at port 5005
            socketConnection = new TcpClient("localhost", 5005);
            Byte[] bytes = new Byte[1024];
            while(true) {
                using (NetworkStream stream = socketConnection.GetStream()) {
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0){
                        var incommingData = new byte[length];
                        Array.Copy(bytes, 0, incommingData, 0, length);
                        string serverMessage = Encoding.UTF8.GetString(incommingData);

                        //Incomming data is the data that was recieved, use this variable to gain heart rate
                        Debug.Log($"Server message received as: {serverMessage}");
                        
                    }
                }
            }
        }
        catch (SocketException socketException) {
            Debug.Log($"Socket Exception: {socketException}");
        }
    }


//SendMessage() function not required for our uses, but still handy
    private void SendMessage() {         
		if (socketConnection == null) {             
			return;
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {                 
				string clientMessage = "This is a message from one of your clients."; 				
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.ASCII.GetBytes(clientMessage); 				
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	} 


}
