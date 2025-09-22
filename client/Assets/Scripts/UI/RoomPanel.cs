using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomPanel : MonoBehaviour
    {
        [SerializeField] private InputField roomCodeInput;
        [SerializeField] private Button createRoomButton;
        [SerializeField] private Button joinRoomButton;
        [SerializeField] private Text statusText;

        private void Start()
        {
            createRoomButton.onClick.AddListener(OnCreateRoom);
            joinRoomButton.onClick.AddListener(OnJoinRoom);
        }

        private void OnCreateRoom()
        {
            // Logic to create a room and display the room code
            string roomCode = GenerateRoomCode();
            statusText.text = "Room created! Code: " + roomCode;
        }

        private void OnJoinRoom()
        {
            string roomCode = roomCodeInput.text;
            // Logic to join a room using the provided room code
            statusText.text = "Joining room: " + roomCode;
        }

        private string GenerateRoomCode()
        {
            // Generate a random room code (for simplicity, using a fixed code here)
            return "ABCD";
        }
    }
}