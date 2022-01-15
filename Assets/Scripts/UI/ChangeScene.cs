using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    // TODO: Maybe have this change the icon/image as well
    // TODO: Hook this up with level/fossil data
    public class ChangeScene : MonoBehaviour
    {
        public void ChangeSceneByID(int buildID)
        {
            SceneManager.LoadScene(buildID);
        }
    }
}
