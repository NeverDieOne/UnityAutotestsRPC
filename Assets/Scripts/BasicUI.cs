using UnityEngine;

public class BasicUI : MonoBehaviour {

    void OnGUI() {
        int posX = 10;
        int posY = 10;
        int width = 100;
        int height = 30;

        int keysCount = Managers.Inventory.GetItemCount("key");
        string text = "No keys";
        if (keysCount != 0) {
            text = $"Keys {keysCount}";
        }

        GUI.Box(new Rect(posX, posY, width, height), text);
    }
}
