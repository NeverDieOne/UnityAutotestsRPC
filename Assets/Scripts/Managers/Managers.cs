using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(InventoryManager))]
public class Managers : MonoBehaviour {
    public static InventoryManager Inventory {get; private set;}

    private List<IGameManager> _managers;

    void Awake() {
        Inventory = GetComponent<InventoryManager>();
        _managers = new List<IGameManager> {
            Inventory,
        };

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers() {
        foreach (IGameManager manager in _managers) {
            manager.Startup();
        }

        yield return null;

        int numManagers = _managers.Count;
        int numReady = 0;

        while (numReady < numManagers) {
            int lastReady = numReady;
            numReady = 0;

            foreach (IGameManager manager in _managers) {
                if (manager.status == ManagerStatus.Started) {
                    numReady += 1;
                }
            }

            if (numReady > lastReady) {
                Debug.Log("Progress: " + numReady + "/" + numManagers);
            }
            yield return null;
        }

        Debug.Log("All managers started up");
    }
}
