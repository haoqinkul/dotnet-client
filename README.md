# C# implementation of watchdog and communication client

This repository is the C# implemntation of watchdog and communication client in the OSAI PC. 

The code can be used in two ways: 
1. Import the `clientSocket2` as a dependency in your project 
The clientSocket2 will send the image to SUDE PC and get a response. The output is a list of an interface.

![Communication architecture](/figures/case2.png)

2. Use the watchdog to monitor a shared folder 
When a new image is saved to the shared folder, the watchdog will call the clientSocket2. The output is a JSON file. The shared folder and results folder is configurable in the ‘APP.config’

![Communication architecture](/figures/case1.png)
 

In the `clientSocket2`, I implemented the `IElaboratedImageCoordinates` interface and communication client. The module can parse the received JSON data to X,Y coordinates. The client (your PC) and SUDE API communication are based on websocket.

 •	If you use the first way, you may need following adaptations:  you may need following adaptations: 
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

•	If you use the second way (watchdog), you can modify the shared folder and result folder in the configuration file `App.config`. The returned JSON results are saved with the same name as the image. Additionally, I have attached our JSON file for your reference. 
