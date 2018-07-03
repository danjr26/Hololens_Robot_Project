import socketserver
class MyServer(socketserver.BaseRequestHandler):
    def handle(self):
        while(True):
            print(self.request.recv(2048).strip().decode("utf-8"))

server = socketserver.TCPServer(("192.168.100.61", 9999), MyServer)
server.serve_forever()
    
