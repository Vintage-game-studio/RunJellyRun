using System.Security.Cryptography;
using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour
{
    public int Chance = 10;
    public int Decay = 5;
    public int Restore = 5;
    public int Hope = 0;

    public Transform Out;

    protected Transform SourceBlock ;

    private GameObject _blocksGameObject;


    public virtual void Generate()
    {
        DeleteBlocks();
        foreach (Block block in GetComponentsInChildren<Block>())
            block.DeleteBlocks();

        _blocksGameObject = new GameObject("Blocks");
        _blocksGameObject.transform.parent = transform;
    }

    public void DeleteBlocks()
    {
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).name == "Blocks")
                DestroyImmediate(transform.GetChild(i).gameObject);
    }

    protected Block CreateBlock(Block block)
    {
        return CreateBlock(block, 0);
    }
    protected Block CreateBlock(Block block, float angel,bool noCollider=false)
    {
        Block newBlock = ((GameObject)Instantiate(block.gameObject, Out.position, Quaternion.identity)).GetComponent<Block>();

        newBlock.SourceBlock = block.transform;

        newBlock.transform.parent = _blocksGameObject.transform;
        newBlock.Generate();

        Out = newBlock.Out;
        newBlock.transform.localRotation = Quaternion.Euler(0, 0, Mathf.Ceil(angel*10)/10f);

        if (noCollider)
        {
            newBlock.transform.GetChild(1).GetComponent<BoxCollider2D>().size=new Vector2(0.9f,1f);
            newBlock.transform.GetChild(1).gameObject.layer = 0;
        }
        
        return newBlock;
    }

}
