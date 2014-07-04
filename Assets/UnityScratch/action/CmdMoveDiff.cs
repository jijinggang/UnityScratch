using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityScratch.core;
public class CmdMoveDiff : BaseAction
{
    public Vector3 Pos = Vector3.zero;
    public override void Run()
    {
        _go.transform.position += Pos;
    }
    public override string Id()
    {
        return "MoveDiff";
    }
    public override string ToString()
    {
        return string.Format("移动相对位置({0},{1},{2})", Pos.x, Pos.y, Pos.z);
    }
    public override BaseAction Create()
    {
        return new CmdMoveDiff();
    }
}

