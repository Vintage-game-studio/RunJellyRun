using UnityEngine;
using System.Collections;

public class Killer : MonoBehaviour {

    public delegate void KillEventHandler();

    public static event KillEventHandler KillEvent;
	// Use this for initialization
	void Start () {
	
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
	    PlayerController player = collision.gameObject.GetComponent<PlayerController>();
	    if (player != null)
	    {
	        player.Kill();
	        //Destroy (collision.gameObject);
	        collision.gameObject.SetActive(false);
	        //Spawner.SpawnerInstance.SpawnPlayer ();
	        KillEvent.Invoke();
	    }
	}
	// Update is called once per frame
	void Update () {
	
	}
}
