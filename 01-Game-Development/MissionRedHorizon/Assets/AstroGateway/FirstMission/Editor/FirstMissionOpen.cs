using UnityEditor;
using UnityEditor.SceneManagement;

// Opens the Missions scene in the Editor (used to launch Unity straight into the game). Does not enter Play Mode.
namespace AstroGateway.FirstMission.EditorTools
{
    public static class FirstMissionOpen
    {
        [MenuItem("AstroGateway/Open Missions Scene")]
        public static void OpenMissions()
        {
            EditorSceneManager.OpenScene(FirstMissionBuilder.MissionsScene);
            try { PlayModeWindow.SetCustomRenderingResolution(2400, 1080, "Android 20:9"); } catch { }
        }
    }
}
