using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;

namespace FPSAdjuster
{
    public class FPSAdjuster
    {
        public int Fps { get; set; }

        // 直前の時間
        private double _lastTick;
        private Stopwatch _watch;
        
        public void Start()
        {
            _watch = new Stopwatch();
            _lastTick = 0;
            _watch.Start();
        }

        public void Adjust()
        {
            // 重みを求める(1000 / fpsを求める)
            double weight = 1000.0 / Fps;
            double timeNow = _watch.Elapsed.TotalMilliseconds;

            while (timeNow < _lastTick + weight)
            {
                // 気休め程度に待つ、ここは適した処理を書く
                System.Threading.Thread.Sleep(0);
                timeNow = _watch.Elapsed.TotalMilliseconds;
            }
            _lastTick = timeNow;
        }
    }
}
