using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using ChickenHunt.Scripts.FirstPerson;
using Random = UnityEngine.Random;

namespace ChickenHunt.Scripts.FirstPerson
{
	[RequireComponent(typeof (CharacterController))]
	[RequireComponent(typeof (AudioSource))]
	public class FirstPersonController_ksi : MonoBehaviour
	{
		[SerializeField] private bool m_IsWalking;
		[SerializeField] private float m_WalkSpeed;
		[SerializeField] private float m_RunSpeed;
		[SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
		[SerializeField] private float m_JumpSpeed;
		[SerializeField] private float m_StickToGroundForce;
		[SerializeField] private float m_GravityMultiplier;
		[SerializeField] private MouseLook m_MouseLook;
		[SerializeField] private bool m_UseFovKick;
		[SerializeField] private FOVKick m_FovKick = new FOVKick();
		[SerializeField] private bool m_UseHeadBob;
		[SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
		[SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
		[SerializeField] private float m_StepInterval;
		[SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
		[SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
		[SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.



		private Camera m_Camera;
		private bool m_Jump;
		private float m_YRotation;
		private Vector2 m_Input;
		private Vector3 m_MoveDir = Vector3.zero;
		private CharacterController m_CharacterController;
		private CollisionFlags m_CollisionFlags;
		private bool m_PreviouslyGrounded;
		private Vector3 m_OriginalCameraPosition;

		private AudioSource m_AudioSource;
		private Animator m_Animator;
		private Animator m_waepon_Animator;
		private GameObject m_crossbow;
		private Transform m_arrow_spawn_position_top;
		private Transform m_arrow_spawn_position_bottom;

		//External setable
		public GameObject arrow;
		public int arrow_speed;

		private GameObject arrow_1;
		private GameObject arrow_2;

        public int amunition;
        public int health;

		//Variables for ingame states
		private float m_StepCycle;
		private float m_NextStep;
		private bool m_Jumping;
		private bool m_hold_fire_button;
		private bool m_reaload;

        //Mouse look disable option
        public bool MouseLookEnabled;

		// Use this for initialization
		private void Start()
		{
			m_CharacterController = GetComponent<CharacterController>();
			m_Camera = Camera.main;
			m_OriginalCameraPosition = m_Camera.transform.localPosition;
			m_FovKick.Setup(m_Camera);
			m_HeadBob.Setup(m_Camera, m_StepInterval);
			m_StepCycle = 0f;
			m_NextStep = m_StepCycle/2f;
			m_Jumping = false;
			m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);
			m_hold_fire_button = false;
			m_reaload = false;
			m_Animator = GetComponent<Animator>();
			m_crossbow = GameObject.Find("crossbow");
			m_waepon_Animator = GameObject.Find("crossbow").GetComponent<Animator>();
			m_arrow_spawn_position_top = GameObject.Find("arrow_spawn_top").transform;
			m_arrow_spawn_position_bottom = GameObject.Find("arrow_spawn_bottom").transform;
		}


		// Update is called once per frame
		private void Update()
		{

            if (MouseLookEnabled)
            {
                RotateView();
            }
			// the jump state needs to read here to make sure it is not missed
			if (!m_Jump)
			{
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			}

			if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
			{
				StartCoroutine(m_JumpBob.DoBobCycle());
				PlayLandingSound();
				m_MoveDir.y = 0f;
				m_Jumping = false;
			}
			if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
			{
				m_MoveDir.y = 0f;
			}

			m_PreviouslyGrounded = m_CharacterController.isGrounded;

            UpdateAnimator();
            UpdateWaeponAnimator();
        }


		private void PlayLandingSound()
		{
			m_AudioSource.clip = m_LandSound;
			m_AudioSource.Play();
			m_NextStep = m_StepCycle + .5f;
		}


		private void LateUpdate(){

		}

		private void FixedUpdate()
		{

			float speed;
			GetInput(out speed);
			// always move along the camera forward as it is the direction that it being aimed at
			Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

			// get a normal for the surface that is being touched to move along it
			RaycastHit hitInfo;
			Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
				m_CharacterController.height/2f);
			desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

			m_MoveDir.x = desiredMove.x*speed;
			m_MoveDir.z = desiredMove.z*speed;


			if (m_CharacterController.isGrounded)
			{
				m_MoveDir.y = -m_StickToGroundForce;

				if (m_Jump)
				{
					m_MoveDir.y = m_JumpSpeed;
					PlayJumpSound();
					m_Jump = false;
					m_Jumping = true;
				}
			}
			else
			{
				m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
			}
			m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

			ProgressStepCycle(speed);
			UpdateCameraPosition(speed);



		}


		private void PlayJumpSound()
		{
			m_AudioSource.clip = m_JumpSound;
			m_AudioSource.Play();
		}


		private void ProgressStepCycle(float speed)
		{
			if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
			{
				m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
					Time.fixedDeltaTime;
			}

			if (!(m_StepCycle > m_NextStep))
			{
				return;
			}

			m_NextStep = m_StepCycle + m_StepInterval;

			PlayFootStepAudio();
		}


		private void PlayFootStepAudio()
		{
			if (!m_CharacterController.isGrounded)
			{
				return;
			}
			// pick & play a random footstep sound from the array,
			// excluding sound at index 0
			int n = Random.Range(1, m_FootstepSounds.Length);
			m_AudioSource.clip = m_FootstepSounds[n];
			m_AudioSource.PlayOneShot(m_AudioSource.clip);
			// move picked sound to index 0 so it's not picked next time
			m_FootstepSounds[n] = m_FootstepSounds[0];
			m_FootstepSounds[0] = m_AudioSource.clip;
		}
			
		private void UpdateAnimator(){

			m_Animator.SetBool("hold_attack_button",m_hold_fire_button);

		}

		private void UpdateWaeponAnimator(){

			m_waepon_Animator.SetBool("hold_attack_button",m_hold_fire_button);
			m_waepon_Animator.SetBool("reload",m_reaload);
			if(m_reaload == true){
				reload();
			}
		}

		private void UpdateCameraPosition(float speed)
		{
			Vector3 newCameraPosition;
			if (!m_UseHeadBob)
			{
				return;
			}
			if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
			{
				m_Camera.transform.localPosition =
					m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
						(speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
				newCameraPosition = m_Camera.transform.localPosition;
				newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
			}
			else
			{
				newCameraPosition = m_Camera.transform.localPosition;
				newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
			}
			m_Camera.transform.localPosition = newCameraPosition;
		}

        //Set the new arrow type but only when there is a Arrow_Projectile script.
        private void switchArrowType(GameObject arrowType)
        {
            if (arrowType.GetComponent<Arrow_projectile>() != null){
                arrow = arrowType;
            }
        }

		private void fireArrows(){
			if(arrow_1 != null && arrow_2 != null){
				print("fire");
				//First Arrow
				arrow_1.transform.parent = null;
				MeshCollider collider1 = arrow_1.GetComponent<MeshCollider>();
				collider1.enabled = true;
				Rigidbody r1 = arrow_1.GetComponent<Rigidbody>();
				r1.AddForce(m_crossbow.transform.forward *- arrow_speed);
				//r1.velocity = m_crossbow.transform.forward *- arrow_speed;
				r1.useGravity = true;
				arrow_1 = null;

				//Second Arrow
				arrow_2.transform.parent = null;
				MeshCollider collider2 = arrow_2.GetComponent<MeshCollider>();
				collider2.enabled = true;
				Rigidbody r2 = arrow_2.GetComponent<Rigidbody>();
				r2.AddForce(m_crossbow.transform.forward *- arrow_speed);
				//r2.velocity = m_crossbow.transform.forward *- arrow_speed;
				r2.useGravity = true;
				arrow_2 = null;
			}
		}


		private void reload(){
			//Just spawn arrows when there are none others
			if(arrow_1 == null && arrow_2 == null && amunition > 0){
				print("reload");
				arrow_1 = (GameObject) Instantiate(arrow,m_arrow_spawn_position_top.position,m_arrow_spawn_position_top.rotation);
				arrow_1.transform.SetParent(m_arrow_spawn_position_top);
				arrow_1.transform.Rotate(new Vector3(0,90));
				arrow_1.transform.localPosition = new Vector3(arrow_1.transform.localPosition.x,arrow_1.transform.localPosition.y + 0.05f ,arrow_1.transform.localPosition.z - 1f);

				arrow_2 = (GameObject) Instantiate(arrow,m_arrow_spawn_position_bottom.position,m_arrow_spawn_position_bottom.rotation);
				arrow_2.transform.SetParent(m_arrow_spawn_position_bottom);
				arrow_2.transform.Rotate(new Vector3(0,-90));
				arrow_2.transform.localPosition = new Vector3(arrow_2.transform.localPosition.x,arrow_2.transform.localPosition.y + 0.05f  ,arrow_2.transform.localPosition.z + 1f);

                amunition -= 2;
			}
		}

		private void GetInput(out float speed)
		{
			// Read input
			float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
			float vertical = CrossPlatformInputManager.GetAxis("Vertical");
			//TODO Use CrossPlatformInputManager
			m_hold_fire_button = Input.GetMouseButton(0);

            bool arrowTypeSwitch = false;

            //Get the choosen arrow type
            Input.GetKeyDown(KeyCode.Alpha1);
            if (arrowTypeSwitch)
            {
                switchArrowType(arrow);
            }



			if(m_hold_fire_button == true){
				fireArrows();
			}

			m_reaload = Input.GetKeyDown(KeyCode.R);

			bool waswalking = m_IsWalking;

			#if !MOBILE_INPUT
			// On standalone builds, walk/run speed is modified by a key press.
			// keep track of whether or not the character is walking or running
			m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
			#endif
			// set the desired speed to be walking or running
			speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
			m_Input = new Vector2(horizontal, vertical);

			// normalize input if it exceeds 1 in combined length:
			if (m_Input.sqrMagnitude > 1)
			{
				m_Input.Normalize();
			}

			// handle speed change to give an fov kick
			// only if the player is going to a run, is running and the fovkick is to be used
			if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
			{
				StopAllCoroutines();
				StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
			}
		}


		private void RotateView()
		{
			m_MouseLook.LookRotation (transform, m_Camera.transform);
		}


		private void OnControllerColliderHit(ControllerColliderHit hit)
		{
			Rigidbody body = hit.collider.attachedRigidbody;
			//dont move the rigidbody if the character is on top of it
			if (m_CollisionFlags == CollisionFlags.Below)
			{
				return;
			}

			if (body == null || body.isKinematic)
			{
				return;
			}
			body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
		}

        //Refils the ammo
        public void refillArrows(int ammoCount)
        {
            this.amunition += ammoCount;
        }
    }


}
