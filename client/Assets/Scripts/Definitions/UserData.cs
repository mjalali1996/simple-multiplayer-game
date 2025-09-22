using Common;
using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(menuName = "Definitions/User Data", fileName = "UserData", order = 0)]
    public class UserData : ScriptableSingleton<UserData>
    {
        [SerializeField] private string _userId;
        public string UserId => _userId;

        [SerializeField] private string _userName;
        public string UserName => _userName;

        [SerializeField] private int _avatarId;
        public int AvatarId => _avatarId;

        public void Set(string userId, string userName, int avatarId)
        {
            _userId = userId;
            _userName = userName;
            _avatarId = avatarId;
        }
    }
}