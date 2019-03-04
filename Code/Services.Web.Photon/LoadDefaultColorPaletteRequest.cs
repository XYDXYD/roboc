using ExitGames.Client.Photon;
using Services.Web.Internal;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections.Generic;

namespace Services.Web.Photon
{
	internal sealed class LoadDefaultColorPaletteRequest : WebServicesRequest<ColorPaletteData>, ILoadDefaultColorPaletteRequest, ITask, IServiceRequest, IAnswerOnComplete<ColorPaletteData>, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		protected override byte OperationCode => 31;

		private event Action<bool> _onComplete;

		public LoadDefaultColorPaletteRequest()
			: base("strRobocloudError", "strUnableToLoadDefaultColorPalette", 3)
		{
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete += action;
			return this;
		}

		public override void Execute()
		{
			if (CacheDTO.defaultColorPalette != null)
			{
				isDone = true;
				progress = 1f;
				if (base.answer != null && base.answer.succeed != null)
				{
					base.answer.succeed(new ColorPaletteData(CacheDTO.defaultColorPalette));
				}
				if (this._onComplete != null)
				{
					this._onComplete(obj: true);
				}
			}
			else
			{
				base.Execute();
			}
		}

		protected override OperationRequest BuildOpRequest()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			OperationRequest val = new OperationRequest();
			val.Parameters = new Dictionary<byte, object>();
			val.OperationCode = OperationCode;
			return val;
		}

		protected override ColorPaletteData ProcessResponse(OperationResponse response)
		{
			byte[] serialisedData = (byte[])response.Parameters[34];
			byte[] order = (byte[])response.Parameters[149];
			ColorPaletteData colorPaletteData = CacheDTO.defaultColorPalette = new ColorPaletteData(serialisedData, order);
			isDone = true;
			progress = 1f;
			if (this._onComplete != null)
			{
				this._onComplete(obj: true);
			}
			return new ColorPaletteData(CacheDTO.defaultColorPalette);
		}
	}
}
