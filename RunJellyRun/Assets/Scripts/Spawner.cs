using System;
using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{

    public GameObject PlayerGO;
    public static Spawner SpawnerInstance;
    public float ShootForce = 5;
    private Vector3 defualtPos;
    void Awake()
    {
        defualtPos = PlayerGO.transform.position;
        SpawnerInstance = this;

    }

    // Use this for initialization
    void Start()
    {
        Killer.KillEvent +=SpawnPlayer;
        SpawnPlayer();
    }


    public void SpawnPlayer()
    {
        PlayerGO.SetActive(true);
        PlayerGO.transform.position = defualtPos;
        PlayerGO.GetComponent<Rigidbody2D>().isKinematic = false;
        if (Camera.main.transform.parent == null)
            Camera.main.GetComponent<CameraFollow>().Player = PlayerGO.transform;
        else
            Camera.main.transform.parent.GetComponent<CameraFollow>().Player = PlayerGO.transform;

        //player.GetComponent<Rigidbody2D>().AddForce(transform.right * ShootForce, ForceMode2D.Impulse);

    }


    // Update is called once per frame
    void Update()
    {

    }
}
