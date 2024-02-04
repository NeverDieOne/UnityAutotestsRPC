using UnityEngine;

public class CollectableItem : MonoBehaviour {
    [SerializeField] private string itemName;

    void OnTriggerEnter2D(Collider2D other) {
        Managers.Inventory.AddItem(itemName);
        Debug.Log("Item collected" + itemName);
        Destroy(gameObject);
    }
}
