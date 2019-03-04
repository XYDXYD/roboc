using Simulation.Hardware.Movement.Rotors;
using Simulation.Hardware.Movement.TankTracks;
using Simulation.Hardware.Movement.Thruster;
using Simulation.Hardware.Movement.Wheeled;
using Simulation.Hardware.Movement.Wheeled.Wheels;
using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Movement
{
	internal sealed class AutomaticButtonAssignment
	{
		private Byte3 pos = Byte3.zero;

		private const float centreBoundary = 0.75f;

		private const float OPERATION_TIME = 0.001f;

		public void PopulateCubeControls(FasterList<Transform> allCubes)
		{
			PopulateMotorAndSteeringControls(allCubes);
			PopulateTankTrackControls(allCubes);
			PopulateJetControls(allCubes);
			PopulateLegControls(allCubes);
			PopulateRotorBladeControls(allCubes);
		}

		private void PopulateMotorAndSteeringControls(FasterList<Transform> allCubes)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			List<WheelComponentImplementor> allWheels = GetAllWheels(allCubes);
			Int3 min = -Int3.one;
			Int3 max = -Int3.one;
			for (int i = 0; i < allWheels.Count; i++)
			{
				WheelComponentImplementor wheelComponentImplementor = allWheels[i];
				pos = GridScaleUtility.WorldToGridByte3(wheelComponentImplementor.get_transform().get_localPosition(), TargetType.Player);
				wheelComponentImplementor.gridPosition = pos;
				UpdateBounds(ref min, ref max, pos);
			}
			int num = (min.z + max.z) / 2;
			float num2 = (float)(min.x + max.x) / 2f;
			for (int j = 0; j < allWheels.Count; j++)
			{
				WheelComponentImplementor wheelComponentImplementor2 = allWheels[j];
				pos = wheelComponentImplementor2.gridPosition;
				if (pos.z < num)
				{
					wheelComponentImplementor2.InitialiseMachineSide(WheelZSide.Rear);
				}
				else
				{
					wheelComponentImplementor2.InitialiseMachineSide(WheelZSide.Front);
				}
				if ((float)(int)pos.x >= num2)
				{
					wheelComponentImplementor2.InitialiseMachineSide(WheelXSide.right);
				}
				else
				{
					wheelComponentImplementor2.InitialiseMachineSide(WheelXSide.left);
				}
			}
		}

		private void PopulateTankTrackControls(FasterList<Transform> allCubes)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			Vector3 right = Vector3.get_right();
			List<TankTrackComponentImplementor> allTankTracks = GetAllTankTracks(allCubes);
			Int3 min = -Int3.one;
			Int3 max = -Int3.one;
			foreach (TankTrackComponentImplementor item in allTankTracks)
			{
				pos = GridScaleUtility.WorldToGridByte3(item.get_transform().get_localPosition(), TargetType.Player);
				pos += new Int3(item.get_transform().get_up() * item.distanceToCentreLine);
				UpdateBounds(ref min, ref max, pos);
				TankTrackGraphicsComponentImplementor component = item.GetComponent<TankTrackGraphicsComponentImplementor>();
				component.SetWheelScrollScale(Mathf.Sign(Vector3.Dot(item.get_transform().get_up(), right)));
			}
			int num = (min.x + max.x) / 2;
			foreach (TankTrackComponentImplementor item2 in allTankTracks)
			{
				pos = GridScaleUtility.WorldToGridByte3(item2.get_transform().get_localPosition(), TargetType.Player);
				pos += new Int3(item2.get_transform().get_up() * item2.distanceToCentreLine);
				bool flag = pos.x == min.x || pos.x == max.x;
				if ((pos.x > num && flag) || pos.x > num + 1)
				{
					item2.InitialiseMachineSide(MachineSide.right);
				}
				else if ((pos.x < num && flag) || pos.x < num - 1)
				{
					item2.InitialiseMachineSide(MachineSide.left);
				}
				else
				{
					item2.InitialiseMachineSide(MachineSide.center);
				}
			}
		}

		private void PopulateJetControls(FasterList<Transform> allCubes)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Int3 min = -Int3.one;
			Int3 max = -Int3.one;
			List<ThrusterComponentImplementor> allJets = GetAllJets(allCubes);
			foreach (ThrusterComponentImplementor item in allJets)
			{
				pos = GridScaleUtility.WorldToGridByte3(item.get_transform().get_localPosition(), TargetType.Player);
				UpdateBounds(ref min, ref max, pos);
			}
			int centreZOffset = (min.z + max.z) / 2;
			float centreXOffset = (float)(min.x + max.x) * 0.5f;
			foreach (ThrusterComponentImplementor item2 in allJets)
			{
				SetJetFacingDirectionRelativeToTransform(item2, centreZOffset, centreXOffset);
			}
		}

		private void SetJetFacingDirectionRelativeToTransform(ThrusterComponentImplementor jet, int centreZOffset, float centreXOffset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			pos = GridScaleUtility.WorldToGridByte3(jet.get_transform().get_localPosition(), TargetType.Player);
			Vector3 direction = jet.direction;
			float num = Vector3.Dot(direction, Vector3.get_forward());
			float num2 = Vector3.Dot(direction, Vector3.get_right());
			float num3 = Vector3.Dot(direction, Vector3.get_up());
			if (num > 0.9f)
			{
				jet.SetFacingDirection(CubeFace.Front);
			}
			else if (num < -0.9f)
			{
				jet.SetFacingDirection(CubeFace.Back);
			}
			else if (num3 > 0.9f)
			{
				CubeFace cubeFace = CubeFace.Up;
				CubeFace pitchFacingDirection = (pos.z < centreZOffset) ? CubeFace.Down : CubeFace.Up;
				jet.SetFacingDirection(cubeFace, cubeFace, pitchFacingDirection);
			}
			else if (num3 < -0.9f)
			{
				CubeFace cubeFace2 = CubeFace.Down;
				CubeFace pitchFacingDirection2 = (pos.z >= centreZOffset) ? CubeFace.Down : CubeFace.Up;
				jet.SetFacingDirection(cubeFace2, cubeFace2, pitchFacingDirection2);
			}
			else if (num2 > 0.9f)
			{
				CubeFace cubeFace3 = CubeFace.Right;
				CubeFace legacyFacingDirection = (pos.z < centreZOffset) ? CubeFace.Left : CubeFace.Right;
				jet.SetFacingDirection(legacyFacingDirection, cubeFace3, cubeFace3);
			}
			else if (num2 < -0.9f)
			{
				CubeFace cubeFace4 = CubeFace.Left;
				CubeFace legacyFacingDirection2 = (pos.z < centreZOffset) ? CubeFace.Right : CubeFace.Left;
				jet.SetFacingDirection(legacyFacingDirection2, cubeFace4, cubeFace4);
			}
		}

		private void UpdateBounds(ref Int3 min, ref Int3 max, Byte3 pos)
		{
			if (min == -Int3.one)
			{
				min.x = (max.x = pos.x);
				min.z = (max.z = pos.z);
				return;
			}
			min.z = Mathf.Min(min.z, (int)pos.z);
			max.z = Mathf.Max(max.z, (int)pos.z);
			min.x = Mathf.Max(min.x, (int)pos.x);
			max.x = Mathf.Min(max.x, (int)pos.x);
		}

		private List<WheelComponentImplementor> GetAllWheels(FasterList<Transform> allCubes)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			WheelComponentImplementor wheelComponentImplementor = null;
			List<WheelComponentImplementor> list = new List<WheelComponentImplementor>();
			FasterListEnumerator<Transform> enumerator = allCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = enumerator.get_Current();
					wheelComponentImplementor = current.GetComponent<WheelComponentImplementor>();
					if (wheelComponentImplementor != null)
					{
						list.Add(wheelComponentImplementor);
					}
				}
				return list;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private List<TankTrackComponentImplementor> GetAllTankTracks(FasterList<Transform> allCubes)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			TankTrackComponentImplementor tankTrackComponentImplementor = null;
			List<TankTrackComponentImplementor> list = new List<TankTrackComponentImplementor>();
			FasterListEnumerator<Transform> enumerator = allCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = enumerator.get_Current();
					tankTrackComponentImplementor = current.GetComponent<TankTrackComponentImplementor>();
					if (tankTrackComponentImplementor != null)
					{
						list.Add(tankTrackComponentImplementor);
					}
				}
				return list;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private List<ThrusterComponentImplementor> GetAllJets(FasterList<Transform> allCubes)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			ThrusterComponentImplementor thrusterComponentImplementor = null;
			List<ThrusterComponentImplementor> list = new List<ThrusterComponentImplementor>();
			FasterListEnumerator<Transform> enumerator = allCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = enumerator.get_Current();
					thrusterComponentImplementor = current.GetComponent<ThrusterComponentImplementor>();
					if (thrusterComponentImplementor != null)
					{
						list.Add(thrusterComponentImplementor);
					}
				}
				return list;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private List<CubeLeg> GetAllLegs(FasterList<Transform> allCubes)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			CubeLeg cubeLeg = null;
			List<CubeLeg> list = new List<CubeLeg>();
			FasterListEnumerator<Transform> enumerator = allCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = enumerator.get_Current();
					cubeLeg = current.GetComponent<CubeLeg>();
					if (cubeLeg != null)
					{
						cubeLeg.UpdateCachedValues();
						list.Add(cubeLeg);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			list.Sort(delegate(CubeLeg a, CubeLeg b)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = a.position;
				ref float z = ref position.z;
				Vector3 position2 = b.position;
				return z.CompareTo(position2.z);
			});
			return list;
		}

		private List<CubeMechLeg> GetAllMechLegs(FasterList<Transform> allCubes)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			CubeMechLeg cubeMechLeg = null;
			List<CubeMechLeg> list = new List<CubeMechLeg>();
			FasterListEnumerator<Transform> enumerator = allCubes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform current = enumerator.get_Current();
					cubeMechLeg = current.GetComponent<CubeMechLeg>();
					if (cubeMechLeg != null)
					{
						cubeMechLeg.UpdateCachedValues();
						list.Add(cubeMechLeg);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			list.Sort(delegate(CubeMechLeg a, CubeMechLeg b)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0014: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = a.position;
				ref float z = ref position.z;
				Vector3 position2 = b.position;
				return z.CompareTo(position2.z);
			});
			return list;
		}

		private List<RotorBladeComponentImplementor> GetAllRotors(FasterList<Transform> allCubes)
		{
			RotorBladeComponentImplementor rotorBladeComponentImplementor = null;
			List<RotorBladeComponentImplementor> list = new List<RotorBladeComponentImplementor>();
			for (int i = 0; i < allCubes.get_Count(); i++)
			{
				Transform val = allCubes.get_Item(i);
				rotorBladeComponentImplementor = val.GetComponent<RotorBladeComponentImplementor>();
				if (rotorBladeComponentImplementor != null)
				{
					list.Add(rotorBladeComponentImplementor);
				}
			}
			return list;
		}

		private void PopulateLegControls(FasterList<Transform> allCubes)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			Int3 min = -Int3.one;
			Int3 max = -Int3.one;
			List<CubeLeg> allLegs = GetAllLegs(allCubes);
			List<CubeMechLeg> allMechLegs = GetAllMechLegs(allCubes);
			foreach (CubeLeg item in allLegs)
			{
				pos = GridScaleUtility.WorldToGridByte3(item.get_transform().get_localPosition(), TargetType.Player);
				UpdateBounds(ref min, ref max, pos);
			}
			UpdateLegSyncGroups(allLegs);
			UpdateMechLegSyncGroups(allMechLegs);
			int centreZOffset = (min.z + max.z) / 2;
			foreach (CubeLeg item2 in allLegs)
			{
				SetLegFacingDirectionRelativeToTransform(item2, centreZOffset);
			}
		}

		private void UpdateLegSyncGroups(List<CubeLeg> allLegs)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			bool flag = true;
			bool flag2 = false;
			List<CubeLeg> list = new List<CubeLeg>();
			int num2;
			for (num2 = 0; num2 < allLegs.Count; num2++)
			{
				Vector3 position = allLegs[num2].position;
				float z = position.z;
				list.Clear();
				for (int i = num2; i < allLegs.Count; i++)
				{
					Vector3 position2 = allLegs[i].position;
					if (Mathf.Abs(position2.z - z) > -0.001f && Mathf.Abs(position2.z - z) < 0.001f)
					{
						list.Add(allLegs[i]);
						continue;
					}
					break;
				}
				list.Sort(delegate(CubeLeg a, CubeLeg b)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					Vector3 position3 = a.position;
					ref float x = ref position3.x;
					Vector3 position4 = b.position;
					return x.CompareTo(position4.x);
				});
				if (flag)
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].legGraphics.syncGroup = num;
						if (++num > 1)
						{
							num = 0;
						}
					}
				}
				else
				{
					for (int num3 = list.Count - 1; num3 >= 0; num3--)
					{
						list[num3].legGraphics.syncGroup = num;
						if (++num > 1)
						{
							num = 0;
						}
					}
				}
				if (flag2 || list.Count > 1)
				{
					flag = !flag;
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
				num2 += list.Count - 1;
			}
		}

		private void UpdateMechLegSyncGroups(List<CubeMechLeg> allLegs)
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			bool flag = true;
			bool flag2 = false;
			int num2 = 0;
			num2 = ((allLegs.Count != 3) ? 1 : 2);
			List<CubeMechLeg> list = new List<CubeMechLeg>();
			int num3;
			for (num3 = 0; num3 < allLegs.Count; num3++)
			{
				Vector3 position = allLegs[num3].position;
				float z = position.z;
				list.Clear();
				for (int i = num3; i < allLegs.Count; i++)
				{
					Vector3 position2 = allLegs[i].position;
					if (Mathf.Abs(position2.z - z) > -0.001f && Mathf.Abs(position2.z - z) < 0.001f)
					{
						list.Add(allLegs[i]);
						continue;
					}
					break;
				}
				list.Sort(delegate(CubeMechLeg a, CubeMechLeg b)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0006: Unknown result type (might be due to invalid IL or missing references)
					//IL_000f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0014: Unknown result type (might be due to invalid IL or missing references)
					Vector3 position3 = a.position;
					ref float x = ref position3.x;
					Vector3 position4 = b.position;
					return x.CompareTo(position4.x);
				});
				if (flag)
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].legGraphics.syncGroup = num;
						if (++num > num2)
						{
							num = 0;
						}
					}
				}
				else
				{
					for (int num4 = list.Count - 1; num4 >= 0; num4--)
					{
						list[num4].legGraphics.syncGroup = num;
						if (++num > num2)
						{
							num = 0;
						}
					}
				}
				if (flag2 || list.Count > 1)
				{
					flag = !flag;
					flag2 = false;
				}
				else
				{
					flag2 = true;
				}
				num3 += list.Count - 1;
			}
		}

		private void SetLegFacingDirectionRelativeToTransform(CubeLeg leg, int centreZOffset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			pos = GridScaleUtility.WorldToGridByte3(leg.get_transform().get_localPosition(), TargetType.Player);
			float num = Vector3.Dot(leg.forward, Vector3.get_forward());
			float num2 = Vector3.Dot(leg.forward, Vector3.get_right());
			float num3 = Vector3.Dot(leg.forward, Vector3.get_up());
			if (num3 > 0.9f)
			{
				leg.legData.facingDirection = CubeFace.Up;
			}
			else if (num3 < -0.9f)
			{
				leg.legData.facingDirection = CubeFace.Down;
			}
			else if (num > 0.9f)
			{
				leg.legData.facingDirection = CubeFace.Front;
				if (pos.z >= centreZOffset)
				{
					leg.legData.xInputFlipped = false;
				}
				else
				{
					leg.legData.xInputFlipped = true;
				}
			}
			else if (num < -0.9f)
			{
				leg.legData.facingDirection = CubeFace.Back;
				if (pos.z >= centreZOffset)
				{
					leg.legData.xInputFlipped = false;
				}
				else
				{
					leg.legData.xInputFlipped = true;
				}
			}
			else if (num2 > 0.9f)
			{
				leg.legData.facingDirection = CubeFace.Right;
				if (pos.z >= centreZOffset)
				{
					leg.legData.xInputFlipped = false;
				}
				else
				{
					leg.legData.xInputFlipped = true;
				}
			}
			else if (num2 < -0.9f)
			{
				leg.legData.facingDirection = CubeFace.Left;
				if (pos.z >= centreZOffset)
				{
					leg.legData.xInputFlipped = false;
				}
				else
				{
					leg.legData.xInputFlipped = true;
				}
			}
		}

		private void PopulateRotorBladeControls(FasterList<Transform> allCubes)
		{
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			Int3 min = -Int3.one;
			Int3 max = -Int3.one;
			List<RotorBladeComponentImplementor> allRotors = GetAllRotors(allCubes);
			for (int i = 0; i < allRotors.Count; i++)
			{
				RotorBladeComponentImplementor rotorBladeComponentImplementor = allRotors[i];
				pos = GridScaleUtility.WorldToGridByte3(rotorBladeComponentImplementor.get_transform().get_localPosition(), TargetType.Player);
				UpdateBounds(ref min, ref max, pos);
			}
			int centreZOffset = (min.z + max.z) / 2;
			for (int j = 0; j < allRotors.Count; j++)
			{
				RotorBladeComponentImplementor rotor = allRotors[j];
				SetRotorFacingDirectionRelativeToTransform(rotor, centreZOffset);
			}
		}

		private void SetRotorFacingDirectionRelativeToTransform(RotorBladeComponentImplementor rotor, int centreZOffset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			pos = GridScaleUtility.WorldToGridByte3(rotor.get_transform().get_localPosition(), TargetType.Player);
			float num = Vector3.Dot(rotor.get_transform().get_up(), Vector3.get_forward());
			float num2 = Vector3.Dot(rotor.get_transform().get_up(), Vector3.get_right());
			float num3 = Vector3.Dot(rotor.get_transform().get_up(), Vector3.get_up());
			if (num > 0.9f)
			{
				rotor.rotorData.facingDirection = CubeFace.Front;
			}
			else if (num < -0.9f)
			{
				rotor.rotorData.facingDirection = CubeFace.Back;
			}
			else if (num3 > 0.9f)
			{
				rotor.rotorData.facingDirection = CubeFace.Up;
				if (pos.z < centreZOffset)
				{
					rotor.rotorData.xInputFlipped = true;
				}
			}
			else if (num3 < -0.9f)
			{
				rotor.rotorData.facingDirection = CubeFace.Down;
				if (pos.z >= centreZOffset)
				{
					rotor.rotorData.xInputFlipped = true;
				}
			}
			else if (num2 > 0.9f)
			{
				if (pos.z >= centreZOffset)
				{
					rotor.rotorData.facingDirection = CubeFace.Right;
				}
				else
				{
					rotor.rotorData.facingDirection = CubeFace.Left;
				}
			}
			else if (num2 < -0.9f)
			{
				if (pos.z >= centreZOffset)
				{
					rotor.rotorData.facingDirection = CubeFace.Left;
				}
				else
				{
					rotor.rotorData.facingDirection = CubeFace.Right;
				}
			}
		}
	}
}
