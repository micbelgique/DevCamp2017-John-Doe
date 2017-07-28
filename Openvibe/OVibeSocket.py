import websocket
import thread
import json
from collections import deque

global debug 
debug = False

class FrequencyCommand(object):
    def __init__(self):
        self.type = "frequency"
        self.value = 0

class FocusCommand(object):
	def __init__(self):
		self.type="focus"
		self.value=0

class OVibeSocket(OVBox):
	def __init__(self):
		OVBox.__init__(self)
		
		self.socket = None
		self.ws = None
		self.wst = None
		self.accumulator = 3
		self.server = "ws://localhost:8080"
		self.freqQueue = deque([])
		self.focusQueue = deque([])
				
	def initialize(self):
		def on_open(ws): 
			print "open socket"
			
		def on_message(ws, message):
			print "received: %s", message

		def on_error(ws, error):
			print "error: %s", error

		def on_close(ws):
			print "### closed ###"
			
		self.server=self.setting['server']

		websocket.enableTrace(debug)
		self.ws = websocket.WebSocketApp(self.server,
											on_message = on_message,
											on_error = on_error,
											on_close = on_close)
		self.ws.on_open = on_open
		
		self.ws.keep_running = True 
		self.wst = threading.Thread(target=self.ws.run_forever)
		self.wst.daemon = True
		self.wst.start()		
				
	def sendFrequency(self, value):
		frequencyCommand = FrequencyCommand()
		frequencyCommand.value=value
		
		self.ws.send(json.dumps(frequencyCommand.__dict__))
	
	def sendFocus(self, value):
		focusCommand = FrequencyCommand()
		focusCommand.value=value

		self.ws.send(json.dumps(focusCommand.__dict__))
			
	def process(self):
		for chunkIndex in range( len(self.input[0]) ):
			addFrequency(self.input[0].pop())
			
		for chunkIndex in range(len (self.input[1]) ):
			accFocus(self.input[1].pop())			
		
	def uninitialize(self):
		self.ws.keep_running = False;
	
box = OVibeSocket()	
