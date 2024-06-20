# C# implementation of watchdog and communication client

This repository is the C# implemntation of watchdog and communication client in the OSAI PC. 


![Communication architecture](/figures/FPD_v2.drawio.png)

The communication client is part of a class library called File Watchdog. When `File Watchdog` detects a new image being created, it calls the `communication client to send the image to the backend and receive the response`.

The code can be used in two ways: 
1. Import the `clientSocket2` as a dependency in your project 
2. Use the watchdog to monitor a shared folder 

Based on your current use case, I suggest using the first way. In the `clientSocket2`, I implemented the `IElaboratedImageCoordinates` interface and communication client. The module also parses the received JSON data to X,Y coordinates. The communication between client (your PC) and SUDE API is based on websocket. When you use the code, you may need fallowing adaptations: 
1. Creat websocket connection when you start the OSAI app"
* Refer to `Watchdog.cs` lines 23, 31-38:
```csharp 
WebSocketImageSender wbSender = new WebSocketImageSender(serverUrl, resultsFolder);

try
{
    wbSender.Connect();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

```
2. Send the Image using `wbSender`
* The input format of the image is byte array. Refer to `Watchdog.cs` lines 88-90: 
```csharp
List<IElaboratedImageCoordinates> coordinates = await wbSender.sendImageAsync(imageData, imageName, file_path);
```
The `wbSender` will return a list of an interface.


