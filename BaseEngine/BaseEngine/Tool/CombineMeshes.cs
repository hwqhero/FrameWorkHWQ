using UnityEngine;
using System.Collections;

public class CombineMeshes : MonoBehaviour
{

    void Start()
    {

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;

        while (i < meshFilters.Length)
        {

            combine[i].mesh = meshFilters[i].sharedMesh;

            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            meshFilters[i].gameObject.SetActive(false);

            i++;

        }
        if (GetComponent<MeshFilter>() == null)
        {
            gameObject.AddComponent<MeshFilter>();
            gameObject.AddComponent<MeshRenderer>();
        }
        GetComponent<MeshFilter>().mesh = new Mesh();

        GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        gameObject.SetActive(true);
    }


}
