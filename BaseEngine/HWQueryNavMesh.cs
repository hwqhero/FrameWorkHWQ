using UnityEngine;
using System.Collections;
using org.critterai.nav.u3d;
using org.critterai.nav;

public class HWQueryNavMesh : MonoBehaviour {

    public ScriptableObject navmeshData;
    public NavGroup mGroup;
    public Transform target;
    private Vector3 lastPosition;
    public PathCorridor pathcorridor;
    public float moveSpeed;
    public float rotationSpeed;
    public Vector3[] temp123;
    public INavmeshData NavmeshData
    {
        get
        {
            return navmeshData ? (INavmeshData)navmeshData : null;
        }
    }
	// Use this for initialization
	void Start () {
        if (navmeshData != null && NavmeshData.HasNavmesh)
        {
            Navmesh navmesh = NavmeshData.GetNavmesh();
            NavmeshQuery query;
            NavStatus status = NavmeshQuery.Create(navmesh, 2048, out query);
            CrowdManager crowd = CrowdManager.Create(1, 0.4f, navmesh);
            mGroup = new NavGroup(navmesh, query, crowd, crowd.QueryFilter, Vector3.one, false);
            pathcorridor = new PathCorridor(2048, 1, query, mGroup.filter);
            NavmeshPoint start;
            mGroup.query.GetNearestPoint(transform.position, mGroup.extents, mGroup.filter, out start);
            pathcorridor.Reset(start);
            ChangeMove(target.position);
           
            lastPosition = target.position;
        }
     
        
	}
	
	// Update is called once per frame
	void Update () {
        if (target.position != lastPosition)
        {
            ChangeMove(target.position);
            lastPosition = target.position;
        }
        Vector3 movePos = Vector3.MoveTowards(transform.position, pathcorridor.Corners.verts[0], Time.deltaTime * moveSpeed);
        if (Vector3.Distance(movePos, transform.position) > float.Epsilon)
        {
            Vector3 temp = (movePos - pathcorridor.Position.point).normalized;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(new Vector3(temp.x, 0, temp.z)), Time.deltaTime * rotationSpeed);
        }
        pathcorridor.MovePosition(movePos);
        transform.position = pathcorridor.Position.point;
        temp123 = pathcorridor.Corners.verts;
	}


    private NavStatus ChangeMove(Vector3 position)
    {
        
        NavmeshPoint result;
        NavStatus ns = mGroup.query.GetNearestPoint(position, mGroup.extents, mGroup.filter, out result);
        if (ns == NavStatus.Sucess)
        {
            pathcorridor.MoveTarget(result.point);
        }
        return ns;
    }
}