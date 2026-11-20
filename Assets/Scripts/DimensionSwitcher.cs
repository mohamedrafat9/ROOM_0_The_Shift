using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class DimensionSwitcher : MonoBehaviour
{
    [Header("Dimension Roots")]
    [SerializeField] private GameObject dimensionA;
    [SerializeField] private GameObject dimensionB;

    [Header("Safety")]
    [SerializeField] private float capsuleShrink = 0.05f;
    [SerializeField] private float footRayLength = 0.3f;
    [SerializeField] private float footRaySpread = 0.2f;

    public bool IsDimensionA { get; private set; } = true;
    public bool LastSwitchSucceeded { get; private set; }
    public string LastBlockedReason { get; private set; } = "";

    private CharacterController controller;
    private readonly Collider[] overlapResults = new Collider[8];
    private readonly Vector3[] footRayOffsets = new Vector3[5];

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        BuildFootRayOffsets();

        if (dimensionA == null || dimensionB == null)
        {
            Debug.LogError("DimensionSwitcher: assign both dimension roots.", this);
            enabled = false;
        }
    }

    private void Start()
    {
        if (enabled)
        {
            SetDimension(true);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.qKey.wasPressedThisFrame)
        {
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked)
        {
            return;
        }

        LastSwitchSucceeded = TrySwitch(out string reason);
        LastBlockedReason = LastSwitchSucceeded ? "" : reason;

        if (!LastSwitchSucceeded)
        {
            Debug.Log("SHIFT BLOCKED: " + reason);
        }
    }

    public bool TrySwitch(out string reason)
    {
        reason = "";

        if (!controller.isGrounded)
        {
            reason = "No stable footing.";
            return false;
        }

        bool targetIsA = !IsDimensionA;
        SetDimension(targetIsA);

        if (IsCapsuleBlocked())
        {
            SetDimension(IsDimensionA);
            reason = "Solid geometry in the other dimension.";
            return false;
        }

        if (!HasGroundBelow())
        {
            SetDimension(IsDimensionA);
            reason = "No supporting floor in the other dimension.";
            return false;
        }

        IsDimensionA = targetIsA;
        return true;
    }

    private void SetDimension(bool useA)
    {
        dimensionA.SetActive(useA);
        dimensionB.SetActive(!useA);
        Physics.SyncTransforms();
    }

    private bool IsCapsuleBlocked()
    {
        Vector3 center = transform.TransformPoint(controller.center);
        float halfSegment = Mathf.Max(0f, controller.height * 0.5f - controller.radius);
        Vector3 bottom = center - Vector3.up * halfSegment;
        Vector3 top = center + Vector3.up * halfSegment;
        float radius = Mathf.Max(0.01f, controller.radius - capsuleShrink);

        int count = Physics.OverlapCapsuleNonAlloc(
            bottom,
            top,
            radius,
            overlapResults,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);

        for (int i = 0; i < count; i++)
        {
            Collider hit = overlapResults[i];

            if (hit == null || hit == controller || hit.transform.IsChildOf(transform))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private bool HasGroundBelow()
    {
        float halfHeight = controller.height * 0.5f;
        Vector3 feet = transform.TransformPoint(controller.center - Vector3.up * halfHeight);
        Vector3 origin = feet + Vector3.up * 0.1f;

        for (int i = 0; i < footRayOffsets.Length; i++)
        {
            Vector3 point = origin + transform.rotation * footRayOffsets[i];

            if (Physics.Raycast(
                point,
                Vector3.down,
                footRayLength,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            {
                return true;
            }
        }

        return false;
    }

    private void BuildFootRayOffsets()
    {
        footRayOffsets[0] = Vector3.zero;
        footRayOffsets[1] = new Vector3(footRaySpread, 0f, 0f);
        footRayOffsets[2] = new Vector3(-footRaySpread, 0f, 0f);
        footRayOffsets[3] = new Vector3(0f, 0f, footRaySpread);
        footRayOffsets[4] = new Vector3(0f, 0f, -footRaySpread);
    }
}
