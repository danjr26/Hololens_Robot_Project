__author__ = 'J1NEIDIG'
# Echo client program
import socket
import time
import math
joint1 = math.radians(-101.52)
joint2 = math.radians(-144.57)
joint3 = math.radians(-50.48)
joint4 = math.radians(-79.95)
joint5 = math.radians(91.37)
joint6 = math.radians(-34.94)
joint11 = math.radians(-172.19)
joint21 = math.radians(-86.27)
joint31 = math.radians(-129.61)
joint41 = math.radians(-55.21)
joint51 = math.radians(91.37)
joint61 = math.radians(-34.94)
t1 = "1.5"
r1 = "1.0"
p = "p"
HOST = "192.168.0.3"
PORT = 30002
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))
s.send ("movej([" + str(joint1) + " ," + str(joint2) + " ," + str(joint3) + " ," + str(joint4) + " ," + str(joint5) + " ," + str(joint6) + "], a=" + str(t1) + " ," + "v=" + str(r1) + ")" + "\n")
#s.send ("movej(p[0,0.005,0.005,0,0,0], a=.5, v=.5)" + "\n")
#s.send ("global i_var_1(get_actual_tcp_pose())" + "\n")
time.sleep(3)
s.send ("movej([" + str(joint11) + " ," + str(joint21) + " ," + str(joint31) + " ," + str(joint41) + " ," + str(joint51) + " ," + str(joint61) + "], a=" + str(t1) + " ," + "v=" + str(r1) + ")" + "\n")
time.sleep(3)
data = s.recv(1024)
s.close()
print ("Received", repr(data))
print ("joint1", repr(joint1))
