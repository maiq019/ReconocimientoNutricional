using WhateverDevs.Core.Runtime.Common;
using WhateverDevs.SceneManagement.Runtime.SceneManagement;

namespace ITCL.VisionNutricional.Runtime.Login
{
    public class Session : Loggable<Session>
    {
        public static string Email = null;

        public static string UserName = null;
        
        public static string Passwd = null;

        public static SceneReference? PreviousScene = null;
    }
}
