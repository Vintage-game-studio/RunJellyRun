using UnityEngine;
using System.Collections;

public class PlayerCharacter : MonoBehaviour
{
    private GameObject playerGO;

    private PlayerController playerController;

    public ControllerConfigurations controllerConfigurations;

    private void Awake()
    {
        playerController = new PlayerController(controllerConfigurations);
    }
}
