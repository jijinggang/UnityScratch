using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityScratch.core;
public class CmdLoop : BaseAction
{
    public int Times = 1;
    public override void Run()
    {
        
    }
    public override string ToString()
    {
        return string.Format("循环执行{0}次", Times);
    }
    public override string Id()
    {
        return "Loop";
    }
    public override BaseAction Create()
    {
        return new CmdLoop();
    }
}

