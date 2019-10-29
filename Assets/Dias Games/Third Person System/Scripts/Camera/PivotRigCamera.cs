using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DiasGames.ThirdPersonSystem.Cameras {

    public abstract class PivotRigCamera : AbstractFollowerCamera
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera

        protected Camera m_Camera; // the transform of the camera
        protected Transform m_Pivot; // the point at which the camera pivots around
        protected Vector3 m_LastTargetPosition;


        protected virtual void Awake()
        {
            // find the camera in the object hierarchy
            m_Camera = GetComponentInChildren<Camera>();
            m_Pivot = m_Camera.transform.parent;
        }
    }
}