using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// ロボットの動きを創るときに継承
/// </summary>
interface IRobotAction
{
    void Action(List<Human> list_human);
}    

