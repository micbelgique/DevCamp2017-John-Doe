using Assets.Scripts.Network.Message;
using BestHTTP;
using BestHTTP.WebSocket;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

// dependency UniRx
// dependency Best HTTP Pro

public class CloseMessage {
    public CloseMessage(ushort code, string message)
    {
        this.code = code;
        this.message = message;
    }

    public UInt16 code { get; set; }
    public string message { get; set; }
}

public class ErrorMessage {
    public ErrorMessage(Exception exception, string message)
    {
        this.exception = exception;
        this.message = message;
    }

    public Exception exception { get; set; }
    public string message { get; set; }
}

public class InstantiedSocket {
    public Subject<string> onReceiveMessage { get; set; }
    public Subject<CloseMessage> onCloseSocket { get; set; }
    public Subject<ErrorMessage> onErrorSocket { get; set; }
    public Subject<Unit> onOpenSocket { get; set; }
    public WebSocket webSocket { get; set; }
}

[Serializable]
public class ObservableWebSocket
{
    public string name;
    public string url;

    public InstantiedSocket instance { get; set; }

    ObservableWebSocket() {
        this.instance = new InstantiedSocket();
    }
}

public class ObservableWebSocketTrigger : ObservableTriggerBase {

    #region Documentation
    // WebSocketTrigger.instance.onReceiveMessageAsObservable().Subscribe(x => Debug.Log(x));
    #endregion

    #region Class Fields
    public static ObservableWebSocketTrigger instance = null;
    public ObservableWebSocket[] sockets;
    #endregion

    #region Business logic
    private ObservableWebSocket findSocket(string name) {
        if (sockets.Length == 1)
            return sockets.First();

        var socket = from s in sockets
                     where s.name == name
                     select s;

        return socket.First();
    }

    private ObservableWebSocket findSocket(WebSocket ws)
    {
        if (sockets.Length == 1)
            return sockets.First();

        var socket = from s in sockets
                     where s.instance.webSocket == ws
                     select s;

        return socket.First();
    }

    private void createSocket(ObservableWebSocket socket) {
        // Create the WebSocket instance
        WebSocket webSocket = new WebSocket(new Uri(socket.url));
#if !BESTHTTP_DISABLE_PROXY && !UNITY_WEBGL
        if (HTTPManager.Proxy != null)
            webSocket.InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
#endif
        // Subscribe to the WS events
        webSocket.OnOpen += OnOpen;
        webSocket.OnMessage += OnMessageReceived;
        webSocket.OnClosed += OnClosed;
        webSocket.OnError += OnError;

        webSocket.Open();

    }
    #endregion

    #region Observable Getter
    public IObservable<string> onReceiveMessageAsObservable(string name)
    {
        InstantiedSocket iSocket = findSocket(name).instance;
        return iSocket.onReceiveMessage ?? (iSocket.onReceiveMessage = new Subject<string>());
    }
    public IObservable<CloseMessage> onCloseSocketAsObservable(string name)
    {
        InstantiedSocket iSocket = findSocket(name).instance;
        return iSocket.onCloseSocket ?? (iSocket.onCloseSocket = new Subject<CloseMessage>());
    }
    public IObservable<ErrorMessage> onErrorSocketAsObservable(string name)
    {
        InstantiedSocket iSocket = findSocket(name).instance;
        return iSocket.onErrorSocket ?? (iSocket.onErrorSocket = new Subject<ErrorMessage>());
    }
    public IObservable<Unit> onOpenSocketAsObservable(string name)
    {
        InstantiedSocket iSocket = findSocket(name).instance;
        return iSocket.onOpenSocket ?? (iSocket.onOpenSocket = new Subject<Unit>());
    }
    #endregion

    #region Rx Events
    protected override void RaiseOnCompletedOnDestroy()
    {
        this.sockets.ToList().ForEach(socket => {
            if (socket.instance.onOpenSocket != null)
                socket.instance.onOpenSocket.OnCompleted();
            if (socket.instance.onReceiveMessage != null)
                socket.instance.onReceiveMessage.OnCompleted();
            if (socket.instance.onCloseSocket != null)
                socket.instance.onCloseSocket.OnCompleted();
            if (socket.instance.onErrorSocket != null)
                socket.instance.onErrorSocket.OnCompleted();
        });

    }
    #endregion

    #region Unity Events
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        this.sockets.ToList().ForEach(socket => {
            createSocket(socket);
        });
    }

    void OnDestroy()
    {
        this.sockets.ToList().ForEach(socket => {
            if (socket.instance.webSocket != null)
                socket.instance.webSocket.Close();
        });
    }

    #endregion

    #region Websocket Events
    /// <summary>
    /// Called when the web socket is open, and we are ready to send and receive data
    /// </summary>
    void OnOpen(WebSocket ws)
    {
        InstantiedSocket iSocket = findSocket(ws).instance;
        //Text += string.Format("-WebSocket Open!\n");
        if (iSocket.onOpenSocket != null) iSocket.onOpenSocket.OnNext(Unit.Default);
        
    }

    /// <summary>
    /// Called when we received a text message from the server
    /// </summary>
    void OnMessageReceived(WebSocket ws, string message)
    {
        Debug.Log(message);
        InstantiedSocket iSocket = findSocket(ws).instance;
        if (iSocket.onReceiveMessage != null) iSocket.onReceiveMessage.OnNext(message);

        //Text += string.Format("-Message received: {0}\n", message);
        //Debug.Log(message);
    }

    /// <summary>
    /// Called when the web socket closed
    /// </summary>
    void OnClosed(WebSocket ws, UInt16 code, string message)
    {
        InstantiedSocket iSocket = findSocket(ws).instance;
        //Text += string.Format("-WebSocket closed! Code: {0} Message: {1}\n", code, message);
        if (iSocket.onCloseSocket != null) iSocket.onCloseSocket.OnNext(new CloseMessage(code, message));
        iSocket.webSocket = null;
        //Debug.Log(Text);
    }

    /// <summary>
    /// Called when an error occured on client side
    /// </summary>
    void OnError(WebSocket ws, Exception ex)
    {
        InstantiedSocket iSocket = findSocket(ws).instance;

        string errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws.InternalRequest.Response != null)
            errorMsg = string.Format("Status Code from Server: {0} and Message: {1}", ws.InternalRequest.Response.StatusCode, ws.InternalRequest.Response.Message);
#endif

        if (iSocket.onErrorSocket != null) iSocket.onErrorSocket.OnNext(new ErrorMessage(ex, errorMsg));

        //Text += string.Format("-An error occured: {0}\n", (ex != null ? ex.Message : "Unknown Error " + errorMsg));

        iSocket.webSocket = null;
        //Debug.Log(Text);
    }
    #endregion

}
