using Unity.Robotics.UrdfImporter.Control;
using UnityEngine;

/// <summary>
/// 起動時に子のRevoluteJoint全てについてプロパティを設定します
/// 個々に値を設定した場合はこのスクリプトは不要です
/// </summary>
/// <remarks>
/// Unity.Robotics.UrdfImporter.Control.Controller クラスの代替として用意したものです
/// </remarks>
public class RevoluteJointsInitializer : MonoBehaviour
{
    //public ControlType control = ControlType.PositionControl;
    [Header("Hover the mouse pointer to show units")]

    [Tooltip("Units: perhaps [-]")]
    public float jointFriction = 10f;

    [Tooltip("Units: perhaps [Ns/deg]")]
    public float angularDamping = 10f;
    
    [Space]
    [Tooltip("Units: [N/m]")]
    public float stiffness;
    
    [Tooltip("Units: [Ns/m]")]
    public float damping;
    
    [Tooltip("Units: [Nm]")]
    public float torqueLimit;
    
    void Start()
    {
        InitializeRevoluteJointProperties();
    }

    /// <summary>
    /// ReboluteJointのStiffnessやDamping等をまとめて設定します
    /// </summary>
    void InitializeRevoluteJointProperties()
    {
        var articulationBodies = this.GetComponentsInChildren<ArticulationBody>();
        
        foreach (var joint in articulationBodies)
        {
            if (joint.jointType == ArticulationJointType.RevoluteJoint)
            {
                joint.jointFriction = jointFriction;
                joint.angularDamping = angularDamping;

                var drive = joint.xDrive;
                drive.stiffness = stiffness;
                drive.damping = damping;
                drive.forceLimit = torqueLimit;
                joint.xDrive = drive;
            }
        }
    }
}
