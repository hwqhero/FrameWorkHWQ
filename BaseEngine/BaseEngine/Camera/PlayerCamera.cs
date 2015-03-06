using UnityEngine;
using System.Collections;
using System;
namespace BaseEngine
{
[RequireComponent(typeof(Camera))]
public class PlayerCamera : MetaHWQ
{

    public static PlayerCamera Instance;
    public Vector2 MouseSensitivity = new Vector2(100, 100);
    public float WheelSensitivity = 5;
    public Vector2 RotationLimit = new Vector2(80, -40);
    public Vector2 DistanceLimit = new Vector2(10, 0.25f);
    public float Smoothing = 0.2f;
    public Transform ClipUL;
    public Transform ClipUR;
    public Transform ClipLL;
    public Transform ClipLR;
    public Transform TargetLookAt;
    public Renderer targetRenderer;
    public Vector3 angleVector;

    private Vector2 MouseInput;
    private float DesiredFollowDistance;
    private float CurFollowDistance;
    private float FollowVel;
    private Vector2 RotOffset;
    private Vector3 Offset;
    private float NearDistance;
    private float MaxDistance;
    private bool IsOccluded;
    private float OcclusionStepOut = 0.1f;
    private float PreOccludedDistance;

    private AudioSource audioSource;
    //private UniSkyAPI uniSky;

    public bool IsControl = true;
    protected override void Awake()
    {
        float start = Time.realtimeSinceStartup;
        Instance = this;
        audioSource = GetComponent<AudioSource>();
        float FoV = (GetComponent<Camera>().fieldOfView / 2) * Mathf.Deg2Rad;
        float Aspect = GetComponent<Camera>().aspect;
        float NearClip = GetComponent<Camera>().nearClipPlane;
        float Height = Mathf.Tan(FoV);
        float Width = Height * Aspect;

        ClipLR.position = transform.position + transform.right * Width;
        ClipLR.position -= transform.up * Height;
        ClipLR.position += transform.forward * NearClip;

        ClipLL.position = transform.position - transform.right * Width;
        ClipLL.position -= transform.up * Height;
        ClipLL.position += transform.forward * NearClip;

        ClipUR.position = transform.position + transform.right * Width;
        ClipUR.position += transform.up * Height;
        ClipUR.position += transform.forward * NearClip;

        ClipUL.position = transform.position - transform.right * Width;
        ClipUL.position += transform.up * Height;
        ClipUL.position += transform.forward * NearClip;

        DesiredFollowDistance = DistanceLimit.x;
        CurFollowDistance = DesiredFollowDistance;
        MouseInput.y = 20;
        Debug.Log(Time.realtimeSinceStartup - start);
    }



    void Update()
    {
        if (null == TargetLookAt)
        {
            return;
        }
        if (!IsControl)
        {
            return;
        }
        Offset = new Vector3(0, 0, -CurFollowDistance);
        if (Input.GetMouseButton(1))
        {
            MouseInput.x += Input.GetAxis("Mouse X") * MouseSensitivity.x * Time.deltaTime;
            MouseInput.y -= Input.GetAxis("Mouse Y") * MouseSensitivity.y * Time.deltaTime;
            MouseInput.y = ClampAngle(MouseInput.y, RotationLimit.y, RotationLimit.x);
        }
    }

    private void CheckRay()
    {
        Vector3 CameraFollowDistance;
        RaycastHit hitInfo;
        RaycastHit hitInfoR;
        NearDistance = -1;
        MaxDistance = DistanceLimit.x;
        if (!IsOccluded)
        {
            CameraFollowDistance = transform.forward * -(DesiredFollowDistance - CurFollowDistance);
        }
        else
        {
            CameraFollowDistance = transform.forward * -(PreOccludedDistance - CurFollowDistance);
        }
        if (Physics.Linecast(TargetLookAt.position, ClipUL.position, out hitInfo, 1 << 28))
        {
            NearDistance = hitInfo.distance;
        }
        if (Physics.Linecast(TargetLookAt.position, ClipLL.position, out hitInfo, 1 << 28))
        {
            if (hitInfo.distance < NearDistance || NearDistance == -1)
            {
                NearDistance = hitInfo.distance;
            }
        }
        if (Physics.Linecast(TargetLookAt.position, ClipUR.position, out hitInfo, 1 << 28))
        {
            if (hitInfo.distance < NearDistance || NearDistance == -1)
            {
                NearDistance = hitInfo.distance;
            }
        }

        if (Physics.Linecast(TargetLookAt.position, ClipLR.position, out  hitInfo, 1 << 28))
        {
            if (hitInfo.distance < NearDistance || NearDistance == -1)
            {
                NearDistance = hitInfo.distance;
            }
        }

        //Center Camera
        if (Physics.Linecast(TargetLookAt.position, transform.position, out hitInfo, 1 << 28))
        {
            if (hitInfo.distance < NearDistance || NearDistance == -1)
            {
                NearDistance = hitInfo.distance;
            }
        }

        //Rear Camera Raycast

        if (Physics.Linecast(ClipUL.position, ClipUL.position + CameraFollowDistance, out hitInfoR, 1 << 28))
        {
            MaxDistance = hitInfoR.distance;
        }
        if (Physics.Linecast(ClipUR.position, ClipUR.position + CameraFollowDistance, out hitInfoR, 1 << 28))
        {
            if (hitInfoR.distance < MaxDistance || MaxDistance == DistanceLimit.x)
            {
                MaxDistance = hitInfoR.distance;
            }
        }
        if (Physics.Linecast(ClipLL.position, ClipLL.position + CameraFollowDistance, out hitInfoR, 1 << 28))
        {
            if (hitInfoR.distance < MaxDistance || MaxDistance == DistanceLimit.x)
            {
                MaxDistance = hitInfoR.distance;
            }
        }
        if (Physics.Linecast(ClipLR.position, ClipLR.position + CameraFollowDistance, out hitInfoR, 1 << 28))
        {
            if (hitInfoR.distance < MaxDistance || MaxDistance == DistanceLimit.x)
            {
                MaxDistance = hitInfoR.distance;
            }
        }

    }

    void CheckCameraOcclustion()
    {
        if (NearDistance != -1 && CurFollowDistance > DistanceLimit.y && CurFollowDistance > NearDistance)
        {
            if (IsOccluded == false)
            {
                IsOccluded = true;
                PreOccludedDistance = DesiredFollowDistance;
            }
            CurFollowDistance = NearDistance;
            DesiredFollowDistance = NearDistance;
        }

        if (IsOccluded)
        {
            if (NearDistance == -1)
            {

                if (MaxDistance == DistanceLimit.x)
                {
                    if ((CurFollowDistance + OcclusionStepOut) < PreOccludedDistance)
                    {
                        DesiredFollowDistance += OcclusionStepOut;
                    }
                    else
                    {
                        DesiredFollowDistance = PreOccludedDistance;
                    }
                }
                else if (DesiredFollowDistance < MaxDistance && (CurFollowDistance + OcclusionStepOut) < PreOccludedDistance)
                {
                    DesiredFollowDistance += OcclusionStepOut;
                }
            }

            if (DesiredFollowDistance >= PreOccludedDistance)
            {
                IsOccluded = false;
            }
        }
    }

    private void UpdateCameraFollowDistance()
    {
        #if !UNITY_ANDROID


        if (Input.GetAxis("Mouse ScrollWheel") > .01 || Input.GetAxis("Mouse ScrollWheel") < -.01)
        {
            if (!IsOccluded)
            {
                DesiredFollowDistance = Mathf.Clamp(DesiredFollowDistance - Input.GetAxis("Mouse ScrollWheel") * WheelSensitivity, DistanceLimit.y, DistanceLimit.x);
            }
            else
            {
                PreOccludedDistance -= Input.GetAxis("Mouse ScrollWheel") * WheelSensitivity;
                if (PreOccludedDistance > DistanceLimit.x)
                {
                    PreOccludedDistance = DistanceLimit.x;
                }
            }
        }
#endif
        CurFollowDistance = Mathf.SmoothDamp(CurFollowDistance, DesiredFollowDistance, ref FollowVel, Smoothing);

    }

    private void UpdateCameraPosition()
    {
        // Quaternion.Euler(MouseInput.y, MouseInput.x, 0);
        Quaternion rot = Quaternion.Euler(angleVector);
        Vector3 DesiredCamPos = rot * new Vector3(Offset.x, Offset.y, Offset.z) + TargetLookAt.position;
        Vector3 DesiredCamDir = (DesiredCamPos - TargetLookAt.position).normalized;

        transform.position = TargetLookAt.position + (DesiredCamDir * CurFollowDistance);

        transform.eulerAngles = new Vector3(rot.eulerAngles.x - RotOffset.x, rot.eulerAngles.y - RotOffset.y, 0);
    }

    void LateUpdate()
    {
        if (TargetLookAt)
        {
            CheckRay();
            CheckCameraOcclustion();

            UpdateCameraFollowDistance();

            UpdateCameraPosition();
            if (targetRenderer != null)
            {
                if (CurFollowDistance < 1)
                    targetRenderer.enabled = false;
                else
                {
                    targetRenderer.enabled = true;
                }
            }
        }
    }





    public static float ClampAngle(float angle, float min, float max)
    {
        do
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        } while (angle < -360 || angle > 360);
        return Mathf.Clamp(angle, min, max);
    }
}
}
