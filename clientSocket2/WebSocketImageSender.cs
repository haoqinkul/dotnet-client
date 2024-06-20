using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using WebSocketSharp;
using Newtonsoft.Json;
//using System.Text.Json;


namespace clientSocket
{
    public class WebSocketImageSender
    {
        private WebSocket _webSocket;
        private string _resultsFolder;
        private string _imgPath;

        public WebSocketImageSender(string serverUri, string resultsFolder)
        {
            _resultsFolder = resultsFolder;
            //_imgPath = imgPath;
            _webSocket = new WebSocket(serverUri);
            _webSocket.OnMessage += OnMessage;
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnClose += Onclose;
            _webSocket.OnError += OnError;

        }
        public void Connect()
        { 
            _webSocket.Connect();
        }

        public void Disconnect()
        { _webSocket.Close(); }

        public void sendImage(byte[] imageBytes, string imageName, string imgPath)
        {
            _imgPath = imgPath;
            var metadata = new
            {
                image_name = imageName
            };

            if (!_webSocket.IsAlive)
            {
                try {
                    _webSocket.Connect();
                }
                catch
                {
                    throw new InvalidCastException("WebSocket connection is not open.");
                }   
            }

            string metadataJson = JsonConvert.SerializeObject(metadata);
            //string metadataJson = JsonSerializer.Serialize(metadata);

            _webSocket.Send(metadataJson);
            _webSocket.Send(imageBytes);
            //await Task.Run(() => _webSocket.Send(imageBytes));

        }

        private void SaveResults(string imagePath, MessageEventArgs e, string resultsFolder)
        {
            string resultPath = Path.Combine(resultsFolder, Path.GetFileNameWithoutExtension(imagePath) + ".json");
            File.WriteAllText(resultPath, e.Data);
        }



        private void OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText)
            {   
                //var response = JsonConvert.DeserializeObject(e.Data);
                //File.WriteAllText(resultsFolder, e.Data);
                SaveResults(_imgPath, e, _resultsFolder);
                Console.WriteLine("Prediction Get");
            }
            
        }

        private void Onclose(object sender, CloseEventArgs e)
        {
            Console.WriteLine("Websocket connection closed", e.Reason);
        }

        private void OnOpen(object sender, EventArgs e)
        {
            Console.WriteLine("Websocket connection opened.");
        }

        private void OnError(object sender, WebSocketSharp.ErrorEventArgs e)
        {
            Console.WriteLine("WebSocket error: " + e.Message);
        }


    }
}
