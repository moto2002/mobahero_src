using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
	[RequireComponent(typeof(CharacterController))]
	public class LocalAvoidance : MonoBehaviour
	{
		public enum ResolutionType
		{
			Sampled,
			Geometric
		}

		public struct VOLine
		{
			public LocalAvoidance.VO vo;

			public Vector3 start;

			public Vector3 end;

			public bool inf;

			public int id;

			public bool wrongSide;

			public VOLine(LocalAvoidance.VO vo, Vector3 start, Vector3 end, bool inf, int id, bool wrongSide)
			{
				this.vo = vo;
				this.start = start;
				this.end = end;
				this.inf = inf;
				this.id = id;
				this.wrongSide = wrongSide;
			}
		}

		public struct VOIntersection
		{
			public LocalAvoidance.VO vo1;

			public LocalAvoidance.VO vo2;

			public float factor1;

			public float factor2;

			public bool inside;

			public VOIntersection(LocalAvoidance.VO vo1, LocalAvoidance.VO vo2, float factor1, float factor2, bool inside = false)
			{
				this.vo1 = vo1;
				this.vo2 = vo2;
				this.factor1 = factor1;
				this.factor2 = factor2;
				this.inside = inside;
			}
		}

		public class HalfPlane
		{
			public Vector3 point;

			public Vector3 normal;

			public bool Contains(Vector3 p)
			{
				p -= this.point;
				return Vector3.Dot(this.normal, p) >= 0f;
			}

			public Vector3 ClosestPoint(Vector3 p)
			{
				p -= this.point;
				Vector3 vector = Vector3.Cross(this.normal, Vector3.up);
				float d = Vector3.Dot(vector, p);
				return this.point + vector * d;
			}

			public Vector3 ClosestPoint(Vector3 p, float minX, float maxX)
			{
				p -= this.point;
				Vector3 vector = Vector3.Cross(this.normal, Vector3.up);
				if (vector.x < 0f)
				{
					vector = -vector;
				}
				float num = Vector3.Dot(vector, p);
				float min = (minX - this.point.x) / vector.x;
				float max = (maxX - this.point.x) / vector.x;
				num = Mathf.Clamp(num, min, max);
				return this.point + vector * num;
			}

			public Vector3 Intersection(LocalAvoidance.HalfPlane hp)
			{
				Vector3 dir = Vector3.Cross(this.normal, Vector3.up);
				Vector3 dir2 = Vector3.Cross(hp.normal, Vector3.up);
				return Polygon.IntersectionPointOptimized(this.point, dir, hp.point, dir2);
			}

			public void DrawBounds(float left, float right)
			{
				Vector3 a = Vector3.Cross(this.normal, Vector3.up);
				if (a.x < 0f)
				{
					a = -a;
				}
				float d = (left - this.point.x) / a.x;
				float d2 = (right - this.point.x) / a.x;
				Debug.DrawLine(this.point + a * d + Vector3.up * 0.1f, this.point + a * d2 + Vector3.up * 0.1f, Color.yellow);
			}

			public void Draw()
			{
				Vector3 a = Vector3.Cross(this.normal, Vector3.up);
				Debug.DrawLine(this.point - a * 10f, this.point + a * 10f, Color.blue);
				Debug.DrawRay(this.point, this.normal, new Color(0.8f, 0.1f, 0.2f));
			}
		}

		public enum IntersectionState
		{
			Inside,
			Outside,
			Enter,
			Exit
		}

		public struct IntersectionPair : IComparable<LocalAvoidance.IntersectionPair>
		{
			public float factor;

			public LocalAvoidance.IntersectionState state;

			public IntersectionPair(float factor, bool inside)
			{
				this.factor = factor;
				this.state = ((!inside) ? LocalAvoidance.IntersectionState.Outside : LocalAvoidance.IntersectionState.Inside);
			}

			public void SetState(LocalAvoidance.IntersectionState s)
			{
				this.state = s;
			}

			public int CompareTo(LocalAvoidance.IntersectionPair o)
			{
				if (o.factor < this.factor)
				{
					return 1;
				}
				if (o.factor > this.factor)
				{
					return -1;
				}
				return 0;
			}
		}

		public class VO
		{
			public Vector3 origin;

			public Vector3 direction;

			public float angle;

			public float limit;

			public Vector3 pLeft;

			public Vector3 pRight;

			public Vector3 nLeft;

			public Vector3 nRight;

			public List<LocalAvoidance.IntersectionPair> ints1 = new List<LocalAvoidance.IntersectionPair>();

			public List<LocalAvoidance.IntersectionPair> ints2 = new List<LocalAvoidance.IntersectionPair>();

			public List<LocalAvoidance.IntersectionPair> ints3 = new List<LocalAvoidance.IntersectionPair>();

			public void AddInt(float factor, bool inside, int id)
			{
				switch (id)
				{
				case 1:
					this.ints1.Add(new LocalAvoidance.IntersectionPair(factor, inside));
					break;
				case 2:
					this.ints2.Add(new LocalAvoidance.IntersectionPair(factor, inside));
					break;
				case 3:
					this.ints3.Add(new LocalAvoidance.IntersectionPair(factor, inside));
					break;
				}
			}

			public bool FinalInts(Vector3 target, Vector3 closeEdgeConstraint, bool drawGizmos, out Vector3 closest)
			{
				this.ints1.Sort();
				this.ints2.Sort();
				this.ints3.Sort();
				float num = (float)Math.Atan2((double)this.direction.z, (double)this.direction.x);
				Vector3 vector = Vector3.Cross(this.direction, Vector3.up);
				Vector3 b = vector * (float)Math.Tan((double)this.angle) * this.limit;
				Vector3 vector2 = this.origin + this.direction * this.limit + b;
				Vector3 vector3 = this.origin + this.direction * this.limit - b;
				Vector3 vector4 = vector2 + new Vector3((float)Math.Cos((double)(num + this.angle)), 0f, (float)Math.Sin((double)(num + this.angle))) * 100f;
				Vector3 vector5 = vector3 + new Vector3((float)Math.Cos((double)(num - this.angle)), 0f, (float)Math.Sin((double)(num - this.angle))) * 100f;
				bool flag = false;
				closest = Vector3.zero;
				int num2 = (Vector3.Dot(closeEdgeConstraint - this.origin, vector) <= 0f) ? 1 : 2;
				for (int i = 1; i <= 3; i++)
				{
					if (i != num2)
					{
						List<LocalAvoidance.IntersectionPair> list = (i != 1) ? ((i != 2) ? this.ints3 : this.ints2) : this.ints1;
						Vector3 vector6 = (i != 1 && i != 3) ? vector3 : vector2;
						Vector3 vector7 = (i != 1) ? ((i != 2) ? vector3 : vector5) : vector4;
						float num3 = AstarMath.NearestPointFactor(vector6, vector7, target);
						float num4 = float.PositiveInfinity;
						float num5 = float.NegativeInfinity;
						bool flag2 = false;
						for (int j = 0; j < list.Count - ((i != 3) ? 0 : 1); j++)
						{
							if (drawGizmos)
							{
								Debug.DrawRay(vector6 + (vector7 - vector6) * list[j].factor, Vector3.down, (list[j].state != LocalAvoidance.IntersectionState.Outside) ? Color.red : Color.green);
							}
							if (list[j].state == LocalAvoidance.IntersectionState.Outside && ((j == list.Count - 1 && (j == 0 || list[j - 1].state != LocalAvoidance.IntersectionState.Outside)) || (j < list.Count - 1 && list[j + 1].state == LocalAvoidance.IntersectionState.Outside)))
							{
								flag2 = true;
								float factor = list[j].factor;
								float num6 = (j != list.Count - 1) ? list[j + 1].factor : ((i != 3) ? float.PositiveInfinity : 1f);
								if (drawGizmos)
								{
									Debug.DrawLine(vector6 + (vector7 - vector6) * factor + Vector3.up, vector6 + (vector7 - vector6) * Mathf.Clamp01(num6) + Vector3.up, Color.green);
								}
								if (factor <= num3 && num6 >= num3)
								{
									num4 = num3;
									num5 = num3;
									break;
								}
								if (num6 < num3 && num6 > num5)
								{
									num5 = num6;
								}
								else if (factor > num3 && factor < num4)
								{
									num4 = factor;
								}
							}
						}
						if (flag2)
						{
							float d = (num4 != float.NegativeInfinity) ? ((num5 != float.PositiveInfinity) ? ((Mathf.Abs(num3 - num4) >= Mathf.Abs(num3 - num5)) ? num5 : num4) : num4) : num5;
							Vector3 vector8 = vector6 + (vector7 - vector6) * d;
							if (!flag || (vector8 - target).sqrMagnitude < (closest - target).sqrMagnitude)
							{
								closest = vector8;
							}
							if (drawGizmos)
							{
								Debug.DrawLine(target, closest, Color.yellow);
							}
							flag = true;
						}
					}
				}
				return flag;
			}

			public bool Contains(Vector3 p)
			{
				return Vector3.Dot(this.nLeft, p - this.origin) > 0f && Vector3.Dot(this.nRight, p - this.origin) > 0f && Vector3.Dot(this.direction, p - this.origin) > this.limit;
			}

			public float ScoreContains(Vector3 p)
			{
				return 0f;
			}

			public void Draw(Color c)
			{
				float num = (float)Math.Atan2((double)this.direction.z, (double)this.direction.x);
				Vector3 b = Vector3.Cross(this.direction, Vector3.up) * (float)Math.Tan((double)this.angle) * this.limit;
				Debug.DrawLine(this.origin + this.direction * this.limit + b, this.origin + this.direction * this.limit - b, c);
				Debug.DrawRay(this.origin + this.direction * this.limit + b, new Vector3((float)Math.Cos((double)(num + this.angle)), 0f, (float)Math.Sin((double)(num + this.angle))) * 10f, c);
				Debug.DrawRay(this.origin + this.direction * this.limit - b, new Vector3((float)Math.Cos((double)(num - this.angle)), 0f, (float)Math.Sin((double)(num - this.angle))) * 10f, c);
			}

			public static explicit operator LocalAvoidance.HalfPlane(LocalAvoidance.VO vo)
			{
				return new LocalAvoidance.HalfPlane
				{
					point = vo.origin + vo.direction * vo.limit,
					normal = -vo.direction
				};
			}
		}

		public const float Rad2Deg = 57.29578f;

		private const int maxVOCounter = 50;

		public float speed = 2f;

		public float delta = 1f;

		public float responability = 0.5f;

		public LocalAvoidance.ResolutionType resType = LocalAvoidance.ResolutionType.Geometric;

		private Vector3 velocity;

		public float radius = 0.5f;

		public float maxSpeedScale = 1.5f;

		public Vector3[] samples;

		public float sampleScale = 1f;

		public float circleScale = 0.5f;

		public float circlePoint = 0.5f;

		public bool drawGizmos;

		protected CharacterController controller;

		protected LocalAvoidance[] agents;

		private Vector3 preVelocity;

		private List<LocalAvoidance.VO> vos = new List<LocalAvoidance.VO>();

		private void Start()
		{
			this.controller = base.GetComponent<CharacterController>();
			this.agents = (UnityEngine.Object.FindObjectsOfType(typeof(LocalAvoidance)) as LocalAvoidance[]);
		}

		public void Update()
		{
			this.SimpleMove(base.transform.forward * this.speed);
		}

		public Vector3 GetVelocity()
		{
			return this.preVelocity;
		}

		public void LateUpdate()
		{
			this.preVelocity = this.velocity;
		}

		public void SimpleMove(Vector3 desiredMovement)
		{
			Vector3 b = UnityEngine.Random.insideUnitSphere * 0.1f;
			b.y = 0f;
			Vector3 vector = this.ClampMovement(desiredMovement + b);
			if (vector != Vector3.zero)
			{
				vector /= this.delta;
			}
			if (this.drawGizmos)
			{
				Debug.DrawRay(base.transform.position, desiredMovement, Color.magenta);
				Debug.DrawRay(base.transform.position, vector, Color.yellow);
				Debug.DrawRay(base.transform.position + vector, Vector3.up, Color.yellow);
			}
			this.controller.SimpleMove(vector);
			this.velocity = this.controller.velocity;
			Debug.DrawRay(base.transform.position, this.velocity, Color.blue);
		}

		public Vector3 ClampMovement(Vector3 direction)
		{
			Vector3 vector = direction * this.delta;
			Vector3 vector2 = base.transform.position + direction;
			Vector3 vector3 = vector2;
			float num = 0f;
			int num2 = 0;
			this.vos.Clear();
			float magnitude = this.velocity.magnitude;
			LocalAvoidance[] array = this.agents;
			for (int i = 0; i < array.Length; i++)
			{
				LocalAvoidance localAvoidance = array[i];
				if (!(localAvoidance == this) && !(localAvoidance == null))
				{
					Vector3 vector4 = localAvoidance.transform.position - base.transform.position;
					float magnitude2 = vector4.magnitude;
					float num3 = this.radius + localAvoidance.radius;
					if (magnitude2 <= vector.magnitude * this.delta + num3 + magnitude + localAvoidance.GetVelocity().magnitude)
					{
						if (num2 <= 50)
						{
							num2++;
							LocalAvoidance.VO vO = new LocalAvoidance.VO();
							vO.origin = base.transform.position + Vector3.Lerp(this.velocity * this.delta, localAvoidance.GetVelocity() * this.delta, this.responability);
							vO.direction = vector4.normalized;
							if (num3 > vector4.magnitude)
							{
								vO.angle = 1.57079637f;
							}
							else
							{
								vO.angle = (float)Math.Asin((double)(num3 / magnitude2));
							}
							vO.limit = magnitude2 - num3;
							if (vO.limit < 0f)
							{
								vO.origin += vO.direction * vO.limit;
								vO.limit = 0f;
							}
							float num4 = Mathf.Atan2(vO.direction.z, vO.direction.x);
							vO.pRight = new Vector3(Mathf.Cos(num4 + vO.angle), 0f, Mathf.Sin(num4 + vO.angle));
							vO.pLeft = new Vector3(Mathf.Cos(num4 - vO.angle), 0f, Mathf.Sin(num4 - vO.angle));
							vO.nLeft = new Vector3(Mathf.Cos(num4 + vO.angle - 1.57079637f), 0f, Mathf.Sin(num4 + vO.angle - 1.57079637f));
							vO.nRight = new Vector3(Mathf.Cos(num4 - vO.angle + 1.57079637f), 0f, Mathf.Sin(num4 - vO.angle + 1.57079637f));
							this.vos.Add(vO);
						}
					}
				}
			}
			if (this.resType == LocalAvoidance.ResolutionType.Geometric)
			{
				for (int j = 0; j < this.vos.Count; j++)
				{
					if (this.vos[j].Contains(vector3))
					{
						num = float.PositiveInfinity;
						if (this.drawGizmos)
						{
							Debug.DrawRay(vector3, Vector3.down, Color.red);
						}
						vector3 = base.transform.position;
						break;
					}
				}
				if (this.drawGizmos)
				{
					for (int k = 0; k < this.vos.Count; k++)
					{
						this.vos[k].Draw(Color.black);
					}
				}
				if (num == 0f)
				{
					return vector;
				}
				List<LocalAvoidance.VOLine> list = new List<LocalAvoidance.VOLine>();
				for (int l = 0; l < this.vos.Count; l++)
				{
					LocalAvoidance.VO vO2 = this.vos[l];
					float num5 = (float)Math.Atan2((double)vO2.direction.z, (double)vO2.direction.x);
					Vector3 vector5 = vO2.origin + new Vector3((float)Math.Cos((double)(num5 + vO2.angle)), 0f, (float)Math.Sin((double)(num5 + vO2.angle))) * vO2.limit;
					Vector3 vector6 = vO2.origin + new Vector3((float)Math.Cos((double)(num5 - vO2.angle)), 0f, (float)Math.Sin((double)(num5 - vO2.angle))) * vO2.limit;
					Vector3 end = vector5 + new Vector3((float)Math.Cos((double)(num5 + vO2.angle)), 0f, (float)Math.Sin((double)(num5 + vO2.angle))) * 100f;
					Vector3 end2 = vector6 + new Vector3((float)Math.Cos((double)(num5 - vO2.angle)), 0f, (float)Math.Sin((double)(num5 - vO2.angle))) * 100f;
					int num6 = (!Polygon.Left(vO2.origin, vO2.origin + vO2.direction, base.transform.position + this.velocity)) ? 2 : 1;
					list.Add(new LocalAvoidance.VOLine(vO2, vector5, end, true, 1, num6 == 1));
					list.Add(new LocalAvoidance.VOLine(vO2, vector6, end2, true, 2, num6 == 2));
					list.Add(new LocalAvoidance.VOLine(vO2, vector5, vector6, false, 3, false));
					bool flag = false;
					bool flag2 = false;
					if (!flag)
					{
						for (int m = 0; m < this.vos.Count; m++)
						{
							if (m != l && this.vos[m].Contains(vector5))
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						for (int n = 0; n < this.vos.Count; n++)
						{
							if (n != l && this.vos[n].Contains(vector6))
							{
								flag2 = true;
								break;
							}
						}
					}
					vO2.AddInt(0f, flag, 1);
					vO2.AddInt(0f, flag2, 2);
					vO2.AddInt(0f, flag, 3);
					vO2.AddInt(1f, flag2, 3);
				}
				for (int num7 = 0; num7 < list.Count; num7++)
				{
					for (int num8 = num7 + 1; num8 < list.Count; num8++)
					{
						LocalAvoidance.VOLine vOLine = list[num7];
						LocalAvoidance.VOLine vOLine2 = list[num8];
						if (vOLine.vo != vOLine2.vo)
						{
							float num9;
							float num10;
							if (Polygon.IntersectionFactor(vOLine.start, vOLine.end, vOLine2.start, vOLine2.end, out num9, out num10))
							{
								if (num9 >= 0f && num10 >= 0f && (vOLine.inf || num9 <= 1f) && (vOLine2.inf || num10 <= 1f))
								{
									Vector3 p = vOLine.start + (vOLine.end - vOLine.start) * num9;
									bool flag3 = vOLine.wrongSide || vOLine2.wrongSide;
									if (!flag3)
									{
										for (int num11 = 0; num11 < this.vos.Count; num11++)
										{
											if (this.vos[num11] != vOLine.vo && this.vos[num11] != vOLine2.vo && this.vos[num11].Contains(p))
											{
												flag3 = true;
												break;
											}
										}
									}
									vOLine.vo.AddInt(num9, flag3, vOLine.id);
									vOLine2.vo.AddInt(num10, flag3, vOLine2.id);
									if (this.drawGizmos)
									{
										Debug.DrawRay(vOLine.start + (vOLine.end - vOLine.start) * num9, Vector3.up, (!flag3) ? Color.green : Color.magenta);
									}
								}
							}
						}
					}
				}
				for (int num12 = 0; num12 < this.vos.Count; num12++)
				{
					Vector3 vector7;
					if (this.vos[num12].FinalInts(vector2, base.transform.position + this.velocity, this.drawGizmos, out vector7))
					{
						float sqrMagnitude = (vector7 - vector2).sqrMagnitude;
						if (sqrMagnitude < num)
						{
							vector3 = vector7;
							num = sqrMagnitude;
							if (this.drawGizmos)
							{
								Debug.DrawLine(vector2 + Vector3.up, vector3 + Vector3.up, Color.red);
							}
						}
					}
				}
				if (this.drawGizmos)
				{
					Debug.DrawLine(vector2 + Vector3.up, vector3 + Vector3.up, Color.red);
				}
				return Vector3.ClampMagnitude(vector3 - base.transform.position, vector.magnitude * this.maxSpeedScale);
			}
			else
			{
				if (this.resType == LocalAvoidance.ResolutionType.Sampled)
				{
					Vector3 a = vector;
					Vector3 normalized = a.normalized;
					Vector3 a2 = Vector3.Cross(normalized, Vector3.up);
					int num13 = 10;
					int num14 = 0;
					while (num14 < 10)
					{
						float num15 = (float)(3.1415926535897931 * (double)this.circlePoint / (double)num13);
						float num16 = (float)(3.1415926535897931 - (double)this.circlePoint * 3.1415926535897931) * 0.5f;
						for (int num17 = 0; num17 < num13; num17++)
						{
							float num18 = num15 * (float)num17;
							Vector3 vector8 = base.transform.position + vector - (a * (float)Math.Sin((double)(num18 + num16)) * (float)num14 * this.circleScale + a2 * (float)Math.Cos((double)(num18 + num16)) * (float)num14 * this.circleScale);
							if (this.CheckSample(vector8, this.vos))
							{
								return vector8 - base.transform.position;
							}
						}
						num14++;
						num13 += 2;
					}
					for (int num19 = 0; num19 < this.samples.Length; num19++)
					{
						Vector3 vector9 = base.transform.position + this.samples[num19].x * a2 + this.samples[num19].z * normalized + this.samples[num19].y * a;
						if (this.CheckSample(vector9, this.vos))
						{
							return vector9 - base.transform.position;
						}
					}
					return Vector3.zero;
				}
				return Vector3.zero;
			}
		}

		public bool CheckSample(Vector3 sample, List<LocalAvoidance.VO> vos)
		{
			bool flag = false;
			for (int i = 0; i < vos.Count; i++)
			{
				if (vos[i].Contains(sample))
				{
					if (this.drawGizmos)
					{
						Debug.DrawRay(sample, Vector3.up, Color.red);
					}
					flag = true;
					break;
				}
			}
			if (this.drawGizmos && !flag)
			{
				Debug.DrawRay(sample, Vector3.up, Color.yellow);
			}
			return !flag;
		}
	}
}
