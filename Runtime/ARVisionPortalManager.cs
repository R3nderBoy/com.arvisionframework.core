using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;


/// <summary>
/// Adding Documentation
/// </summary>
namespace ARVisionFrameWork.Core
{

    public enum RoomShadersMode { Outisde = 0, Inside = 1 };
    //public enum PortalEnum { Portal_1 = 0, Portal_2 = 1 };

    public class ARVisionPortalManager : MonoBehaviour
    {
        //bool for checking if the device is not in the same direction as it was
        public bool wasInFront;
        //bool for knowing that on the next change of state, what to set the stencil test
        public bool inOtherWorld;
        //This bool is on while device colliding, done so we ensure the shaders are being updated before render frames
        //Avoids flickering
        public bool isColliding;


        public RoomShadersMode shaderStatus;
        //public PortalEnum portalName;
        //public Material[] materials;
        public List<Material> materials;

        public Transform device;
        public bool hasEntered = false;

        public UnityEvent portalEnter;

        // Start is called before the first frame update
        void Start()
        {

            shaderStatus = RoomShadersMode.Outisde;
            Debug.Log("Room initialization.");
            hasEntered = false;
            //FillPortalMaterialList();
            device = Camera.main.transform;
            SetMaterials(false);
        }

        private void OnEnable()
        {

            SetMaterials(false);
            wasInFront = false;
            inOtherWorld = false;
            hasEntered = false;
        }

        void SetMaterials(bool fullRender)
        {
            var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;
            //Shader.SetGlobalInt("_myStandard", (int)stencilTest);

            foreach (var mat in materials)
            {
                mat.SetInt("_PortalWindowA", (int)stencilTest);
            }

            if (stencilTest == CompareFunction.Equal)
            {
                shaderStatus = RoomShadersMode.Outisde;
            }
            else
            {
                shaderStatus = RoomShadersMode.Inside;
            }
        }

        bool GetIsInFront()
        {
            Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;

            Vector3 pos = transform.InverseTransformPoint(worldPos);
            return pos.z >= 0 ? true : false;
        }


        //This technique registeres if the device has hit the portal, flipping the bool

        void OnTriggerEnter(Collider other)
        {
            if (other.transform != device)
                return;
            //Important to do this for if the user re-enters the portal from the same side
            wasInFront = GetIsInFront();
            isColliding = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.transform != device)
                return;

            if (!hasEntered)
                hasEntered = true;

            isColliding = false;
        }

        /*If there has been a change in the relative position of the device to the portal, flip the
        *Stencil Test
        */

        void WhileCameraColliding()
        {
            if (!isColliding)
                return;

            bool isInFront = GetIsInFront();

            if ((isInFront && !wasInFront) || (wasInFront && !isInFront))
            {
                if (!hasEntered)
                {
                    inOtherWorld = !inOtherWorld;
                    SetMaterials(true);
                    portalEnter?.Invoke();
                }
                else
                {
                    inOtherWorld = !inOtherWorld;

                    if (inOtherWorld)
                    {
                        SetMaterials(true);
                        portalEnter?.Invoke();
                    }
                    else
                    {
                        SetMaterials(false);
                    }
                }
            }
            wasInFront = isInFront;
        }

        void OnDestroy()
        {
            //ensure geometry renders in the editor
            SetMaterials(true);
        }


        void Update()
        {
            WhileCameraColliding();
        }

        //private void OnEnable()
        //{
        //    hasEntered = false;

        //    SetMaterial(false);        
        //}

        //void SetMaterial(bool fullRender)
        //{
        //    var stencilTest = fullRender ? CompareFunction.NotEqual : CompareFunction.Equal;

        //    Debug.Log("Set materials value to: " + stencilTest + " -->(Equal= OutsideRoom / NotEqual= InsideRoom)");

        //    foreach (var mat in materials)
        //    {
        //        mat.SetInt("_PortalWindowA", (int)stencilTest);
        //    }

        //    if(stencilTest == CompareFunction.Equal)
        //    {
        //        shaderStatus = RoomShadersMode.Outisde;
        //    }
        //    else
        //    {
        //        shaderStatus = RoomShadersMode.Inside;
        //    }
        //}

        //private void OnTriggerEnter(Collider other)
        //{
        //    if (other.transform != device)
        //        return;


        //    if(!hasEntered)
        //    {
        //        if (!GetIsInFront())
        //        {
        //            Debug.Log("First portal collition detected, collision from Outside the room");
        //            hasEntered = true;
        //            SetMaterial(true);
        //            portalEnter?.Invoke();
        //        }
        //    }
        //    else
        //    {

        //        if (GetIsInFront() )
        //        {
        //            Debug.Log("Collision from inside the room");

        //            SetMaterial(false);
        //        }
        //        else
        //        {
        //            Debug.Log("Collision from Outside the room");
        //            SetMaterial(true);
        //        }
        //    }

        //}


        //bool GetIsInFront()
        //{
        //    Vector3 worldPos = device.position + device.forward * Camera.main.nearClipPlane;

        //    // Vector3 pos = transform.InverseTransformPoint(device.position);
        //    Vector3 pos = transform.InverseTransformPoint(worldPos);
        //    return pos.z >= 0 ? true : false;
        //}

        public void FillPortalMaterialList()
        {
            materials = new List<Material>();
            int count = 0;
            string shaderName = "NewShaders/myStandard";
            string stArea = "Materials using shader " + shaderName + ":\n\n";

            List<Material> armat = new List<Material>();

            Renderer[] arrend = (Renderer[])Resources.FindObjectsOfTypeAll(typeof(Renderer));
            foreach (Renderer rend in arrend)
            {
                foreach (Material mat in rend.sharedMaterials)
                {
                    if (!armat.Contains(mat))
                    {
                        armat.Add(mat);
                    }
                }
            }

            foreach (Material mat in armat)
            {
                if (mat != null && mat.shader != null && mat.shader.name != null && mat.shader.name == shaderName)
                {
                    materials.Add(mat);
                    stArea += ">" + mat.name + "\n";
                    count++;
                }
            }
            Debug.Log(stArea += "\n" + count + " materials using shader " + shaderName + " found.");
        }

    }

}

