﻿using System;
using UnityEngine;

namespace ScienceLabInfo
{
    public static class Logging
    {
        private static readonly string PREFIX = "<color=green>[ScienceLabInfo]</color> ";
        private static readonly bool time = false;

        public static void Log<T>(T msg, params object[] args)
        {
            Debug.Log(PREFIX +
                (time ? DateTime.Now.ToString("HH:mm:ss.f ") : "") +
                String.Format(msg.ToString(), args)
                );
        }


    }
}
