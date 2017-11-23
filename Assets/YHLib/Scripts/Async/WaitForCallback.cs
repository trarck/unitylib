using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace YH
{ 
    public class WaitForCallback<T> : CustomYieldInstruction
    {
        private bool done;
        public T Result { get; private set; }

        public WaitForCallback(Action<Action<T>> callCallback)
        {
            callCallback(r =>
            {
                Result = r;
                done = true;
            });
        }

        public override bool keepWaiting
        {
            get
            {
                return done == false;
            }
        }
    }
}