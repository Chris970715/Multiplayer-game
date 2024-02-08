using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using ExitGames.Client.Photon.StructWrapping;

public class ItemSpawnController : NetworkBehaviour
{
    public static ItemSpawnController Instance;
    [SerializeField] private LayerMask itemMask;

    [Header("Item Spawn Settings")]
    [SerializeField] private NetworkPrefabRef[] itemNetworkPrefabRef;
    [SerializeField] private GameObject[] itemPoints;
    [SerializeField] protected float RespawnTimeAfterItems = 10f;

    [Networked] private NetworkBool ItemIsStillThere { get; set; }
    [Networked] private TickTimer RespawnTimeTimer { get; set; }
    [Networked] private int EmptyItemPosIndex { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            // Destroy
            Destroy(gameObject);
        }
    }

    public override void Spawned()
    {
        ItemIsStillThere = true;
    }

    public override void FixedUpdateNetwork()
    {
        if (ItemIsStillThere)
        {
            CheckToRespawnItems();
        }

        if (RespawnTimeTimer.Expired(Runner) && !ItemIsStillThere)
        {
            //StartSpawningItems();
            SpawnItems();
        }
    }

    public void StartSpawningItems()
    {
        for (int i = 0; i < itemNetworkPrefabRef.Length && i < itemPoints.Length; i++)
        {
            NetworkObject gameObject = Runner.Spawn(itemNetworkPrefabRef[Random.Range(0, itemNetworkPrefabRef.Length)], itemPoints[i].gameObject.transform.position, Quaternion.identity);
            Rigidbody2D rb2D = gameObject.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;
        }
    }

    private void SpawnItems()
    {
        if(Runner.ActivePlayers.First() == Runner.LocalPlayer){
            NetworkObject gameObject = Runner.Spawn(itemNetworkPrefabRef[Random.Range(0, itemNetworkPrefabRef.Length)], itemPoints[EmptyItemPosIndex].transform.position, Quaternion.identity);
            Rigidbody2D rb2D = gameObject.GetComponent<Rigidbody2D>();
            rb2D.isKinematic = true;
            ItemIsStillThere = true;
        }

    }

    public void CheckToRespawnItems()
    {
        for (int i = 0; i < itemPoints.Length; i++)
        {

            var collider = Runner.GetPhysicsScene2D().OverlapBox(itemPoints[i].transform.position,
                itemPoints[i].GetComponent<BoxCollider2D>().bounds.size,
                0,
                itemMask
            );

            if (collider != default)
            {
                ItemIsStillThere = true;
            }
            else
            {
                EmptyItemPosIndex = i;
                RespawnTimeTimer = TickTimer.CreateFromSeconds(Runner, RespawnTimeAfterItems);
                ItemIsStillThere = false;
                break;
            }

        }

    }

}
