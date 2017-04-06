using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace YH
{
    public class SimpleEditorCoroutine
    {
        public static SimpleEditorCoroutine start(IEnumerator _routine)
        {
            SimpleEditorCoroutine coroutine = new SimpleEditorCoroutine(_routine);
            coroutine.start();
            return coroutine;
        }

        readonly IEnumerator routine;
        SimpleEditorCoroutine(IEnumerator _routine)
        {
            routine = _routine;
        }

        void start()
        {
            Debug.Log("start");
            EditorApplication.update += update;
        }
        public void stop()
        {
            Debug.Log("stop");
            EditorApplication.update -= update;
        }

        void update()
        {
            /* NOTE: no need to try/catch MoveNext,
			 * if an IEnumerator throws its next iteration returns false.
			 * Also, Unity probably catches when calling EditorApplication.update.
			 */

            Debug.Log("update:"+Time.frameCount);
            if (routine.Current != null)
            {
                Debug.Log(routine.Current.GetType()+":"+Time.frameCount);

                if (routine.Current is WWW)
                {
                    WWW www = routine.Current as WWW;
                    if (www.isDone)
                    {
                        if (!routine.MoveNext())
                        {
                            stop();
                        }
                    }
                    return;
                }
            }

            if (!routine.MoveNext())
            {
                stop();
            }
            //if (!routine.MoveNext())
            //{
            //    stop();
            //}
        }

    }
}