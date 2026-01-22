using UnityEngine;

public class Bouncy : Zone
{
    internal override void Enter(Collider collider)
    {
        Player playerOptional = collider.gameObject.GetComponent<Player>();
        if (playerOptional)
        {
            playerOptional.SignalBounce();
        }
    }

    internal override void Exit(Collider collider)
    {
        /**/
    }

}
