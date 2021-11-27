using System.Net;
using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

/*
    Documentation: https://mirror-networking.gitbook.io/docs/components/network-discovery
    API Reference: https://mirror-networking.com/docs/api/Mirror.Discovery.NetworkDiscovery.html
*/

public class DiscoveryRequest : NetworkMessage
{
    // Add properties for whatever information you want sent by clients
    // in their broadcast messages that servers will consume.
}

public class DiscoveryResponse : NetworkMessage
{
    private string hostName;
    private short connectedCount;

    public string HostName => hostName;
    public short ConnectedCount => connectedCount;

    public DiscoveryResponse()
    {
        hostName = "default";
        connectedCount = 0;
    }

    public DiscoveryResponse(string hName, short plrCount)
    {
        hostName = hName;
        connectedCount = plrCount;
    }
}

public class MyNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, DiscoveryResponse>
{
    #region Server

    /// <summary>
    /// Reply to the client to inform it of this server
    /// </summary>
    /// <remarks>
    /// Override if you wish to ignore server requests based on
    /// custom criteria such as language, full server game mode or difficulty
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    protected override void ProcessClientRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        base.ProcessClientRequest(request, endpoint);
    }

    /// <summary>
    /// Process the request from a client
    /// </summary>
    /// <remarks>
    /// Override if you wish to provide more information to the clients
    /// such as the name of the host player
    /// </remarks>
    /// <param name="request">Request coming from client</param>
    /// <param name="endpoint">Address of the client that sent the request</param>
    /// <returns>A message containing information about this server</returns>
    protected override DiscoveryResponse ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
    {
        var netManager = FindObjectOfType<MyNetworkManager>();
        var manager = FindObjectOfType<GameManager>();
        Debug.Log("MyNetworkDiscovery: netManager.HostName = \"" + netManager.HostName + "\", manager.PlayersConnected = " + manager.PlayersConnected);
        return new DiscoveryResponse(netManager.HostName, manager.PlayersConnected);
    }

    #endregion Server

    #region Client

    /// <summary>
    /// Create a message that will be broadcasted on the network to discover servers
    /// </summary>
    /// <remarks>
    /// Override if you wish to include additional data in the discovery message
    /// such as desired game mode, language, difficulty, etc... </remarks>
    /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
    protected override DiscoveryRequest GetRequest()
    {
        return new DiscoveryRequest();
    }

    /// <summary>
    /// Process the answer from a server
    /// </summary>
    /// <remarks>
    /// A client receives a reply from a server, this method processes the
    /// reply and raises an event
    /// </remarks>
    /// <param name="response">Response that came from the server</param>
    /// <param name="endpoint">Address of the server that replied</param>
    protected override void ProcessResponse(DiscoveryResponse response, IPEndPoint endpoint)
    {
        var serverBrowser = GameObject.FindGameObjectWithTag("serverBrowser").GetComponent<ServerBrowserScript>();
        serverBrowser.SetData(response, endpoint.ToString());
    }

    #endregion Client
}