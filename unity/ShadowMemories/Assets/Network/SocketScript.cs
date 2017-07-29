using BestHTTP.JSON;
using BestHTTP.SocketIO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocketScript : MonoBehaviour
{
    #region class member fields
    public static SocketScript instance = null;
    private SocketManager Manager;
    #endregion

    #region Socket events
    private void onChatMessage(Socket socket, Packet packet, params object[] args)
    {
        Debug.Log(args[0]);
    }

    private void onObjectMessage(Socket socket, Packet packet, params object[] args)
    {
        Dictionary<string, object> data = args[0] as Dictionary<string, object>;
        Debug.Log(Json.Encode(data));
    }
    #endregion

    #region Unity events
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Use this for initialization
    void Start()
    {
        // Change an option to show how it should be done
        SocketOptions options = new SocketOptions();
        options.AutoConnect = false;

        // Create the Socket.IO manager
        Manager = new SocketManager(new Uri("http://localhost:3000/socket.io/"), options);

        // Set up custom chat events
        Manager.Socket.On("chat message", onChatMessage);       
        Manager.Socket.On("object message", onObjectMessage);

        // The argument will be an Error object.
        Manager.Socket.On(SocketIOEventTypes.Error,
                          (socket, packet, args) => Debug.LogError(string.Format("Error: {0}", args[0].ToString())));

        // We set SocketOptions' AutoConnect to false, so we have to call it manually.
        Manager.Open();
    }

    void OnDestroy()
    {
        Manager.Close();
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion
}
