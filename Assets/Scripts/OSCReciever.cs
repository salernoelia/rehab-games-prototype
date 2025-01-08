// OSCReciever.cs

using UnityEngine;
using System;
using System.Net.Sockets;
using System.Linq;
using System.Collections;
using CoreOSC.IO;

public class OscReceiver : MonoBehaviour
{
    public static OscReceiver Instance { get; private set; }

    private UdpClient udpClient;

    public float gyroX { get; private set; }
    public float gyroY { get; private set; }
    public float gyroZ { get; private set; }

    public float accelX { get; private set; }
    public float accelY { get; private set; }
    public float accelZ { get; private set; }

    public event Action<float, float, float> OnGyroDataReceived;

    public event Action<float, float, float> OnAccelDataReceived;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        udpClient = new UdpClient(57100);
        StartCoroutine(ReceiveOscMessages());
    }

    private IEnumerator ReceiveOscMessages()
    {
        while (true)
        {
            var receiveTask = udpClient.ReceiveMessageAsync();
            while (!receiveTask.IsCompleted) yield return null;
            var response = receiveTask.Result;

            if (response.Address.Value == "/zigsim/gyro")
            {
                var args = response.Arguments.ToArray();
                gyroX = Convert.ToSingle(args[0]);
                gyroY = Convert.ToSingle(args[1]);
                gyroZ = Convert.ToSingle(args[2]);

                OnGyroDataReceived?.Invoke(gyroX, gyroY, gyroZ);
            }

            if (response.Address.Value == "/zigsim/accel")
            {
                var args = response.Arguments.ToArray();
                accelX = Convert.ToSingle(args[0]);
                accelY = Convert.ToSingle(args[1]);
                accelZ = Convert.ToSingle(args[2]);

                OnAccelDataReceived?.Invoke(accelX, accelY, accelZ);


            }
        }
    }

    private void OnDestroy()
    {
        udpClient?.Close();
    }
}
