var fs = require('fs')
var WebSocketServer = require('websocket').server;
var http = require('http');

var clients = [];
var server = http.createServer(function(request, response) {
    console.log((new Date()) + ' Received request for ' + request.url);
    response.writeHead(404);
    response.end();
});
server.listen(8080, function() {
    console.log((new Date()) + ' Server is listening on port 8080');
});

wsServer = new WebSocketServer({
    httpServer: server,
    // You should not use autoAcceptConnections for production
    // applications, as it defeats all standard cross-origin protection
    // facilities built into the protocol and the browser.  You should
    // *always* verify the connection's origin and decide whether or not
    // to accept it.
    autoAcceptConnections: false
});

function originIsAllowed(origin) {
  // put logic here to detect whether the specified origin is allowed.
  return true;
}

wsServer.on('request', function(request) {
    console.log('on request');
    if (!originIsAllowed(request.origin)) {
      // Make sure we only accept requests from an allowed origin
      request.reject();
      console.log((new Date()) + ' Connection from origin ' + request.origin + ' rejected.');
      return;
    }

    var connection = request.accept(null, request.origin);
	clients.push(connection);
    console.log((new Date()) + ' Connection accepted.');
    connection.on('message', function(message) {
		
		function getFile(file) {
				var file='./data/'+file.substring(4, file.length)+'.json';
				fs.readFile(file, 'utf8', function (err,data) {
				  if (err) {
					return console.log(err);
				  }
					console.log('Send Message: ' + data);
					clients.forEach(function(client) {
					  client.sendUTF(data);
					});
				});
		}
		
        if (message.type === 'utf8') {
            console.log('Received Message: ' + message.utf8Data);
			
			
			var echo=message.utf8Data;
			if(echo=='get frame') {
				console.log('find frame');

				getFile('get /star75/B0');
				getFile('get /star75/B1');
				getFile('get /star75/B2');
				getFile('get /star75/B3');
			}
			else if(echo=='get opening') {
				console.log('find opening');
				getFile('get /star75/O0');
				getFile('get /star75/O1');
				getFile('get /star75/O2');
				getFile('get /star75/O3');
				getFile('get /star75/handle');
			}
			else if(echo=='get transom') {
				getFile('get /star75/transom');
			}
			else if(echo.startsWith('get ')) {
				console.log('find get');
				getFile(echo);
			} 
			else {
				console.log('Send Message: ' + echo);
				clients.forEach(function(client) {
				  client.sendUTF(echo);
				});
			}
			

			//connection.sendUTF(message.utf8Data);
        }
        else if (message.type === 'binary') {
            console.log('Received Binary Message of ' + message.binaryData.length + ' bytes');
            connection.sendBytes(message.binaryData);
        }
    });
    connection.on('close', function(reasonCode, description) {
		clients = clients.filter(item => item !== connection);
        console.log((new Date()) + ' Peer ' + connection.remoteAddress + ' disconnected.');
    });
});