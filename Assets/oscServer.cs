using UnityEngine;
using CoreOSC;
using CoreOSC.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System;
using System.Linq;



public class OscServer : MonoBehaviour
{
    private UdpClient udpClient;

    private Vector3 gyroData = Vector3.zero;  // Gyroscope data
    private Vector3 accelData = Vector3.zero; // Accelerometer data

    public Transform square; // Reference to the square object

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the UdpClient to listen on the specified port
        udpClient = new UdpClient(57100);

        // Start the coroutine to receive OSC messages
        StartCoroutine(ReceiveOscMessages());
    }

    private System.Collections.IEnumerator ReceiveOscMessages()
    {
        while (true)
        {
            // Wait for an incoming message asynchronously
            var receiveTask = udpClient.ReceiveMessageAsync();
            while (!receiveTask.IsCompleted)
            {
                yield return null; // Wait for the task to complete
            }

            // Process the received message
            var response = receiveTask.Result;

            if (response.Address.Value == "/zigsim/gyro")
            {
                // Convert Arguments to an array
                var args = response.Arguments.ToArray();
                gyroData = new Vector3(
                    Convert.ToSingle(args[0]),
                    Convert.ToSingle(args[1]),
                    Convert.ToSingle(args[2])
                );
            }

            if (response.Address.Value == "/zigsim/accel")
            {
                // Convert Arguments to an array
                var args = response.Arguments.ToArray();
                accelData = new Vector3(
                    Convert.ToSingle(args[0]),
                    Convert.ToSingle(args[1]),
                    Convert.ToSingle(args[2])
                );
            }

        }
    }

    void Update()
    {
        // Use the gyro data to move the square
        if (square != null)
        {
            square.position += gyroData * Time.deltaTime; // Scale movement with delta time
        }
    }

    void OnGUI()
    {
        // Display the gyro and accel data on the screen
        GUI.Label(new Rect(10, 10, 300, 20), $"Gyro: {gyroData}");
        GUI.Label(new Rect(10, 40, 300, 20), $"Accel: {accelData}");
    }

    // OnApplicationQuit is called when the application is closing
    void OnApplicationQuit()
    {
        // Close the UdpClient when the application quits
        udpClient.Close();
    }
}
