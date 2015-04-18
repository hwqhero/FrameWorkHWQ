using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, AddComponentMenu("Dynamic Navigation/ NavigationPortal")]
public class NavigationPortal : MonoBehaviour
{
    public bool addlist;
    public float AxisHeight = 1.5f;
    public int cg;
    private bool changerows;
    public bool closestfound;
    public bool constantsearch;
    private int count;
    public int cubex;
    public int cubez;
    public List<Vector3> CurrentConnections;
    public List<int> CurrentIDConnections;
    public int CurrentWaypoint;
    public Vector3 CurrentWaypointVec;
    public bool DetectGridLocation = true;
    public Transform Grid;
    public bool gridfound;
    private int lastway;
    public float MaxDistanceDetection = 5f;
    public Transform PortalConnection;
    public bool PortalOpen;
    private bool swit;
    public float totcube;
    private Vector3 zero;

    private void OnDisable()
    {
        if (this.Grid)
        {
            Grid component = (Grid) this.Grid.GetComponent("Grid");
            if (this.CurrentWaypoint != 0)
            {
                component.PortalConnections[this.CurrentWaypoint].x = 0f;
                component.PortalConnections[this.CurrentWaypoint].y = 0f;
                if (this.PortalConnection)
                {
                    NavigationPortal portal = (NavigationPortal) this.PortalConnection.GetComponent("NavigationPortal");
                    portal.PortalConnection = null;
                }
            }
        }
    }

    private void OnEnable()
    {
        this.CurrentConnections = new List<Vector3>(0);
        this.CurrentIDConnections = new List<int>(0);
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (this.Grid)
        {
            if (this.DetectGridLocation)
            {
                int num2;
                int num6;
                if (this.constantsearch)
                {
                    this.closestfound = false;
                }
                LayerMask mask = 15;
                Grid component = (Grid) this.Grid.GetComponent("Grid");
                if (component && component.WaypointVAR.Length > 0 && this.PortalConnection & this.Grid)
                {
                    if (this.PortalOpen)
                    {
                        component.PortalConnections[this.CurrentWaypoint].y = 1f;
                    }
                    else
                    {
                        component.PortalConnections[this.CurrentWaypoint].y = 0f;
                    }
                    NavigationPortal portal = (NavigationPortal) this.PortalConnection.GetComponent("NavigationPortal");
                    if (!portal.PortalConnection)
                    {
                        portal.PortalConnection = base.transform;
                    }
                    if (portal)
                    {
                        if (this.CurrentWaypoint != this.lastway)
                        {
                            component.PortalConnections[this.lastway].x = 0f;
                            component.PortalConnections[this.lastway].y = 0f;
                            this.lastway = this.CurrentWaypoint;
                        }
                        component.PortalConnections[this.CurrentWaypoint].x = portal.CurrentWaypoint;
                    }
                }
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(base.transform.position, -base.transform.up, out hitInfo))
                {
                }
                if (this.Grid)
                {
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
                if (this.addlist)
                {
                    Grid grid2 = (Grid) this.Grid.GetComponent("Grid");
                    this.CurrentConnections.Clear();
                    this.CurrentIDConnections.Clear();
                    this.CurrentConnections.Add(this.CurrentWaypointVec);
                    num6 = 8;
                    num2 = 0;
                    while (num2 < num6)
                    {
                        if (grid2.ConnectionsVAR[(this.CurrentWaypoint * 8) + num2] != this.zero)
                        {
                            this.CurrentConnections.Add(grid2.ConnectionsVAR[(this.CurrentWaypoint * 8) + num2]);
                            this.CurrentIDConnections.Add(grid2.ConnectionsIDAR[(this.CurrentWaypoint * 8) + num2]);
                        }
                        num2++;
                    }
                    int count = this.CurrentIDConnections.Count;
                    for (int i = 0; i < count; i++)
                    {
                        num2 = 0;
                        while (num2 < num6)
                        {
                            if (grid2.ConnectionsVAR[(this.CurrentIDConnections[i] * 8) + num2] != this.zero)
                            {
                                bool flag2 = true;
                                for (int j = 0; j < count; j++)
                                {
                                    if (grid2.ConnectionsIDAR[(this.CurrentIDConnections[i] * 8) + num2] == this.CurrentIDConnections[j])
                                    {
                                        flag2 = false;
                                    }
                                }
                                if (flag2)
                                {
                                    this.CurrentConnections.Add(grid2.ConnectionsVAR[(this.CurrentIDConnections[i] * 8) + num2]);
                                    this.CurrentIDConnections.Add(grid2.ConnectionsIDAR[(this.CurrentIDConnections[i] * 8) + num2]);
                                }
                            }
                            num2++;
                        }
                    }
                }
                this.addlist = false;
                if (this.CurrentConnections.Count > 0)
                {
                    if ((Vector3.Distance(this.CurrentWaypointVec, base.transform.position) > (component.CubeSize * 1.2f)) | Physics.Linecast(base.transform.position, this.CurrentWaypointVec, (int) mask))
                    {
                        this.closestfound = false;
                    }
                    float num11 = 999999f;
                    Vector3 zero = this.zero;
                    int num12 = 0;
                    num6 = this.CurrentIDConnections.Count;
                    for (num2 = 1; num2 < num6; num2++)
                    {
                        float num5 = Vector3.Distance(this.CurrentConnections[num2], base.transform.position);
                        if (num5 <= num11)
                        {
                            num11 = num5;
                            zero = this.CurrentConnections[num2];
                            num12 = this.CurrentIDConnections[num2];
                        }
                    }
                    if (zero != this.CurrentWaypointVec)
                    {
                        this.CurrentWaypoint = num12;
                        this.CurrentWaypointVec = zero;
                        this.addlist = true;
                    }
                }
            }
            if (this.count < (this.CurrentConnections.Count - 1))
            {
                this.count++;
            }
            else
            {
                this.count = 0;
            }
            if (this.PortalConnection)
            {
                Debug.DrawLine(base.transform.position, this.PortalConnection.position, Color.blue);
            }
            if (this.CurrentWaypointVec != this.zero)
            {
                Debug.DrawLine(base.transform.position, this.CurrentWaypointVec, Color.magenta);
            }
        }
    }
}

