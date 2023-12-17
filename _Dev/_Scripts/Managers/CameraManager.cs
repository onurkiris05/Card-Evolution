using Cinemachine;
using UnityEngine;

namespace Game.Managers
{
    public class CameraManager : StaticInstance<CameraManager>
    {
        [Header("Components")]
        [SerializeField] private CinemachineVirtualCamera runningCam;
        [SerializeField] private CinemachineVirtualCamera upgradeZoneCam;
        public CinemachineVirtualCamera endingCams;

        public void SetCamera(CameraType state)
        {
            runningCam.Priority = state == CameraType.Running ? 10 : 0;
            upgradeZoneCam.Priority = state == CameraType.UpgradeZone ? 10 : 0;
            endingCams.Priority = state == CameraType.Ending? 10 : 0;
        }
    }

    public enum CameraType
    {
        Running,
        UpgradeZone,
        Ending
    }
}