using Game.Cards;
using UnityEngine;

namespace Game.Inputs
{
    public class SpawnInput
    {
        public Card Card { get; }
        public Vector3 Position { get; }

        public SpawnInput(Card card, Vector3 position)
        {
            Card = card;
            Position = position;
        }
    }
}