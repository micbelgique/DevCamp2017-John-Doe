import websocket
import thread

global debug 
debug = False

class OVibeSocket(OVBox):
	def __init__(self):
		OVBox.__init__(self)
		
		self.socket = None
		self.ws = None
		self.wst = None
		
	def initialize(self):
		def on_open(ws): 
			print "open socket"
			
		def on_message(ws, message):
			print "received: %s", message

		def on_error(ws, error):
			print "error: %s", error

		def on_close(ws):
			print "### closed ###"

		websocket.enableTrace(debug)
		self.ws = websocket.WebSocketApp("ws://echo.websocket.org/",
									on_message = on_message,
									on_error = on_error,
									on_close = on_close)
		self.ws.on_open = on_open

		self.ws.keep_running = True 
		self.wst = threading.Thread(target=self.ws.run_forever)
		self.wst.daemon = True
		self.wst.start()		
		
	def process(self):
		for chunkIndex in range( len(self.input[0]) ):
			chunk = self.input[0].pop()
			
			self.ws.send("{}".format(chunk))
		
	def uninitialize(self):
		self.ws.keep_running = False;
	
box = OVibeSocket()	
