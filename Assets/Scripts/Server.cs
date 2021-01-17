using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLib;
using LiteNetLib.Utils;

public class Server : MonoBehaviour
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

        Debug.LogError("Server Start");
        netListener = new EventBasedNetListener();
        netManager = new NetManager(netListener);
        netProcessor = new NetPacketProcessor();

        netManager.Start(9050);

        netListener.ConnectionRequestEvent += (request) => {
            request.Accept();
        };

        netListener.PeerConnectedEvent += (client) => {
            Debug.LogError($"Client connected: {client}");

            //netProcessor.Send(client, new MethodPacket() { MethodValue = 0 }, DeliveryMethod.ReliableOrdered);
        };
    }

    void Update()
    {
        netManager.PollEvents();

        CubePos();

        DestroyCube();

        text.text = "Count: " + count;
    }

    public void NewCube()
    {
        CubeRef = Instantiate(Cube);

        netManager.SendToAll(netProcessor.Write(new MethodPacket() { MethodValue = 0 , CountValue = count }), DeliveryMethod.ReliableOrdered);
    }

    void CubePos()
    {
        CubeRef.GetComponent<Gravity>().PosUpdate();

        netManager.SendToAll(netProcessor.Write(new MethodPacket() { MethodValue = 1, CountValue = count }), DeliveryMethod.ReliableOrdered);
    }

    void DestroyCube()
    {
        if(CubeRef.GetComponent<Gravity>().gone == true)
        {
            Destroy(CubeRef);

            count += 1;

            netManager.SendToAll(netProcessor.Write(new MethodPacket() { MethodValue = 2, CountValue = count }), DeliveryMethod.ReliableOrdered);
        }
    }
}
