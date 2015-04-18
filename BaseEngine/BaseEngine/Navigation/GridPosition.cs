using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Navigation/ GridPosition")]
public class GridPosition : MonoBehaviour
{
    private int axisupdate;
    [HideInInspector]
    public int cg;
    private bool changerows;
    private int count;
    private int cubex;
    private int cubez;
    [HideInInspector]
    public List<Vector3> CurrentConnections;
    [HideInInspector]
    public List<int> CurrentIDConnections;
    public int CurrentWaypoint;
    public Vector3 CurrentWaypointVec;
    public bool DetectGrid = true;
    public Grid Grid;
    private bool gridfound;
    public float MaxDistanceDetection = 5f;
    public bool statictarget;
    private bool swit;
    private float totcube;
    public bool UpdateStatic;
    private Vector3 zero;

    private void Start()
    {
        this.axisupdate = 6;
    }

    private void Update()
    {
        this.axisupdate++;
        RaycastHit hitInfo = new RaycastHit();
        if ((this.axisupdate > 0) && Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo))
        {
        }
        if (this.DetectGrid & this.Grid)
        {
            int num2;
            Grid component = this.Grid;
            if ((component.WaypointVectors.Count > 0) && (this.cg == 0))
            {
                this.cubex = (int) Mathf.Abs((float) ((component.GridSearch[0].x - base.transform.position.x) / component.CubeSize));
                this.cubez = (int) Mathf.Abs((float) ((component.GridSearch[0].z - base.transform.position.z) / component.CubeSize));
                if (this.cubex >= (component.GridSize - 2))
                {
                    this.cubex = component.GridSize - 2;
                }
                if (this.cubez >= ((this.cubez * component.GridSize) - 2))
                {
                    this.cubez = (this.cubez * component.GridSize) - 2;
                }
                this.cg = (this.cubex + (this.cubez * component.GridSize)) + 2;
                this.gridfound = true;
            }
            int layers = component.Layers;
            for (num2 = 0; num2 < layers; num2++)
            {
                if (component.IsObstacle[component.GridSearch2[this.cg + (component.GridSearch.Length * num2)]])
                {
                    if (component.IsObstacle[component.GridSearch2[(this.cg + 1) + (component.GridSearch.Length * num2)]])
                    {
                        if (component.IsObstacle[component.GridSearch2[(this.cg - 1) + (component.GridSearch.Length * num2)]])
                        {
                            if (component.IsObstacle[component.GridSearch2[(this.cg + component.GridSize) + (component.GridSearch.Length * num2)]])
                            {
                                if (component.IsObstacle[component.GridSearch2[((this.cg + component.GridSize) + 1) + (component.GridSearch.Length * num2)]])
                                {
                                    if (component.IsObstacle[component.GridSearch2[((this.cg + component.GridSize) - 1) + (component.GridSearch.Length * num2)]])
                                    {
                                        if (component.IsObstacle[component.GridSearch2[(this.cg - component.GridSize) + (component.GridSearch.Length * num2)]])
                                        {
                                            if (component.IsObstacle[component.GridSearch2[((this.cg - component.GridSize) - 1) + (component.GridSearch.Length * num2)]])
                                            {
                                                if (!component.IsObstacle[component.GridSearch2[((this.cg - component.GridSize) + 1) + (component.GridSearch.Length * num2)]])
                                                {
                                                    this.cg = (this.cg - component.GridSize) + 1;
                                                }
                                            }
                                            else
                                            {
                                                this.cg = (this.cg - component.GridSize) - 1;
                                            }
                                        }
                                        else
                                        {
                                            this.cg -= component.GridSize;
                                        }
                                    }
                                    else
                                    {
                                        this.cg = (this.cg + component.GridSize) - 1;
                                    }
                                }
                                else
                                {
                                    this.cg = (this.cg + component.GridSize) + 1;
                                }
                            }
                            else
                            {
                                this.cg += component.GridSize;
                            }
                        }
                        else
                        {
                            this.cg--;
                        }
                    }
                    else
                    {
                        this.cg++;
                    }
                }
            }
            if (this.gridfound & (this.cg != 0))
            {
                float num3 = 999999f;
                for (num2 = 0; num2 < layers; num2++)
                {
                    if ((((this.cg + (component.GridSearch.Length * num2)) >= 0) & ((this.cg + (component.GridSearch.Length * num2)) < component.GridSearch2.Length)) && (component.GridSearch2[this.cg + (component.GridSearch.Length * num2)] != 0))
                    {
                        int num4 = (int) Mathf.Abs((float) (component.WaypointVectors[component.GridSearch2[this.cg + (component.GridSearch.Length * num2)]].y - (hitInfo.point.y + component.StepHeight)));
                        if ((num4 <= num3) && !component.IsObstacle[component.GridSearch2[this.cg + (component.GridSearch.Length * num2)]])
                        {
                            if (false)
                            {
                            }
                            if (Vector3.Distance(base.transform.position, component.WaypointVectors[component.GridSearch2[this.cg + (component.GridSearch.Length * num2)]]) < this.MaxDistanceDetection)
                            {
                                this.CurrentWaypoint = component.GridSearch2[this.cg + (component.GridSearch.Length * num2)];
                                this.CurrentWaypointVec = component.WaypointVectors[component.GridSearch2[this.cg + (component.GridSearch.Length * num2)]];
                                num3 = num4;
                            }
                        }
                    }
                }
            }
            this.cg = 0;
            this.gridfound = false;
            this.cubex = 0;
            this.cubez = 0;
        }
        if (this.statictarget)
        {
            this.DetectGrid = false;
        }
        if (this.count < (this.CurrentConnections.Count - 1))
        {
            this.count++;
        }
        else
        {
            this.count = 0;
        }
        Debug.DrawLine(base.transform.position, this.CurrentWaypointVec, Color.red);
    }
}








