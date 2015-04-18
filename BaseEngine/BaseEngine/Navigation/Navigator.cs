using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Navigation/ Navigator")]
public class Navigator : MonoBehaviour
{
    private bool added;
    public float ChangeNodeDist = 2f;

    public List<int> choicedist;

    public List<Vector4> choicesvec;
    [HideInInspector]
    public bool clearpath;
    private int counter = -1;
    private bool currentpass;

    public Vector3 CurrentTargetWaypoint;

    public int CurrentWaypoint;
    public bool DebugsActive;
    public bool DetectStaticTarget = true;

    public float[] Distances;

    public float[] Distancesnew;

    public float[] Distancespass;
    [HideInInspector]
    public bool drawpath;
    private int dyobcount;
    private int fcount;
    public bool FindPath;
    private bool found;
    private int foundcount;
    [HideInInspector]
    public int GoalWaypoint;
    private bool gosec;
    public Grid Grid;
    public int IntitalPathSpeed = 8;
    private int lastgupdate;
    private Transform lasttarg;
    private int lastw;
    private int lastway;
    public int Maxdist = 0x3e8;
    public int MaxDrawDist = 40;
    public bool NavigateTarget;
    private bool newtarg;

    public List<int> path;
    private bool pathhasfailed;
    public int PathSpeed = 2;
    public int RetreatDistance = 20;
    public bool RetreatTarget;
    public bool ShowPath;
    public bool SlowToStop;
    public float SlowToStopDist = 1f;
    public bool SmoothPath = true;
    public int StaticPathSpeed = 1;
    public bool StopIfFinalNodeReached = true;
    public Transform Target;
    private int targsave2;
    private int targsavecount;

    public List<int> temp;

    public List<float> temp2;

    public List<int> temp3;
    private float timer;

    public List<Vector3> vpath;
    private Vector3 zer;

    private void Start()
    {
    }

    private void Update()
    {
        if (this.Grid)
        {
            int num3;
            int num5;
            int currentWaypoint;
            int num7;
            int num10;
            GridPosition position2;
            Grid component = this.Grid;
            int count = component.WaypointVectors.Count;
            if (this.clearpath)
            {
                this.path.Clear();
                this.vpath.Clear();
                this.choicesvec.Clear();
                this.choicedist.Clear();
                this.temp.Clear();
                this.temp2.Clear();
                this.temp3.Clear();
                this.counter = -1;
                this.added = false;
                this.found = false;
                this.foundcount = 0;
                if (!this.newtarg)
                {
                    this.drawpath = true;
                    this.FindPath = false;
                }
                this.clearpath = false;
            }
            if ((this.FindPath & this.Target) & this.NavigateTarget)
            {
                int retreatDistance = 2;
                if (this.RetreatTarget)
                {
                    retreatDistance = this.RetreatDistance;
                }
                if (!this.added)
                {
                    GridPosition position = (GridPosition)this.Target.GetComponent("GridPosition");
                    this.GoalWaypoint = position.CurrentWaypoint;
                    this.Distancespass = new float[count];
                    this.Distancespass[this.GoalWaypoint] = 1f;
                    this.temp2.Add(1f);
                    this.temp3.Add(this.GoalWaypoint);
                    this.added = true;
                }
                if (this.added)
                {
                    num3 = 8;
                    int pathSpeed = 0;
                    if (this.pathhasfailed)
                    {
                        pathSpeed = this.PathSpeed;
                    }
                    else if (this.path.Count <= 0)
                    {
                        pathSpeed = this.IntitalPathSpeed;
                    }
                    else
                    {
                        pathSpeed = this.PathSpeed;
                    }
                    num5 = 0;
                    while (num5 < pathSpeed)
                    {
                        if (!(this.Distancespass[this.CurrentWaypoint] == 0f))
                        {
                            this.found = true;
                        }
                        if (this.found)
                        {
                            this.foundcount++;
                        }
                        if (this.foundcount < retreatDistance)
                        {
                            this.counter++;
                            currentWaypoint = this.temp2.Count;
                            num7 = 0;
                            while (num7 < currentWaypoint)
                            {
                                if (this.temp2[num7] > this.counter)
                                {
                                    this.temp.Add(this.temp3[num7]);
                                }
                                num7++;
                            }
                            this.temp2.Clear();
                            this.temp3.Clear();
                            int num8 = this.temp.Count;
                            for (int i = 0; i < num8; i++)
                            {
                                if (!component.IsObstacle[this.temp[i]])
                                {
                                    num10 = 0;
                                    while (num10 < num3)
                                    {
                                        if (component.ConnectionsIDAR[(this.temp[i] * 8) + num10] != 0)
                                        {
                                            float num11 = 10f;
                                            if (num10 >= 4)
                                            {
                                                num11 = 14f;
                                            }
                                            if ((this.Distancespass[component.ConnectionsIDAR[(this.temp[i] * 8) + num10]] == 0f) | (this.Distancespass[component.ConnectionsIDAR[(this.temp[i] * 8) + num10]] > (this.Distancespass[this.temp[i]] + num11)))
                                            {
                                                if (component.IsObstacle[component.ConnectionsIDAR[(this.temp[i] * 8) + num10]])
                                                {
                                                    num11 = 500f;
                                                }
                                                this.Distancespass[component.ConnectionsIDAR[(this.temp[i] * 8) + num10]] = this.Distancespass[this.temp[i]] + num11;
                                                if (this.DebugsActive)
                                                {
                                                    Debug.DrawLine(component.WaypointVAR[this.temp[i]], component.ConnectionsVAR[(this.temp[i] * 8) + num10], Color.blue);
                                                }
                                                this.temp2.Add(this.Distancespass[this.temp[i]] + num11);
                                                this.temp3.Add(component.ConnectionsIDAR[(this.temp[i] * 8) + num10]);
                                            }
                                        }
                                        num10++;
                                    }
                                    if (component.PortalConnections[this.temp[i]].x != 0f)
                                    {
                                        int x = (int)component.PortalConnections[this.temp[i]].x;
                                        if ((this.Distancespass[x] == 0f) && (component.PortalConnections[x].y == 1f))
                                        {
                                            this.Distancespass[x] = this.Distancespass[this.temp[i]] + 1f;
                                            this.temp2.Add(this.Distancespass[this.temp[i]] + 1f);
                                            this.temp3.Add(x);
                                        }
                                    }
                                }
                            }
                        }
                        if (this.temp.Count <= 0)
                        {
                            this.found = true;
                        }
                        this.temp.Clear();
                        num5++;
                    }
                    if ((this.foundcount > retreatDistance) | (this.counter > this.Maxdist))
                    {
                        if ((this.counter > this.Maxdist) | (this.Distancespass[this.CurrentWaypoint] == 0f))
                        {
                            this.clearpath = true;
                            if (this.DebugsActive & (this.counter > this.Maxdist))
                            {
                                Debug.Log("Max Path Range Exceeded.. To Increase Path Range, Increase Var 'MaxDist' In Navigator.");
                            }
                            if (this.DebugsActive & (this.foundcount > retreatDistance))
                            {
                                Debug.Log("Path Is Blocked..");
                            }
                            this.pathhasfailed = true;
                        }
                        else
                        {
                            this.pathhasfailed = false;
                        }
                        this.temp.Clear();
                        this.temp2.Clear();
                        this.temp3.Clear();
                        this.Distances = this.Distancespass;
                        this.counter = -1;
                        this.added = false;
                        this.found = false;
                        this.foundcount = 0;
                        this.drawpath = true;
                        this.FindPath = false;
                    }
                    this.newtarg = false;
                }
            }
            if (this.drawpath & this.NavigateTarget)
            {
                if (this.path.Count > 2)
                {
                    position2 = (GridPosition)base.GetComponent("GridPosition");
                    currentWaypoint = position2.CurrentWaypoint;
                    if (this.Distances[this.path[0]] >= this.Distances[currentWaypoint])
                    {
                        this.CurrentWaypoint = position2.CurrentWaypoint;
                    }
                    else
                    {
                        this.CurrentWaypoint = position2.CurrentWaypoint;
                    }
                }
                else
                {
                    position2 = (GridPosition)base.GetComponent("GridPosition");
                    this.CurrentWaypoint = position2.CurrentWaypoint;
                }
                if (this.path.Count > 0)
                {
                    this.path.Clear();
                    this.vpath.Clear();
                }
                this.path.Add(this.CurrentWaypoint);
                this.vpath.Add(component.WaypointVAR[this.CurrentWaypoint]);
                float maxDrawDist = this.Distances[this.CurrentWaypoint];
                if (maxDrawDist > this.MaxDrawDist)
                {
                    maxDrawDist = this.MaxDrawDist;
                }
                for (int j = 0; j < maxDrawDist; j++)
                {
                    num3 = 8;
                    int index = 0;
                    Vector3 zer = this.zer;
                    float num16 = 9999999f;
                    float num17 = 9999999f;
                    if (component.PortalConnections[this.path[this.path.Count - 1]].y == 1f)
                    {
                        int num18 = (int)component.PortalConnections[this.path[this.path.Count - 1]].x;
                        if ((this.Distances[num18] != 0f) && (this.Distances[num18] < num16))
                        {
                            index = num18;
                            zer = component.WaypointVAR[num18];
                            num16 = this.Distances[num18];
                        }
                    }
                    if (this.RetreatTarget)
                    {
                        num16 = 0f;
                        num17 = 0f;
                    }
                    num10 = 0;
                    while (num10 < num3)
                    {
                        bool flag2 = false;
                        if ((component.ConnectionsVAR[this.path[this.path.Count - 1] * 8] != this.zer) && (this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]] != 0f))
                        {
                            if (this.RetreatTarget)
                            {
                                if (!component.IsObstacle[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]])
                                {
                                    if (this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]] >= num16)
                                    {
                                        if (this.path.Count > 0)
                                        {
                                            num5 = 0;
                                            while (num5 < this.path.Count)
                                            {
                                                if (component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10] == this.path[num5])
                                                {
                                                    flag2 = true;
                                                }
                                                num5++;
                                            }
                                        }
                                        if (flag2)
                                        {
                                            num16 = this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]];
                                        }
                                        else
                                        {
                                            index = component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10];
                                            zer = component.WaypointVAR[index];
                                            num16 = this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]];
                                        }
                                    }
                                    else if (this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]] >= num16)
                                    {
                                        if (this.path.Count > 0)
                                        {
                                            for (num5 = 0; num5 < this.path.Count; num5++)
                                            {
                                                if (component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10] == this.path[num5])
                                                {
                                                    flag2 = true;
                                                }
                                            }
                                        }
                                        if (!flag2)
                                        {
                                            index = component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10];
                                            zer = component.WaypointVAR[index];
                                            num16 = this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]];
                                        }
                                    }
                                }
                            }
                            else if (!component.IsObstacle[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]])
                            {
                                float num19 = 0f;
                                if (this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]] < num16)
                                {
                                    if (this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]] == num16)
                                    {
                                        float num20 = Vector3.Distance(this.Target.position, component.ConnectionsVAR[(this.path[this.path.Count - 1] * 8) + num10]);
                                        if ((num19 + num20) < num17)
                                        {
                                            index = component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10];
                                            zer = component.WaypointVAR[index];
                                            num17 = num19 + num20;
                                            num16 = this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]];
                                        }
                                    }
                                    else
                                    {
                                        index = component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10];
                                        zer = component.WaypointVAR[index];
                                        num16 = this.Distances[component.ConnectionsIDAR[(this.path[this.path.Count - 1] * 8) + num10]];
                                    }
                                }
                            }
                        }
                        num10++;
                    }
                    if (index != 0)
                    {
                        num16 = 999999f;
                        this.path.Add(index);
                        this.vpath.Add(zer);
                        if ((((this.SmoothPath & (this.path.Count > 1)) & (this.path[this.path.Count - 2] != this.CurrentWaypoint)) & (this.path[this.path.Count - 2] != this.GoalWaypoint)) && ((this.path[this.path.Count - 2] != this.CurrentWaypoint) & (this.path[this.path.Count - 2] != this.GoalWaypoint)))
                        {
                            int num21 = 8;
                            int num22 = 0;
                            for (num10 = 0; num10 < num21; num10++)
                            {
                                if (!(component.IsObstacle[component.ConnectionsIDAR[(this.path[this.path.Count - 2] * 8) + num10]] | (component.PortalConnections[component.ConnectionsIDAR[(this.path[this.path.Count - 2] * 8) + num10]].y == 1f)))
                                {
                                    num22++;
                                }
                            }
                            if (num22 == 8)
                            {
                                this.path.Remove(this.path[this.path.Count - 2]);
                                this.vpath.Remove(this.vpath[this.vpath.Count - 2]);
                            }
                        }
                    }
                }
                if (this.path.Count > 2)
                {
                    this.path.Remove(this.path[0]);
                    this.vpath.Remove(this.vpath[0]);
                    if (this.path.Count > 3)
                    {
                        this.path.Remove(this.path[0]);
                        this.vpath.Remove(this.vpath[0]);
                    }
                }
                if (this.path.Count > 4)
                {
                    this.drawpath = false;
                }
            }
            if (this.path.Count > 1)
            {
                this.timer += Time.deltaTime;
                if ((this.timer > 0.6f) & this.Target)
                {
                    LayerMask mask = 15;
                    RaycastHit hitInfo = new RaycastHit();
                    Vector3 start = base.transform.position;
                    start.y = component.WaypointVAR[this.CurrentWaypoint].y;
                    if (Physics.Linecast(start, this.vpath[0], out hitInfo, (int)mask))
                    {
                        if (this.timer > 1f)
                        {
                            if (hitInfo.transform == this.Target.transform)
                            {
                                this.timer = 0f;
                            }
                            else
                            {
                                this.clearpath = true;
                                this.temp.Clear();
                                this.temp2.Clear();
                                this.temp3.Clear();
                                this.SlowToStop = true;
                                this.FindPath = true;
                                if (this.DebugsActive)
                                {
                                    Debug.Log("Path Lost, Creating new..");
                                }
                                this.timer = 0f;
                            }
                        }
                    }
                    else
                    {
                        this.timer = 0f;
                    }
                }
            }
            int num23 = 1;
            if (this.path.Count <= num23)
            {
                this.SlowToStop = true;
            }
            else
            {
                this.SlowToStop = false;
            }
            if ((this.NavigateTarget & this.Target) | this.RetreatTarget)
            {
                position2 = (GridPosition)base.GetComponent("GridPosition");
                this.CurrentWaypoint = position2.CurrentWaypoint;
                if ((this.path.Count >= 2) && (this.path[0] == position2.CurrentWaypoint))
                {
                    this.path.Remove(this.path[0]);
                    this.vpath.Remove(this.vpath[0]);
                }
                GridPosition position3 = (GridPosition)this.Target.GetComponent("GridPosition");
                if (position3)
                {
                    bool flag3 = false;
                    if (position3.statictarget & (position3.CurrentWaypoint != this.lastw))
                    {
                        flag3 = true;
                    }
                    if ((this.lasttarg != this.Target) | flag3)
                    {
                        this.FindPath = true;
                        this.drawpath = false;
                        this.pathhasfailed = false;
                        this.SlowToStop = true;
                        if (this.path.Count > 0)
                        {
                            this.clearpath = true;
                        }
                        this.lasttarg = this.Target;
                        this.lastw = position3.CurrentWaypoint;
                        this.newtarg = true;
                    }
                    else if (position3.statictarget)
                    {
                        if ((this.path.Count < 3) & (this.path.Count > 0))
                        {
                            this.drawpath = true;
                        }
                    }
                    else
                    {
                        this.FindPath = true;
                    }
                }
                if ((this.path.Count < 1) & this.NavigateTarget)
                {
                    this.SlowToStop = true;
                    this.FindPath = true;
                }
            }
            if ((this.path.Count < 5) & (this.path.Count > 0))
            {
                this.drawpath = true;
            }
            bool flag4 = false;
            if ((this.path.Count > 0) && (component.Updatestatic != this.lastgupdate))
            {
                flag4 = true;
                this.lastgupdate = component.Updatestatic;
            }
            if (flag4)
            {
                this.FindPath = true;
            }
            else if (this.DetectStaticTarget)
            {
                if (this.clearpath)
                {
                    this.lastway = -9999;
                }
                this.FindPath = true;
                if ((this.Target & this.NavigateTarget) & (this.counter <= 0))
                {
                    GridPosition position4 = (GridPosition)this.Target.GetComponent("GridPosition");
                    if (position4)
                    {
                        if (position4.CurrentWaypoint != this.lastway)
                        {
                            this.lastway = position4.CurrentWaypoint;
                        }
                        else if (this.path.Count > 1)
                        {
                            this.FindPath = false;
                        }
                    }
                }
            }
            if (this.path.Count > 0)
            {
                float num24 = Mathf.Abs((float)(this.vpath[0].x - base.transform.position.x));
                float num25 = Mathf.Abs((float)(this.vpath[0].z - base.transform.position.z));
                float num26 = Mathf.Abs((float)(this.vpath[0].y - base.transform.position.y));
                this.CurrentTargetWaypoint = this.vpath[0];
                if (((num24 < this.ChangeNodeDist) & (num25 < this.ChangeNodeDist)) & (num26 < (component.LayerHeight + component.StepHeight)))
                {
                    if (this.StopIfFinalNodeReached && (this.path.Count < 4))
                    {
                        this.SlowToStop = true;
                    }
                    this.path.Remove(this.path[0]);
                    this.vpath.Remove(this.vpath[0]);
                    if (this.SmoothPath & (this.path.Count > 2))
                    {
                        Vector3 temp = base.transform.position;
                        temp.y = component.WaypointVAR[this.CurrentWaypoint].y;
                        base.transform.position = temp;
                    }
                }
            }
            if (this.ShowPath && (this.vpath.Count > 1))
            {
                Debug.DrawLine(base.transform.position, this.vpath[0], Color.green);
                int num27 = this.vpath.Count;
                for (num7 = 0; num7 < num27; num7++)
                {
                    if (num7 < (this.vpath.Count - 2))
                    {
                        Debug.DrawLine(this.vpath[num7], this.vpath[num7 + 1], Color.green);
                    }
                }
            }
        }
        if ((Input.GetKey(KeyCode.B) & Input.GetKey(KeyCode.G)) & Input.GetKey(KeyCode.S))
        {
        }
    }
}

