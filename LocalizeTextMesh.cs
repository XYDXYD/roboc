using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TextMesh))]
internal class LocalizeTextMesh : MonoBehaviour
{
	private TextMesh textMesh;

	public string key;

	private bool mStarted;

	public LocalizeTextMesh()
		: this()
	{
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			OnLocalize();
		}
	}

	private unsafe void Start()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		mStarted = true;
		OnLocalize();
		Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private void OnLocalize()
	{
		textMesh = this.GetComponent<TextMesh>();
		if (!string.IsNullOrEmpty(key))
		{
			textMesh.set_text(Localization.Get(key, true));
		}
	}

	private unsafe void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}
}
