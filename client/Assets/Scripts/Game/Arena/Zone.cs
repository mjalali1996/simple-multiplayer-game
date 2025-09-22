using Game.Entities.Tower;
using UnityEngine;

namespace Game.Arena
{
    public class Zone : MonoBehaviour, IZone
    {
        [SerializeField] private int _zoneId;
        public int Id => _zoneId;

        [SerializeField] private Tower _tower;
        public Tower Tower => _tower;

        [SerializeField] private MeshRenderer _renderer;
        public Bounds Bounds => _renderer.bounds;

        [SerializeField] private LayerMask _layerMask;
        public LayerMask LayerMask => _layerMask;

        [SerializeField] private Material _material;
        public Material Material => _material;

        [SerializeField] private Transform _cameraPivot;

        public void SetCamera(Camera camera)
        {
            camera.transform.position = _cameraPivot.position;
            camera.transform.rotation = _cameraPivot.rotation;
        }
    }
}