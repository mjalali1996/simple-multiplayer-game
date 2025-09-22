using Game.Entities.Tower;
using UnityEngine;

namespace Game.Arena
{
    public interface IZone
    {
        public int Id { get; }
        public Tower Tower { get; }
        public Bounds Bounds { get; }
        public LayerMask LayerMask { get; }
        
        public Material Material { get; }
        void SetCamera(Camera camera);
    }
}