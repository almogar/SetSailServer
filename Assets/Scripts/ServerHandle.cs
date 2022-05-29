using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
        if (_fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[_fromClient].SendIntoGame(_username);
        //Debug.Log("finish WelcomeReceived");
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        //Debug.Log("start PlayerMovement");
        object[] _inputs = new object[_packet.ReadInt()];
        /*
        for (int i = 0; i < _inputs.Length; i++)
        {
            if(i == 2)
                _inputs[i] = _packet.ReadFloat();
            else
                _inputs[i] = _packet.ReadBool();
        }
        */
        if (_inputs[0] != null)
        {
            _inputs[0] = _packet.ReadBool();
            _inputs[1] = _packet.ReadFloat();
            _inputs[2] = _packet.ReadFloat();
        }
        
        Quaternion _rotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        //Debug.Log("finish PlayerMovement");
    }
}
