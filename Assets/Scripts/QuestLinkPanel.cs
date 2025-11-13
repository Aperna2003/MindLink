using Oculus.Interaction;
using UnityEngine;

public class QuestLikePanel : MonoBehaviour
{
    [Tooltip("Il transform della camera utente (es. CenterEyeAnchor o Main Camera).")]
    public Transform userCamera;

    void LateUpdate()
    {
        if (userCamera == null) return;

        this.transform.LookAt(userCamera, Vector3.up);
    }
}
