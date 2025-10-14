using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerRangedController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float runMultiplier = 1.5f;
    public float jumpPower = 6f;
    public float gravity = -18f;
    public Transform cameraPivot;

    [Header("Ranged Only")]
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float projectileSpeed = 22f;
    public int projectileDamage = 1;
    public float shootCooldown = 0.18f;
    public bool holdToFire = true;

    [Header("HP UI")]
    public Health playerHealth;
    public Slider hpSlider;

    [Header("Camera Ref")]
    public CameraController_ToggleView camController;  // 카메라 참조

    [Header("Aim")]
    public LayerMask aimMask;           // 바닥/벽/적 등 조준에 사용할 레이어
    public float aimMaxDistance = 1000f;

    private CharacterController controller;
    private Vector3 velocity;
    private float lastShootTime = -999f;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!playerHealth) playerHealth = GetComponent<Health>();
        if (!camController && Camera.main)
            camController = Camera.main.GetComponent<CameraController_ToggleView>();

        if (playerHealth && hpSlider)
        {
            playerHealth.onHealthChanged.AddListener((cur, max) =>
            {
                hpSlider.maxValue = max;
                hpSlider.value = cur;
            });
            playerHealth.onDeath.AddListener(OnPlayerDeath);
        }
    }

    void Update()
    {
        // 입력 먼저
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(h, v);

        bool isFreeLook = (camController != null) ? camController.isFreeLook : false;

        // 1) 캐릭터 몸을 카메라 Yaw에 맞추기
        UpdateCharacterFacing(moveInput, isFreeLook);

        // 2) 이동
        HandleMovement(h, v);

        // 3) 사격
        HandleShooting();
    }

    void HandleMovement(float h, float v)
    {
        Vector3 input = new Vector3(h, 0f, v).normalized;

        Vector3 camFwd = cameraPivot ? Vector3.Scale(cameraPivot.forward, new Vector3(1, 0, 1)).normalized : Vector3.forward;
        Vector3 camRight = cameraPivot ? cameraPivot.right : Vector3.right;
        Vector3 moveDir = (camFwd * input.z + camRight * input.x).normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);
        float spd = moveSpeed * (running ? runMultiplier : 1f);

        controller.Move(moveDir * spd * Time.deltaTime);

        if (controller.isGrounded)
        {
            velocity.y = -2f;
            if (Input.GetButtonDown("Jump"))
                velocity.y = jumpPower;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleShooting()
    {
        bool fireInput = holdToFire ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
        if (!fireInput) return;
        if (!projectilePrefab || !firePoint) return;
        if (Time.time - lastShootTime < shootCooldown) return;

        lastShootTime = Time.time;

        // ▶ 카메라 중앙 레이캐스트로 조준점 구해서 그쪽으로 발사
        Vector3 targetPoint = GetAimPoint();
        Vector3 dir = (targetPoint - firePoint.position).normalized;   // 총구→조준점
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject go = Instantiate(projectilePrefab, firePoint.position, rot);
        if (go.TryGetComponent<SimpleProjectile>(out var proj))
        {
            proj.Init(projectileDamage, projectileSpeed, gameObject.tag);
        }
        else if (go.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = dir * projectileSpeed;
        }
    }

    // === 캐릭터 회전 보정 ===
    void UpdateCharacterFacing(Vector2 moveInput, bool isFreeLook)
    {
        // 자유시점이 아닐 땐 항상 카메라를 본다.
        if (!isFreeLook)
        {
            AlignCharacterYawToCamera();
            return;
        }

        // 자유시점일 땐 이동 중일 때만 카메라로 맞춘다.
        if (moveInput.sqrMagnitude > 0.01f)
            AlignCharacterYawToCamera();
    }

    void AlignCharacterYawToCamera()
    {
        if (Camera.main == null) return;
        Vector3 camFwdFlat = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        if (camFwdFlat.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(camFwdFlat, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 12f * Time.deltaTime);
    }

    // === 카메라 중앙 조준점 ===
    Vector3 GetAimPoint()
    {
        Camera cam = Camera.main;
        if (cam == null)
            return firePoint ? firePoint.position + transform.forward * 10f
                             : transform.position + transform.forward * 10f;

        Ray ray = new Ray(cam.transform.position, cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, aimMaxDistance, aimMask))
            return hit.point;

        // 아무 것도 안 맞으면 멀리 가상 지점
        return cam.transform.position + cam.transform.forward * aimMaxDistance;
    }

    void OnPlayerDeath()
    {
        enabled = false;
    }
}