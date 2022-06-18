using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRouter : MonoBehaviour
{
    public float startDelay = 0.1f;
    private bool hasStarted = false;

    private List<RaceUser> racers;

    public int PlayerID = 0;
    private Spacecraft targetCraft;

    private MainControls localControls;
    private MainControls.DefaultActions actions;

    private void FixedUpdate()
    {

        if (!hasStarted)
        {
            if (startDelay <= 0)
            {
                Initialize();
            }
            else
            {
                startDelay -= Time.fixedDeltaTime;
                return;
            }
        }
        else if (targetCraft)
        {
            ProcessPlayerInput();

            //ProcessCPUInput();
        }

    }

    private void Initialize ()
    {
        racers = RaceManager.GetRaceUsers();

        int tcIndex = racers.FindIndex(x => x.Equals(PlayerID));

        if(tcIndex > -1)
        {
            Debug.Log("Found player!");
            targetCraft = racers[tcIndex].spacecraft;

            localControls = new MainControls();
            actions = localControls.Default;
            actions.Enable();
        }

        hasStarted = true;
    }

    private void ProcessPlayerInput ()
    {
        InputSnapshot snap = new InputSnapshot()
        {
            Player = PlayerID,
            Throttle = actions.Throttle.ReadValue<float>(),
            Boost = actions.Boost.WasPerformedThisFrame(),
            Pitch = actions.Pitch.ReadValue<float>(),
            Yaw = actions.Yaw.ReadValue<float>(),

            Used = false
        };

        targetCraft.SetFrameInput(snap);
    }
}

public struct InputSnapshot
{
    public int Player;

    public float Throttle;
    public bool Boost;
    public float Pitch;
    public float Yaw;

    public bool Used;
}
