using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;

public class TCPManager : Singleton<TCPManager>
{
    //a true/false variable for connection status
    public bool SocketReady = false;

    TcpClient _socket;
    NetworkStream _stream;
    StreamWriter _writer;
    StreamReader _reader;

    //try to initiate connection
    public void SetupSocket(string host, int port)
    {
        try
        {
            _socket = new TcpClient(host, port);
            _stream = _socket.GetStream();
            _writer = new StreamWriter(_stream) {AutoFlush = true};
            _reader = new StreamReader(_stream);
            SocketReady = true;
            Log.Info("Socket is ready");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
        }
    }

    //send message to server
    public void WriteSocket(string message)
    {
        if (!SocketReady)
            return;
        var tmpString = message + "\r\n";
        _writer.Write(tmpString);
        Log.Info("[CLIENT] " + message );
    }

    //read message from server
    public string ReadSocket()
    {
        var result = "";
        if (_stream.DataAvailable)
        {
            var inStream = new Byte[_socket.SendBufferSize];
            _stream.Read(inStream, 0, inStream.Length);
            result += System.Text.Encoding.UTF8.GetString(inStream);
        }
        return result;
    }

    //disconnect from the socket
    public void CloseSocket()
    {
        if (!SocketReady)
            return;
        _writer.Close();
        _reader.Close();
        _socket.Close();
        SocketReady = false;
    }

    private void OnApplicationQuit()
    {
        Log.Info("Closing socket");
        if (AppManager.Instance.Settings["Enable Remote"].BoolValue)
        {
            CloseSocket();
        }
    }

    //keep connection alive, reconnect if connection lost
//    public void MaintainConnection()
//    {
//        if (!_stream.CanRead)
//        {
//            SetupSocket();
//        }
//    }
}