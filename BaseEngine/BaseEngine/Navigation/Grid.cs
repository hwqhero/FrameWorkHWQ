using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Dynamic Navigation/ Grid"), ExecuteInEditMode]
public class Grid : MonoBehaviour
{
    /// <summary>
    /// 格子大小
    /// </summary>
    public float CubeSize = 1f;
    /// <summary>
    /// 人物宽度
    /// </summary>
    public float CapsuleSize = 0.8f;
    public bool CheckCollisionAtstart;
    /// <summary>
    /// 执行start方法时创建网格
    /// </summary>
    public bool CreatGridAtStart;
    /// <summary>
    /// 创建网格
    /// </summary>
    public bool CreatGridNow;
    public bool DebugShowConnections;
    public bool DebugShowNonwalkable;
    public bool DebugShowSizeOutline = true;
    public bool DestroyGrid;
    /// <summary>
    /// 网格大小
    /// </summary>
    public int GridSize = 100;
    public LayerMask characterLayerMask;
    public LayerMask dynamicObstacleLayerMask;
    public LayerMask IgnoreCollisionLayerMask;
    public LayerMask notWalkableLayerMask;
    
    /// <summary>
    /// 层高度
    /// </summary>
    public float LayerHeight = 3f;
    /// <summary>
    /// 多少层
    /// </summary>
    public int Layers = 2;
    /// <summary>
    /// 人物高度
    /// </summary>
    public float SlopeHeight = 1f;
    /// <summary>
    /// 能走的高度
    /// </summary>
    public float StepHeight = 0.3f;
    public int Updatestatic;


    public List<int> ConnectionID = new List<int>();
    public int[] ConnectionsIDAR = new int[0];
    public Vector3[] ConnectionsVAR = new Vector3[0];
    public List<Vector3> ConnectionVectors = new List<Vector3>();
    [HideInInspector]
    public List<int> gridref = new List<int>();
    [HideInInspector]
    public List<int> gridref2 = new List<int>();
    public List<Vector3> gridvectors = new List<Vector3>();
    /// <summary>
    /// GridSize *　GridSize寻路大小
    /// </summary>
    public Vector3[] GridSearch = new Vector3[0];
    /// <summary>
    /// N层网格ID
    /// </summary>
    public int[] GridSearch2 = new int[0];
    public List<bool> IsObstacle = new List<bool>();
    public List<Vector3> Layer = new List<Vector3>();
    public Vector2[] PortalConnections = new Vector2[0];
    public Vector3[] WaypointVAR = new Vector3[0];//28.89041 1.785965
    /// <summary>
    /// 寻路点
    /// </summary>
    public List<Vector3> WaypointVectors = new List<Vector3>();


    public LayerMask layy;
    private Vector3 positionadder;
    private Vector3 positionadder2;
    private float savecapsize;
    private float savecubesize;
    private bool startpos;
    private bool swit;

    private Vector3 zero;

    public void MakeGrid2()
    {
    }

    private void OnDisable()
    {
        GC.Collect();
    }

    private void OnEnable()
    {
        GC.Collect();
    }

    private void Start()
    {
        if (this.CheckCollisionAtstart)
        {
            this.layy = ~(IgnoreCollisionLayerMask | dynamicObstacleLayerMask | characterLayerMask | notWalkableLayerMask);
            int count = this.WaypointVectors.Count;
            for (int i = 0; i < count; i++)
            {
                Vector3 end = this.WaypointVectors[i];
                end.y += this.CubeSize;
                Vector3 start = this.WaypointVectors[i];
                start.y = end.y + (this.StepHeight * 1.5f);
                if (Physics.CheckCapsule(start, end, this.CapsuleSize, (int)this.layy))
                {
                    this.IsObstacle[i] = true;
                }
                else
                {
                    this.IsObstacle[i] = false;
                }
            }
        }
        if (this.CreatGridAtStart)
        {
            this.CreatGridNow = true;
        }
    }

    public void Update()
    {
        int num11;
        int num19;
        if (this.DebugShowSizeOutline)
        {
            Vector3 position = base.transform.position;
            Vector3 start = base.transform.position;
            Vector3 vector3 = base.transform.position;
            Vector3 end = base.transform.position;
            position.x = base.transform.position.x + (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            position.z = base.transform.position.z + ((this.GridSize * this.CubeSize) * 0.5f);
            start.x = base.transform.position.x - (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            start.z = base.transform.position.z + ((this.GridSize * this.CubeSize) * 0.5f);
            vector3.x = base.transform.position.x - (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            vector3.z = base.transform.position.z - (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            end.x = base.transform.position.x + (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            end.z = base.transform.position.z - (((this.GridSize * this.CubeSize) * 0.5f) - this.CubeSize);
            Debug.DrawLine(start, position);
            Debug.DrawLine(vector3, start);
            Debug.DrawLine(vector3, end);
            Debug.DrawLine(position, end);
        }
        if (this.savecubesize != this.CubeSize)
        {
            this.savecubesize = this.CubeSize;
            if (((((this.CubeSize - ((int)this.CubeSize)) * 10000f) / 625f) - (((int)((this.CubeSize - ((int)this.CubeSize)) * 10000f)) / 0x271)) > 0f)
            {
                Debug.Log("The cube size is not compatible! ");
                this.CreatGridNow = false;
            }
        }
        if (this.savecapsize != this.CapsuleSize)
        {
            this.savecapsize = this.CapsuleSize;
            if ((this.CapsuleSize / this.CubeSize) <= 0.6f)
            {
                Debug.Log("Your capsule size is significantly small than your cube size.  The results may not be desirable.. ");
            }
        }
        if (this.CreatGridNow)
        {
            Create();
        }
        if (this.DestroyGrid)
        {
            this.Layer.Clear();
            this.gridvectors.Clear();
            this.WaypointVectors.Clear();
            this.GridSearch = new Vector3[0];
            this.GridSearch2 = new int[0];
            this.ConnectionsIDAR = new int[0];
            this.ConnectionsVAR = new Vector3[0];
            this.gridref.Clear();
            this.DestroyGrid = false;
        }
        if (this.DebugShowConnections | this.DebugShowNonwalkable)
        {
            List<Vector3> position = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            int num27 = this.WaypointVectors.Count;
            for (int k = 0; k < num27; k++)
            {
                Vector3 vector10 = base.transform.position;
                vector10.x = 0f;
                vector10.y = 0f;
                vector10.z = 0f;
                num19 = 8;
                for (num11 = 0; num11 < num19; num11++)
                {
                    if (this.DebugShowConnections && ((this.ConnectionsVAR[(k * 8) + num11] != vector10) & (this.ConnectionsVAR[(k * 8) + num11] != this.WaypointVectors[k])))
                    {
                        Debug.DrawLine(this.WaypointVectors[k], this.ConnectionsVAR[(k * 8) + num11], Color.blue);
                        position.Add(WaypointVectors[k]);
                        uv.Add(new Vector2(0, 1));
                    }
                    if (((this.ConnectionsVAR[(k * 8) + num11] != vector10) & (this.ConnectionsVAR[(k * 8) + num11] != this.WaypointVectors[k])) && (this.DebugShowNonwalkable && this.IsObstacle[k]))
                    {
                        Debug.DrawLine(this.WaypointVectors[k], this.ConnectionsVAR[(k * 8) + num11], Color.red);
                    }
                }
            }
        }
    }

    public void Create()
    {
        int num7;
        this.layy = ~(IgnoreCollisionLayerMask | dynamicObstacleLayerMask | characterLayerMask | notWalkableLayerMask);
        this.Layer.Clear();
        this.gridref.Clear();
        this.gridvectors.Clear();
        this.IsObstacle.Clear();
        this.WaypointVectors.Clear();
        this.positionadder = base.transform.position;
        this.positionadder.x = base.transform.position.x - ((this.GridSize * this.CubeSize) / 2f);
        this.positionadder.z = base.transform.position.z + ((this.GridSize * this.CubeSize) / 2f);
        this.gridvectors.Add(this.positionadder);//添加起点
        if (this.gridvectors.Count > 0)
        {
            this.gridref.Add(this.gridvectors.Count - 1);
        }
        for (int i = 0; i < GridSize; i++)//z
        {
            for (int j = 0; j < GridSize; j++)//x
            {
                this.gridvectors.Add(this.positionadder);
                this.gridref.Add(this.gridvectors.Count - 1);
                this.positionadder.x += this.CubeSize;
            }
            this.positionadder.x = base.transform.position.x - ((this.GridSize * this.CubeSize) / 2f);
            this.positionadder.z -= this.CubeSize;
        }
        this.GridSearch = this.gridvectors.ToArray();//所有的网格点
        this.GridSearch2 = new int[GridSearch.Length * Layers];//所有层网格点
        for (int i = 0; i < this.Layers; i++)
        {
            for (int j = 0; j < gridvectors.Count; j++)
            {
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(this.gridvectors[j], -base.transform.up, out hitInfo, 3000f, this.layy))
                {
                    DynamicObstacle component = (DynamicObstacle)hitInfo.transform.GetComponent("DynamicObstacle");
                    Vector3 point = hitInfo.point;
                    point.y += this.StepHeight;
                    Vector3 vector7 = hitInfo.point;
                    vector7.y -= this.LayerHeight;
                    this.Layer.Add(vector7);
                    this.gridref2.Add(this.gridref[j]);
                    Vector3 vector8 = point;
                    vector8.y += this.CubeSize;
                    Vector3 vector9 = point;
                    vector9.y = vector8.y + (this.StepHeight * 1.5f);
                    if (!component)
                    {
                        this.WaypointVectors.Add(point);
                        LayerMask mask = ~(this.dynamicObstacleLayerMask | this.IgnoreCollisionLayerMask);
                        if (Physics.CheckCapsule(vector9, vector8, this.CapsuleSize, (int)mask))
                        {
                            this.IsObstacle.Add(true);
                        }
                        else
                        {
                            this.IsObstacle.Add(false);
                        }
                        for (int m = 0; m < 8; m++)
                        {
                            this.ConnectionVectors.Add(Vector3.zero);
                            this.ConnectionID.Add(0);
                        }
                    }
                    this.GridSearch2[j + (this.GridSearch.Length * i)] = this.WaypointVectors.Count - 1;
                }
            }
            gridref.Clear();
            gridvectors.Clear();
            for (int n = 0; n < this.Layer.Count; n++)
            {
                this.gridref.Add(this.gridref2[n]);
                this.gridvectors.Add(this.Layer[n]);
            }
            this.Layer.Clear();
            this.gridref2.Clear();
        }

        for (num7 = 0; num7 < WaypointVectors.Count; num7++)//关联八个方向
        {
            Vector3 current = WaypointVectors[num7];
            float num14 = 0f;
            float num15 = Mathf.Abs((float)((this.GridSearch[0].x - this.WaypointVectors[num7].x) / this.CubeSize));
            float num16 = Mathf.Abs((float)((this.GridSearch[0].z - this.WaypointVectors[num7].z) / this.CubeSize));
            num14 = (num15 + (num16 * this.GridSize)) + 1f;
            int num17 = (int)(num14 + 0.5f);
            int num18 = 0;
            for (int m = 0; m < 8; m++)//八个方向
            {
                switch (m)
                {
                    case 0://后面
                        num18 = num17 - this.GridSize;
                        break;
                    case 1://左面
                        num18 = num17 - 1;
                        break;
                    case 2://前面
                        num18 = num17 + this.GridSize;
                        break;
                    case 3://右面
                        num18 = num17 + 1;
                        break;
                    case 4://前左
                        num18 = (num17 + this.GridSize) - 1;
                        break;
                    case 5://前右
                        num18 = (num17 + this.GridSize) + 1;
                        break;
                    case 6://后右
                        num18 = (num17 - this.GridSize) + 1;
                        break;
                    case 7://后左
                        num18 = (num17 - this.GridSize) - 1;
                        break;
                }
                if (num18 != 0)
                {
                    for (int i = 0; i < Layers; i++)
                    {
                        if ((num18 + GridSearch.Length * i >= 0) & ((num18 + GridSearch.Length * i) < GridSearch2.Length))
                        {
                            Vector3 temp = WaypointVectors[GridSearch2[num18 + GridSearch.Length * i]];
                            if ((Mathf.Abs(temp.x - current.x) <= CubeSize * 1.7f && Mathf.Abs(temp.z - current.z) <= this.CubeSize * 1.7f && (Mathf.Abs(temp.y - current.y)) < this.SlopeHeight))
                            {

                                ConnectionVectors[num7 * 8 + m] = temp;
                                ConnectionID[num7 * 8 + m] = GridSearch2[num18 + GridSearch.Length * i];
                            }
                        }
                    }
                }
            }
        }
        for (num7 = 0; num7 < WaypointVectors.Count; num7++)//判断八方向是不是存在
        {
            int num26 = 0;
            for (int num6 = 0; num6 < 8; num6++)
            {
                if (this.ConnectionVectors[(num7 * 8) + num6] != this.zero)
                {
                    num26++;
                }
            }
            if (num26 < 8)
            {
                this.IsObstacle[num7] = true;
            }
        }
        this.gridref.Clear();
        this.PortalConnections = new Vector2[this.WaypointVectors.Count];
        this.ConnectionsVAR = this.ConnectionVectors.ToArray();
        this.ConnectionsIDAR = this.ConnectionID.ToArray();
        this.WaypointVAR = this.WaypointVectors.ToArray();
        this.ConnectionID.Clear();
        this.ConnectionVectors.Clear();
        GC.Collect();
        Debug.Log("Grid Created");
        this.CreatGridNow = false;
    }
}
