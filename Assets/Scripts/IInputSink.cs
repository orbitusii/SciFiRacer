using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputSink
{
    public int player { get; }

    public void SetFrameInput(InputSnapshot snap);
}
