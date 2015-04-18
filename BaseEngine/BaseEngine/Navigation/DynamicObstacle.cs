using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Navigation/ Dynamic Obstacle"), ExecuteInEditMode]
public class DynamicObstacle : MonoBehaviour
{
    [HideInInspector]
    public int cg;
    public bool Clear;
    [HideInInspector]
    public List<int> CurrentGridUpdate;
    public bool DebugsActive;
    private int fcount;
    public GameObject Grid;
    private Vector3 lastposition;
    private Vector3 lastrotation;
    private Vector3 lastscale;
    public bool Paint;
    public bool updategrid;

    private void OnDisable()
    {
        int count = this.CurrentGridUpdate.Count;
        if (this.Grid)
        {
            Grid component = (Grid) this.Grid.GetComponent("Grid");
            for (int i = 0; i < count; i++)
            {
                component.IsObstacle[this.CurrentGridUpdate[i]] = false;
            }
            this.CurrentGridUpdate.Clear();
        }
    }

    private void OnEnable()
    {
        this.CurrentGridUpdate = new List<int>(0);
    }

    private void Start()
    {
    }

    private void Update()
    {
        Grid component;
        int num10;
        if (this.Grid!=null)
        {
            if (((base.transform.eulerAngles != this.lastrotation) || (base.transform.position != this.lastposition)) || (base.transform.localScale != this.lastscale))
            {
                this.updategrid = true;
                this.lastposition = base.transform.position;
                this.lastrotation = base.transform.eulerAngles;
                this.lastscale = base.transform.localScale;
            }
            if (this.fcount > 5)
            {
                this.updategrid = false;
                this.fcount = 0;
            }
            if (this.updategrid)
            {
                int num6;
                this.fcount++;
                if (this.DebugsActive)
                {
                    Debug.Log("Updating grid and Navigators..");
                }
                component = (Grid) this.Grid.GetComponent("Grid");
                component.Updatestatic++;
                LayerMask mask = ~(component.IgnoreCollisionLayerMask);
                int num = (int) Mathf.Abs((float) ((component.GridSearch[0].x - base.transform.position.x) / component.CubeSize));
                int num2 = (int) Mathf.Abs((float) ((component.GridSearch[0].z - base.transform.position.z) / component.CubeSize));
                this.cg = (num + (num2 * component.GridSize)) + 1;
                int layers = component.Layers;
                float num4 = 999999f;
                int cg = this.cg;
                for (num6 = 0; num6 < layers; num6++)
                {
                    if (((this.cg + (component.GridSearch.Length * num6)) >= 0) & ((this.cg + (component.GridSearch.Length * num6)) < component.GridSearch2.Length))
                    {
                        float num7 = Vector3.Distance(component.WaypointVAR[component.GridSearch2[this.cg + (component.GridSearch.Length * num6)]], base.transform.position);
                        if (num7 < num4)
                        {
                            cg = component.GridSearch2[this.cg + (component.GridSearch.Length * num6)];
                            if (!component.IsObstacle[cg])
                            {
                                num4 = num7;
                            }
                        }
                    }
                }
                if (cg < component.WaypointVAR.Length)
                {
                    if (!component.IsObstacle[cg])
                    {
                        this.CurrentGridUpdate.Add(cg);
                    }
                    int count = this.CurrentGridUpdate.Count;
                    for (num6 = 0; num6 < count; num6++)
                    {
                        if ((this.CurrentGridUpdate[num6] < component.WaypointVectors.Count) & (this.CurrentGridUpdate[num6] >= 0))
                        {
                            Vector3 end = component.WaypointVectors[this.CurrentGridUpdate[num6]];
                            end.y += component.CubeSize;
                            Vector3 start = component.WaypointVectors[this.CurrentGridUpdate[num6]];
                            start.y = end.y + (component.StepHeight * 1.5f);
                            count = this.CurrentGridUpdate.Count;
                            if (Physics.CheckCapsule(start, end, component.CapsuleSize, (int) mask))
                            {
                                int num9 = 8;
                                num10 = 0;
                                while (num10 < num9)
                                {
                                    if (!component.IsObstacle[component.ConnectionsIDAR[(this.CurrentGridUpdate[num6] * 8) + num10]])
                                    {
                                        Vector3 vector3 = component.WaypointVectors[component.ConnectionsIDAR[(this.CurrentGridUpdate[num6] * 8) + num10]];
                                        vector3.y = end.y + component.CubeSize;
                                        Vector3 vector4 = component.WaypointVectors[component.ConnectionsIDAR[(this.CurrentGridUpdate[num6] * 8) + num10]];
                                        vector4.y = end.y + (component.StepHeight * 1.5f);
                                        if (Physics.CheckCapsule(vector4, vector3, component.CapsuleSize, (int) mask))
                                        {
                                            component.IsObstacle[component.ConnectionsIDAR[(this.CurrentGridUpdate[num6] * 8) + num10]] = true;
                                            this.CurrentGridUpdate.Add(component.ConnectionsIDAR[(this.CurrentGridUpdate[num6] * 8) + num10]);
                                        }
                                    }
                                    num10++;
                                }
                            }
                            else if (!this.Paint)
                            {
                                component.IsObstacle[this.CurrentGridUpdate[num6]] = false;
                                component.IsObstacle[this.CurrentGridUpdate[num6]] = false;
                                this.CurrentGridUpdate.Remove(this.CurrentGridUpdate[num6]);
                                count = this.CurrentGridUpdate.Count;
                            }
                        }
                    }
                }
            }
        }
        if (this.Clear)
        {
            int num11 = this.CurrentGridUpdate.Count;
            component = (Grid) this.Grid.GetComponent("Grid");
            for (num10 = 0; num10 < num11; num10++)
            {
                component.IsObstacle[this.CurrentGridUpdate[num10]] = false;
            }
            this.CurrentGridUpdate.Clear();
            this.Clear = false;
        }
    }
}

