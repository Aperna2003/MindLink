using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class QuestLikePanelHandle : MonoBehaviour
{
    [Header("References")]
    public Transform panelRoot;
    public Transform userCamera;

    private Grabbable grab;

    private Quaternion lastPanelRotation;   // rotazione pannello
    private Quaternion lastHandleRotation;  // rotazione barra

    private Vector3 grabOffset;
    private Vector3 initialLocalPos;

    void Awake()
    {
        grab = GetComponent<Grabbable>();

        if (panelRoot == null)
            panelRoot = transform.parent;

        grabOffset = panelRoot.position - transform.position;
        initialLocalPos = transform.localPosition;

        lastPanelRotation = panelRoot.rotation;
        lastHandleRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (userCamera == null || panelRoot == null) return;

        bool isGrabbed = grab.SelectingPointsCount > 0;

        if (isGrabbed)
        {
            MovePanelWithHandle();
            RotateHandleTowardsCamera();

            lastHandleRotation = transform.rotation;
        }
        else
        {
   
            panelRoot.rotation = lastPanelRotation;
            transform.rotation = lastHandleRotation;

            LockHandleLocalZ();
        }
    }

    void MovePanelWithHandle()
    {
        panelRoot.position = transform.position + grabOffset;

        Vector3 lookDir = userCamera.position - panelRoot.position;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(-lookDir.normalized, Vector3.up);
            panelRoot.rotation = Quaternion.Slerp(panelRoot.rotation, targetRot, Time.deltaTime * 10f);

            lastPanelRotation = panelRoot.rotation;
        }

        LockHandleLocalZ();
    }

    void RotateHandleTowardsCamera()
    {
        Vector3 dir = userCamera.position - transform.position;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    void LockHandleLocalZ()
    {
        Vector3 local = transform.localPosition;
        local.z = initialLocalPos.z;
        transform.localPosition = local;
    }
}
