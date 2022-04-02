// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;

class dtg {
    public string name { get; set; }
    public string msg { get; set; }
}

class Program {
   static void Main(string[] args){
        ClientUDP.ConnectServer("127.0.0.1", 26950, new dtg());
        ClientUDP.OnReceivedNewDataServer += new Eventos.OnReceivedNewDataServer(OnReceivedNewDataServer);
        ClientUDP.OnStatusConnection += new Eventos.OnStatusConnection(OnStatusConnection);
   }

   //========================= Evento =========================
   static void OnStatusConnection(StatusConnection _status){
      Console.WriteLine("[CLIENT] STATUS: {0}", _status);
      if(_status == StatusConnection.Connected){
         var _data = new dtg();
         _data.name = "CLIENT";
         _data.msg = "OI";
         Console.WriteLine("[CLIENT] {0}: {1}", _data.name, _data.msg);
         ClientUDP.SendData(_data);
      }
   }
   static void OnReceivedNewDataServer(object _data){
      var data = (dtg)_data;
      Console.WriteLine("[CLIENT] {0}: {1}", data.name, data.msg);
   }
}