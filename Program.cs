// Software desenvolvido por Trevias Xk
// Redes sociais:       treviasxk
// Github:              https://github.com/treviasxk

// Script Application Example
using NetworkUDP;

class Program {
   static void Main(string[] args){
      ClientUDP.ConnectServer("127.0.0.1", 26950);
      ClientUDP.OnReceivedNewDataServer += OnReceivedNewDataServer;
      ClientUDP.OnStatusConnection += OnStatusConnection;
   }

//========================= Events =========================
   static void OnStatusConnection(StatusConnection _status){
      Console.WriteLine("[STATUS] {0}", _status);
      if(_status == StatusConnection.Connected){
         string _text = "OI";
         Console.WriteLine("[CLIENT] {0}", _text);
         ClientUDP.SendText(_text);
      }
   }
   static void OnReceivedNewDataServer(string _text){
      Console.WriteLine("[SERVER] {0}", _text);
   }
}

/* Script Unity Example - Network.cs
using UnityEngine;
using NetworkUDP;

public class Network : MonoBehaviour{
    void Start(){
        ClientUDP.ConnectServer("127.0.0.1", 26950);
        ClientUDP.OnReceivedNewDataServer += OnReceivedNewDataServer;
        ClientUDP.OnStatusConnection += OnStatusConnection;
    }

    void Update() {
        ClientUDP.MainThread();
    }

//========================= Events =========================
    void OnStatusConnection(StatusConnection _status){
        Debug.Log("<color=green>STATUS</color>: " + _status);
        if(_status == StatusConnection.Connected)
            ClientUDP.SendText("OI");
    }

    void OnReceivedNewDataServer(string _text){
        Debug.Log("<color=green>MESSAGE:</color>: " + _text);
        ClientUDP.RunOnMainThread(() => {
            gameObject.name = _text;
        });
    }
}
*/