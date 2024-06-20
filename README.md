# C# implementation of watchdog and communication client

This repository is the C# implemntation of watchdog and communication client in the OSAI PC. 


![Communication architecture](/figures/FPD_v2.drawio.png)

The communication client is part of a class library called File Watchdog. When `File Watchdog` detects a new image being created, it calls the `communication client to send the image to the backend and receive the response`.



