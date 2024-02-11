using System.Collections.Generic;
using AustinHarris.JsonRpc;
using UnityEngine;
using UnityEngine.SceneManagement;


public class RpcMethods : JsonRpcService {

    private Player GetPlayer() {
        return GameObject.Find("Player").GetComponent<Player>();
    }

    [JsonRpcMethod]
    void LoadScene(string sceneName) {
        Debug.Log("Load scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

    [JsonRpcMethod]
    Dictionary<string, float> GetObjectCoordinates(string objectName) {
        Debug.Log("Get coords: " + objectName);
        GameObject gameObject = GameObject.Find(objectName);
        Vector3 gameObjectPosition = gameObject.transform.position;
        return new Dictionary<string, float> {
            {"x", gameObjectPosition.x},
            {"y", gameObjectPosition.y},
        };
    }

    [JsonRpcMethod]
    void TeleportPlayer(float x, float y) {
        Player _player = GetPlayer();
        Vector3 newPosition = new(x, y);
        _player.transform.position = newPosition;

    }

    [JsonRpcMethod]
    bool IsObjectPresent(string objectName) {
        GameObject gameObject = GameObject.Find(objectName);
        return gameObject;
    }

    [JsonRpcMethod]
    int GetItemCount(string itemName) {
        return Managers.Inventory.GetItemCount(itemName);
    }
}
