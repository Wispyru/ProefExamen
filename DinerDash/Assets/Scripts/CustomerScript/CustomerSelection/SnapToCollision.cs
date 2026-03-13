using UnityEngine;

public class SnapToCollision : MonoBehaviour
{
    private Transform _rootParent;
    
    private float? _minX, _maxX, _minZ, _maxZ;
    
    [Header("Manual Boundaries")]
    [SerializeField] private bool _useFrontBoundary = false;
    [SerializeField] private float _frontBoundaryZ;
    
    [SerializeField] private bool _useRightBoundary = false;
    [SerializeField] private float _rightBoundaryX;
    
    private bool _boundariesInitialized = false;
    private bool _minXLocked, _maxXLocked, _minZLocked, _maxZLocked;
    
    private Vector3? _pendingSnapX = null;
    private Vector3? _pendingSnapZ = null;

    private void Start()
    {
        _rootParent = transform.root;
    }

    private void LateUpdate()
    {
        if (!_boundariesInitialized)
        {
            if (_useRightBoundary)
            {
                _minX = _rightBoundaryX;
                _minXLocked = true;
            }
            if (_useFrontBoundary)
            {
                _maxZ = _frontBoundaryZ;
                _maxZLocked = true;
            }
            _boundariesInitialized = true;
        }

        Vector3 pos = _rootParent.position;
        
        if (_pendingSnapX.HasValue)
        {
            pos.x = _pendingSnapX.Value.x;
            _pendingSnapX = null;
        }
        if (_pendingSnapZ.HasValue)
        {
            pos.z = _pendingSnapZ.Value.z;
            _pendingSnapZ = null;
        }
        
        if (_minX.HasValue && pos.x < _minX.Value) pos.x = _minX.Value;
        if (_maxX.HasValue && pos.x > _maxX.Value) pos.x = _maxX.Value;
        if (_minZ.HasValue && pos.z < _minZ.Value) pos.z = _minZ.Value;
        if (_maxZ.HasValue && pos.z > _maxZ.Value) pos.z = _maxZ.Value;

        _rootParent.position = pos;
    }

    public void CollisionSnap(RaycastHit hit)
    {
        Vector3 hitNormal = hit.normal;

        Renderer[] renderers = _rootParent.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds combinedBounds = renderers[0].bounds;
        foreach (Renderer r in renderers)
            combinedBounds.Encapsulate(r.bounds);

        float snapOffset = Mathf.Abs(Vector3.Dot(combinedBounds.extents, hitNormal));

        if (Mathf.Abs(hitNormal.x) > 0.5f)
        {
            float snappedX = hit.point.x + hitNormal.x * snapOffset;
            _pendingSnapX = new Vector3(snappedX, 0, 0);

            if (hitNormal.x > 0 && !_minXLocked) _minX = snappedX;
            else if (hitNormal.x < 0 && !_maxXLocked) _maxX = snappedX;
        }
        else if (Mathf.Abs(hitNormal.z) > 0.5f)
        {
            float snappedZ = hit.point.z + hitNormal.z * snapOffset;
            _pendingSnapZ = new Vector3(0, 0, snappedZ);

            if (hitNormal.z > 0 && !_minZLocked) _minZ = snappedZ;
            else if (hitNormal.z < 0 && !_maxZLocked) _maxZ = snappedZ;
        }
    }
    
}