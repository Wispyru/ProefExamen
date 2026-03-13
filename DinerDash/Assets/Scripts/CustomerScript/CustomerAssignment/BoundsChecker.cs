using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    private GameObject _customerGroup;
    [SerializeField] private float _rayDist = 0.6f;
    private SnapToCollision _snapToCollision;

    private void Start()
    {
        _customerGroup = this.gameObject;
        _snapToCollision = gameObject.GetComponent<SnapToCollision>();
    }
    
    private void Update()
    {
        RaycastHit hitForward;
        RaycastHit hitRight;

        if (Physics.Raycast(_customerGroup.transform.position, -_customerGroup.transform.forward, out hitForward) &&
            hitForward.distance <= _rayDist && hitForward.transform.gameObject.CompareTag("Wall"))
        {
            _snapToCollision.CollisionSnap(hitForward);
        }

        if (Physics.Raycast(_customerGroup.transform.position, _customerGroup.transform.right, out hitRight))
        {
            if (hitRight.distance <= _rayDist && hitRight.transform.gameObject.CompareTag("Wall"))
            {
                _snapToCollision.CollisionSnap(hitRight);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 _endPosX = new Vector3(_customerGroup.transform.position.x + _rayDist,
            _customerGroup.transform.position.y + 1, _customerGroup.transform.position.z);
        Vector3 _endPosZ = new Vector3(_customerGroup.transform.position.x, _customerGroup.transform.position.y + 1,
            _customerGroup.transform.position.z - _rayDist);
        Gizmos.DrawLine(_customerGroup.transform.position, _endPosX);
        Gizmos.DrawLine(_customerGroup.transform.position, _endPosZ);
    }
}
