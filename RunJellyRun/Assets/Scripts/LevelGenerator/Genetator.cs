using UnityEngine;
using System.Collections;

public class Genetator : MonoBehaviour
{
    public int Length = 100;
    public Transform StartupPoint;
    public Block[] Blocks;

    private int[] _chances;
    private Vector3 _nextBlockPosition;
    public void Generate()
    {
        Initialize();
        for (int i = 0; i < Length; i++)
        {
            Block b = SelectBlock();
            CreateBlock(b);
            //UpdateChances(b);
        }
    }

    private void CreateBlock(Block block)
    {
        Block newBlock = ((GameObject) Instantiate( block.gameObject, _nextBlockPosition,Quaternion.identity)).GetComponent<Block>();
        _nextBlockPosition = newBlock.Out.position;
    }

    private Block SelectBlock()
    {
        int sum = 0;

        for (int i = 0; i < _chances.Length; i++)
            sum += _chances[i];

        int rNumber = Random.Range(0, sum);

        sum = 0;
        for (int i = 0; i < _chances.Length; i++)
        {
            sum += _chances[i];
            if (sum > rNumber)
                return Blocks[i];
        }

        return Blocks[Blocks.Length - 1];
    }

    private void Initialize()
    {
        
        _chances = new int[Blocks.Length];

        for (int i = 0; i < Blocks.Length; i++)
            _chances[i] = Blocks[i].Chance;

        _nextBlockPosition = StartupPoint.position;
    }
}
