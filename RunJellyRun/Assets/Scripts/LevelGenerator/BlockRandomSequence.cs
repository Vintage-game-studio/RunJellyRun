using UnityEngine;
using System.Collections;

public class BlockRandomSequence : Block
{
    public int MinLenght = 1;
    public int MaxLenght = 5;
    public Block[] Blocks;

    private int[] _chances;
    
    public override void Generate()
    {
        base.Generate();

        _chances = new int[Blocks.Length];

        for (int i = 0; i < Blocks.Length; i++)
            _chances[i] = Blocks[i].Chance;

        Out = transform;

        int Len = Random.Range(MinLenght,MaxLenght);

        for (int i = 0; i < Len; i++)
        {
            Block b = SelectBlock();
            CreateBlock(b);
        }

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

}
