using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class TronTrailSection
{
    public Vector3 point;
    public Vector3 upDir;
    public float time;
    public TronTrailSection()
    {

    }
    public TronTrailSection(Vector3 p, float t)
    {
        point = p;
        time = t;
    }
}
[RequireComponent(typeof(MeshFilter),typeof(MeshCollider))]
public class WeaponTrail : MonoBehaviour {

    public float height = 2.0f;
    Vector3 position;
    TronTrailSection currentSection;
    Matrix4x4 localSpaceTransform;
    public MeshCollider mc;
    public Bounds BoundsMesh;
    private Mesh mesh;
    private Vector3[] vertices = new Vector3[4];
    private List<TronTrailSection> sections = new List<TronTrailSection>();
    private int[] triangles;
    private void Awake() {

        MeshFilter meshF = GetComponent(typeof(MeshFilter)) as MeshFilter;
        mesh = meshF.mesh;
        mc = GetComponent<MeshCollider>();
        triangles = new int[6];
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;
            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }
        //if (rigidbody == null)
        //{
        //   gameObject.AddComponent<Rigidbody>();
        //}
        //Rigidbody r = rigidbody;
        //r.isKinematic = true;
        //r.useGravity = false;
        //r.constraints = RigidbodyConstraints.FreezeAll;
    }


    public void Itterate(float itterateTime)
    {
        position = transform.position;
        if (sections.Count == 2)
            sections.RemoveAt(1);
        sections.Insert(0, new TronTrailSection() { point = position, upDir = transform.TransformDirection(Vector3.up) });
    }

    public void UpdateTrail(float currentTime, float deltaTime)
    {
        mesh.Clear();
        localSpaceTransform = transform.worldToLocalMatrix;
        for (var i = 0; i < sections.Count; i++)
        {
            currentSection = sections[i];
            vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);
            vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + currentSection.upDir * height);
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mc.sharedMesh = mesh;
        BoundsMesh = mc.bounds;
    }


    public void ClearTrail() {

        if (mesh != null) {
            mesh.Clear();
            sections.Clear();
        }
    }
}


