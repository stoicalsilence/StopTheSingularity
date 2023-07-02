// RigidEnemy
using System;
using UnityEngine;

[RequireComponent(typeof(IkEnemy))]
public class RigidEnemy : MonoBehaviour
{
	public enum EnemyState
	{
		active,
		tumbling,
		falling,
		recovering,
		dead
	}

	public Transform root;

	public Transform head;

	public Transform torso;

	private Rigidbody rb;

	private Rigidbody headRb;

	private Rigidbody torsoRb;

	private float force;

	[HideInInspector]
	public EnemyState state = EnemyState.recovering;

	[HideInInspector]
	public IkEnemy ik;

	private Transform[] groundChecks;

	public bool minOneGrounded;

	private int nLegs;

	public float legPushForce = 0.55f;

	public float groundCheckRadius = 0.2f;

	public float moveLegsWithSpeedScale = 0.25f;

	public float moveSpeed = 10f;

	private float rotationForce = 0.01f;

	public float maxRotationForce = 0.05f;

	private float stabilizeForce = 1f;

	public float recoverTime = 2f;

	public float recoveryForce = 0.3f;

	public float tumbleAngle = 30f;

	public float fallAngle = 70f;

	public float getupMagT = 0.2f;

	public float getupAng = 15f;

	private bool ragdoll;

	[HideInInspector]
	public bool recovering;

	private void Start()
	{
		rb = root.GetComponent<Rigidbody>();
		if ((bool)head)
		{
			headRb = head.GetComponent<Rigidbody>();
		}
		else
		{
			headRb = rb;
		}
		if ((bool)torso)
		{
			torsoRb = torso.GetComponent<Rigidbody>();
		}
		else
		{
			torsoRb = rb;
		}
		ik = GetComponent<IkEnemy>();
		CaluclateForce();
		UpdateState(EnemyState.active);
		nLegs = ik.legs.Length;
		groundChecks = new Transform[nLegs];
		for (int i = 0; i < nLegs; i++)
		{
			groundChecks[i] = ik.legs[i].transform;
		}
		DisableSelfCollision(ignore: true);
	}

	private void FixedUpdate()
	{
		if (state == EnemyState.dead)
		{
			return;
		}
		minOneGrounded = false;
		for (int i = 0; i < nLegs; i++)
		{
			if (Physics.CheckSphere(groundChecks[i].position, groundCheckRadius, ik.whatIsGround))
			{
				minOneGrounded = true;
			}
		}
		float num = 0f;
		if (state == EnemyState.active || state == EnemyState.tumbling || state == EnemyState.recovering || state == EnemyState.falling)
		{
			if (!Physics.Raycast(root.position, Vector3.down, out var hitInfo, ik.heightAboveGround * 3f, ik.whatIsGround))
			{
				UpdateState(EnemyState.falling);
			}
			else
			{
				num = hitInfo.distance;
			}
		}
		float num2 = Vector3.Angle(Vector3.up, root.up);
		if (state == EnemyState.falling)
		{
			if (num != 0f && num < ik.heightAboveGround * 1.5f && num2 < 50f)
			{
				UpdateState(EnemyState.active);
				CancelInvoke("GetUp");
				ConfigureLegs(makeRagdoll: false);
				recovering = false;
			}
			else if (!IsInvoking("GetUp"))
			{
				Invoke("GetUp", recoverTime);
			}
			return;
		}
		if (state == EnemyState.recovering)
		{
			bool flag = Physics.CheckSphere(root.position, 0.5f, ik.whatIsGround);
			if (num < ik.heightAboveGround || flag)
			{
				headRb.AddForce(Vector3.up * force * recoveryForce * 1.1f);
				rb.AddForce(Vector3.up * force * recoveryForce * 0.9f);
			}
			if ((num2 < getupAng && torsoRb.velocity.magnitude < getupMagT) || (num > ik.heightAboveGround * 0.85f && num < ik.heightAboveGround * 1.85f && num2 < 30f))
			{
				UpdateState(EnemyState.active);
				CancelInvoke("RecoveryCooldown");
				Invoke("RecoveryCooldown", 2f);
			}
			return;
		}
		if (state == EnemyState.active && rb.velocity.magnitude < 1f && num > ik.heightAboveGround && num < ik.heightAboveGround + ik.heightAboveGround * 0.1f)
		{
			headRb.AddForce(Vector3.up * force * 0.86f);
			return;
		}
		float num3 = Mathf.Clamp(1f - RootHeight() / ik.heightAboveGround, -1f, 1f);
		if (num2 < tumbleAngle)
		{
			UpdateState(EnemyState.active);
		}
		else if (num2 < fallAngle)
		{
			UpdateState(EnemyState.tumbling);
		}
		else if (num2 > fallAngle)
		{
			UpdateState(EnemyState.falling);
		}
		if (minOneGrounded)
		{
			rb.AddForce(root.up * force * num3 * 2f);
			rb.AddForce(root.up * force * legPushForce);
		}
		if (num < ik.heightAboveGround * 2f)
		{
			StabilizingBody();
		}
	}

	private void RecoveryCooldown()
	{
		recovering = false;
	}

	public void Concuss()
	{
		UpdateState(EnemyState.falling);
		ConfigureLegs(makeRagdoll: true);
		recovering = true;
		Invoke("GetUp", recoverTime * UnityEngine.Random.Range(0.7f, 1.5f));
	}

	private void GetUp()
	{
		if (Physics.CheckSphere(root.position, ik.heightAboveGround * 0.5f, ik.whatIsGround))
		{
			UpdateState(EnemyState.recovering);
			ConfigureLegs(makeRagdoll: false);
		}
		else
		{
			Invoke("GetUp", recoverTime);
		}
	}

	private void ConfigureLegs(bool makeRagdoll)
	{
		if (makeRagdoll == ragdoll)
		{
			return;
		}
		ragdoll = makeRagdoll;
		for (int i = 0; i < ik.legs.Length; i++)
		{
			int num = ik.legs[i].ChainLength;
			Transform parent = ik.legs[i].transform;
			while (num > 0)
			{
				parent = parent.parent;
				if (makeRagdoll)
				{
					parent.gameObject.AddComponent<HingeJoint>().connectedBody = parent.parent.GetComponent<Rigidbody>();
				}
				else
				{
					UnityEngine.Object.Destroy(parent.gameObject.GetComponent<Joint>());
				}
				num--;
			}
			Rigidbody[] componentsInChildren = parent.GetComponentsInChildren<Rigidbody>();
			foreach (Rigidbody obj in componentsInChildren)
			{
				obj.isKinematic = !makeRagdoll;
				obj.interpolation = (makeRagdoll ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
			}
			ik.legs[i].enabled = !makeRagdoll;
			ik.ForceCurrentPosition(i);
		}
	}

	private float RootHeight()
	{
		if (Physics.Raycast(root.position, Vector3.down, out var hitInfo, 10f, ik.whatIsGround))
		{
			return hitInfo.distance;
		}
		return 0f;
	}

	private void StabilizingBody()
	{
		headRb.AddForce(Vector3.up * force * stabilizeForce);
		torsoRb.AddForce(Vector3.down * force * stabilizeForce);
	}

	private void CaluclateForce()
	{
		float num = 0f;
		Rigidbody[] componentsInChildren = GetComponentsInChildren<Rigidbody>();
		foreach (Rigidbody rigidbody in componentsInChildren)
		{
			if (!rigidbody.isKinematic)
			{
				num += rigidbody.mass;
			}
		}
		force = num * (0f - Physics.gravity.y);
	}

	public void RotateBody(Vector3 dir)
	{
		float y = root.transform.eulerAngles.y;
		float y2 = Quaternion.LookRotation(dir).eulerAngles.y;
		float value = Mathf.DeltaAngle(y, y2);
		value = Mathf.Clamp(value, -2f, 2f);
		rb.AddTorque(Vector3.up * value * force * rotationForce);
	}

	public void MoveBody(Vector3 dir)
	{
		rb.AddForce(dir * moveSpeed * rb.mass);
		headRb.AddForce(dir * moveSpeed * headRb.mass);
		torsoRb.AddForce(dir * moveSpeed * torsoRb.mass);
	}

	public void UpdateState(EnemyState s)
	{
		if (state != s)
		{
			state = s;
			switch (s)
			{
				case EnemyState.active:
					ConfigureRb(5f, 5f, maxRotationForce, 1f);
					break;
				case EnemyState.tumbling:
					ConfigureRb(1f, 4f, 0f, 0.1f);
					break;
				case EnemyState.falling:
					ConfigureRb(0f, 0f, 0f, 0f);
					Concuss();
					break;
				case EnemyState.recovering:
					ConfigureRb(4f, 4f, maxRotationForce, 0.15f);
					break;
				case EnemyState.dead:
					ConfigureRb(0f, 0f, 0f, 0f);
					KillRigidEnemy();
					break;
				default:
					rb.drag = 0f;
					rb.angularDrag = 0f;
					break;
			}
		}
	}

	public void KillRigidEnemy()
	{
		DisableSelfCollision(ignore: false);
		ConfigureLegs(makeRagdoll: true);
		CancelInvoke();
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>();
		foreach (Transform transform in componentsInChildren)
		{
			if (transform.CompareTag("GrapplePoint"))
			{
				UnityEngine.Object.Destroy(transform.gameObject);
			}
			transform.tag = "Dead";
		}
		ik.CollectGarbage();
		Destroy(this.gameObject, 10f);
		UnityEngine.Object.Destroy(this);
		UnityEngine.Object.Destroy(ik);
	}

	private void ConfigureRb(float drag, float angularDrag, float rotation, float stabilize)
	{
		if (drag != -1f)
		{
			rb.drag = drag;
			torsoRb.drag = drag;
		}
		if (angularDrag != -1f)
		{
			rb.angularDrag = angularDrag;
			torsoRb.angularDrag = angularDrag;
		}
		if (rotationForce != -1f)
		{
			rotationForce = rotation;
		}
		if (stabilize != -1f)
		{
			stabilizeForce = stabilize;
		}
	}

	public Vector3 GetVelocity()
	{
		if (!rb)
		{
			return Vector3.zero;
		}
		Vector3 result = rb.velocity * moveLegsWithSpeedScale;
		if (result.magnitude > 1f)
		{
			return result.normalized;
		}
		return result;
	}

	private void DisableSelfCollision(bool ignore)
	{
		try
		{
			Collider[] componentsInChildren = GetComponentsInChildren<Collider>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].gameObject.CompareTag("Ignore") && !ignore)
				{
					continue;
				}
				for (int j = i; j < componentsInChildren.Length; j++)
				{
					if (!componentsInChildren[j].gameObject.CompareTag("Ignore") || ignore)
					{
						Physics.IgnoreCollision(componentsInChildren[i], componentsInChildren[j], ignore);
					}
				}
			}
		}
		catch (Exception)
		{
		}
	}
}
