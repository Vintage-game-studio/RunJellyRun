using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public float XSmoothTime = 0.8f;
    public float YSmoothTime = 0.8f;
    private float xVelocity = 0;

    private float yVelocity = 0;

    // Use this for initialization
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (Player == null)
            return;
        Vector3 CamPos = transform.position;
        Vector3 PlayerPos = Player.position;

        PlayerPos = new Vector3(Mathf.SmoothDamp(CamPos.x, PlayerPos.x, ref xVelocity, XSmoothTime),
            Mathf.SmoothDamp(CamPos.y, PlayerPos.y, ref yVelocity, XSmoothTime), CamPos.z);

        transform.position = PlayerPos;
    }
}
