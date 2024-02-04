using AustinHarris.JsonRpc;
using UnityEngine;


public class RpcMethods : JsonRpcService {
    public Player GetPlayer() {
        return GameObject.Find("Player").GetComponent<Player>();
    }

    [JsonRpcMethod]
    void ExampleMethod() {
        Debug.Log("Example Method");
        Player player = GetPlayer();
        // player.DoSomething();
    }

    [JsonRpcMethod]
    int Some(int i) {
        Debug.Log(i);
        return i;
    }
}
