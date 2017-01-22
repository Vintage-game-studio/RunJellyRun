using UnityEngine;
using System.Collections;

public class Replacer : MonoBehaviour
{
    public string Tag = "Hazards";
    public Transform[] Prefabs;

    public void Replace()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(Tag);

        foreach (GameObject g in gameObjects)
        {
            Replace(g.transform,Prefabs[Random.Range(0,Prefabs.Length)]);
        }
    }

    private void Replace(Transform oldTransform, Transform prefab)
    {
        Transform newTransform = (Transform)Instantiate(prefab, oldTransform.position, oldTransform.rotation);


        Vector3 deffer = newTransform.GetComponentInChildren<BoxCollider>().transform.position
                          - oldTransform.GetComponentInChildren<BoxCollider>().transform.position;

        newTransform.position= newTransform.position-deffer;

        newTransform.parent = oldTransform.parent;

        DestroyImmediate(oldTransform.gameObject);
    }
}
