using System;
using UnityEngine;
using UnityEngine.Serialization;
using WhateverDevs.Core.Runtime.Common;

namespace ITCL.VisionNutricional.Runtime.Login
{
    public class Session : Singleton<Session>
    {
        public static User CurrentSession;
        
        public static string Email;
        
        public static string Passwd;
        
        [Serializable]
        public class User
        {
            [FormerlySerializedAs("check")] public int Check=-1;
            [FormerlySerializedAs("session_code")] public string SessionCode;
        
        }
    }
}
