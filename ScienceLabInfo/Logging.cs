using System;
using UnityEngine;

namespace ScienceLabInfo
{
    public static class Logging
    {
        private static readonly string PREFIX = "<color=green>[ScienceLabInfo]</color> ";
        private static readonly bool time = false;

        

        public static void Log(String msg, params object[] args)
        {
            Debug.Log(PREFIX +
                (time ? DateTime.Now.ToString("HH:mm:ss.f ") : "") +
                String.Format(msg??"null", args)
                );
        }

        public static void Log(object arg, params object[] args)
        {
            String log = PREFIX +
                (time ? DateTime.Now.ToString("HH:mm:ss.f ") : "") + arg;

            foreach (var a in args) log += ", " + a;

            Debug.Log(log);
        }
    }
}
