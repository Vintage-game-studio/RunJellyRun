using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

public class AdvanceBlock : Block
{
    private List<Block> _blocks;

    [HideInInspector]
    public string Program = "";

    private List<string> _lines;
    private int _curLine;
    private Stack<Vector3> _posStack = new Stack<Vector3>();
    private Dictionary<string, float> _varDictionary = new Dictionary<string, float>();

    public override void Generate()
    {
        base.Generate();
        Out = transform;

        SourceBlock = SourceBlock == null ? transform : SourceBlock;

        _blocks = new List<Block>(
            (SourceBlock.parent == null ? SourceBlock : SourceBlock.parent).GetComponentsInChildren<Block>()
        );

        _lines = Program.Split('\n', '\r', '\t').Select(l => l.ToLower().Trim()).Where(l => l.Length > 0).ToList();
        _curLine = 0;

        while (_curLine < _lines.Count)
        {
            if (_lines[_curLine][0] == '-')
                RunCommand(_lines[_curLine].Remove(0, 1));
            else if (_lines[_curLine][0] != '/')
                CreateBlock(_lines[_curLine]);
            _curLine++;
        }

    }

    private void CreateBlock(string command)
    {
        List<string> param = command.Split(' ').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

        Block newBlock = GetBlock(param[0]);

        int length = 1;

        float startAngle = 0, endAngle = 0, deltaAngle = 0;

        if (param.Count > 1)
            length = (int) GetValue(param[1]);

        if (param.Count > 2)
            startAngle = GetValue(param[2]);

        if (param.Count > 3)
            endAngle = GetValue(param[3]);
        else
            endAngle = startAngle;

        if (length > 1)
            deltaAngle = (endAngle - startAngle) / (length - 1);

        if (newBlock != null)
            for (int i = 0; i < length; i++)
                CreateBlock(newBlock, startAngle + deltaAngle * i);
    }

    private float GetValue(string s)
    {
        float value = 0;

        if (float.TryParse(s,out value) )
            return value;
        else if (s[0] == '$')
        {
            if (_varDictionary.ContainsKey(s))
                return _varDictionary[s];
            Debug.LogError(" var " + s);
        }
        else if (char.IsLetter(s[0]))
            value = (int) EvaluateFunction(s);

        return value;
    }

    private float EvaluateFunction(string s)
    {
        List<string> paramList = new List<string>();

        #region Get parameters

        int level = 0, lastPos = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '(')
            {
                level++;
                if (level == 1)
                {
                    paramList.Add(s.Substring(lastPos, i - lastPos));
                    lastPos = i + 1;
                }
            }
            if (s[i] == ',')
            {
                if (level == 1)
                {
                    paramList.Add(s.Substring(lastPos, i - lastPos));
                    lastPos = i + 1;
                }
            }
            if (s[i] == ')')
            {
                level--;
                if (level == 0)
                {
                    paramList.Add(s.Substring(lastPos, i - lastPos));
                    break;
                }
            }
        }

        #endregion

        switch (paramList[0])
        {
            case "rand":
                if (paramList.Count == 3)
                    return Random.Range(GetValue(paramList[1]), GetValue(paramList[2]));
                else
                    Debug.LogError("Rand must has 2 params !!!");
                break;
            case "div":
                if (paramList.Count == 3)
                    return GetValue(paramList[1]) / GetValue(paramList[2]);
                else
                    Debug.LogError("Div must has 2 params !!!");
                break;
            case "mul":
                if (paramList.Count == 3)
                    return GetValue(paramList[1]) * GetValue(paramList[2]);
                else
                    Debug.LogError("Div must has 2 params !!!");
                break;
            case "add":
                if (paramList.Count == 3)
                    return GetValue(paramList[1]) + GetValue(paramList[2]);
                else
                    Debug.LogError("Div must has 2 params !!!");
                break;
            case "sub":
                if (paramList.Count == 3)
                    return GetValue(paramList[1]) - GetValue(paramList[2]);
                else
                    Debug.LogError("Div must has 2 params !!!");
                break;
            default:
                Debug.LogError(paramList[0] + " not defined !!!");
                break;

        }

        return 1;
    }

    private Block GetBlock(string name)
    {
        foreach (Block block in _blocks)
        {
            if (block.gameObject.name.ToLower() == name)
                return block;
        }
        Block assetblock = Resources.Load<Block>("Blocks/" + name);

        if (assetblock == null)
            assetblock = Resources.Load<Block>("Module/" + name);

        if (assetblock == null)
            Debug.LogError(gameObject.name + "can't find block " + name + " not found!!");

        return assetblock;
    }

    private void RunCommand(string command)
    {
        List<string> param = command.Split(' ').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();

        int pCount = param.Count;


        switch (param[0])
        {
            case "sp":
                _posStack.Push(Out.position);
                break;

            case "ret":
            case "return":
                Out.position = _posStack.Pop();
                break;

            case "set":
                if (_varDictionary.ContainsKey(param[1]))
                    _varDictionary[param[1]] = GetValue(param[2]);
                else
                    _varDictionary.Add(param[1], GetValue(param[2]));
                break;

            case "rseq":
                float sAngle = pCount > 2 ? GetValue(param[2]) : 0;
                GenerateRandomSequnce(
                    pCount>1 ? (int) GetValue(param[1]) : 1,
                    sAngle,
                    pCount>3 ? GetValue(param[3]) : sAngle
                    );
                break;

            default:
                Debug.LogError(param[0] + " not defined !!!");
                break;
        }
    }

    private void GenerateRandomSequnce(int len,float sAngle,float eAngle)
    {
        List<Block> inBlocks = new List<Block>();

        while (_curLine < _lines.Count)
        {
            _curLine++;
            if (_lines[_curLine][0] == '-')
                break;
            inBlocks.Add(GetBlock(_lines[_curLine]));
        }

        float deltaAngle = len > 1 ? (eAngle - sAngle)/(len - 1) :0;

        for (int i = 0; i < len; i++)
            CreateBlock(inBlocks[Random.Range(0, inBlocks.Count)], sAngle + deltaAngle*i);
    }
}
