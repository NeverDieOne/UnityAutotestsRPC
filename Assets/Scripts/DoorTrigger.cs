using System.Collections;
using UnityEngine;

public class DoorTrigger : MonoBehaviour {
    private bool _showPopup = false;
    readonly private string _text = "Can not open door without key";

    void OnGUI() {
        if (_showPopup) {
            float popupWidth = 200;
            float popupHeight = 30;
            float popupX = (Screen.width - popupWidth) / 2;
            float popupY = (Screen.height - popupHeight) / 2;

            GUI.Box(new Rect(popupX + 50, popupY + 50, popupWidth, popupHeight), _text);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        int keysCount = Managers.Inventory.GetItemCount("key");
        Debug.Log($"Keys: {keysCount}");
        if (keysCount == 0) {
            _showPopup = true;
            StartCoroutine(ClosePopup());
        } else {
            Destroy(gameObject);
        }
    }

    private IEnumerator ClosePopup() {
        yield return new WaitForSeconds(2);
        _showPopup = false;
    }
}
