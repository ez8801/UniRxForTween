# UniRxForTween

About
---

A simple script that makes it easy to use tween using [Urirx](https://github.com/neuecc/UniRx).

Usage
---

```csharp
void DoTween()
{
	SimpleTweener.EaseIn(0.25f, OnNext, OnCompleted);

	// You can also change curve method and range
	SimpleTweener.EaseInOut(0f, 100f, 0.25f, OnNext, OnCompleted);

	// Available Linear, Ease in, Ease out, Ease in-out, Bounce in, Bounce out
}

void OnNext(float value)
{
	
}

void OnCompleted()
{
	// Done!
}

```

<!-- LICENSE -->
License
---

Distributed under the MIT License. See `LICENSE` for more information.