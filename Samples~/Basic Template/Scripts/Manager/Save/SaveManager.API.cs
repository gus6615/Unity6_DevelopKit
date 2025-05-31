using System;
using System.IO;
using UnityEngine;

namespace DevelopKit.BasicTemplate
{
    public partial class SaveManager
    {
        private UserData _userData;
        public UserData UserData => _userData;
        
        public void SaveUserData()
        {
            Debug.Log($"{DateTime.Now} | 유저 데이터 저장 성공");
            string encrypted = JsonUtility.ToJson(_userData, true);
            File.WriteAllText(_userDataPath, encrypted);
        }

        public bool LoadUserData()
        {
            if (FindUserData() == false)
            {
                Debug.Log("UserData 데이터 파일을 찾지 못했습니다. 새로운 유저 데이터를 생성합니다.");
                return false;
            } 
            
            Debug.Log($"{DateTime.Now} | 유저 데이터 로드 성공");
            
            string json = File.ReadAllText(_userDataPath);
            _userData = JsonUtility.FromJson<UserData>(json);
            return true;
        }

        public void ResetData()
        {
            if (FindUserData() == false)
            {
                DeleteUserData();
            }
            
            Debug.Log($"{DateTime.Now} | 유저 데이터 리셋 성공");
            
            _userData = CreateUserData();
            SaveUserData();
        }
    }
}