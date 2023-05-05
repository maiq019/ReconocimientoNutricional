using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Serialization;
using WhateverDevs.Core.Runtime.Common;

namespace ITCL.VisionNutricional.Runtime.Login
{
    public class Session : Singleton<Session>
    {
        public static string Email = null;

        public static string UserName = null;
        
        public static string Passwd = null;
    }
}
