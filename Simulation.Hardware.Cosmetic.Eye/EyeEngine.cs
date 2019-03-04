using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Cosmetic.Eye
{
	internal class EyeEngine : MultiEntityViewsEngine<EyeCatNode, EyeVigiliantNode, EyeCyborgNode>, IQueryingEntityViewEngine, IEngine
	{
		private ITaskRoutine _taskRoutine;

		private Dictionary<int, FasterList<EyeNode>> eyeCatNodesPerMachine;

		private Dictionary<int, FasterList<EyeNode>> eyeVigilantNodesPerMachine;

		private Dictionary<int, FasterList<EyeNode>> eyeCyborgNodesPerMachine;

		private Dictionary<int, EyeLastTimeStore> eyeCatTimesPerMachine;

		private Dictionary<int, EyeLastTimeStore> eyeVigilantTimesPerMachine;

		private Dictionary<int, EyeLastTimeStore> eyeCyborgTimesPerMachine;

		private const float TIME_TO_CLOSE_EYES = 0.1f;

		private const float TIME_BETWEEN_BLINKS_MIN = 0.75f;

		private const float TIME_BETWEEN_BLINKS_MAX = 3f;

		private const float TIME_TO_LOOK = 0.2f;

		private const float TIME_BETWEEN_LOOKS_MIN = 0.5f;

		private const float TIME_BETWEEN_LOOKS_MAX = 2f;

		private const float X_LOOK_OFFSET = 0.5f;

		private const float Y_LOOK_OFFSET = 0.5f;

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public EyeEngine()
		{
			eyeCatNodesPerMachine = new Dictionary<int, FasterList<EyeNode>>();
			eyeVigilantNodesPerMachine = new Dictionary<int, FasterList<EyeNode>>();
			eyeCyborgNodesPerMachine = new Dictionary<int, FasterList<EyeNode>>();
			eyeCatTimesPerMachine = new Dictionary<int, EyeLastTimeStore>();
			eyeVigilantTimesPerMachine = new Dictionary<int, EyeLastTimeStore>();
			eyeCyborgTimesPerMachine = new Dictionary<int, EyeLastTimeStore>();
			TaskRunnerExtensions.Run(Update());
		}

		public void Ready()
		{
		}

		protected override void Add(EyeCatNode node)
		{
			if (!eyeCatNodesPerMachine.ContainsKey(node.hardwareOwnerComponent.machineId))
			{
				FasterList<EyeNode> value = new FasterList<EyeNode>();
				eyeCatNodesPerMachine.Add(node.hardwareOwnerComponent.machineId, value);
				EyeLastTimeStore eyeLastTimeStore = new EyeLastTimeStore();
				eyeLastTimeStore.nextBlinkTime = Random.Range(0.75f, 3f);
				eyeLastTimeStore.nextLookTime = Random.Range(0.5f, 2f);
				eyeCatTimesPerMachine.Add(node.hardwareOwnerComponent.machineId, eyeLastTimeStore);
			}
			eyeCatNodesPerMachine[node.hardwareOwnerComponent.machineId].Add((EyeNode)node);
		}

		protected override void Remove(EyeCatNode node)
		{
			eyeCatNodesPerMachine[node.hardwareOwnerComponent.machineId].Remove((EyeNode)node);
			if (eyeCatNodesPerMachine[node.hardwareOwnerComponent.machineId].get_Count() == 0)
			{
				eyeCatTimesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
				eyeCatNodesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
			}
		}

		protected override void Add(EyeVigiliantNode node)
		{
			if (!eyeVigilantNodesPerMachine.ContainsKey(node.hardwareOwnerComponent.machineId))
			{
				FasterList<EyeNode> value = new FasterList<EyeNode>();
				eyeVigilantNodesPerMachine.Add(node.hardwareOwnerComponent.machineId, value);
				EyeLastTimeStore eyeLastTimeStore = new EyeLastTimeStore();
				eyeLastTimeStore.nextBlinkTime = Random.Range(0.75f, 3f);
				eyeLastTimeStore.nextLookTime = Random.Range(0.5f, 2f);
				eyeVigilantTimesPerMachine.Add(node.hardwareOwnerComponent.machineId, eyeLastTimeStore);
			}
			eyeVigilantNodesPerMachine[node.hardwareOwnerComponent.machineId].Add((EyeNode)node);
		}

		protected override void Remove(EyeVigiliantNode node)
		{
			eyeVigilantNodesPerMachine[node.hardwareOwnerComponent.machineId].Remove((EyeNode)node);
			if (eyeVigilantNodesPerMachine[node.hardwareOwnerComponent.machineId].get_Count() == 0)
			{
				eyeVigilantTimesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
				eyeVigilantNodesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
			}
		}

		protected override void Add(EyeCyborgNode node)
		{
			if (!eyeCyborgNodesPerMachine.ContainsKey(node.hardwareOwnerComponent.machineId))
			{
				FasterList<EyeNode> value = new FasterList<EyeNode>();
				eyeCyborgNodesPerMachine.Add(node.hardwareOwnerComponent.machineId, value);
				EyeLastTimeStore eyeLastTimeStore = new EyeLastTimeStore();
				eyeLastTimeStore.nextBlinkTime = Random.Range(0.75f, 3f);
				eyeLastTimeStore.nextLookTime = Random.Range(0.5f, 2f);
				eyeCyborgTimesPerMachine.Add(node.hardwareOwnerComponent.machineId, eyeLastTimeStore);
			}
			eyeCyborgNodesPerMachine[node.hardwareOwnerComponent.machineId].Add((EyeNode)node);
		}

		protected override void Remove(EyeCyborgNode node)
		{
			eyeCyborgNodesPerMachine[node.hardwareOwnerComponent.machineId].Remove((EyeNode)node);
			if (eyeCyborgNodesPerMachine[node.hardwareOwnerComponent.machineId].get_Count() == 0)
			{
				eyeCyborgTimesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
				eyeCyborgNodesPerMachine.Remove(node.hardwareOwnerComponent.machineId);
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				foreach (int key in eyeCatNodesPerMachine.Keys)
				{
					if (Time.get_time() > eyeCatTimesPerMachine[key].nextBlinkTime)
					{
						eyeCatTimesPerMachine[key].nextBlinkTime = Time.get_time() + Random.Range(0.75f, 3f);
						BlinkEyesInNodeList(eyeCatNodesPerMachine[key]);
					}
					if (Time.get_time() > eyeCatTimesPerMachine[key].nextLookTime)
					{
						eyeCatTimesPerMachine[key].nextLookTime = Time.get_time() + Random.Range(0.5f, 2f);
						LookEyesInNodeList(eyeCatNodesPerMachine[key]);
					}
				}
				foreach (int key2 in eyeVigilantNodesPerMachine.Keys)
				{
					if (Time.get_time() > eyeVigilantTimesPerMachine[key2].nextBlinkTime)
					{
						eyeVigilantTimesPerMachine[key2].nextBlinkTime = Time.get_time() + Random.Range(0.75f, 3f);
						BlinkEyesInNodeList(eyeVigilantNodesPerMachine[key2]);
					}
					if (Time.get_time() > eyeVigilantTimesPerMachine[key2].nextLookTime)
					{
						eyeVigilantTimesPerMachine[key2].nextLookTime = Time.get_time() + Random.Range(0.5f, 2f);
						LookEyesInNodeList(eyeVigilantNodesPerMachine[key2]);
					}
				}
				foreach (int key3 in eyeCyborgNodesPerMachine.Keys)
				{
					if (Time.get_time() > eyeCyborgTimesPerMachine[key3].nextBlinkTime)
					{
						eyeCyborgTimesPerMachine[key3].nextBlinkTime = Time.get_time() + Random.Range(0.75f, 3f);
						BlinkEyesInNodeList(eyeCyborgNodesPerMachine[key3]);
					}
					if (Time.get_time() > eyeCyborgTimesPerMachine[key3].nextLookTime)
					{
						eyeCyborgTimesPerMachine[key3].nextLookTime = Time.get_time() + Random.Range(0.5f, 2f);
						LookEyesInNodeList(eyeCyborgNodesPerMachine[key3]);
					}
				}
				yield return null;
			}
		}

		private void BlinkEyesInNodeList(FasterList<EyeNode> eyeNodes)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<EyeNode> enumerator = eyeNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					EyeNode current = enumerator.get_Current();
					if (!current.visibilityComponent.offScreen && current.hardwareDisabledComponent.enabled)
					{
						TaskRunnerExtensions.Run(BlinkEye(current));
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private IEnumerator BlinkEye(EyeNode eyeNode)
		{
			int numLids = eyeNode.eyeComponent.lids.Length;
			Quaternion[] startLidRotations = (Quaternion[])new Quaternion[numLids];
			Quaternion[] endLidRotations = (Quaternion[])new Quaternion[numLids];
			for (int i = 0; i < numLids; i++)
			{
				startLidRotations[i] = eyeNode.eyeComponent.lids[i].get_localRotation();
				Quaternion val = Quaternion.AngleAxis(eyeNode.eyeComponent.rotateAmounts[i], eyeNode.eyeComponent.axis[i]);
				endLidRotations[i] = startLidRotations[i] * val;
			}
			float blinkStartTime = Time.get_time();
			float timePassed = Time.get_time() - blinkStartTime;
			while (timePassed < 0.2f)
			{
				timePassed = Time.get_time() - blinkStartTime;
				float lerp = Mathf.PingPong(timePassed / 0.1f, 1f);
				for (int j = 0; j < numLids; j++)
				{
					eyeNode.eyeComponent.lids[j].set_localRotation(Quaternion.Lerp(startLidRotations[j], endLidRotations[j], lerp));
				}
				yield return null;
			}
			for (int k = 0; k < numLids; k++)
			{
				eyeNode.eyeComponent.lids[k].set_localRotation(startLidRotations[k]);
			}
		}

		private void LookEyesInNodeList(FasterList<EyeNode> eyeNodes)
		{
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = default(Vector3);
			val._002Ector(1f, Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
			Vector3 normalized = val.get_normalized();
			FasterListEnumerator<EyeNode> enumerator = eyeNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					EyeNode current = enumerator.get_Current();
					if (!current.visibilityComponent.offScreen && current.hardwareDisabledComponent.enabled)
					{
						TaskRunnerExtensions.Run(LookEye(current, normalized));
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private IEnumerator LookEye(EyeNode eyeNode, Vector3 newForward)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			Quaternion currentRotation = eyeNode.eyeComponent.eyeBall.get_localRotation();
			Quaternion desiredRotation = Quaternion.LookRotation(newForward, Vector3.get_up());
			float lookStartTime = Time.get_time();
			float timePassed = Time.get_time() - lookStartTime;
			while (timePassed < 0.2f)
			{
				timePassed = Time.get_time() - lookStartTime;
				float lerp = timePassed / 0.2f;
				eyeNode.eyeComponent.eyeBall.set_localRotation(Quaternion.Lerp(currentRotation, desiredRotation, lerp));
				yield return null;
			}
		}
	}
}
