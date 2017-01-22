using UnityEngine;
using System.Collections;

public class Killer : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	void OnCollisionEnter2D(Collision2D collision)
	{
	    CharacterController player = collision.gameObject.GetComponent<CharacterController>();
	    if (player != null)
	    {
	        //player.Kill();
	        Destroy (collision.gameObject);
	        //Spawner.SpawnerInstance.SpawnPlayer ();
	    }
	}
	// Update is called once per frame
	void Update () {
	
	}
}
