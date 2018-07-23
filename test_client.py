#import socket
#s = socket.socket()
#s.connect(("10.0.3.83", 30002))
#s.send(bytes(string("movej(p[0.638, 0.0159, 0.7129z], v=0.3)\nmovej(p[], v=0.3)\n"), "ASCII"));
#s.close()

import socket
s = socket.socket()
s.connect(("10.0.3.83", 502))
message = b'\x00\x04\x00\x00\x00\x06\x00\x03\x01\x05\x00\x01'
s.send(message)
print(s.recv(64))
s.close()

