using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace DevelopKit.BasicTemplate
{
    public partial class SaveManager : Manager
    {
        private static string _userDataPath;
        public override ManagerPriority Priority => ManagerPriority.Save;

        public override UniTask StartUp()
        {
            _userDataPath = $"{Application.persistentDataPath}/userdata.json";

            bool isSuccessLoad = LoadUserData();
            if (isSuccessLoad == false)
            {
                _userData = CreateUserData();
                SaveUserData();
            }
            
            GameLifeManager.OnQuitedCallback += SaveUserData;
            return UniTask.CompletedTask;
        }

        public bool FindUserData() => File.Exists(_userDataPath);
        
        private bool DeleteUserData()
        {
            if (FindUserData())
            {
                File.Delete(_userDataPath);
                return true;
            }
            return false;
        }
        
        private UserData CreateUserData()
        {
            UserData userData = new UserData("New User", 0, 0, 0);
            return userData;
        }
    }
}