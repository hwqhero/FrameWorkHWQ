using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class WeaponTrailHWQ1 : MonoBehaviour
{


    #region Public
    public float height = 2.0f;
    public float time = 2.0f;
    public bool alwaysUp = false;
    public float minDistance = 0.1f;
    public float timeTransitionSpeed = 1f;
    public float desiredTime = 2.0f;
    public Color startColor = Color.white;
    public Color endColor = new Color(0, 0, 0, 0);
    public MeshCollider mc;
    public
    #endregion
        //
    #region Temporary
    Vector3 position;
    float now = 0;
    TronTrailSection currentSection;
    Matrix4x4 localSpaceTransform;
    #endregion

    #region Internal
    private Mesh mesh;
    public Vector3[] vertices;
    public Color[] colors;
    public Vector2[] uv;
    #endregion

    #region Customisers
    private MeshRenderer meshRenderer;
    private Material trailMaterial;
    #endregion
    private List<GameObject> allList = new List<GameObject>();
    private List<TronTrailSection> sections = new List<TronTrailSection>();

    public void Init(bool isMine)
    {
        MeshFilter meshF = GetComponent<MeshFilter>();
        mesh = meshF.mesh;

        mc = GetComponent<MeshCollider>();
        if (mc == null)
        {
            gameObject.AddComponent<MeshCollider>();
        }
        meshRenderer = GetComponent<MeshRenderer>(); 
        trailMaterial = meshRenderer.material;
        Enblaed(false);
        if (isMine)
        {

            mc.isTrigger = true;
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.constraints = RigidbodyConstraints.None;
        }
        else
        {
            DestroyImmediate(mc);
        }
    }

    private void Enblaed(bool enabled)
    {
        if (mc != null)
        {
            mc.enabled = enabled;
        }

    }

    public void SetTrailColor(Color color)
    {
        trailMaterial.SetColor("_TintColor", color);
    }
    public void Itterate(float itterateTime)
    { // ** call everytime you sample animation **

        position = transform.position;
        Enblaed(true);
        now = itterateTime;

        // Add a new trail section
        if (sections.Count == 0 || (sections[0].point - position).sqrMagnitude > minDistance * minDistance)
        {
            TronTrailSection section = new TronTrailSection();
            section.point = position;
            if (alwaysUp)
                section.upDir = Vector3.up;
            else
                section.upDir = transform.TransformDirection(Vector3.up);

            section.time = now;
            sections.Insert(0, section);

        }
    }
    public void UpdateTrail(float currentTime, float deltaTime)
    { // ** call once a frame **

        // Rebuild the mesh	
        mesh.Clear();
        //
        // Remove old sections
        while (sections.Count > 0 && currentTime > sections[sections.Count - 1].time + time)
        {
            sections.RemoveAt(sections.Count - 1);
        }
        // We need at least 2 sections to create the line
        if (sections.Count < 2)
            return;
        //
        vertices = new Vector3[sections.Count * 2];
        colors = new Color[sections.Count * 2];
        uv = new Vector2[sections.Count * 2];
        //
        currentSection = sections[0];
        //
        // Use matrix instead of transform.TransformPoint for performance reasons
        localSpaceTransform = transform.worldToLocalMatrix;

        // Generate vertex, uv and colors
        for (var i = 0; i < sections.Count; i++)
        {
            //
            currentSection = sections[i];
            // Calculate u for texture uv and color interpolation
            float u = 0.0f;
            if (i != 0)
                u = Mathf.Clamp01((currentTime - currentSection.time) / time);
            //
            // Calculate upwards direction
            Vector3 upDir = currentSection.upDir;

            // Generate vertices
            vertices[i * 2 + 0] = localSpaceTransform.MultiplyPoint(currentSection.point);
            vertices[i * 2 + 1] = localSpaceTransform.MultiplyPoint(currentSection.point + upDir * height);

            uv[i * 2 + 0] = new Vector2(u, 0);
            uv[i * 2 + 1] = new Vector2(u, 1);

            // fade colors out over time
            Color interpolatedColor = Color.Lerp(startColor, endColor, u);
            colors[i * 2 + 0] = interpolatedColor;
            colors[i * 2 + 1] = interpolatedColor;
        }

        // Generate triangles indices
        int[] triangles = new int[(sections.Count - 1) * 2 * 3];
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        // Assign to mesh	
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
        //
        // Tween to the desired time
        //
        if (time > desiredTime)
        {
            time -= deltaTime * timeTransitionSpeed;
            if (time <= desiredTime) time = desiredTime;
        }
        else if (time < desiredTime)
        {
            time += deltaTime * timeTransitionSpeed;
            if (time >= desiredTime) time = desiredTime;
        }
        if (mc != null)
            mc.sharedMesh = mesh;
    }
    public void ClearTrail()
    {
        desiredTime = 0;
        time = 0;
        Enblaed(false);

        if (mesh != null)
        {
            mesh.Clear();
            sections.Clear();
        }
    }

    public void ClearAllCollider()
    {
        allList.Clear();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!allList.Contains(other.gameObject))
        {
            allList.Add(other.gameObject);
        }
    }
}


