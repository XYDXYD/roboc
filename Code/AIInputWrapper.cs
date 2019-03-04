using Simulation.SinglePlayer;
using Svelto.ECS.Legacy;
using System;
using UnityEngine;

[Serializable]
public sealed class AIInputWrapper : MonoBehaviour, IMachineControl, IAIInputMovementWrapper, IAIAlignmentRectifierControlComponent, IAIInputWrapper
{
	private Dispatcher<IAIAlignmentRectifierControlComponent, float> _alignmentRectifierComplete;

	private Dispatcher<IAIAlignmentRectifierControlComponent, int> _alignmentRectifierStarted;

	private float _lastCompletedAlignmentRectifierTimestamp = float.MinValue;

	float IAIAlignmentRectifierControlComponent.lastCompletedAlignmentRectifierTimestamp
	{
		get
		{
			return _lastCompletedAlignmentRectifierTimestamp;
		}
	}

	bool IAIAlignmentRectifierControlComponent.alignmentRectifierExecuting
	{
		get;
		set;
	}

	public float horizontalAxis
	{
		get;
		set;
	}

	public float forwardAxis
	{
		get;
		set;
	}

	public bool moveUpwards
	{
		get;
		set;
	}

	public bool moveDown
	{
		get;
		set;
	}

	public float mouseX
	{
		get;
		set;
	}

	public float mouseY
	{
		get;
		set;
	}

	public float fire1
	{
		get;
		set;
	}

	public float fire2
	{
		get;
		set;
	}

	public bool pulseAR
	{
		get;
		set;
	}

	public bool toggleLight
	{
		get;
		set;
	}

	public bool strafeLeft
	{
		get;
		set;
	}

	public bool strafeRight
	{
		get;
		set;
	}

	public bool taunt
	{
		get;
		set;
	}

	public Dispatcher<IAIAlignmentRectifierControlComponent, int> alignmentRectifierStarted => _alignmentRectifierStarted;

	public float lastCompletedAlignmentRectifierTimestamp
	{
		set
		{
			_lastCompletedAlignmentRectifierTimestamp = value;
		}
	}

	public Dispatcher<IAIAlignmentRectifierControlComponent, float> alignmentRectifierComplete => _alignmentRectifierComplete;

	public AIInputWrapper()
		: this()
	{
	}

	private void Awake()
	{
		_alignmentRectifierComplete = new Dispatcher<IAIAlignmentRectifierControlComponent, float>(this);
		_alignmentRectifierStarted = new Dispatcher<IAIAlignmentRectifierControlComponent, int>(this);
	}

	public void Reset()
	{
		horizontalAxis = 0f;
		forwardAxis = 0f;
		moveUpwards = false;
		moveDown = false;
		mouseX = 0f;
		mouseY = 0f;
		fire1 = 0f;
		fire2 = 0f;
		pulseAR = false;
		toggleLight = false;
		strafeLeft = false;
		strafeRight = false;
		taunt = false;
	}
}
