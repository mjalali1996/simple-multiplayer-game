using Common;
using Newtonsoft.Json;
using UnityEngine;

namespace Definitions
{
    [CreateAssetMenu(menuName = "Definitions/User Wallet", fileName = "UserWallet", order = 0)]
    public class UserWallet : ScriptableSingleton<UserWallet>
    {
        [JsonProperty("wins")] [SerializeField] private int _wins;
        public int Wins => _wins;

        public void Dispatch(string data)
        {
            JsonConvert.PopulateObject(data, this);
        }
    }
}