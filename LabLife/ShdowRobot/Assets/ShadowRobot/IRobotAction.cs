using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public interface IRobotAction
{
    GameObject robot { set; }
    GameObject robotLight { set; }
}