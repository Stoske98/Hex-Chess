using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public Transform target;
    public float h = 16f;
    public float gravity = -10f;
    private Vector3 position;
    private Vector3 destination;
    private Vector3 velocity;
    private float time = 0;
    private Unit cast_unit;
    private Unit enemy_unit;

    public void Initialization(Unit _cast_unit, Unit _enemy_unit, float hight, float speed)
    {
        position = _cast_unit.game_object.transform.position + Vector3.up;
        destination = _enemy_unit.game_object.transform.position + Vector3.up;
        h = hight;
        cast_unit = _cast_unit;
        enemy_unit = _enemy_unit;

        velocity = GetParabolaInitVelocity(position, destination, gravity, hight, 0);
        gameObject.transform.LookAt(GetParabolaNextPosition(position, velocity, gravity, Time.deltaTime * speed));
    }

    public void Update()
    {
        float deltaTime = Time.deltaTime;
        position = GetParabolaNextPosition(position, velocity, gravity, deltaTime);
        gameObject.transform.position = position;
        time += deltaTime;
        velocity.y += gravity * deltaTime;

        gameObject.transform.LookAt(GetParabolaNextPosition(position, velocity, gravity, deltaTime));

        if (position.y <= destination.y)
        {
            enemy_unit.ReceiveDamage(cast_unit.stats.damage);
            cast_unit.enemy_unit = null;
            Destroy(gameObject);
        }
    }

    public static Vector3 GetParabolaInitVelocity(Vector3 from, Vector3 to, float gravity = 9.8f, float heightOff = 0.0f, float rangeOff = 0.11f)
    {
        // get our return value ready. Default to (0f, 0f, 0f)
        Vector3 newVel = new Vector3();
        // Find the direction vector without the y-component
        //// Find the direction vector without y component//
        Vector3 direction = new Vector3(to.x, 0f, to.z) - new Vector3(from.x, 0f, from.z);
        // Find the distance between the two points (without the y-component)
        // Find the distance between the two points (not y component)//
        float range = direction.magnitude;
        // Add a little bit to the range so that the ball is aiming at hitting the back of the rim.
        // Back of the rim shots have a better chance of going in.
        // This accounts for any rounding errors that might make a shot miss (when we don't want it to).
        range += rangeOff;
        // Find unit direction of motion without the y component
        Vector3 unitDirection = direction.normalized;
        // Find the max height
        // Start at a reasonable height above the hoop, so short range shots will have enough clearance to go in the basket
        // without hitting the front of the rim on the way up or down.
        float maxYPos = to.y + heightOff;
        // check if the range is far enough away where the shot may have flattened out enough to hit the front of the rim
        // if it has, switch the height to match a 45 degree launch angle
        //if (range / 2f > maxYPos)
        //  maxYPos = range / 2f;
        if (maxYPos < from.y)
            maxYPos = from.y;

        // find the initial velocity in y direction
        //// We find the initial velocity in the Y direction.//
        float ft;
        ft = -2.0f * gravity * (maxYPos - from.y);
        if (ft < 0) ft = 0f;
        newVel.y = Mathf.Sqrt(ft);
        // find the total time by adding up the parts of the trajectory
        // time to reach the max
        // The parts of the trajectory that the total time of discovery adds up//
        // Maximum time//

        ft = -2.0f * (maxYPos - from.y) / gravity;
        if (ft < 0)
            ft = 0f;

        float timeToMax = Mathf.Sqrt(ft);
        // time to return to y-target
        // Time returns to the y-axis target//

        ft = -2.0f * (maxYPos - to.y) / gravity;
        if (ft < 0)
            ft = 0f;

        float timeToTargetY = Mathf.Sqrt(ft);
        // add them up to find the total flight time
        // Add them up to find the total flight time.//
        float totalFlightTime;

        totalFlightTime = timeToMax + timeToTargetY;

        // find the magnitude of the initial velocity in the xz direction
        //// The magnitude of the initial velocity of the search is in the XZ direction//
        float horizontalVelocityMagnitude = range / totalFlightTime;
        // use the unit direction to find the x and z components of initial velocity
        // Using the direction of the element to find the X and Z components of the initial velocity//
        newVel.x = horizontalVelocityMagnitude * unitDirection.x;
        newVel.z = horizontalVelocityMagnitude * unitDirection.z;
        return newVel;
    }

    /// <summary> Calculate the position of parabolic object in the next frame </summary>.
    /// <param name="position">initial position </param>
    /// <param name="velocity">moving speed</param>
    /// <param name="gravity">gravity acceleration </param>
    /// <param name="time">flight time </param>
    /// <returns></returns>
    public static Vector3 GetParabolaNextPosition(Vector3 position, Vector3 velocity, float gravity, float time)
    {
        velocity.y += gravity * time;
        return position + velocity * time;
    }
}
