import socket
s = socket.socket()
s.connect(("10.0.3.83", 502))
message = b'\x00\x04\x00\x00\x00\x06\x00\x03\x01\x90\x00\x06'
s.send(message)
print(s.recv(64))
s.close()
