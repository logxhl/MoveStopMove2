using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private AnimationController animationController;
    [SerializeField] private JoystickController joystick; // Kéo JoystickBG vào đây
    [SerializeField] private WeaponAttack weaponAttack; // Kéo WeaponAttack vào đây nếu cần
    [SerializeField] private GameObject fixedJoyStick;
    [SerializeField] private GameObject topUI;
    [SerializeField] private GameObject panelCountDown;
    [SerializeField] private TextMeshProUGUI textRank;

    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody rigid;

    private PlayerState playerState;

    private Vector2 dir;
    private Vector3 move;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        if (animationController == null)
            animationController = GetComponentInChildren<AnimationController>();
    }

    void Update()
    {
        dir = joystick.inputDir; // Lấy hướng từ joystick
        move = new Vector3(dir.x, 0, dir.y); // Di chuyển XZ

        if (move != Vector3.zero)
            if (!animationController.IsPlayingUnStopAnimation)
                animationController.OnSpecialAnimationEnd();

        if (!animationController.IsPlayingSpecialAnimation && !animationController.IsPlayingUnStopAnimation)
        {
            transform.Translate(move.normalized * moveSpeed * Time.deltaTime, Space.World);

            // Nếu muốn xoay mặt player về hướng di chuyển:
            if (move != Vector3.zero)
            {
                playerState = PlayerState.Run;
                transform.forward = move.normalized;
                animationController.SetRunAnimation();
                weaponAttack.SetCanAttack(false);
            }
            else
            {
                playerState = PlayerState.Idle;
                animationController.SetIdleAnimation();
                weaponAttack.SetCanAttack(true);
            }
        }



        //if (Input.GetKey(KeyCode.A))
        //{
        //    animationController.SetDeadAnimation();
        //}
        //else if (Input.GetKey(KeyCode.D))
        //{
        //    animationController.SetDanceWinAnimation();
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    animationController.SetDanceAnimation();
        //}
        //else if (Input.GetKey(KeyCode.W))
        //{
        //    animationController.SetUltiAnimation();
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            playerState = PlayerState.Die;
            animationController.SetDeadAnimation();
            Invoke(nameof(SetDeactiveGameObj), 2);
            if(GameController.instance != null) 
                GameController.instance.SetPlayerAlive(false);

            SpawnEnemy.Instance?.NotifyCharacterDied(true);
            textRank.text = "#" + (SpawnEnemy.Instance?.GetRemainingCount() + 1);
            fixedJoyStick.SetActive(false);
            topUI.SetActive(false);
            panelCountDown.SetActive(true);
            //UIManager.instance.Load();
            UIManager.instance.isDead = true;
        }
    }

    void SetDeactiveGameObj() => gameObject.SetActive(false);
}