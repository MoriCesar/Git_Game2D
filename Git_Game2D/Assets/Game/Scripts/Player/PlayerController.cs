using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Platformer2D.Character;

[RequireComponent(typeof(CharacterMovement2D))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(CharacterFacing2D))]
[RequireComponent(typeof(IDamageable))]

public class PlayerController : MonoBehaviour
{
    CharacterMovement2D playerMovement;
    CharacterFacing2D playerFacing;
    PlayerInput playerInput;
    IDamageable damageable;

    [SerializeField] GameObject weaponObjet;
    public IWeapon Weapon { get; private set; }


    [Header("Camera")]
    
    [SerializeField]
    private Transform cameraTarget;
    [Range(0.0f,5.0f)]
    [SerializeField]
    private float cameraTargetOffsetX = 2.0f;
    [Range(0.5f,50.0f)]
    [SerializeField]
    private float cameraTargetFlipSpeed = 2.0f;
    [Range(0.0f,5.0f)]
    [SerializeField]
    private float characterSpeedInfluence = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<CharacterMovement2D>();

        playerInput = GetComponent<PlayerInput>();
        playerFacing = GetComponent<CharacterFacing2D>();
        damageable = GetComponent<IDamageable>();

        damageable.DeathEvent += OnDeath;

        if (weaponObjet != null)
        { 
            Weapon = weaponObjet.GetComponent<IWeapon>();
        }
    }


    private void OnDestroy()
    { 
        if (damageable != null)
        { 
            damageable.DeathEvent -= OnDeath;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Movimentacao
        Vector2 movementInput = playerInput.GetMovementInput();
        playerMovement.ProcessMovementInput(movementInput);

        playerFacing.UpdateFacing(movementInput);

        // Pulo
        if (playerInput.IsJumpButtonDown())
        { 
            playerMovement.Jump();
        }
        if (playerInput.IsJumpButtonHeld() == false)
        { 
            playerMovement.UpdateJumpAbort();
        }

        // Agachar
        if (playerInput.IsCrouchButtonDown())
        { 
            playerMovement.Crouch();
        }
        else if (playerInput.IsCrouchButtonUp())
        { 
            playerMovement.UnCrouch();
        } 

        if (Weapon != null && playerInput.IsAttackButtonDown())
        { 
            Weapon.Attack();
        }
    }

    private void FixedUpdate()
    { 
        // Controle do target da camera dependendo da direcao e da velocidade do jogador
        bool isFacingright = playerFacing.IsFacingRight();
        float targetOffsetX = isFacingright ? cameraTargetOffsetX : -cameraTargetOffsetX;

        float currentOffsetX = Mathf.Lerp(cameraTarget.localPosition.x, targetOffsetX, Time.fixedDeltaTime * cameraTargetFlipSpeed);

        currentOffsetX += playerMovement.CurrentVelocity.x * Time.fixedDeltaTime * characterSpeedInfluence;

        cameraTarget.localPosition = new Vector3(currentOffsetX, cameraTarget.localPosition.y, cameraTarget.localPosition.z);
        //
    }

    private void OnDeath()
    { 
        // Morrer c/ dano
        playerMovement.StopImmediately();
        enabled = false;
    }
}
