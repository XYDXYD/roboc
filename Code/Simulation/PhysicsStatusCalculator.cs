using Simulation.Hardware.Weapons;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Simulation
{
	internal sealed class PhysicsStatusCalculator
	{
		internal class MachineValues
		{
			public Vector3 centerOfMass;

			public Vector3 inertiaTensor;

			public Quaternion inertiaTensorRotation;

			public float drag;

			public float angularDrag;

			public void SetValues(Vector3 com, Vector3 it, Quaternion itr, float d, float ad)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				centerOfMass = com;
				inertiaTensor = it;
				inertiaTensorRotation = itr;
				drag = d;
				angularDrag = ad;
			}
		}

		private const float INERTIA_TENSOR_DIRECTION = 0.8f;

		private const float INERTIA_TENSOR_BUFFER = 0.2f;

		private readonly float[,] IDENTITY = new float[3, 3]
		{
			{
				1f,
				0f,
				0f
			},
			{
				0f,
				1f,
				0f
			},
			{
				0f,
				0f,
				1f
			}
		};

		private MachineValues _results = new MachineValues();

		private Action<MachineValues> _resultsCallback;

		private FasterList<InstantiatedCube> _processingCubes;

		private Dictionary<InstantiatedCube, Vector3> _relativePositions = new Dictionary<InstantiatedCube, Vector3>();

		private TargetType _targetType;

		public PhysicsStatusCalculator(Action<MachineValues> resultsCallback, int machineSize, TargetType targetType)
		{
			_relativePositions = new Dictionary<InstantiatedCube, Vector3>(machineSize);
			_resultsCallback = resultsCallback;
			_targetType = targetType;
		}

		public void CalculatePhysicsValues(FasterList<InstantiatedCube> processingCubes, bool computeInertiaTensor)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			Quaternion val = Quaternion.get_identity();
			Vector3 it = Vector3.get_one();
			_processingCubes = processingCubes;
			CalculateCenterOfMass(out Vector3 calculatedCenterOfMass, out float calculatedDrag, out float calculatedAngularDrag);
			if (computeInertiaTensor)
			{
				val = CalculateInertiaTensorRotation(calculatedCenterOfMass);
				it = CalculateInertiaTensor(calculatedCenterOfMass, val);
			}
			_results.SetValues(calculatedCenterOfMass, it, val, calculatedDrag, calculatedAngularDrag);
			_resultsCallback(_results);
		}

		private void CalculateCenterOfMass(out Vector3 calculatedCenterOfMass, out float calculatedDrag, out float calculatedAngularDrag)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			calculatedCenterOfMass = Vector3.get_zero();
			calculatedDrag = 0f;
			calculatedAngularDrag = 0f;
			float num = 0f;
			InstantiatedCube[] array = _processingCubes.ToArrayFast();
			for (int i = 0; i < _processingCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = array[i];
				float num2 = instantiatedCube.mass * instantiatedCube.physcisMassScalar;
				num += num2;
				calculatedCenterOfMass += num2 * GetCubePosition(instantiatedCube);
				calculatedDrag += instantiatedCube.drag;
				calculatedAngularDrag += instantiatedCube.angularDrag;
			}
			calculatedCenterOfMass /= num;
			calculatedDrag /= _processingCubes.get_Count();
			calculatedAngularDrag /= _processingCubes.get_Count();
		}

		private Quaternion CalculateInertiaTensorRotation(Vector3 centerOfMass)
		{
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_018c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0191: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0230: Unknown result type (might be due to invalid IL or missing references)
			_relativePositions.Clear();
			InstantiatedCube[] array = _processingCubes.ToArrayFast();
			for (int i = 0; i < _processingCubes.get_Count(); i++)
			{
				InstantiatedCube instantiatedCube = array[i];
				Vector3 cubePosition = GetCubePosition(instantiatedCube);
				Vector3 value = cubePosition - centerOfMass;
				_relativePositions[instantiatedCube] = value;
			}
			Vector3 intertiaRot = Vector3.get_zero();
			for (int j = 0; j < 3; j++)
			{
				for (int k = j + 1; k < 3; k++)
				{
					int num = OtherIndex(j, k);
					float num2 = 0f;
					float num3 = 0f;
					for (int l = 0; l < _processingCubes.get_Count(); l++)
					{
						InstantiatedCube instantiatedCube2 = array[l];
						Vector3 val = _relativePositions[instantiatedCube2];
						float num4 = val.get_Item(j);
						float num5 = val.get_Item(k);
						if (num == 0 || num == 2)
						{
							num4 *= -1f;
						}
						num2 += instantiatedCube2.mass * num4 * num5;
						num3 += instantiatedCube2.mass * (num4 * num4 - num5 * num5);
					}
					num2 *= 2f;
					float num6 = (num2 == 0f) ? 0f : ((num3 != 0f) ? (Mathf.Atan((0f - num2) / num3) * 0.5f) : 0f);
					intertiaRot.set_Item(num, num6 * 57.29578f);
					for (int m = 0; m < _processingCubes.get_Count(); m++)
					{
						InstantiatedCube key = array[m];
						Vector3 value2 = _relativePositions[key];
						float num7 = value2.get_Item(j);
						float num8 = value2.get_Item(k);
						float num9 = Mathf.Cos(num6);
						float num10 = Mathf.Sin(num6);
						value2.set_Item(j, num7 * num9 - num8 * num10);
						value2.set_Item(k, num7 * num10 + num8 * num9);
						_relativePositions[key] = value2;
					}
				}
			}
			ReduceOrthogonalInertiaRotation(ref intertiaRot);
			return Quaternion.Euler(intertiaRot);
		}

		private Vector3 CalculateInertiaTensor(Vector3 centerOfMass, Quaternion inertiaTensorRotation)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = Vector3.get_zero();
			for (int i = 0; i < _processingCubes.get_Count(); i++)
			{
				InstantiatedCube cube = _processingCubes.get_Item(i);
				Vector3 cubePosition = GetCubePosition(cube);
				Vector3 relativePos = cubePosition - centerOfMass;
				Vector3 val2 = CalculateIndividualCubeTensor(cube, relativePos, inertiaTensorRotation);
				val += val2;
			}
			return val;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private void ReduceOrthogonalInertiaRotation(ref Vector3 intertiaRot)
		{
			for (int i = 0; i < 3; i++)
			{
				while (intertiaRot.get_Item(i) <= -45f)
				{
					ReduceAxisRotation(ref intertiaRot, i, 1f);
				}
				while (intertiaRot.get_Item(i) > 45f)
				{
					ReduceAxisRotation(ref intertiaRot, i, -1f);
				}
			}
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private void ReduceAxisRotation(ref Vector3 intertiaRot, int iter, float sign)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			Vector3 zero = Vector3.get_zero();
			zero.set_Item(iter, sign * 90f);
			intertiaRot = Quaternion.Euler(zero) * intertiaRot;
			int num;
			intertiaRot.set_Item(num = iter, intertiaRot.get_Item(num) + sign * 90f);
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private int Index(int i)
		{
			while (i < 0)
			{
				i += 3;
			}
			while (i >= 3)
			{
				i -= 3;
			}
			return i;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private int OtherIndex(int index1, int index2)
		{
			for (int i = 0; i < 3; i++)
			{
				if (i != index1 && i != index2)
				{
					return i;
				}
			}
			return 0;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private float[,] OuterProduct(Vector3 u, Vector3 v)
		{
			float[,] array = new float[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					array[i, j] = u.get_Item(i) * v.get_Item(j);
				}
			}
			return array;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private float[,] MultiplyMatrix(float[,] mat, float f)
		{
			float[,] array = new float[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					array[i, j] = mat[i, j] * f;
				}
			}
			return array;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private float[,] SubtactMatrix(float[,] mat1, float[,] mat2)
		{
			float[,] array = new float[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					array[i, j] = mat1[i, j] - mat2[i, j];
				}
			}
			return array;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private Vector3 MainDiagonal(float[,] mat)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			return new Vector3(mat[0, 0], mat[1, 1], mat[2, 2]);
		}

		private Vector3 CalculateIndividualCubeTensor(InstantiatedCube cube, Vector3 relativePos, Quaternion inertiaTensorRotation)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			Vector3 inertiaTensor = Vector3.get_zero();
			float cubeVolume = GetCubeVolume(cube.colliderInfo);
			if (cubeVolume > 0f)
			{
				int num = cube.colliderInfo.Length;
				for (int i = 0; i < num; i++)
				{
					IColliderData colliderData = cube.colliderInfo[i].colliderData;
					float colliderVolume = GetColliderVolume(colliderData);
					float mass = cube.mass * (colliderVolume / cubeVolume);
					Vector3 v = CubeTensor(mass, colliderData.size);
					Vector3 offset = colliderData.offset;
					Vector3 relativePos2 = Quaternion.Inverse(inertiaTensorRotation) * (relativePos + offset);
					Vector3 v2 = CaluclateParallelPointMass(mass, relativePos2);
					MakePositive(ref v, ref v2);
					Vector3 val = v + v2;
					inertiaTensor += val;
				}
			}
			AddInertiaBuffer(ref inertiaTensor);
			return inertiaTensor;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private Vector3 CaluclateParallelPointMass(float mass, Vector3 relativePos)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			float f = Vector3.Dot(relativePos, relativePos);
			float[,] mat = MultiplyMatrix(IDENTITY, f);
			float[,] mat2 = OuterProduct(relativePos, relativePos);
			float[,] mat3 = SubtactMatrix(mat, mat2);
			float[,] mat4 = MultiplyMatrix(mat3, mass);
			return MainDiagonal(mat4);
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private Vector3 CubeTensor(float mass, Vector3 size)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			float num = mass / 12f;
			Vector3 zero = Vector3.get_zero();
			for (int i = 0; i < 3; i++)
			{
				float num2 = size.get_Item(Index(i + 1));
				float num3 = size.get_Item(Index(i + 2));
				zero.set_Item(i, num * (num2 * num2 + num3 * num3));
			}
			return zero;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private void MakePositive(ref Vector3 v1, ref Vector3 v2)
		{
			for (int i = 0; i < 3; i++)
			{
				v1.set_Item(i, Mathf.Abs(v1.get_Item(i)));
				v2.set_Item(i, Mathf.Abs(v2.get_Item(i)));
			}
		}

		private void AddInertiaBuffer(ref Vector3 inertiaTensor)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			float magnitude = inertiaTensor.get_magnitude();
			Vector3 normalized = inertiaTensor.get_normalized();
			Vector3 val = normalized * magnitude * 0.8f;
			Vector3 one = Vector3.get_one();
			Vector3 val2 = one.get_normalized() * magnitude * 0.2f;
			inertiaTensor = val + val2;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private Vector3 GetCubePosition(InstantiatedCube cube)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			return GridScaleUtility.GridToWorld(cube.gridPos, _targetType) + GridScaleUtility.WorldScale(CubeData.IndexToQuat(cube.rotationIndex) * cube.comOffset, _targetType);
		}

		private float GetCubeVolume(CubeColliderInfo[] colliders)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			float num = 0f;
			for (int i = 0; i < colliders.Length; i++)
			{
				Vector3 size = colliders[i].colliderData.size;
				num += Mathf.Abs(size.x * size.y * size.z);
			}
			return num;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		private float GetColliderVolume(IColliderData collider)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			Vector3 size = collider.size;
			return Mathf.Abs(size.x * size.y * size.z);
		}
	}
}
