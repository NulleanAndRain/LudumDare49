using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(WalkerComponent), typeof(Health))]
public class PlayerControl : MonoBehaviour
{
    [Header("Scene objects")]
    public GameUI gameUI;
    [SerializeField] private EventSystem _eventSystem;

    [Header("Self components")]
    public Transform viewPos;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationControl _controller;
    private Health health;
    private WalkerComponent walker;
    private Inventory inventory;

    [Header("Inventory Control")]
    public bool IsMouseWheelActive;
    public float MouseClickCD;

    private float _leftClickLastUse;
    private float _rightClickLastUse;


    private Vector3 _localScale = Vector3.one;
    private Vector2 _vel;

    public int animNum { get; private set; }

    private void Start()
    {
        health = GetComponent<Health>();
        walker = GetComponent<WalkerComponent>();
        inventory = GetComponent<Inventory>();

        void updateHpBar(float currHP, float maxHP)
        {
            gameUI.UpdateHpBar(currHP / maxHP);
        }

        health.onHealthUpdate += updateHpBar;

        void revive()
        {
            _vel.x = 0;
            _vel.y = 0;
            walker.UpdateMoveVector(_vel);
            _animator.SetBool("isWalking", false);
            //_animator.SetBool("isDowned", true);
            _controller.TriggerAnimation(AnimationTrigger.Downed);
            if (animNum < 4)
            {
                _localScale.x *= -1;
                transform.localScale = _localScale;
            }

            StartCoroutine(waitForRevive());
        }

        IEnumerator waitForRevive()
        {
            yield return new WaitForSeconds(GameManager.RespawnTime);
            _controller.TriggerAnimation(AnimationTrigger.Reset);
            health.Revive();
            health.Heal(health.MaxHealth);
            _localScale = Vector3.one;
            transform.localScale = _localScale;
        }

        health.onDowned += revive;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel")) PauseControl.TogglePause();

        CheckInvInput();
        if (health.isDowned || PauseControl.isPaused) return;

        CheckMotion();
        CheckMouseClicks();
    }

    private void CheckMotion()
    {
        _vel.x = Input.GetAxis("Horizontal");
        _vel.y = Input.GetAxis("Vertical");

        var _magn = _vel.magnitude;

        //_animator.SetBool("isWalking", _magn > 1e-3);

        //animNum = Mathf.FloorToInt((viewAngle + 22.5f) / 45) % 8;
        if (_vel.x > 1e-3)
            animNum = 2;
        else if (_vel.x < -1e-3)
            animNum = 6;
        //else
        //    animNum = 0;

        //_animator.SetInteger("currDir", animNum);

        walker.UpdateMoveVector(_vel);

        if (Input.GetButtonDown("Jump"))
            // todo: add animation
            walker.Jump();
        else if (Input.GetButtonUp("Jump")) walker.StopJump();
    }

    private void CheckInvInput()
    {
        if (IsMouseWheelActive)
        {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll != 0)
            {
                if (scroll < -0.01f)
                {
                    inventory.currCell++;
                }
                if (scroll > 0.01f)
                {
                    inventory.currCell--;
                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha1))
            inventory.currCell = 0;
        if (Input.GetKey(KeyCode.Alpha2))
            inventory.currCell = 1;
        if (Input.GetKey(KeyCode.Alpha3))
            inventory.currCell = 2;
        if (Input.GetKey(KeyCode.Alpha4))
            inventory.currCell = 3;
    }

    private void CheckMouseClicks()
    {
        if (Input.GetMouseButtonDown(0) && Time.time > _leftClickLastUse + MouseClickCD)
        { // left click down
            if (!_eventSystem.IsPointerOverGameObject())
            {
                _leftClickLastUse = Time.time;
                inventory.MainClickDown();
            }
        }

        if (Input.GetMouseButtonUp(0)) // left click up
            inventory.SecondaryClickDown();

        if (Input.GetMouseButtonDown(1) && Time.time > _rightClickLastUse + MouseClickCD)
        { // right click down
            if (!_eventSystem.IsPointerOverGameObject())
            {
                _rightClickLastUse = Time.time;
                inventory.SecondaryClickDown();
            }
        }

        if (Input.GetMouseButtonUp(1)) // right click up
            inventory.SecondaryClickUp();
    }
}