// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

using NetworkUDP;
using static NetworkUDP.Eventos;


public class dtg {
    public string? name { get; set; }
    public string? msg { get; set; }
}

class Program {
   static void Main(string[] args){

        ClientUDP.ConnectServer("127.0.0.1", 26950, new dtg());
        ClientUDP.OnReceivedNewDataServer += new OnReceivedNewDataServer(OnReceivedNewDataServer);
        var _data = new dtg();
        _data.name = "VOCÊ";
        _data.msg = "OI";
        Console.WriteLine("{0}: {1}", _data.name, _data.msg);
        ClientUDP.SendData(_data);
        Console.ReadKey();
   }

   //========================= Evento =========================
   static void OnReceivedNewDataServer(object _data){
      var data = (dtg)_data;
      Console.WriteLine("{0}: {1}", data.name, data.msg);
   }
}