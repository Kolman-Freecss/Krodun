using Kolman_Freecss.HitboxHurtboxSystem;
using Kolman_Freecss.Krodun.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace Kolman_Freecss.Krodun
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerInput))]
    public class KrodunController : NetworkBehaviour, IHitboxResponder, IHurtboxResponder
    {
        #region Inspector Variables
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        public AudioClip RunAudioClip;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        
        #endregion

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDWalk;
        private int _animIDRun;
        private int _animIDAttack01;
        private int _animIDJump;
        private int _animIDOnGround;
        private int _animIDJumpVelocity;

        private PlayerInput _playerInput;
        private Animator _animator;
        private CharacterController _controller;
        private RPGInputs _input;
        public RPGInputs Input => _input;
        private GameObject _mainCamera;
        private MenuManager _menuManager;
        private Hitbox _hitbox;
        private Hurtbox _hurtbox;

        private const float _threshold = 0.01f;
        
        private bool _hasAnimator;
        
        // Multiplayer variables
        private bool _gameLoaded;
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
                return _playerInput.currentControlScheme == "KeyboardMouse";
            }
        }

        public event IHitboxResponder.FacingDirectionChanged OnFacingDirectionChangedHitbox;
        public event IHurtboxResponder.FacingDirectionChanged OnFacingDirectionChangedHurtbox;

        public override void OnNetworkSpawn()
        {
            Debug.Log("OnNetworkSpawn");
            //Always add ourselves to the list at first
            ConnectionManager.Instance.PlayersInGame.Add(NetworkManager.LocalClientId, false);
            //If we are hosting, then handle the server side for detecting when clients have connected
            //and when their lobby scenes are finished loading.
            if (IsServer)
            {

                //Server will be notified when a client connects
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene += ClientLoadedScene;
                RegisterServerCallbacks();
            }
        }

        // This is called when a client connects to the server
        // Invoked when a client has loaded this scene
        private void ClientLoadedScene(ulong clientId)
        {
            Debug.Log("Client loaded scene");
            if (IsServer)
            {
                if (!ConnectionManager.Instance.PlayersInGame.ContainsKey(clientId))
                {
                    ConnectionManager.Instance.PlayersInGame.Add(clientId, false);
                }
            }

        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            Debug.Log($"Client {clientId} connected");
        }
        
        private void CheckInGame(SceneTransitionHandler.SceneStates state)
        {
            if (state == SceneTransitionHandler.SceneStates.Kolman)
            {
                _gameLoaded = true;
            }
        }

        // summary
        // This is called when the object is spawned
        private void InitData(ulong clientId)
        {
            if (IsServer)
            {
                Debug.Log("InitData called to clientId -> " + clientId);
                AwakeData(clientId);
            }
            else
            {
                SendClientInitData(clientId);
            }
            
        }
        
        [ClientRpc]
        private void SendClientInitData(ulong clientId)
        {
            Debug.Log("SendClientInitData called to clientId -> " + clientId);
            AwakeData(clientId);
        }

        public void AwakeData(ulong clientId)
        {
            Debug.Log("AwakeData called to clientId -> " + clientId);
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            
            if (CinemachineCameraTarget == null)
            {
                CinemachineCameraTarget = GameObject.FindGameObjectWithTag("KrodunFollowCamera");
            }

            if (_menuManager == null)
            {
                _menuManager = FindObjectOfType<MenuManager>();
            }

            _hitbox = GetComponentInChildren<Hitbox>();
            _hurtbox = GetComponentInChildren<Hurtbox>();

            RegisterPostCallbacks();
            GetReferences();
        }

        private void RegisterPostCallbacks()
        {
            OnFacingDirectionChangedHitbox += _hitbox.OnFacingDirectionChangedHandler;
            OnFacingDirectionChangedHurtbox += _hurtbox.OnFacingDirectionChangedHandler;
        }

        private void RegisterServerCallbacks()
        {
            SceneTransitionHandler.sceneTransitionHandler.OnClientLoadedScene += InitData;
            SceneTransitionHandler.sceneTransitionHandler.OnSceneStateChanged += CheckInGame;
        }
        
        private void GetReferences()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<RPGInputs>();
            _playerInput = GetComponent<PlayerInput>();
            _menuManager.Init();

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            
            // set our initial facing direction hitbox
            OnFacingDirectionChangedHitbox?.Invoke(transform);
            OnFacingDirectionChangedHurtbox?.Invoke(transform);
            
            // Set Scene loaded to true
            SceneTransitionHandler.sceneTransitionHandler.SetSceneState(SceneTransitionHandler.SceneStates.Kolman);
        }
        
        private void Update()
        {
            if (_gameLoaded)
            {
                _hasAnimator = TryGetComponent(out _animator);

                JumpAndGravity();
                GroundedCheck();
                Move();
                Attack();
            }
        }
        
        private void AssignAnimationIDs()
        {
            _animIDWalk = Animator.StringToHash("Walk");
            _animIDRun = Animator.StringToHash("Run");
            _animIDAttack01 = Animator.StringToHash("Attack01");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDOnGround = Animator.StringToHash("OnGround");
            _animIDJumpVelocity = Animator.StringToHash("JumpVelocity");
        }

        private void Attack()
        {
            _animator.ResetTrigger(_animIDAttack01);
            if (_input.action1)
            {
                if (_hasAnimator)
                {
                    _animator.SetTrigger(_animIDAttack01);
                }
            }
        }
        
        // Method assigned to the animation event
        private void AttackPlayerHitEvent()
        {
            _hitbox.Attack();
        }

        private void LateUpdate()
        {
            if (_gameLoaded)
            {
                CameraRotation();
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
            _animator.SetBool(_animIDOnGround, Grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDWalk, false);
                _animator.SetBool(_animIDRun, false);
            }
            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                // set our facing direction hitbox
                OnFacingDirectionChangedHitbox?.Invoke(transform);
                OnFacingDirectionChangedHurtbox?.Invoke(transform);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            
            // update animator if using character
            if (_hasAnimator)
            {
                if (_input.sprint && _input.move != Vector2.zero)
                {
                    _animator.SetBool(_animIDRun, true);
                }
                else if (_input.move != Vector2.zero)
                {
                    _animator.SetBool(_animIDWalk, true);
                }
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;
                
                if (_hasAnimator)
                {
                    _animator.ResetTrigger(_animIDJump);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetTrigger(_animIDJump);
                    }
                    
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                
                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _animator.SetFloat(_animIDJumpVelocity, _verticalVelocity);
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), SoundManager.instance.EffectsAudioVolume);
                }
            }
        }
        
        private void OnRun(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(RunAudioClip, transform.TransformPoint(_controller.center), SoundManager.instance.EffectsAudioVolume);
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), SoundManager.instance.EffectsAudioVolume);
            }
        }
        
    }
}
