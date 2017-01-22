using UnityEngine;
using System.Collections;

public class BlockSeqence : Block 
{
    public Block[] Blocks;
    public int MinLenght = 0, MaxLength = 0;
    public bool Reverse = false;
    private int _lenght = 0;

    public override void Generate()
    {
        base.Generate();
        Out = transform;

        if (MaxLength == 0)
            _lenght = Blocks.Length;
        else
            _lenght = Random.Range(MinLenght, MaxLength);

        for (int i = 0; i < _lenght; i++)
            CreateBlock(Blocks[i]);

        if (Reverse)
        {
            for (int i = _lenght-1; i >=0; i--)
                CreateBlock(Blocks[i]);
            
        }

    }
}
