// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;
using System.Text.Json;

class dtg {
    public string name { get; set; }
    public string msg { get; set; }
}

class Program {
   static void Main(string[] args){
        ClientUDP.ConnectServer("127.0.0.1", 26950);
        ClientUDP.OnReceivedNewDataServer += new Eventos.OnReceivedNewDataServer(OnReceivedNewDataServer);
        ClientUDP.OnStatusConnection += new Eventos.OnStatusConnection(OnStatusConnection);
   }

   //========================= Evento =========================
   static void OnStatusConnection(StatusConnection _status){
      Console.WriteLine("[CLIENT] STATUS: {0}", _status);
      if(_status == StatusConnection.Connected){
         var data = new dtg();
         data.name = "CLIENT";
         data.msg = "OI";
         Console.WriteLine("[CLIENT] {0}: {1}", data.name, data.msg);
         ClientUDP.SendData(JsonSerializer.Serialize(data));
      }
   }
   static void OnReceivedNewDataServer(string _text){
      var data = JsonSerializer.Deserialize<dtg>(_text);
      Console.WriteLine("[CLIENT] {0}: {1}", data.name, data.msg);
   }
}