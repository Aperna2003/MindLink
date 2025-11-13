using Oculus.Interaction;
using UnityEngine;

[RequireComponent(typeof(Grabbable))]
public class QuestLikePanelHandle : MonoBehaviour
{
    [Header("References")]
    public Transform panelRoot;
    public Transform userCamera;

    private Grabbable grab;

    private Quaternion lastRotation;
    private Vector3 grabOffset;

    // Offset locale iniziale della barra rispetto al pannello
    private Vector3 initialLocalPos;

    void Awake()
    {
        grab = GetComponent<Grabbable>();

        if (panelRoot == null)
            panelRoot = transform.parent;

        // Salva offset mondo → pannello
        grabOffset = panelRoot.position - transform.position;

        // Salva posizione locale della barra (per bloccare asse Z)
        initialLocalPos = transform.localPosition;

        lastRotation = panelRoot.rotation;
    }

    void LateUpdate()
    {
        if (userCamera == null || panelRoot == null) return;

        bool isGrabbed = grab.SelectingPointsCount > 0;

        if (isGrabbed)
        {
            MovePanelWithHandle();
            RotateHandleTowardsCamera();
        }
        else
        {
            // Mantieni l'ultima rotazione
            panelRoot.rotation = lastRotation;

            // Assicura che la barra non scappi mai
            LockHandleLocalZ();
        }
    }

    void MovePanelWithHandle()
    {
        // Mantieni offset originale barra -> pannello
        panelRoot.position = transform.position + grabOffset;

        // Rotazione pannello verso camera
        Vector3 lookDir = userCamera.position - panelRoot.position;

        if (lookDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(-lookDir.normalized, Vector3.up);

            panelRoot.rotation = Quaternion.Slerp(
                panelRoot.rotation, targetRot,
                Time.deltaTime * 10f
            );

            lastRotation = panelRoot.rotation;
        }

        // Blocca la barra all'offset locale corretto (no fuga Z)
        LockHandleLocalZ();
    }

    void RotateHandleTowardsCamera()
    {
        // La barra guarda sempre la camera
        Vector3 dir = userCamera.position - transform.position;

        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    void LockHandleLocalZ()
    {
        // Mantieni posizione locale originale
        Vector3 local = transform.localPosition;

        // Blocca la componente Z
        local.z = initialLocalPos.z;

        transform.localPosition = local;
    }
}
