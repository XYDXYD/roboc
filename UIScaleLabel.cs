using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("NGUI/UI/NGUI Scale Label")]
public class UIScaleLabel : UIWidget
{
	public delegate string ModifierFunc(string s);

	public Crispness keepCrispWhenShrunk = 1;

	[HideInInspector]
	[SerializeField]
	private Font mTrueTypeFont;

	[HideInInspector]
	[SerializeField]
	private UIFont mFont;

	[Multiline(6)]
	[HideInInspector]
	[SerializeField]
	private string mText = string.Empty;

	[HideInInspector]
	[SerializeField]
	private int mFontSize = 16;

	[HideInInspector]
	[SerializeField]
	private FontStyle mFontStyle;

	[HideInInspector]
	[SerializeField]
	private Alignment mAlignment;

	[HideInInspector]
	[SerializeField]
	private bool mEncoding = true;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineCount;

	[HideInInspector]
	[SerializeField]
	private Effect mEffectStyle;

	[HideInInspector]
	[SerializeField]
	private Color mEffectColor = Color.get_black();

	[HideInInspector]
	[SerializeField]
	private SymbolStyle mSymbols = 1;

	[HideInInspector]
	[SerializeField]
	private Vector2 mEffectDistance = Vector2.get_one();

	[HideInInspector]
	[SerializeField]
	private Overflow mOverflow;

	[HideInInspector]
	[SerializeField]
	private bool mApplyGradient;

	[HideInInspector]
	[SerializeField]
	private Color mGradientTop = Color.get_white();

	[HideInInspector]
	[SerializeField]
	private Color mGradientBottom = new Color(0.7f, 0.7f, 0.7f);

	[HideInInspector]
	[SerializeField]
	private int mSpacingX;

	[HideInInspector]
	[SerializeField]
	private int mSpacingY;

	[HideInInspector]
	[SerializeField]
	private bool mUseFloatSpacing;

	[HideInInspector]
	[SerializeField]
	private float mFloatSpacingX;

	[HideInInspector]
	[SerializeField]
	private float mFloatSpacingY;

	[HideInInspector]
	[SerializeField]
	private bool mOverflowEllipsis;

	[HideInInspector]
	[SerializeField]
	private int mOverflowWidth;

	[HideInInspector]
	[SerializeField]
	private Modifier mModifier;

	[HideInInspector]
	[SerializeField]
	private bool mShrinkToFit;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineWidth;

	[HideInInspector]
	[SerializeField]
	private int mMaxLineHeight;

	[HideInInspector]
	[SerializeField]
	private float mLineWidth;

	[HideInInspector]
	[SerializeField]
	private bool mMultiline = true;

	[NonSerialized]
	private Font mActiveTTF;

	[NonSerialized]
	private float mDensity = 1f;

	[NonSerialized]
	private bool mShouldBeProcessed = true;

	[NonSerialized]
	private string mProcessedText;

	[NonSerialized]
	private bool mPremultiply;

	[NonSerialized]
	private Vector2 mCalculatedSize = Vector2.get_zero();

	[NonSerialized]
	private float mScale = 1f;

	[NonSerialized]
	private int mFinalFontSize;

	[NonSerialized]
	private int mLastWidth;

	[NonSerialized]
	private int mLastHeight;

	[HideInInspector]
	[SerializeField]
	private int mTargetFontSize;

	[HideInInspector]
	[SerializeField]
	private int mTargetVerticalResolutionPx;

	[HideInInspector]
	[SerializeField]
	private int mTargetVerticalResolutionPxMinY = 768;

	[HideInInspector]
	[SerializeField]
	private int mTargetVerticalResolutionPxMaxY = 2160;

	public ModifierFunc customModifier;

	private static BetterList<UIScaleLabel> mList = new BetterList<UIScaleLabel>();

	private static Dictionary<Font, int> mFontUsage = new Dictionary<Font, int>();

	[NonSerialized]
	private static BetterList<UIDrawCall> mTempDrawcalls;

	private static bool mTexRebuildAdded = false;

	private static List<Vector3> mTempVerts = new List<Vector3>();

	private static List<int> mTempIndices = new List<int>();

	[CompilerGenerated]
	private static Action<Font> _003C_003Ef__mg_0024cache0;

	public int finalFontSize
	{
		get
		{
			if (Object.op_Implicit(trueTypeFont))
			{
				return Mathf.RoundToInt(mScale * (float)mFinalFontSize);
			}
			return Mathf.RoundToInt((float)mFinalFontSize * mScale);
		}
	}

	private bool shouldBeProcessed
	{
		get
		{
			return mShouldBeProcessed;
		}
		set
		{
			if (value)
			{
				base.mChanged = true;
				mShouldBeProcessed = true;
			}
			else
			{
				mShouldBeProcessed = false;
			}
		}
	}

	public override bool isAnchoredHorizontally => this.get_isAnchoredHorizontally() || (int)mOverflow == 2;

	public override bool isAnchoredVertically => this.get_isAnchoredVertically() || (int)mOverflow == 2 || (int)mOverflow == 3;

	public override Material material
	{
		get
		{
			if (base.mMat != null)
			{
				return base.mMat;
			}
			if (mFont != null)
			{
				return mFont.get_material();
			}
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont.get_material();
			}
			return null;
		}
		set
		{
			this.set_material(value);
		}
	}

	public override Texture mainTexture
	{
		get
		{
			if (mFont != null)
			{
				return mFont.get_texture();
			}
			if (mTrueTypeFont != null)
			{
				Material material = mTrueTypeFont.get_material();
				if (material != null)
				{
					return material.get_mainTexture();
				}
			}
			return null;
		}
		set
		{
			this.set_mainTexture(value);
		}
	}

	[Obsolete("Use UIScaleLabel.bitmapFont instead")]
	public UIFont font
	{
		get
		{
			return bitmapFont;
		}
		set
		{
			bitmapFont = value;
		}
	}

	public UIFont bitmapFont
	{
		get
		{
			return mFont;
		}
		set
		{
			if (mFont != value)
			{
				this.RemoveFromPanel();
				mFont = value;
				mTrueTypeFont = null;
				this.MarkAsChanged();
			}
		}
	}

	public Font trueTypeFont
	{
		get
		{
			if (mTrueTypeFont != null)
			{
				return mTrueTypeFont;
			}
			return (!(mFont != null)) ? null : mFont.get_dynamicFont();
		}
		set
		{
			if (mTrueTypeFont != value)
			{
				SetActiveFont(null);
				this.RemoveFromPanel();
				mTrueTypeFont = value;
				shouldBeProcessed = true;
				mFont = null;
				SetActiveFont(value);
				ProcessAndRequest();
				if (mActiveTTF != null)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public Object ambigiousFont
	{
		get
		{
			return ((object)mFont) ?? ((object)mTrueTypeFont);
		}
		set
		{
			UIFont val = value as UIFont;
			if (val != null)
			{
				bitmapFont = val;
			}
			else
			{
				trueTypeFont = (value as Font);
			}
		}
	}

	public string text
	{
		get
		{
			return mText;
		}
		set
		{
			if (mText == value)
			{
				return;
			}
			if (string.IsNullOrEmpty(value))
			{
				if (!string.IsNullOrEmpty(mText))
				{
					mText = string.Empty;
					this.MarkAsChanged();
					ProcessAndRequest();
				}
			}
			else if (mText != value)
			{
				mText = value;
				this.MarkAsChanged();
				ProcessAndRequest();
			}
			if (base.autoResizeBoxCollider)
			{
				this.ResizeCollider();
			}
		}
	}

	public int defaultFontSize => (trueTypeFont != null) ? mFontSize : ((!(mFont != null)) ? 16 : mFont.get_defaultSize());

	public int fontSize
	{
		get
		{
			return mFontSize;
		}
		set
		{
			value = Mathf.Clamp(value, 0, 256);
			if (mFontSize != value)
			{
				mFontSize = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public FontStyle fontStyle
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mFontStyle;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mFontStyle != value)
			{
				mFontStyle = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public Alignment alignment
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mAlignment;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mAlignment != value)
			{
				mAlignment = value;
				shouldBeProcessed = true;
				ProcessAndRequest();
			}
		}
	}

	public bool applyGradient
	{
		get
		{
			return mApplyGradient;
		}
		set
		{
			if (mApplyGradient != value)
			{
				mApplyGradient = value;
				this.MarkAsChanged();
			}
		}
	}

	public Color gradientTop
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mGradientTop;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mGradientTop != value)
			{
				mGradientTop = value;
				if (mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public Color gradientBottom
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mGradientBottom;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mGradientBottom != value)
			{
				mGradientBottom = value;
				if (mApplyGradient)
				{
					this.MarkAsChanged();
				}
			}
		}
	}

	public int spacingX
	{
		get
		{
			return mSpacingX;
		}
		set
		{
			if (mSpacingX != value)
			{
				mSpacingX = value;
				this.MarkAsChanged();
			}
		}
	}

	public int spacingY
	{
		get
		{
			return mSpacingY;
		}
		set
		{
			if (mSpacingY != value)
			{
				mSpacingY = value;
				this.MarkAsChanged();
			}
		}
	}

	public bool useFloatSpacing
	{
		get
		{
			return mUseFloatSpacing;
		}
		set
		{
			if (mUseFloatSpacing != value)
			{
				mUseFloatSpacing = value;
				shouldBeProcessed = true;
			}
		}
	}

	public float floatSpacingX
	{
		get
		{
			return mFloatSpacingX;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingX, value))
			{
				mFloatSpacingX = value;
				this.MarkAsChanged();
			}
		}
	}

	public float floatSpacingY
	{
		get
		{
			return mFloatSpacingY;
		}
		set
		{
			if (!Mathf.Approximately(mFloatSpacingY, value))
			{
				mFloatSpacingY = value;
				this.MarkAsChanged();
			}
		}
	}

	public float effectiveSpacingY => (!mUseFloatSpacing) ? ((float)mSpacingY) : mFloatSpacingY;

	public float effectiveSpacingX => (!mUseFloatSpacing) ? ((float)mSpacingX) : mFloatSpacingX;

	public bool overflowEllipsis
	{
		get
		{
			return mOverflowEllipsis;
		}
		set
		{
			if (mOverflowEllipsis != value)
			{
				mOverflowEllipsis = value;
				this.MarkAsChanged();
			}
		}
	}

	public int overflowWidth
	{
		get
		{
			return mOverflowWidth;
		}
		set
		{
			if (mOverflowWidth != value)
			{
				mOverflowWidth = value;
				this.MarkAsChanged();
			}
		}
	}

	private bool keepCrisp
	{
		get
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (trueTypeFont != null && (int)keepCrispWhenShrunk != 0)
			{
				return true;
			}
			return false;
		}
	}

	public bool supportEncoding
	{
		get
		{
			return mEncoding;
		}
		set
		{
			if (mEncoding != value)
			{
				mEncoding = value;
				shouldBeProcessed = true;
			}
		}
	}

	public SymbolStyle symbolStyle
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mSymbols;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mSymbols != value)
			{
				mSymbols = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Overflow overflowMethod
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mOverflow;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mOverflow != value)
			{
				mOverflow = value;
				shouldBeProcessed = true;
			}
		}
	}

	[Obsolete("Use 'width' instead")]
	public int lineWidth
	{
		get
		{
			return this.get_width();
		}
		set
		{
			this.set_width(value);
		}
	}

	[Obsolete("Use 'height' instead")]
	public int lineHeight
	{
		get
		{
			return this.get_height();
		}
		set
		{
			this.set_height(value);
		}
	}

	public bool multiLine
	{
		get
		{
			return mMaxLineCount != 1;
		}
		set
		{
			if (mMaxLineCount != 1 != value)
			{
				mMaxLineCount = ((!value) ? 1 : 0);
				shouldBeProcessed = true;
			}
		}
	}

	public override Vector3[] localCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return this.get_localCorners();
		}
	}

	public override Vector3[] worldCorners
	{
		get
		{
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return this.get_worldCorners();
		}
	}

	public override Vector4 drawingDimensions
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return this.get_drawingDimensions();
		}
	}

	public int maxLineCount
	{
		get
		{
			return mMaxLineCount;
		}
		set
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			if (mMaxLineCount != value)
			{
				mMaxLineCount = Mathf.Max(value, 0);
				shouldBeProcessed = true;
				if ((int)overflowMethod == 0)
				{
					this.MakePixelPerfect();
				}
			}
		}
	}

	public Effect effectStyle
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEffectStyle;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mEffectStyle != value)
			{
				mEffectStyle = value;
				shouldBeProcessed = true;
			}
		}
	}

	public Color effectColor
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEffectColor;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (mEffectColor != value)
			{
				mEffectColor = value;
				if ((int)mEffectStyle != 0)
				{
					shouldBeProcessed = true;
				}
			}
		}
	}

	public Vector2 effectDistance
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mEffectDistance;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			if (mEffectDistance != value)
			{
				mEffectDistance = value;
				shouldBeProcessed = true;
			}
		}
	}

	public int quadsPerCharacter
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Invalid comparison between Unknown and I4
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Invalid comparison between Unknown and I4
			if ((int)mEffectStyle == 1)
			{
				return 2;
			}
			if ((int)mEffectStyle == 2)
			{
				return 5;
			}
			if ((int)mEffectStyle == 3)
			{
				return 9;
			}
			return 1;
		}
	}

	[Obsolete("Use 'overflowMethod == UIScaleLabel.Overflow.ShrinkContent' instead")]
	public bool shrinkToFit
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Invalid comparison between Unknown and I4
			return (int)mOverflow == 0;
		}
		set
		{
			if (value)
			{
				overflowMethod = 0;
			}
		}
	}

	public string processedText
	{
		get
		{
			if (mLastWidth != base.mWidth || mLastHeight != base.mHeight)
			{
				mLastWidth = base.mWidth;
				mLastHeight = base.mHeight;
				mShouldBeProcessed = true;
			}
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mProcessedText;
		}
	}

	public Vector2 printedSize
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return mCalculatedSize;
		}
	}

	public override Vector2 localSize
	{
		get
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (shouldBeProcessed)
			{
				ProcessText();
			}
			return this.get_localSize();
		}
	}

	private bool isValid => mFont != null || mTrueTypeFont != null;

	public Modifier modifier
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return mModifier;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (mModifier != value)
			{
				mModifier = value;
				this.MarkAsChanged();
				ProcessAndRequest();
			}
		}
	}

	public string printedText
	{
		get
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Invalid comparison between Unknown and I4
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Invalid comparison between Unknown and I4
			if (!string.IsNullOrEmpty(mText))
			{
				if ((int)mModifier == 0)
				{
					return mText;
				}
				if ((int)mModifier == 2)
				{
					return mText.ToLower();
				}
				if ((int)mModifier == 1)
				{
					return mText.ToUpper();
				}
				if ((int)mModifier == 255 && customModifier != null)
				{
					return customModifier(mText);
				}
			}
			return mText;
		}
	}

	public UIScaleLabel()
		: this()
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0022: Unknown result type (might be due to invalid IL or missing references)
	//IL_0027: Unknown result type (might be due to invalid IL or missing references)
	//IL_002e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0034: Unknown result type (might be due to invalid IL or missing references)
	//IL_0039: Unknown result type (might be due to invalid IL or missing references)
	//IL_003f: Unknown result type (might be due to invalid IL or missing references)
	//IL_0044: Unknown result type (might be due to invalid IL or missing references)
	//IL_0059: Unknown result type (might be due to invalid IL or missing references)
	//IL_005e: Unknown result type (might be due to invalid IL or missing references)
	//IL_007d: Unknown result type (might be due to invalid IL or missing references)
	//IL_0082: Unknown result type (might be due to invalid IL or missing references)


	public void ReCalcDynamicFontSize()
	{
		int num = mFontSize;
		int num2 = mTargetFontSize;
		int height = Screen.get_height();
		if (height < mTargetVerticalResolutionPxMinY)
		{
			height = mTargetVerticalResolutionPxMinY;
		}
		if (height > mTargetVerticalResolutionPxMaxY)
		{
			height = mTargetVerticalResolutionPxMaxY;
		}
		float num3 = (float)height / (float)mTargetVerticalResolutionPx;
		num2 = (int)Math.Round(num3 * (float)mTargetFontSize, MidpointRounding.ToEven);
		if (num2 != mFontSize)
		{
			mFontSize = num2;
		}
	}

	protected override void OnInit()
	{
		this.OnInit();
		mList.Add(this);
		SetActiveFont(trueTypeFont);
	}

	protected override void OnDisable()
	{
		SetActiveFont(null);
		mList.Remove(this);
		this.OnDisable();
	}

	protected void SetActiveFont(Font fnt)
	{
		if (!(mActiveTTF != fnt))
		{
			return;
		}
		Font val = mActiveTTF;
		if (val != null && mFontUsage.TryGetValue(val, out int value))
		{
			value = Mathf.Max(0, --value);
			if (value == 0)
			{
				mFontUsage.Remove(val);
			}
			else
			{
				mFontUsage[val] = value;
			}
		}
		mActiveTTF = fnt;
		if (fnt != null)
		{
			int num = 0;
			num = (mFontUsage[fnt] = num + 1);
		}
	}

	private static void OnFontChanged(Font font)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < mList.size; i++)
		{
			UIScaleLabel uIScaleLabel = mList.get_Item(i);
			if (!(uIScaleLabel != null))
			{
				continue;
			}
			Font trueTypeFont = uIScaleLabel.trueTypeFont;
			if (trueTypeFont == font)
			{
				trueTypeFont.RequestCharactersInTexture(uIScaleLabel.mText, uIScaleLabel.mFinalFontSize, uIScaleLabel.mFontStyle);
				uIScaleLabel.MarkAsChanged();
				if (uIScaleLabel.panel == null)
				{
					uIScaleLabel.CreatePanel();
				}
				if (mTempDrawcalls == null)
				{
					mTempDrawcalls = new BetterList<UIDrawCall>();
				}
				if (uIScaleLabel.drawCall != null && !mTempDrawcalls.Contains(uIScaleLabel.drawCall))
				{
					mTempDrawcalls.Add(uIScaleLabel.drawCall);
				}
			}
		}
		if (mTempDrawcalls == null)
		{
			return;
		}
		int j = 0;
		for (int size = mTempDrawcalls.size; j < size; j++)
		{
			UIDrawCall val = mTempDrawcalls.get_Item(j);
			if (val.panel != null)
			{
				val.panel.FillDrawCall(val);
			}
		}
		mTempDrawcalls.Clear();
	}

	public override Vector3[] GetSides(Transform relativeTo)
	{
		if (shouldBeProcessed)
		{
			ProcessText();
		}
		return this.GetSides(relativeTo);
	}

	protected override void OnAnchor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Invalid comparison between Unknown and I4
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if ((int)mOverflow == 2)
		{
			if (this.get_isFullyAnchored())
			{
				mOverflow = 0;
			}
		}
		else if ((int)mOverflow == 3 && base.topAnchor.target != null && base.bottomAnchor.target != null)
		{
			mOverflow = 0;
		}
		this.OnAnchor();
	}

	private void ProcessAndRequest()
	{
		if (ambigiousFont != null)
		{
			ProcessText();
		}
	}

	protected override void OnEnable()
	{
		this.OnEnable();
		if (!mTexRebuildAdded)
		{
			mTexRebuildAdded = true;
			Font.add_textureRebuilt((Action<Font>)OnFontChanged);
		}
	}

	protected override void OnStart()
	{
		this.OnStart();
		if (mLineWidth > 0f)
		{
			mMaxLineWidth = Mathf.RoundToInt(mLineWidth);
			mLineWidth = 0f;
		}
		if (!mMultiline)
		{
			mMaxLineCount = 1;
			mMultiline = true;
		}
		mPremultiply = (this.get_material() != null && this.get_material().get_shader() != null && this.get_material().get_shader().get_name()
			.Contains("Premultiplied"));
		ProcessAndRequest();
	}

	public override void MarkAsChanged()
	{
		shouldBeProcessed = true;
		this.MarkAsChanged();
	}

	public void ProcessText(bool legacyMode = false, bool full = true)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Invalid comparison between Unknown and I4
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Invalid comparison between Unknown and I4
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Invalid comparison between Unknown and I4
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Invalid comparison between Unknown and I4
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Invalid comparison between Unknown and I4
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Invalid comparison between Unknown and I4
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		if (!isValid)
		{
			return;
		}
		ReCalcDynamicFontSize();
		base.mChanged = true;
		shouldBeProcessed = false;
		float num = base.mDrawRegion.z - base.mDrawRegion.x;
		float num2 = base.mDrawRegion.w - base.mDrawRegion.y;
		NGUIText.rectWidth = ((!legacyMode) ? this.get_width() : ((mMaxLineWidth == 0) ? 1000000 : mMaxLineWidth));
		NGUIText.rectHeight = ((!legacyMode) ? this.get_height() : ((mMaxLineHeight == 0) ? 1000000 : mMaxLineHeight));
		NGUIText.regionWidth = ((num == 1f) ? NGUIText.rectWidth : Mathf.RoundToInt((float)NGUIText.rectWidth * num));
		NGUIText.regionHeight = ((num2 == 1f) ? NGUIText.rectHeight : Mathf.RoundToInt((float)NGUIText.rectHeight * num2));
		int num3;
		if (legacyMode)
		{
			Vector3 localScale = this.get_cachedTransform().get_localScale();
			num3 = Mathf.RoundToInt(localScale.x);
		}
		else
		{
			num3 = defaultFontSize;
		}
		mFinalFontSize = Mathf.Abs(num3);
		mScale = 1f;
		if (NGUIText.regionWidth < 1 || NGUIText.regionHeight < 0)
		{
			mProcessedText = string.Empty;
			return;
		}
		bool flag = trueTypeFont != null;
		if (flag && this.keepCrisp)
		{
			UIRoot root = this.get_root();
			if (root != null)
			{
				mDensity = ((!(root != null)) ? 1f : root.get_pixelSizeAdjustment());
			}
		}
		else
		{
			mDensity = 1f;
		}
		if (full)
		{
			UpdateNGUIText();
		}
		if ((int)mOverflow == 2)
		{
			NGUIText.rectWidth = 1000000;
			NGUIText.regionWidth = 1000000;
			if (mOverflowWidth > 0)
			{
				NGUIText.rectWidth = Mathf.Min(NGUIText.rectWidth, mOverflowWidth);
				NGUIText.regionWidth = Mathf.Min(NGUIText.regionWidth, mOverflowWidth);
			}
		}
		if ((int)mOverflow == 2 || (int)mOverflow == 3)
		{
			NGUIText.rectHeight = 1000000;
			NGUIText.regionHeight = 1000000;
		}
		if (mFinalFontSize > 0)
		{
			bool keepCrisp = this.keepCrisp;
			int num4 = mFinalFontSize;
			while (num4 > 0)
			{
				if (keepCrisp)
				{
					mFinalFontSize = num4;
					NGUIText.fontSize = mFinalFontSize;
				}
				else
				{
					mScale = (float)num4 / (float)mFinalFontSize;
					NGUIText.fontScale = ((!flag) ? ((float)mFontSize / (float)mFont.get_defaultSize() * mScale) : mScale);
				}
				NGUIText.Update(false);
				bool flag2 = NGUIText.WrapText(printedText, ref mProcessedText, false, false, (int)mOverflow == 1 && mOverflowEllipsis);
				if ((int)mOverflow == 0 && !flag2)
				{
					if (--num4 <= 1)
					{
						break;
					}
					num4--;
					continue;
				}
				if ((int)mOverflow == 2)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					int num5 = Mathf.Max(this.get_minWidth(), Mathf.RoundToInt(mCalculatedSize.x));
					if (num != 1f)
					{
						num5 = Mathf.RoundToInt((float)num5 / num);
					}
					int num6 = Mathf.Max(this.get_minHeight(), Mathf.RoundToInt(mCalculatedSize.y));
					if (num2 != 1f)
					{
						num6 = Mathf.RoundToInt((float)num6 / num2);
					}
					if ((num5 & 1) == 1)
					{
						num5++;
					}
					if ((num6 & 1) == 1)
					{
						num6++;
					}
					if (base.mWidth != num5 || base.mHeight != num6)
					{
						base.mWidth = num5;
						base.mHeight = num6;
						if (base.onChange != null)
						{
							base.onChange.Invoke();
						}
					}
				}
				else if ((int)mOverflow == 3)
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
					int num7 = Mathf.Max(this.get_minHeight(), Mathf.RoundToInt(mCalculatedSize.y));
					if (num2 != 1f)
					{
						num7 = Mathf.RoundToInt((float)num7 / num2);
					}
					if ((num7 & 1) == 1)
					{
						num7++;
					}
					if (base.mHeight != num7)
					{
						base.mHeight = num7;
						if (base.onChange != null)
						{
							base.onChange.Invoke();
						}
					}
				}
				else
				{
					mCalculatedSize = NGUIText.CalculatePrintedSize(mProcessedText);
				}
				if (legacyMode)
				{
					this.set_width(Mathf.RoundToInt(mCalculatedSize.x));
					this.set_height(Mathf.RoundToInt(mCalculatedSize.y));
					this.get_cachedTransform().set_localScale(Vector3.get_one());
				}
				break;
			}
		}
		else
		{
			this.get_cachedTransform().set_localScale(Vector3.get_one());
			mProcessedText = string.Empty;
			mScale = 1f;
		}
		if (full)
		{
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
	}

	public override void MakePixelPerfect()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		if (ambigiousFont != null)
		{
			Vector3 localPosition = this.get_cachedTransform().get_localPosition();
			localPosition.x = Mathf.RoundToInt(localPosition.x);
			localPosition.y = Mathf.RoundToInt(localPosition.y);
			localPosition.z = Mathf.RoundToInt(localPosition.z);
			this.get_cachedTransform().set_localPosition(localPosition);
			this.get_cachedTransform().set_localScale(Vector3.get_one());
			if ((int)mOverflow == 2)
			{
				AssumeNaturalSize();
				return;
			}
			int width = this.get_width();
			int height = this.get_height();
			Overflow val = mOverflow;
			if ((int)val != 3)
			{
				base.mWidth = 100000;
			}
			base.mHeight = 100000;
			mOverflow = 0;
			ProcessText();
			mOverflow = val;
			int num = Mathf.RoundToInt(mCalculatedSize.x);
			int num2 = Mathf.RoundToInt(mCalculatedSize.y);
			num = Mathf.Max(num, this.get_minWidth());
			num2 = Mathf.Max(num2, this.get_minHeight());
			if ((num & 1) == 1)
			{
				num++;
			}
			if ((num2 & 1) == 1)
			{
				num2++;
			}
			base.mWidth = Mathf.Max(width, num);
			base.mHeight = Mathf.Max(height, num2);
			this.MarkAsChanged();
		}
		else
		{
			this.MakePixelPerfect();
		}
	}

	public void AssumeNaturalSize()
	{
		if (ambigiousFont != null)
		{
			base.mWidth = 100000;
			base.mHeight = 100000;
			ProcessText();
			base.mWidth = Mathf.RoundToInt(mCalculatedSize.x);
			base.mHeight = Mathf.RoundToInt(mCalculatedSize.y);
			if ((base.mWidth & 1) == 1)
			{
				base.mWidth++;
			}
			if ((base.mHeight & 1) == 1)
			{
				base.mHeight++;
			}
			this.MarkAsChanged();
		}
	}

	[Obsolete("Use UIScaleLabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetCharacterIndexAtPosition(worldPos, precise: false);
	}

	[Obsolete("Use UIScaleLabel.GetCharacterAtPosition instead")]
	public int GetCharacterIndex(Vector2 localPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetCharacterIndexAtPosition(localPos, precise: false);
	}

	public int GetCharacterIndexAtPosition(Vector3 worldPos, bool precise)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector2 localPos = Vector2.op_Implicit(this.get_cachedTransform().InverseTransformPoint(worldPos));
		return GetCharacterIndexAtPosition(localPos, precise);
	}

	public int GetCharacterIndexAtPosition(Vector2 localPos, bool precise)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			UpdateNGUIText();
			if (precise)
			{
				NGUIText.PrintExactCharacterPositions(processedText, mTempVerts, mTempIndices);
			}
			else
			{
				NGUIText.PrintApproximateCharacterPositions(processedText, mTempVerts, mTempIndices);
			}
			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int result = (!precise) ? NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, localPos) : NGUIText.GetExactCharacterIndex(mTempVerts, mTempIndices, localPos);
				mTempVerts.Clear();
				mTempIndices.Clear();
				NGUIText.bitmapFont = null;
				NGUIText.dynamicFont = null;
				return result;
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
		}
		return 0;
	}

	public string GetWordAtPosition(Vector3 worldPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int characterIndexAtPosition = GetCharacterIndexAtPosition(worldPos, precise: true);
		return GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	public string GetWordAtPosition(Vector2 localPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int characterIndexAtPosition = GetCharacterIndexAtPosition(localPos, precise: true);
		return GetWordAtCharacterIndex(characterIndexAtPosition);
	}

	public string GetWordAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length)
		{
			int num = printedText.LastIndexOfAny(new char[2]
			{
				' ',
				'\n'
			}, characterIndex) + 1;
			int num2 = printedText.IndexOfAny(new char[4]
			{
				' ',
				'\n',
				',',
				'.'
			}, characterIndex);
			if (num2 == -1)
			{
				num2 = printedText.Length;
			}
			if (num != num2)
			{
				int num3 = num2 - num;
				if (num3 > 0)
				{
					string text = printedText.Substring(num, num3);
					return NGUIText.StripSymbols(text);
				}
			}
		}
		return null;
	}

	public string GetUrlAtPosition(Vector3 worldPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(worldPos, precise: true));
	}

	public string GetUrlAtPosition(Vector2 localPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetUrlAtCharacterIndex(GetCharacterIndexAtPosition(localPos, precise: true));
	}

	public string GetUrlAtCharacterIndex(int characterIndex)
	{
		string printedText = this.printedText;
		if (characterIndex != -1 && characterIndex < printedText.Length - 6)
		{
			int num = (printedText[characterIndex] != '[' || printedText[characterIndex + 1] != 'u' || printedText[characterIndex + 2] != 'r' || printedText[characterIndex + 3] != 'l' || printedText[characterIndex + 4] != '=') ? printedText.LastIndexOf("[url=", characterIndex) : characterIndex;
			if (num == -1)
			{
				return null;
			}
			num += 5;
			int num2 = printedText.IndexOf("]", num);
			if (num2 == -1)
			{
				return null;
			}
			int num3 = printedText.IndexOf("[/url]", num2);
			if (num3 == -1 || characterIndex <= num3)
			{
				return printedText.Substring(num, num2 - num);
			}
		}
		return null;
	}

	public int GetCharacterIndex(int currentIndex, KeyCode key)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Invalid comparison between Unknown and I4
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Invalid comparison between Unknown and I4
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Invalid comparison between Unknown and I4
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Invalid comparison between Unknown and I4
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Invalid comparison between Unknown and I4
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Invalid comparison between Unknown and I4
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Invalid comparison between Unknown and I4
		if (isValid)
		{
			string processedText = this.processedText;
			if (string.IsNullOrEmpty(processedText))
			{
				return 0;
			}
			int defaultFontSize = this.defaultFontSize;
			UpdateNGUIText();
			NGUIText.PrintApproximateCharacterPositions(processedText, mTempVerts, mTempIndices);
			if (mTempVerts.Count > 0)
			{
				ApplyOffset(mTempVerts, 0);
				int i = 0;
				for (int count = mTempIndices.Count; i < count; i++)
				{
					if (mTempIndices[i] == currentIndex)
					{
						Vector2 val = Vector2.op_Implicit(mTempVerts[i]);
						if ((int)key == 273)
						{
							val.y += (float)defaultFontSize + effectiveSpacingY;
						}
						else if ((int)key == 274)
						{
							val.y -= (float)defaultFontSize + effectiveSpacingY;
						}
						else if ((int)key == 278)
						{
							val.x -= 1000f;
						}
						else if ((int)key == 279)
						{
							val.x += 1000f;
						}
						int approximateCharacterIndex = NGUIText.GetApproximateCharacterIndex(mTempVerts, mTempIndices, val);
						if (approximateCharacterIndex == currentIndex)
						{
							break;
						}
						mTempVerts.Clear();
						mTempIndices.Clear();
						return approximateCharacterIndex;
					}
				}
				mTempVerts.Clear();
				mTempIndices.Clear();
			}
			NGUIText.bitmapFont = null;
			NGUIText.dynamicFont = null;
			if ((int)key == 273 || (int)key == 278)
			{
				return 0;
			}
			if ((int)key == 274 || (int)key == 279)
			{
				return processedText.Length;
			}
		}
		return currentIndex;
	}

	public void PrintOverlay(int start, int end, UIGeometry caret, UIGeometry highlight, Color caretColor, Color highlightColor)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (caret != null)
		{
			caret.Clear();
		}
		if (highlight != null)
		{
			highlight.Clear();
		}
		if (!isValid)
		{
			return;
		}
		string processedText = this.processedText;
		UpdateNGUIText();
		int count = caret.verts.Count;
		Vector2 item = default(Vector2);
		item._002Ector(0.5f, 0.5f);
		float finalAlpha = base.finalAlpha;
		if (highlight != null && start != end)
		{
			int count2 = highlight.verts.Count;
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, highlight.verts);
			if (highlight.verts.Count > count2)
			{
				ApplyOffset(highlight.verts, count2);
				Color item2 = default(Color);
				item2._002Ector(highlightColor.r, highlightColor.g, highlightColor.b, highlightColor.a * finalAlpha);
				int i = count2;
				for (int count3 = highlight.verts.Count; i < count3; i++)
				{
					highlight.uvs.Add(item);
					highlight.cols.Add(item2);
				}
			}
		}
		else
		{
			NGUIText.PrintCaretAndSelection(processedText, start, end, caret.verts, (List<Vector3>)null);
		}
		ApplyOffset(caret.verts, count);
		Color item3 = default(Color);
		item3._002Ector(caretColor.r, caretColor.g, caretColor.b, caretColor.a * finalAlpha);
		int j = count;
		for (int count4 = caret.verts.Count; j < count4; j++)
		{
			caret.uvs.Add(item);
			caret.cols.Add(item3);
		}
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
	}

	public override void OnFill(List<Vector3> verts, List<Vector2> uvs, List<Color> cols)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Invalid comparison between Unknown and I4
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Invalid comparison between Unknown and I4
		if (!isValid)
		{
			return;
		}
		int num = verts.Count;
		Color val = this.get_color();
		val.a = base.finalAlpha;
		if (mFont != null && mFont.get_premultipliedAlphaShader())
		{
			val = NGUITools.ApplyPMA(val);
		}
		string processedText = this.processedText;
		int count = verts.Count;
		UpdateNGUIText();
		NGUIText.tint = val;
		NGUIText.Print(processedText, verts, uvs, cols);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		Vector2 val2 = ApplyOffset(verts, count);
		if (mFont != null && mFont.get_packedFontShader())
		{
			return;
		}
		if ((int)effectStyle != 0)
		{
			int count2 = verts.Count;
			val2.x = mEffectDistance.x;
			val2.y = mEffectDistance.y;
			ApplyShadow(verts, uvs, cols, num, count2, val2.x, 0f - val2.y);
			if ((int)effectStyle == 2 || (int)effectStyle == 3)
			{
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, 0f - val2.x, val2.y);
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, val2.x, val2.y);
				num = count2;
				count2 = verts.Count;
				ApplyShadow(verts, uvs, cols, num, count2, 0f - val2.x, 0f - val2.y);
				if ((int)effectStyle == 3)
				{
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f - val2.x, 0f);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, val2.x, 0f);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f, val2.y);
					num = count2;
					count2 = verts.Count;
					ApplyShadow(verts, uvs, cols, num, count2, 0f, 0f - val2.y);
				}
			}
		}
		if (base.onPostFill != null)
		{
			base.onPostFill.Invoke(this, num, verts, uvs, cols);
		}
	}

	public Vector2 ApplyOffset(List<Vector3> verts, int start)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Vector2 pivotOffset = this.get_pivotOffset();
		float num = Mathf.Lerp(0f, (float)(-base.mWidth), pivotOffset.x);
		float num2 = Mathf.Lerp((float)base.mHeight, 0f, pivotOffset.y) + Mathf.Lerp(mCalculatedSize.y - (float)base.mHeight, 0f, pivotOffset.y);
		num = Mathf.Round(num);
		num2 = Mathf.Round(num2);
		int i = start;
		for (int count = verts.Count; i < count; i++)
		{
			Vector3 value = verts[i];
			value.x += num;
			value.y += num2;
			verts[i] = value;
		}
		return new Vector2(num, num2);
	}

	public void ApplyShadow(List<Vector3> verts, List<Vector2> uvs, List<Color> cols, int start, int end, float x, float y)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Color val = mEffectColor;
		val.a *= base.finalAlpha;
		if (bitmapFont != null && bitmapFont.get_premultipliedAlphaShader())
		{
			val = NGUITools.ApplyPMA(val);
		}
		Color value = val;
		for (int i = start; i < end; i++)
		{
			verts.Add(verts[i]);
			uvs.Add(uvs[i]);
			cols.Add(cols[i]);
			Vector3 value2 = verts[i];
			value2.x += x;
			value2.y += y;
			verts[i] = value2;
			Color val2 = cols[i];
			if (val2.a == 1f)
			{
				cols[i] = value;
				continue;
			}
			Color value3 = val;
			value3.a = val2.a * val.a;
			cols[i] = value3;
		}
	}

	public int CalculateOffsetToFit(string text)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		UpdateNGUIText();
		NGUIText.encoding = false;
		NGUIText.symbolStyle = 0;
		int result = NGUIText.CalculateOffsetToFit(text);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	public void SetCurrentProgress()
	{
		if (UIProgressBar.current != null)
		{
			text = UIProgressBar.current.get_value().ToString("F");
		}
	}

	public void SetCurrentPercent()
	{
		if (UIProgressBar.current != null)
		{
			text = Mathf.RoundToInt(UIProgressBar.current.get_value() * 100f) + "%";
		}
	}

	public void SetCurrentSelection()
	{
		if (UIPopupList.current != null)
		{
			text = ((!UIPopupList.current.isLocalized) ? UIPopupList.current.get_value() : Localization.Get(UIPopupList.current.get_value(), true));
		}
	}

	public bool Wrap(string text, out string final)
	{
		return Wrap(text, out final, 1000000);
	}

	public bool Wrap(string text, out string final, int height)
	{
		UpdateNGUIText();
		NGUIText.rectHeight = height;
		NGUIText.regionHeight = height;
		bool result = NGUIText.WrapText(text, ref final, false);
		NGUIText.bitmapFont = null;
		NGUIText.dynamicFont = null;
		return result;
	}

	public void UpdateNGUIText()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Invalid comparison between Unknown and I4
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Invalid comparison between Unknown and I4
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Invalid comparison between Unknown and I4
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Invalid comparison between Unknown and I4
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Invalid comparison between Unknown and I4
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Unknown result type (might be due to invalid IL or missing references)
		Font trueTypeFont = this.trueTypeFont;
		bool flag = trueTypeFont != null;
		NGUIText.fontSize = mFinalFontSize;
		NGUIText.fontStyle = mFontStyle;
		NGUIText.rectWidth = base.mWidth;
		NGUIText.rectHeight = base.mHeight;
		NGUIText.regionWidth = Mathf.RoundToInt((float)base.mWidth * (base.mDrawRegion.z - base.mDrawRegion.x));
		NGUIText.regionHeight = Mathf.RoundToInt((float)base.mHeight * (base.mDrawRegion.w - base.mDrawRegion.y));
		NGUIText.gradient = (mApplyGradient && (mFont == null || !mFont.get_packedFontShader()));
		NGUIText.gradientTop = mGradientTop;
		NGUIText.gradientBottom = mGradientBottom;
		NGUIText.encoding = mEncoding;
		NGUIText.premultiply = mPremultiply;
		NGUIText.symbolStyle = mSymbols;
		NGUIText.maxLines = mMaxLineCount;
		NGUIText.spacingX = effectiveSpacingX;
		NGUIText.spacingY = effectiveSpacingY;
		NGUIText.fontScale = ((!flag) ? ((float)mFontSize / (float)mFont.get_defaultSize() * mScale) : mScale);
		if (mFont != null)
		{
			NGUIText.bitmapFont = mFont;
			while (true)
			{
				UIFont replacement = NGUIText.bitmapFont.get_replacement();
				if (replacement == null)
				{
					break;
				}
				NGUIText.bitmapFont = replacement;
			}
			if (NGUIText.bitmapFont.get_isDynamic())
			{
				NGUIText.dynamicFont = NGUIText.bitmapFont.get_dynamicFont();
				NGUIText.bitmapFont = null;
			}
			else
			{
				NGUIText.dynamicFont = null;
			}
		}
		else
		{
			NGUIText.dynamicFont = trueTypeFont;
			NGUIText.bitmapFont = null;
		}
		if (flag && keepCrisp)
		{
			UIRoot root = this.get_root();
			if (root != null)
			{
				NGUIText.pixelDensity = ((!(root != null)) ? 1f : root.get_pixelSizeAdjustment());
			}
		}
		else
		{
			NGUIText.pixelDensity = 1f;
		}
		if (mDensity != NGUIText.pixelDensity)
		{
			ProcessText(legacyMode: false, full: false);
			NGUIText.rectWidth = base.mWidth;
			NGUIText.rectHeight = base.mHeight;
			NGUIText.regionWidth = Mathf.RoundToInt((float)base.mWidth * (base.mDrawRegion.z - base.mDrawRegion.x));
			NGUIText.regionHeight = Mathf.RoundToInt((float)base.mHeight * (base.mDrawRegion.w - base.mDrawRegion.y));
		}
		if ((int)alignment == 0)
		{
			Pivot pivot = this.get_pivot();
			if ((int)pivot == 3 || (int)pivot == 0 || (int)pivot == 6)
			{
				NGUIText.alignment = 1;
			}
			else if ((int)pivot == 5 || (int)pivot == 2 || (int)pivot == 8)
			{
				NGUIText.alignment = 3;
			}
			else
			{
				NGUIText.alignment = 2;
			}
		}
		else
		{
			NGUIText.alignment = alignment;
		}
		NGUIText.Update();
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused && mTrueTypeFont != null)
		{
			this.Invalidate(false);
		}
	}
}
