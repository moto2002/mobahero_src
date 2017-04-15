using System;
using UnityEngine;

namespace Newbie
{
	public class NewbieFakeMatchFiveDelayStepEnd : MonoBehaviour
	{
		private bool _isCheckFirHpLessNintyEnd;

		private float _firHpLessNintyEndTimeLen = 60f;

		private float _firHpLessNintyEndStartTime;

		private bool _isCheckFirHpLessThirtyEnd;

		private float _firHpLessThirtyEndTimeLen = 60f;

		private float _firHpLessThirtyEndStartTime;

		private bool _isCheckFirSelUpWayEnd;

		private float _firSelUpWayEndTimeLen = 30f;

		private float _firSelUpWayEndStartTime;

		private bool _isCheckFirSelDownWayEnd;

		private float _firSelDownWayEndTimeLen = 30f;

		private float _firSelDownWayEndStartTime;

		private bool _isCheckFirHeroDeadEnd;

		private float _firHeroDeadEndTimeLen = 60f;

		private float _firHeroDeadEndStartTime;

		private bool _isCheckFirNearFirTowerEnd;

		private float _firNearFirTowerEndTimeLen = 60f;

		private float _firNearFirTowerEndStartTime;

		private bool _isCheckFirFreeCamEnd;

		private float _firFreeCamEndTimeLen = 60f;

		private float _firFreeCamEndStartTime;

		private void Update()
		{
			this.TryCheckFirHpLessNintyEnd();
			this.TryCheckFirHpLessThirtyEnd();
			this.TryCheckFirSelUpWayEnd();
			this.TryCheckFirSelDownWayEnd();
			this.TryCheckFirHeroDeadEnd();
			this.TryCheckFirNearFirTowerEnd();
			this.TryCheckFirFreeCamEnd();
		}

		private void TryCheckFirHpLessNintyEnd()
		{
			if (!this._isCheckFirHpLessNintyEnd)
			{
				return;
			}
			if (Time.time - this._firHpLessNintyEndStartTime > this._firHpLessNintyEndTimeLen)
			{
				this.StopCheckFirHpLessNintyEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessNintyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessNinty);
			}
		}

		private void TryCheckFirHpLessThirtyEnd()
		{
			if (!this._isCheckFirHpLessThirtyEnd)
			{
				return;
			}
			if (Time.time - this._firHpLessThirtyEndStartTime > this._firHpLessThirtyEndTimeLen)
			{
				this.StopCheckFirHpLessThirtyEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHpLessThirtyEnd, true, ENewbieStepType.FakeMatchFive_FirHpLessThirty);
			}
		}

		private void TryCheckFirSelUpWayEnd()
		{
			if (!this._isCheckFirSelUpWayEnd)
			{
				return;
			}
			if (Time.time - this._firSelUpWayEndStartTime > this._firSelUpWayEndTimeLen)
			{
				this.StopCheckFirSelUpWayEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirSelUpWayEnd, true, ENewbieStepType.FakeMatchFive_FirSelUpWay);
			}
		}

		private void TryCheckFirSelDownWayEnd()
		{
			if (!this._isCheckFirSelDownWayEnd)
			{
				return;
			}
			if (Time.time - this._firSelDownWayEndStartTime > this._firSelDownWayEndTimeLen)
			{
				this.StopCheckFirSelDownWayEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirSelDownWayEnd, true, ENewbieStepType.FakeMatchFive_FirSelDownWay);
			}
		}

		private void TryCheckFirHeroDeadEnd()
		{
			if (!this._isCheckFirHeroDeadEnd)
			{
				return;
			}
			if (Time.time - this._firHeroDeadEndStartTime > this._firHeroDeadEndTimeLen)
			{
				this.StopCheckFirHeroDeadEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirHeroDeadEnd, true, ENewbieStepType.FakeMatchFive_FirHeroDead);
			}
		}

		private void TryCheckFirNearFirTowerEnd()
		{
			if (!this._isCheckFirNearFirTowerEnd)
			{
				return;
			}
			if (Time.time - this._firNearFirTowerEndStartTime > this._firNearFirTowerEndTimeLen)
			{
				this.StopCheckFirNearFirTowerEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirNearFirTowerEnd, true, ENewbieStepType.FakeMatchFive_FirNearFirTower);
			}
		}

		private void TryCheckFirFreeCamEnd()
		{
			if (!this._isCheckFirFreeCamEnd)
			{
				return;
			}
			if (Time.time - this._firFreeCamEndStartTime > this._firFreeCamEndTimeLen)
			{
				this.StopCheckFirFreeCamEnd();
				NewbieManager.Instance.MoveCertainStep(ENewbieStepType.FakeMatchFive_FirFreeCamEnd, true, ENewbieStepType.FakeMatchFive_FirFreeCam);
			}
		}

		public void StartCheckFirHpLessNintyEnd()
		{
			this._isCheckFirHpLessNintyEnd = true;
			this._firHpLessNintyEndStartTime = Time.time;
		}

		public void StopCheckFirHpLessNintyEnd()
		{
			this._isCheckFirHpLessNintyEnd = false;
		}

		public void StartCheckFirHpLessThirtyEnd()
		{
			this._isCheckFirHpLessThirtyEnd = true;
			this._firHpLessThirtyEndStartTime = Time.time;
		}

		public void StopCheckFirHpLessThirtyEnd()
		{
			this._isCheckFirHpLessThirtyEnd = false;
		}

		public void StartCheckFirSelUpWayEnd()
		{
			this._isCheckFirSelUpWayEnd = true;
			this._firSelUpWayEndStartTime = Time.time;
		}

		public void StopCheckFirSelUpWayEnd()
		{
			this._isCheckFirSelUpWayEnd = false;
		}

		public void StartCheckFirSelDownWayEnd()
		{
			this._isCheckFirSelDownWayEnd = true;
			this._firSelDownWayEndStartTime = Time.time;
		}

		public void StopCheckFirSelDownWayEnd()
		{
			this._isCheckFirSelDownWayEnd = false;
		}

		public void StartCheckFirHeroDeadEnd()
		{
			this._isCheckFirHeroDeadEnd = true;
			this._firHeroDeadEndStartTime = Time.time;
		}

		public void StopCheckFirHeroDeadEnd()
		{
			this._isCheckFirHeroDeadEnd = false;
		}

		public void StartCheckFirNearFirTowerEnd()
		{
			this._isCheckFirNearFirTowerEnd = true;
			this._firNearFirTowerEndStartTime = Time.time;
		}

		public void StopCheckFirNearFirTowerEnd()
		{
			this._isCheckFirNearFirTowerEnd = false;
		}

		public void StartCheckFirFreeCamEnd()
		{
			this._isCheckFirFreeCamEnd = true;
			this._firFreeCamEndStartTime = Time.time;
		}

		public void StopCheckFirFreeCamEnd()
		{
			this._isCheckFirFreeCamEnd = false;
		}
	}
}
