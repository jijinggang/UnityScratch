using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityScratch.core;
public class CmdMove : BaseAction
{
    public Vector3 Pos = Vector3.zero;
    public override void Run()
    {
        _go.transform.position = Pos;
    }
    public override string Id()
    {
        return "Move";
    }
    public override string ToString()
    {
        return string.Format("移动到({0},{1},{2})", Pos.x, Pos.y, Pos.z);
    }
    public override BaseAction Create()
    {
        return new CmdMove();
    }
}

