using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;
using LiteNetLib.Utils;
public class Client : MonoBehaviour
{
    NetManager netManager;

    EventBasedNetListener netListener;

    NetPacketProcessor netProcessor;

    public GameObject Cube;

    public Text text;

    int count;

    GameObject CubeRef;

    void Start()
    {
        count = 0;

        Debug.LogError("Client Start");
        netListener = new EventBasedNetListener();
        netProcessor = new NetPacketProcessor();

        netListener.PeerConnectedEvent += (server) =>
        {
            Debug.LogError($"Connected to server: {server}");
        };

        netListener.NetworkReceiveEvent += (server, reader, deliveryMethod) => {
            netProcessor.ReadAllPackets(reader, server);
        };

        netProcessor.SubscribeReusable<MethodPacket>((packet) => {
            Debug.Log("Packet got.");

            if(packet.MethodValue == 0)
            {
                NewCube();
            }

            if(packet.MethodValue == 1)
            {
                CubePos();
            }

            if(packet.MethodValue == 2)
            {
                DestroyCube();
            }

            count = packet.CountValue;
        });

        netManager = new NetManager(netListener);
        netManager.Start();
        netManager.Connect("localhost", 9050, "Banana");
    }

    void Update()
    {
        netManager.PollEvents();

        text.text = "Count: " + count;
    }
    
    void NewCube()
    {
        CubeRef = Instantiate(Cube);
    }

    void CubePos()
    {
        CubeRef.GetComponent<Gravity>().PosUpdate();
    }

    void DestroyCube()
    {
        Destroy(CubeRef);
    }
}
